using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AtendTelemedicina.Integracao;
using AtendTeleMedicina.Application.Contracts.Integracao;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Application.Contracts.Security;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Integracao;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Integracao;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Integracao
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class IntegracaoController : ControllerBase
    {
        private Integrador _integrador;
        private readonly IIndividuoApplication _individuoApplication;
        private readonly IEstabelecimentoApplication _estabelecimentoApplication;
        private readonly IAtendimentoApplication _atendimentoApplication;
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly ILoteApplication _loteApplication;
        private readonly IUserApplication _userApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private static IHostingEnvironment _environment;

        public IntegracaoController(IIndividuoApplication individuoApplication,
            IEstabelecimentoApplication estabelecimentoApplication,
            IAtendimentoApplication atendimentoApplication,
            IAgendamentoApplication agendamentoApplication,
            ILoteApplication loteApplication,
            IUserApplication userApplication,
            IMapper mapper,
            ILogger<IntegracaoController> logger,
            IHostingEnvironment environment)
        {
            _individuoApplication = individuoApplication;
            _loteApplication = loteApplication;
            _estabelecimentoApplication = estabelecimentoApplication;
            _atendimentoApplication = atendimentoApplication;
            _agendamentoApplication = agendamentoApplication;
            _userApplication = userApplication;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet("ListarLote")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> ListarLote(int mes, int ano, string cnes, int skip = 1, int take = 50)
        {
            try
            {
                var (lotes, totalCount) = await _loteApplication.ListarLote(mes, ano, cnes, skip, take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<LoteModel>>(lotes),
                    skip,
                    take,
                    totalCount,
                    totalPages
                };

                return Ok(response);

            }
            catch (Exception e)
            {
                _logger.LogError($"ListaLote: {mes}/{ano}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPost("GerarLote")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> GerarLote(int mes, int ano, string estabelecimentoId)
        {
            var xsd = Path.Combine(_environment.WebRootPath, "Content", "XSD");
            var estabelecimentoModel = new EstabelecimentoModel();
            if (!string.IsNullOrEmpty(estabelecimentoId))
                estabelecimentoModel.Id = estabelecimentoId;

            var (estabelecimentos, total) = await _estabelecimentoApplication.GetByParam(_mapper.Map<Estabelecimento>(estabelecimentoModel), "NomeFantasia", 1, 999).ConfigureAwait(false);

            foreach (var item in estabelecimentos)
            {
                _integrador = new Integrador(xsd);
                var arquivos = new Dictionary<string, string>();

                try
                {
                    var erros = new List<object>();
                    var lote = _loteApplication.ExisteLote(mes, ano, item.Id);
                    lote.Usuario_Id = User.FindFirstValue("uid");
                    lote.Data = DateTime.Now;
                    lote.Mes = mes;
                    lote.Ano = ano;
                    lote.Estabelecimento_Id = item.Id;

                    BuscarIndividuos(lote, mes, ano, item.Id, arquivos, erros);
                    BuscarFichasDeAtendimento(lote, mes, ano, item.Id, arquivos, erros);
                    BuscarFichasDeAtendimentoDomiciliar(lote, mes, ano, item.Id, arquivos, erros);
                    BuscarFichasProcedimento(lote, mes, ano, item.Id, arquivos, erros);

                    if (lote.LoteIntegracao.Count > 0)
                        _loteApplication.AtualizarOuInserir(lote);
                }
                catch (Exception e)
                {
                    return BadRequest("Não foi possível gerar o lote");
                }
            }

            return Ok();
        }

        private async void BuscarIndividuos(Lote lote, int mes, int ano, string estabelecimentoId, Dictionary<string, string> arquivos, List<object> erros)
        {
            var cpf = User.FindFirstValue(ClaimTypes.Name);
            var usuario = _userApplication.GetByCpf(cpf);

            var (individuos, totalCount) = await _individuoApplication.PendentesIntegracao(mes, ano, estabelecimentoId).ConfigureAwait(false);

            //individuos = individuos.Where(x => x.Id == "48f18db9-bc23-4ce0-8066-de8b96ffa1f4");

            foreach (var individuo in individuos)
            {
                var obj = _integrador.GerarXMLIndividuoSimplificado(lote.Id, individuo, cpf, usuario.Nome);
                if (obj.Status == true)
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(individuo.Id), obj.Tipo, obj.Status, obj.Content, null, lote.Data));
                    arquivos.Add(obj.Id.ToString(), obj.Content);
                    _individuoApplication.ConfirmarIntegracao(lote.Id, individuo);
                }
                else
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(individuo.Id), obj.Tipo, obj.Status, null, obj.Content, lote.Data));
                    erros.Add(new { Id = individuo.Id.ToString(), Messagem = obj.Content });
                }
            }
        }

        private async void BuscarFichasProcedimento(Lote lote, int mes, int ano, string estabelecimentoId, Dictionary<string, string> arquivos, List<object> erros)
        {
            var cpf = User.FindFirstValue(ClaimTypes.Name);
            var usuario = _userApplication.GetByCpf(cpf);

            var (atendimentos, totalCount) = await _atendimentoApplication.ProcedimentosPendentesIntegracao(mes, ano, estabelecimentoId).ConfigureAwait(false);

            //atendimentos = atendimentos.Where(x => x.Id == "1e77b5ed-29d8-43c9-b16f-293248a6c144"); 

            foreach (var atendimento in atendimentos)
            {
                var obj = _integrador.GerarXMLFichaDeProcedimento(lote.Id, atendimento, cpf, usuario.Nome);
                if (obj.Status == true)
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(atendimento.AgendamentoId), obj.Tipo, obj.Status, obj.Content, null, lote.Data));
                    arquivos.Add(obj.Id.ToString(), obj.Content);
                    _atendimentoApplication.ConfirmarIntegracaoProcedimentos(lote.Id, atendimento);
                }
                else
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(atendimento.Id), obj.Tipo, obj.Status, null, obj.Content, lote.Data));
                    erros.Add(new { Id = atendimento.Id.ToString(), Messagem = obj.Content });
                }
            }
        }

        private async void BuscarFichasDeAtendimento(Lote lote, int mes, int ano, string estabelecimentoId, Dictionary<string, string> arquivos, List<object> erros)
        {
            var cpf = User.FindFirstValue(ClaimTypes.Name);
            var usuario = _userApplication.GetByCpf(cpf);

            var (atendimentos, totalCount) = await _atendimentoApplication.PendentesIntegracao(mes, ano, estabelecimentoId, includeAD: false).ConfigureAwait(false);

            //atendimentos = atendimentos.Where(x => x.Id == "b9590c3f-0390-46e9-b218-0b7af1565e16");

            foreach (var atendimento in atendimentos)
            {
                var obj = _integrador.GerarXMLFichaDeAtendimento(lote.Id, atendimento, cpf, usuario.Nome);
                if (obj.Status == true)
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(atendimento.Id), obj.Tipo, obj.Status, obj.Content, null, lote.Data));
                    arquivos.Add(obj.Id.ToString(), obj.Content);
                    _atendimentoApplication.ConfirmarIntegracao(lote.Id, atendimento);
                }
                else
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(atendimento.Id), obj.Tipo, obj.Status, null, obj.Content, lote.Data));
                    erros.Add(new { Id = atendimento.Id.ToString(), Messagem = obj.Content });
                }
            }
        }

        private async void BuscarFichasDeAtendimentoDomiciliar(Lote lote, int mes, int ano, string estabelecimentoId, Dictionary<string, string> arquivos, List<object> erros)
        {
            var cpf = User.FindFirstValue(ClaimTypes.Name);
            var usuario = _userApplication.GetByCpf(cpf);

            var (atendimentos, totalCount) = await _atendimentoApplication.PendentesIntegracao(mes, ano, estabelecimentoId, includeAD: true).ConfigureAwait(false);

            foreach (var atendimento in atendimentos)
            {
                var obj = _integrador.GerarXMLFichaDeAtendimentoDomiciliar(lote.Id, atendimento, cpf, usuario.Nome);
                if (obj.Status == true)
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(atendimento.Id), obj.Tipo, obj.Status, obj.Content, null, lote.Data));
                    arquivos.Add(obj.Id.ToString(), obj.Content);
                    _atendimentoApplication.ConfirmarIntegracao(lote.Id, atendimento);
                }
                else
                {
                    lote.LoteIntegracao.Add(new LoteIntegracao(lote.Id, Guid.Parse(atendimento.Id), obj.Tipo, obj.Status, null, obj.Content, lote.Data));
                    erros.Add(new { Id = atendimento.Id.ToString(), Messagem = obj.Content });
                }
            }
        }


        [HttpGet("BaixarZip")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> BaixarZip(int id)
        {
            var arquivos = new Dictionary<string, string>();

            var lotes = _loteApplication.GetById(id);

            var memoryStream = new MemoryStream();
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            var nomeZip = $"Lote_{id}_{lotes.Estabelecimento.Cnes}-{lotes.Estabelecimento.NomeFantasia.Replace(" ", "_")}.zip";
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {

                foreach (var item in lotes.LoteIntegracao.Where(x => x.Status == true))
                {
                    var file = archive.CreateEntry(item.Cadastro_Id.ToString() + ".esus.xml");
                    using (var entryStream = file.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(item.DadosXml);
                    }
                }
            }

            memoryStream.Position = 0;

            return File(memoryStream, "application/zip", nomeZip);
        }

        [HttpGet("RelatorioErros")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RelatorioErros(int loteId)
        {
            try
            {
                var lote = _loteApplication.GetDetailedById(loteId);
                var erros = lote.LoteIntegracao.Where(x => x.Status == false).ToList();

                var filePath = $"{_environment.WebRootPath}\\App_Data\\Templates\\RelatorioErrosIntegracao.xlsx";
                if (erros == null) return NoContent();
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheet("Erros");

                    int numeroProximaLinha = 1;
                    foreach (LoteIntegracao rel in erros)
                    {
                        sheet.CreateRow(numeroProximaLinha);

                        sheet.GetRow(numeroProximaLinha).CreateCell(0).SetCellValue($"{rel.Lote_Id}");
                        sheet.GetRow(numeroProximaLinha).CreateCell(1).SetCellValue($"{rel.DataGeracao.ToString("dd/MM/yyyy HH:mm")}");
                        sheet.GetRow(numeroProximaLinha).CreateCell(2).SetCellValue($"{rel.Cadastro_Id}");
                        sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue($"{rel.Tipo}");
                        sheet.GetRow(numeroProximaLinha).CreateCell(4).SetCellValue($"{rel.Erros}");

                        numeroProximaLinha++;
                    }
                    workbook.Write(memory);
                }
                memory.Position = 0;

                return File(memory, GetContentType(filePath), $"Erros-Lote_{loteId}_{lote.Estabelecimento.Cnes}-{lote.Estabelecimento.NomeFantasia.Replace(" ", "_")}.xlsx");

            }
            catch (Exception e)
            {
                return BadRequest("Não foi possível gerar o relatório");
            }
        }

        [HttpGet("RelatorioFichasGeradas")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RelatorioFichasGeradas(int loteId)
        {
            try
            {
                var lote = _loteApplication.GetDetailedById(loteId);

                var fichas = lote.LoteIntegracao.Where(x => x.Status == true).GroupBy(x => x.Tipo)
                         .Select(n => new
                         {
                             Tipo = n.Key,
                             Total = n.Count()
                         })
                         .OrderBy(n => n.Tipo).ToList();


                var filePath = $"{_environment.WebRootPath}\\App_Data\\Templates\\RelatorioFichasGeradas.xlsx";
                if (fichas == null) return NoContent();
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheet("Total");

                    ICellStyle cellStyle = workbook.CreateCellStyle();
                    cellStyle.BorderBottom = BorderStyle.Thin;
                    cellStyle.BorderTop = BorderStyle.Thin;
                    cellStyle.BorderRight = BorderStyle.Thin;
                    cellStyle.BorderLeft = BorderStyle.Thin;

                    ICellStyle cellStyleBG = workbook.CreateCellStyle();
                    cellStyleBG.BorderBottom = BorderStyle.Thin;
                    cellStyleBG.BorderTop = BorderStyle.Thin;
                    cellStyleBG.BorderRight = BorderStyle.Thin;
                    cellStyleBG.BorderLeft = BorderStyle.Thin;
                    cellStyleBG.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    cellStyleBG.FillPattern = FillPattern.SolidForeground;

                    sheet.GetRow(1).CreateCell(0).SetCellValue($"Tipo");
                    sheet.GetRow(1).GetCell(0).CellStyle = cellStyleBG;
                    sheet.GetRow(1).CreateCell(1).SetCellValue($"Total");
                    sheet.GetRow(1).GetCell(1).CellStyle = cellStyleBG;

                    int numeroProximaLinha = 2;
                    foreach (var item in fichas)
                    {
                        sheet.CreateRow(numeroProximaLinha);
                        sheet.GetRow(numeroProximaLinha).CreateCell(0).SetCellValue($"{item.Tipo}");
                        sheet.GetRow(numeroProximaLinha).GetCell(0).CellStyle = cellStyle;
                        sheet.GetRow(numeroProximaLinha).CreateCell(1).SetCellValue($"{item.Total}");
                        sheet.GetRow(numeroProximaLinha).GetCell(1).CellStyle = cellStyle;

                        numeroProximaLinha++;
                    }
                    workbook.Write(memory);
                }
                memory.Position = 0;

                return File(memory, GetContentType(filePath), $"Totais-Lote_{loteId}_{lote.Estabelecimento.Cnes}-{lote.Estabelecimento.NomeFantasia.Replace(" ", "_")}.xlsx");

            }
            catch (Exception e)
            {
                return BadRequest("Não foi possível gerar o relatório");
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip", "application/zip"},
                {".7z", "application/x-7z-compressed"},
                {".gz", "application/x-gzip"},
            };
        }

    }
}
