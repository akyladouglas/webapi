using Microsoft.AspNetCore.Mvc;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using Newtonsoft.Json.Linq;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Linq;
using NPOI.SS.Util;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal;
using Microsoft.AspNetCore.Http;
using Spire.Xls;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using AtendTeleMedicina.DistributedInterfaces.Services.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;
using AtendTeleMedicina.Application.Contracts.Security;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ExamesF200")]
    [Produces("application/json")]


    public class ExamesF200Controller : Controller
    {
        private readonly ITipoExameApplication _tipoExameApplication;
        private readonly IExamesF200Application _examesF200Application;
        private readonly IExamesApplication _examesApplication;
        private readonly IUser _user;
        private readonly IUserApplication _userApplication;
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly IIndividuoApplication _individuoApplication;
        private readonly IEstabelecimentoApplication _estabelecimentoApplication;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        private static IHostingEnvironment _environment;
        public ExamesF200Controller(
            IUser user,
            IUserApplication userApplication,
            ITipoExameApplication tipoExameApplication,
            IExamesF200Application examesF200Application,
            IExamesApplication examesApplication,
            IAgendamentoApplication agendamentoApplication,
            IIndividuoApplication individuoApplication,
            IEstabelecimentoApplication estabelecimentoApplication,
            IMapper mapper,
            IHostingEnvironment environment)
        {
            _user = user;
            _userApplication = userApplication;
            _tipoExameApplication = tipoExameApplication;
            _examesF200Application = examesF200Application;
            _examesApplication = examesApplication;
            _agendamentoApplication = agendamentoApplication;
            _individuoApplication = individuoApplication;
            _estabelecimentoApplication = estabelecimentoApplication;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        [Route("GetExamesF200")]
        [Authorize(Roles = "Retaguarda, Individuo, Recepcionista, MédicoEspecialista")]
        public async Task<IActionResult> Get([FromQuery] ExamesF200 exameF200Model, string sort = "Nome ASC",
          int skip = 1, int take = 5)
        {

            try
            {
                var (tipoExame, totalCount) = await _examesF200Application.GetByParam(
                     _mapper.Map<ExamesF200>(exameF200Model),
                     sort,
                     skip,
                     take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<ExamesF200>>(tipoExame),
                    sort,
                    skip,
                    take,
                    totalCount,
                    totalPages
                };

                return Ok(response);

            }
            catch (Exception e)
            {
                _logger.LogError("ERRO");
                return BadRequest(e.Message.ToString());
            }
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

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        [AllowAnonymous]
        [HttpPost("ReceberExamesF200")]
        public async Task<IActionResult> ReceberExamesF200([FromBody] dynamic dynamicJson)
        {
            try
            {
                int qtdOBX = dynamicJson.qtdExames;
                JObject pacienteExame = new JObject();

                // Adicionar as informações MSH_6, PID_3 e OBX.
                pacienteExame["DataTransferenciaEcoPc"] = dynamicJson.MSH_6;
                pacienteExame["CpfPaciente"] = dynamicJson.PID_3;

                #region A partir do CPF, buscar quem é o paciente no banco de dados e adicionar campo Id no Json

                var cpf = dynamicJson.PID_3.ToString();

                var individuo = _individuoApplication.GetByCpf(cpf);
                pacienteExame["IdPaciente"] = individuo.Id;

                #endregion

                for (int i = 1; i <= qtdOBX; i++)
                {
                    pacienteExame[$"TipoExameEco_{i}"] = EcoF200Helper.CleanString((string)dynamicJson[$"OBX_{i}_3"]);
                    pacienteExame[$"ResultadoExameEco_{i}"] = EcoF200Helper.CleanString((string)dynamicJson[$"OBX_{i}_5"]);
                    pacienteExame[$"UnidadeResultadoEco_{i}"] = EcoF200Helper.CleanString((string)dynamicJson[$"OBX_{i}_6"]);

                    pacienteExame[$"ValorReferenciaResultadoEco_{i}"] = dynamicJson[$"OBX_{i}_7"];
                    pacienteExame[$"DataExameEco_{i}"] = dynamicJson[$"OBX_{i}_14"].ToString().Substring(0, 14);
                    pacienteExame[$"OperadorEco_{i}"] = dynamicJson[$"OBX_{i}_16"];

                    var exame = new ExamesF200
                    {
                        CpfPaciente = (string)pacienteExame["CpfPaciente"],
                        IdPaciente = (string)pacienteExame["IdPaciente"],
                        DataTransferenciaEcoPc = DateTime.Now,
                        TipoExameEco = (string)pacienteExame[$"TipoExameEco_{i}"],
                        ResultadoExameEco = (string)pacienteExame[$"ResultadoExameEco_{i}"],
                        UnidadeResultadoEco = (string)pacienteExame[$"UnidadeResultadoEco_{i}"],
                        ValorReferenciaResultadoEco = (string)pacienteExame[$"ValorReferenciaResultadoEco_{i}"],
                        DataExameEco = DateTime.ParseExact(pacienteExame[$"DataExameEco_{i}"].ToString(), "yyyyMMddHHmmssK", CultureInfo.InvariantCulture),
                        OperadorEco = (string)pacienteExame[$"OperadorEco_{i}"],
                        Individuo = individuo
                    };

                    #region Enviando resultados para o banco de dados
                    _examesF200Application.Add(exame);
                    #endregion

                    await GerarPdfF200(individuo, exame.OperadorEco);
                }

                return Ok("Exame enviado com sucesso");

            }
            catch (Exception ex)
            {
                return BadRequest("Não foi possível enviar o exame");
            }

        }

        public async Task<IActionResult> GerarPdfF200(Individuo individuo, string operador)
        {

            try
            {
                #region Gerar Excel
                //var individuo = _individuoApplication.GetByCpf(cpf);
                if (individuo == null) return NoContent();

                var agendamento = _agendamentoApplication.GetByEstabelecimentoByIndividuoId(individuo.Id);
                if (agendamento == null) return NoContent();

                var (examef200, totalCount) = await _examesF200Application.GetByCpfIndividuo(individuo.Cpf).ConfigureAwait(false);
                if (examef200 == null) return NoContent();

                var filePath = $"{_environment.WebRootPath}\\App_Data\\Templates\\ExameF200Novo.xlsx";

                var memory = new MemoryStream();

                foreach (var item in examef200)
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var workbook = new XSSFWorkbook(stream);
                        ISheet sheet = workbook.GetSheet("EXAME");
                        ICellStyle cellStyle = workbook.CreateCellStyle();
                        ICellStyle cellStyle2 = workbook.CreateCellStyle();

                        //XSSFCellStyle evenStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                        //cellStyle2.FillBackgroundColor = IndexedColors.LightGreen.Index;


                        XSSFCellStyle evenStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                        evenStyle.FillPattern = FillPattern.SolidForeground;
                        evenStyle.FillForegroundColor = IndexedColors.LightGreen.Index;


                        XSSFCellStyle cellStyleResult = (XSSFCellStyle)workbook.CreateCellStyle();
                        byte[] rgbResult = new byte[3] { 203, 255, 204 };
                        cellStyleResult.SetFillForegroundColor(new XSSFColor(rgbResult));
                        cellStyleResult.FillPattern = FillPattern.SolidForeground;
                        cellStyleResult.Alignment = HorizontalAlignment.Center;


                        int numeroProximaLinha = 1;

                        sheet.CreateRow(numeroProximaLinha);

                        sheet.GetCell(3, 6).SetCellValue($"{agendamento.Estabelecimento.NomeFantasia}");

                        sheet.GetCell(5, 2).SetCellValue($"{individuo.NomeCompleto}");
                        sheet.GetCell(5, 2).CellStyle = cellStyle;

                        sheet.GetCell(4, 2).SetCellValue($"{individuo.Idade + " anos"}");
                        sheet.GetCell(4, 2).CellStyle = cellStyle;

                        sheet.GetCell(6, 2).SetCellValue($"{individuo.DataNascimento.ToString("dd/MM/yyyy")}");
                        sheet.GetCell(6, 2).CellStyle = cellStyle;

                        sheet.GetCell(8, 2).SetCellValue($"{item.UnidadeResultadoEco}");
                        sheet.GetCell(8, 2).CellStyle = cellStyle;
                        sheet.GetCell(7, 7).SetCellValue($"{item.ValorReferenciaResultadoEco}");
                        sheet.GetCell(8, 7).CellStyle = cellStyle;

                        sheet.GetCell(9, 2).SetCellValue($"{agendamento.Profissional.Nome}");
                        sheet.GetCell(9, 2).CellStyle = cellStyle;

                        sheet.GetCell(4, 4).SetCellValue($"{(individuo.Sexo == 0 ? "Masculino" : "Feminino")}");
                        sheet.GetCell(4, 4).CellStyle = cellStyle;

                        sheet.GetCell(9, 6).SetCellValue($"{agendamento.Profissional.Crm}");
                        sheet.GetCell(9, 6).CellStyle = cellStyle;
                        sheet.GetCell(9, 8).SetCellValue($"{agendamento.Procedimento.Descricao}");

                        sheet.GetCell(6, 7).SetCellValue($"{operador}");
                        sheet.GetCell(6, 7).CellStyle = cellStyle;

                        sheet.GetCell(4, 7).SetCellValue($"{item.DataExameEco.ToString("dd/MM/yyyy")}");
                        sheet.GetCell(4, 7).CellStyle = cellStyle;
                        sheet.GetCell(4, 9).SetCellValue($"{item.DataExameEco.ToString("HH:mm:ss")}");
                        sheet.GetCell(4, 9).CellStyle = cellStyle;

                        sheet.GetCell(5, 7).SetCellValue($"{item.TipoExameEco}");
                        sheet.GetCell(5, 7).CellStyle = cellStyle;

                        sheet.GetCell(7, 2).SetCellValue($"{item.ResultadoExameEco}");
                        sheet.GetCell(7, 2).CellStyle = cellStyleResult;

                        workbook.Write(memory);

                    }
                    memory.Position = 0;


                    Workbook workBookXls = new Workbook();
                    var excelPath = $"\\Exames\\Exame_{individuo.Cpf}";


                    var exameExcel = File(memory, GetContentType(filePath), $"ExameF200.xlsx");

                    SaveToFile(memory.ToArray(), excelPath, ".xlsx");

                    workBookXls.LoadFromFile($"{_environment.WebRootPath}\\{excelPath}.xlsx");
                    workBookXls.ConverterSetting.SheetFitToPage = true;
                    Worksheet worksheet = workBookXls.Worksheets[0];
                    var pdfDateDay = DateTime.Now.ToString("dd_MM_yyyy");
                    var pdfDateHour = DateTime.Now.ToString("HH_mm_ss");
                    var excelFullPath = $"{_environment.WebRootPath}\\Exames\\Exames_{individuo.Cpf}_{pdfDateDay}_{pdfDateHour}";

                    worksheet.SaveToPdf(excelFullPath + ".pdf");
                    if (System.IO.File.Exists($"{_environment.WebRootPath}\\{excelPath}.xlsx"))
                    {
                        System.IO.File.Delete($"{_environment.WebRootPath}\\{excelPath}.xlsx");
                    }

                    //$"\\Exames\\Exame_{exames.IndividuoCpf}_{exames.Id}", exames.Formato);

                    string exameUrlPath = "Exames/Exames_" + individuo.Cpf + "_" + pdfDateDay + "_" + pdfDateHour + ".pdf";

                    item.Url = exameUrlPath;
                    item.Formato = ".pdf";

                    _examesF200Application.Update(item.Id, item);

                }
                #endregion
                return Ok("PDF Gerado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest("Erro ao gerar PDF");
            }
            
        }


        private void SaveToFile(byte[] file, string url, string formato)
        {

            if (file == null) return;
            var path = $"{_environment.WebRootPath}";
            var filename = "";
            if (formato == ".pdf")
            {
                filename = path + url + ".pdf";
            }
            else if (formato == ".jpeg")
            {
                filename = path + url + ".jpeg";
            }
            else if (formato == ".jpg")
            {
                filename = path + url + ".jpg";
            }
            else if (formato == ".png")
            {
                filename = path + url + ".png";
            }
            else if (formato == ".mp3")
            {
                filename = path + url + ".mp3";
            }
            else if (formato == ".wav")
            {
                filename = path + url + ".wav";
            }
            else if (formato == ".xlsx")
            {
                filename = path + url + ".xlsx";
            }
            else
            {

            }

            try
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(file);
                    fs.Close();
                }

            }
            catch (Exception e)
            {
                // ignored
            }
        }


    }
}
