using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class RelatorioController : Controller
    {
        private readonly IAtendimentoApplication _atendimentoApplication;
        private readonly IIndividuoGlicemiaApplication _individuoGlicemiaApplication;
        private readonly IAuditoriaApplication _auditoriaApplication;
        private readonly IIndividuoCiapApplication _individuoCiapApplication;
        private readonly IIndividuoCid10Application _individuoCid10Application;
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private static IHostingEnvironment _environment;

        public RelatorioController(IAtendimentoApplication atendimentoApplication, IIndividuoGlicemiaApplication individuoGlicemiaApplication, IAuditoriaApplication auditoriaApplication, IIndividuoCiapApplication individuoCiapApplication, IIndividuoCid10Application individuoCid10Application, IAgendamentoApplication agendamentoApplication, IUser user, IMapper mapper, ILoggerFactory logger, IHostingEnvironment environment)
        {
            _atendimentoApplication = atendimentoApplication;
            _individuoGlicemiaApplication = individuoGlicemiaApplication;
            _auditoriaApplication = auditoriaApplication;
            _individuoCiapApplication = individuoCiapApplication;
            _individuoCid10Application = individuoCid10Application;
            _agendamentoApplication = agendamentoApplication;
            _user = user;
            _mapper = mapper;
            _logger = logger.CreateLogger<AtendimentoController>();
            _environment = environment;
        }



        //RELATORIO ATENDIMENTO MEDICO
        [HttpGet]
        [Route("ExcelRelatorioAtendimento")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> ExcelRelatorioAtendimento([FromQuery]AtendimentoModel filters, string sort = "DataCadastro DESC", int? skip = null, int? take = null)
        {
            try
            {
                var (atendimentos, totalCount) = await _atendimentoApplication.GetByParam(
                     _mapper.Map<Atendimento>(filters),
                     sort,
                     skip,
                     take).ConfigureAwait(false);

                #region Filtros

                var filePath = $"{_environment.WebRootPath}\\App_Data\\Templates\\RelatorioAtendimento.xlsx";
                if (atendimentos == null) return NoContent();
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheet("RELATORIO");

                    int numeroProximaLinha = 4;
                    foreach (Atendimento rel in atendimentos)
                    {
                        
                        sheet.CreateRow(numeroProximaLinha);
                        if (rel.AtendidoMedico == true && rel.AtendidoTriagem == false) {

                            int hora = 0;
                            int minuto = 0;
                            int segundo = 0;
                            int resto = 0;

                            if (rel.TempoAtendimento != null)
                            {
                                var tempoInt = Int16.Parse(rel.TempoAtendimento);
                                hora = tempoInt / 3600;
                                resto = tempoInt % 3600;

                                minuto = resto / 60;
                                resto = resto % 60;

                                segundo = resto;
                            }
                            

                            sheet.GetRow(numeroProximaLinha).CreateCell(0).SetCellValue($"{rel.DataCadastro}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(1).SetCellValue($"{rel.Individuo.NomeCompleto}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(2).SetCellValue($"{rel.Individuo.DataDoNascimento}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue($"{rel.Profissional.Nome}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(4).SetCellValue((rel.TempoAtendimento != null) ? $"{minuto}"+":"+$"{segundo}"+"min" : "");
                            sheet.GetRow(numeroProximaLinha).CreateCell(5).SetCellValue((rel.InicioDoAtendimento != DateTime.MinValue) ? $"{rel.InicioDoAtendimento}" : "Nenhum dado");
                            sheet.GetRow(numeroProximaLinha).CreateCell(6).SetCellValue((rel.FimDoAtendimento != DateTime.MinValue) ? $"{rel.FimDoAtendimento}" : "Nenhum dado");
                            sheet.GetRow(numeroProximaLinha).CreateCell(7).SetCellValue($"{rel.Individuo.TelefoneCelular}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(8).SetCellValue($"{rel.Individuo.Email}");
                        }


                        numeroProximaLinha++;
                    }
                    //sheet.GetRow(numeroProximaLinha + 1).GetCell(1).SetCellValue(appSettings.WebAppVersao);
                    workbook.Write(memory);
                }
                memory.Position = 0;

                return File(memory, GetContentType(filePath), $"RelatorioAtendimentoMeeds.xlsx");

                #endregion
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }



        //RELATORIO ATENDIMENTO PACIENTE
        [HttpGet]
        [Route("ExcelRelatorioAtendimentoPaciente")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> ExcelRelatorioAtendimentoPaciente([FromQuery] AtendimentoModel filters, string sort = "DataCadastro DESC", int? skip = null, int? take = null)
        {
            try
            {
                var (atendimentos, totalCount) = await _atendimentoApplication.GetByParam(
                     _mapper.Map<Atendimento>(filters),
                     sort,
                     skip,
                     take).ConfigureAwait(false);

                #region Filtros

                var filePath = $"{_environment.WebRootPath}\\App_Data\\Templates\\RelatorioAtendimentoPaciente.xlsx";
                if (atendimentos == null) return NoContent();
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheet("RELATORIO");

                    int numeroProximaLinha = 4;
                    foreach (Atendimento rel in atendimentos)
                    {
                        sheet.CreateRow(numeroProximaLinha);
                        if (rel.AtendidoMedico == true && rel.AtendidoTriagem == false)
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(0).SetCellValue($"{rel.DataCadastro}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(1).SetCellValue((rel.TempoAtendimento != null) ? $"{rel.TempoAtendimento}" : "");
                            sheet.GetRow(numeroProximaLinha).CreateCell(2).SetCellValue($"{rel.Individuo.NomeCompleto}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue($"{rel.Individuo.DataNascimento}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(4).SetCellValue($"{rel.Individuo.Idade}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(5).SetCellValue($"{rel.Individuo.TelefoneCelular}");
                            sheet.GetRow(numeroProximaLinha).CreateCell(6).SetCellValue($"{rel.Individuo.Email}");
                        }


                        numeroProximaLinha++;
                    }
                    //sheet.GetRow(numeroProximaLinha + 1).GetCell(1).SetCellValue(appSettings.WebAppVersao);
                    workbook.Write(memory);
                }
                memory.Position = 0;

                return File(memory, GetContentType(filePath), $"RelatorioAtendimentoPacienteMeeds.xlsx");


                #endregion
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }





        //RELATORIO AUDITORIA
        [HttpGet]
        [Route("ExcelRelatorioAuditoria")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> ExcelRelatorioAuditoria([FromQuery] AuditoriaModel auditoriaModel, [FromQuery] AppParams appParams, string sort = "DataHora", int? skip = null, int? take = null)
        {
            try
            {
                var (auditorias, totalCount) = await _auditoriaApplication.GetByParam(
                    _mapper.Map<Auditoria>(auditoriaModel), appParams,
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                #region Filtros

                var filePath = $"{_environment.WebRootPath}\\App_Data\\Templates\\RelatorioAuditoria.xlsx";
                if (auditorias == null) return NoContent();
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheet("RELATORIO");

                    int numeroProximaLinha = 4;
                    foreach (Auditoria rel in auditorias)
                    {
                        sheet.CreateRow(numeroProximaLinha);

                        sheet.GetRow(numeroProximaLinha).CreateCell(0).SetCellValue((rel.Acao != null) ? $"{rel.Acao}" : "Não foi possivel receber a ação");
                        sheet.GetRow(numeroProximaLinha).CreateCell(1).SetCellValue((rel.Id != null) ? $"{rel.Id}" : "Não foi possivel receber o id");
                        sheet.GetRow(numeroProximaLinha).CreateCell(2).SetCellValue((rel.Ip != null) ? $"{rel.Ip}" : "Não foi possivel receber o ip");
                        sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue((rel.Profissional != null) ? $"{rel.Profissional}" : "Não foi possivel receber o utilizador");
                        sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue((rel.Usuario != null) ? $"{rel.Usuario}" : "Não foi possivel receber o utilizador");
                        sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue((rel.Individuo != null) ? $"{rel.Individuo}" : "Não foi possivel receber o utilizador");
                        sheet.GetRow(numeroProximaLinha).CreateCell(4).SetCellValue((rel.TabelaId != null) ? $"{rel.TabelaId}" : "Não foi possivel receber o registro");
                        sheet.GetRow(numeroProximaLinha).CreateCell(5).SetCellValue((rel.Tabela != null) ? $"{rel.Tabela}" : "Não foi possivel receber a tabela");
                        sheet.GetRow(numeroProximaLinha).CreateCell(6).SetCellValue((rel.Origem != null) ? $"{rel.Origem}" : "Não foi possivel receber a origem");
                        sheet.GetRow(numeroProximaLinha).CreateCell(7).SetCellValue((rel.DataHora != null) ? $"{rel.DataHora}" : "Não foi possivel receber a data e hora");



                        numeroProximaLinha++;
                    }
                    //sheet.GetRow(numeroProximaLinha + 1).GetCell(1).SetCellValue(appSettings.WebAppVersao);
                    workbook.Write(memory);
                }
                memory.Position = 0;

                return File(memory, GetContentType(filePath), $"RelatorioAuditoriaMeeds.xlsx");


                #endregion
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }



        // RELATÓRIO DE GLICEMIA
        [HttpGet]
        [Route("ExcelRelatorioGlicemia")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> ExcelRelatorioGlicemia([FromQuery] IndividuoGlicemia filters, string sort = "ig.DataCadastro DESC", int skip = 1, int take = 10)
        {
            if (filters == null) return BadRequest("Informe os filtros para o relatório");
            try
            {
                var (individuoGlicemias, totalCount) = await _individuoGlicemiaApplication.GetByParam(
                    _mapper.Map<IndividuoGlicemia>(filters),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                #region Filtros

                var filePath = $"{_environment.WebRootPath}\\App_Data\\Templates\\RelatorioGlicemia.xlsx";
                if (individuoGlicemias == null) return NoContent();
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheet("RELATORIO");

                    int numeroProximaLinha = 5;
                    foreach (IndividuoGlicemia rel in individuoGlicemias)
                    {

                        sheet.CreateRow(numeroProximaLinha);

                        sheet.GetRow(numeroProximaLinha).CreateCell(0).SetCellValue($"{rel.DataCadastro.ToString("dd/MM/yyyy")}");
                        ApplyCenterText(sheet, numeroProximaLinha, 0);

                        sheet.GetRow(numeroProximaLinha).CreateCell(1).SetCellValue($"{rel.Individuo.NomeCompleto}");
                        ApplyCenterText(sheet, numeroProximaLinha, 1);

                        if (rel.RespondeuCafe == true)
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(2).SetCellValue($"{rel.CafeAntes}");
                            ApplyCenterText(sheet, numeroProximaLinha, 2);
                            sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue($"{rel.CafeDepois}");
                            ApplyCenterText(sheet, numeroProximaLinha, 3);
                        }
                        else
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(2).SetCellValue($"Sem Resposta");
                            ApplyCenterText(sheet, numeroProximaLinha, 2);
                            sheet.GetRow(numeroProximaLinha).CreateCell(3).SetCellValue($"Sem Resposta");
                            ApplyCenterText(sheet, numeroProximaLinha, 3);
                        }

                        if (rel.RespondeuAlmoco == true)
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(4).SetCellValue($"{rel.AlmocoAntes}");
                            ApplyCenterText(sheet, numeroProximaLinha, 4);
                            sheet.GetRow(numeroProximaLinha).CreateCell(5).SetCellValue($"{rel.AlmocoDepois}");
                            ApplyCenterText(sheet, numeroProximaLinha, 5);
                        }
                        else
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(4).SetCellValue($"Sem Resposta");
                            ApplyCenterText(sheet, numeroProximaLinha, 4);
                            sheet.GetRow(numeroProximaLinha).CreateCell(5).SetCellValue($"Sem Resposta");
                            ApplyCenterText(sheet, numeroProximaLinha, 5);
                        }

                        if (rel.RespondeuJanta == true)
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(6).SetCellValue($"{rel.JantaAntes}");
                            ApplyCenterText(sheet, numeroProximaLinha, 6);
                            sheet.GetRow(numeroProximaLinha).CreateCell(7).SetCellValue($"{rel.JantaDepois}");
                            ApplyCenterText(sheet, numeroProximaLinha, 7);
                        }
                        else
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(6).SetCellValue($"Sem Resposta");
                            ApplyCenterText(sheet, numeroProximaLinha, 6);
                            sheet.GetRow(numeroProximaLinha).CreateCell(7).SetCellValue($"Sem Resposta");
                            ApplyCenterText(sheet, numeroProximaLinha, 7);
                        }

                        if (rel.RespondeuDormirMadrugada == true)
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(8).SetCellValue($"{rel.JantaAntes}");
                            ApplyCenterText(sheet, numeroProximaLinha, 8);
                        }
                        else
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(8).SetCellValue($"Sem Resposta");
                            ApplyCenterText(sheet, numeroProximaLinha, 8);
                        }

                        if (!string.IsNullOrEmpty(rel.Observacoes))
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(9).SetCellValue($"{rel.Observacoes}");
                            ApplyCenterText(sheet, numeroProximaLinha, 9);
                        }
                        else
                        {
                            sheet.GetRow(numeroProximaLinha).CreateCell(9).SetCellValue($"Sem observações");
                            ApplyCenterText(sheet, numeroProximaLinha, 9);
                        }
                        
                        numeroProximaLinha++;
                    }
                    //sheet.GetRow(numeroProximaLinha + 1).GetCell(1).SetCellValue(appSettings.WebAppVersao);
                    workbook.Write(memory);
                }
                memory.Position = 0;

                return File(memory, GetContentType(filePath), $"RelatorioGlicemiaMeeds.xlsx");


                #endregion
            }
            catch (Exception e)
            {
                return BadRequest(e);
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
        public static void ApplyCenterText(ISheet sheet, int numeroProximaLinha, int numeroColuna)
        {
            // Obtém a célula desejada
            ICell cell = sheet.GetRow(numeroProximaLinha).GetCell(numeroColuna);

            // Verifica se a célula existe antes de aplicar o estilo
            if (cell != null)
            {
                // Obtém o estilo da célula
                ICellStyle cellStyle = cell.CellStyle;

                // Configura o alinhamento horizontal para centralizar
                cellStyle.Alignment = HorizontalAlignment.Center;

                // Configura o alinhamento vertical para centralizar
                cellStyle.VerticalAlignment = VerticalAlignment.Center;

                // Configura o tamanho da fonte
                cellStyle.SetFont(new XSSFFont() { FontHeightInPoints = 14, FontName = "Arial", IsBold = false });

                // Confgura as bordas em todos os lados
                cellStyle.BorderTop = BorderStyle.Thin;
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;

                // Aplica o estilo de volta à célula
                cell.CellStyle = cellStyle;
            }
        }
    }
}
