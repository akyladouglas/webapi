using Microsoft.AspNetCore.Mvc;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.IO;
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
using System.Diagnostics;
using NPOI.SS.Formula.Functions;
using Twilio.TwiML.Voice;
using AtendTeleMedicina.Application.Services.Nucleo;
using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.Extensions.Configuration;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Exames")]
    [Produces("application/json")]


    public class ExamesController : Controller
    {
        private readonly ITipoExameApplication _tipoExameApplication;
        private readonly IExamesApplication _examesApplication;
        private readonly IUser _user;
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly IIndividuoApplication _individuoApplication;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private static IHostingEnvironment _environment;
        private readonly IConfiguration _config;

        public ExamesController(IUser user, IConfiguration config, ILogger<ExamesController> logger, ITipoExameApplication tipoExameApplication, IExamesApplication examesApplication, IAgendamentoApplication agendamentoApplication, IIndividuoApplication individuoApplication, IMapper mapper, IHostingEnvironment environment)
        {
            _logger = logger;
            _user = user;
            _tipoExameApplication = tipoExameApplication;
            _examesApplication = examesApplication;
            _agendamentoApplication = agendamentoApplication;
            _individuoApplication = individuoApplication;
            _mapper = mapper;
            _environment = environment;
            _config = config;

        }



        //[HttpGet]
        //[Route("{id}")]
        //[Authorize(Roles = "Retaguarda, App, Médico, Recepcionista")]
        //public IActionResult Get(string id)
        //{
        //    try
        //    {
        //        var examesModel = _mapper.Map<ExamesModel>(_examesApplication.GetById(id));

        //        if (examesModel == null) return NotFound();

        //        return Ok(examesModel);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError("DEU ERRO");
        //        return BadRequest(e.Message.ToString());
        //    }
        //}

        [HttpGet]
        [Route("GetExames")]
        [Authorize(Roles = "Retaguarda, Individuo, Recepcionista, MédicoEspecialista")]
        public async Task<IActionResult> Get([FromQuery] ExamesModel exameModel, string sort = "DataDeEnvio DESC",
          int skip = 1, int take = 5)
        {

            try
            {
                var (tipoExame, totalCount) = await _examesApplication.GetByParam(
                     _mapper.Map<Exames>(exameModel),
                     sort,
                     skip,
                     take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<ExamesModel>>(tipoExame),
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


        [HttpPost]
        [Route("ReceberExames")]
        [Authorize(Roles = "Retaguarda, App, Médico, Recepcionista, Individuo, MédicoEspecialista")]
        public object ReceberExames([FromBody] ExamesModel[] examesModel)
        {
            if (examesModel.Length > 0)
            {
                foreach (ExamesModel img in examesModel)
                {
                    try
                    {
                        var exames = _mapper.Map<Exames>(img);
                        if (exames.Url.Length > 50)
                        {
                            byte[] data = System.Convert.FromBase64String(exames.Url);
                            DateTime dataInicial = DateTime.Now;
                            var dataFinal = dataInicial.ToString("dd_MM_yyyy");
                            exames.Id = Guid.NewGuid().ToString();

                            SaveToFile(data, $"\\Exames\\Exame_{exames.IndividuoId}_{exames.Id}", exames.Formato);
                            exames.Url = $"Exames/Exame_{exames.IndividuoId}_{exames.Id}{exames.Formato}";

                            _examesApplication.Add(exames);
                        }



                    }
                    catch (Exception e)
                    {
                        var erro = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                        return BadRequest(erro);
                    }

                }
                return Created("", "Exames Cadastrados com sucesso.");
            }
            else
            {
                return BadRequest("ERRO");
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


        [HttpPut]
        [Route("AvaliarExames")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, MédicoEspecialista")]
        public IActionResult Put([FromBody] ExamesModel examesModel)
        {
            try
            {
                string id = examesModel.Id;
                if (examesModel == null) return BadRequest("Informe o registro a ser alterado");

                //var agendamentoExists = _examesApplication.GetById(id);
                //if (agendamentoExists == null)
                //{
                //    examesModel.AddNotification("Exames.Id", "Registro não encontrado");
                //    return NotFound();
                //}
                //DateTime.Parse(examesModel.Avaliado).ToString("dd-MM-yyyy");
                _examesApplication.Update(id, _mapper.Map<Exames>(examesModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"ExamesPut: {examesModel}");
                return BadRequest(e.Message.ToString());
            }
        }


        [HttpGet]
        [Route("GetTipoExames")]
        [Authorize(Roles = "Retaguarda, Individuo, Recepcionista, MédicoEspecialista")]
        public async Task<IActionResult> Get([FromQuery] TipoExameModel tipoExameModel, string param, string sort = "a.Dia DESC, a.Hora DESC",
          int skip = 1, int take = 10)
        {

            try
            {
                var (tipoExame, totalCount) = await _tipoExameApplication.GetByParam(
                     _mapper.Map<TipoExame>(tipoExameModel),
                     sort,
                     skip,
                     take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<TipoExameModel>>(tipoExame),
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
                _logger.LogError($"AgendamentoGet: {tipoExameModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetByIdTipoExames/{id}")]
        [Authorize(Roles = "Retaguarda, Individuo, Recepcionista, MédicoEspecialista")]
        public IActionResult Get(string id)
        {
            try
            {
                var tipoExamesModel = _mapper.Map<TipoExameModel>(_tipoExameApplication.GetById(id));

                if (tipoExamesModel == null) return NotFound();

                return Ok(tipoExamesModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"DocumentoGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PARA FUNCIONAR LOCAL SEGUIR INSTRUCOES
        // INSTALE O PYTHON
        // INSTALE A LIB WGET: PIP INSTALL WGET
        // BAIXE O SCRIPT
        // COLOQUE O CAMINHO DO SCRIPT NO CONTROLLER
        [HttpGet]
        [Route("MedicoesEko")]
        [Authorize(Roles = "Retaguarda, App, Médico, Recepcionista, Individuo, MédicoEspecialista")]
        public async Task<IActionResult> GetMedicoesEko([FromQuery] string IndividuoId, string individuoCpf,
        string sort = "DataDeEnvio DESC",
        int skip = 1,
        int take = 10)
        {

            try
            {


                
                //Get FilePath
                var filePath = _config["AppSettings:FilePath"];


                //GET INDIVIDUO
                var individuoModel = _mapper.Map<IndividuoModel>(_individuoApplication.GetById(IndividuoId));

                if (individuoModel == null) return NotFound("Individuo não encontrado.");
                 
                if (individuoCpf == null || individuoCpf == "") return BadRequest("Necessário informar cpf do indivíduo.");


                Process process = new Process();
                //process.StartInfo.FileName = "C:\\Users\\ytalo\\Documents\\project\\Python\\scripts\\estetoscopio\\dist\\listarMedicoes.exe";
                //process.StartInfo.FileName = "D:\\ProjetosNovetech\\Meeds\\CondeIntegrador\\api\\DistributedInterfaces\\AtendTeleMedicina.DistributedInterfaces.Services\\wwwroot\\ExamesEstetoscopio\\baixar.exe";
                process.StartInfo.FileName = filePath + "\\ExamesEstetoscopio\\baixar.exe";

                // process.StartInfo.Arguments = string.Format("{0} {1}", @"C:\\inetpub\\wwwroot\\Meeds\\campinas\\homologacao-api\\wwwroot\\ExamesEstetoscopio\\listarMedicoes.py", individuoNome);
                process.StartInfo.Arguments = string.Format("{0} {1} ", individuoCpf.Replace(".", "").Replace("-", ""), IndividuoId);

                //process.StartInfo.Arguments = string.Format("{0} {1}", @"C:\\Users\\ytalo\\Documents\\project\\campinasv2\\develop-pec\\DistributedInterfaces\\AtendTeleMedicina.DistributedInterfaces.Services\\wwwroot\\ExamesEstetoscopio\\listarMedicoes.py", individuoNome);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true; //Pega saída de erros
                process.StartInfo.RedirectStandardOutput = true; //Pega a saída        
                //process.Start();

                //process.BeginOutputReadLine();
                //StreamReader reader = process.StandardOutput;
                //string output = reader.ReadToEnd();
                var output = "";
                process.Start();
                while (!process.HasExited)
                {
                    // do something with the outputLine
                }
                output = process.StandardOutput.ReadLine();


                process.Close();

                //return Ok(output);
                //for (var i = 0; i < UUID_gravacoes.Length; i++)
                //{

                //    if (string.IsNullOrEmpty(UUID_gravacoes[i])) break;

                //    ExamesModel exameModel = new ExamesModel
                //    {
                //        Nome = "wav_" + UUID_gravacoes[i] 
                //    };

                //    var (tipoExame, totalCount) = await _examesApplication.GetByParam(
                //                    _mapper.Map<Exames>(exameModel),
                //                    sort,
                //                    skip,
                //                    take).ConfigureAwait(false);

                //    if (tipoExame == null)
                //    {
                //        exameModel.IndividuoId = individuoModel.Id;
                //        exameModel.IndividuoCpf = individuoModel.Cpf;
                //        exameModel.TipoExame = new TipoExameModel
                //        {
                //            Id = "13928"
                //        };

                //        exameModel.Formato = ".wav";
                //        //examemodel.nome = examemodel.id;
                //        exameModel.Url = $"ExamesEstetoscopio/{exameModel.IndividuoCpf}/{UUID_gravacoes[i]}{exameModel.Formato}";

                //        //salvando no servidor

                //        Process processbaixar = new Process();
                //        //processbaixar.StartInfo.FileName = "C:\\Users\\ytalo\\Documents\\project\\Python\\scripts\\estetoscopio\\dist\\baixar.exe";
                //        processbaixar.StartInfo.FileName = filePath + "\\ExamesEstetoscopio\\baixar.exe";
                //        processbaixar.StartInfo.Arguments = string.Format(" {0} {1}", UUID_gravacoes[i], exameModel.IndividuoCpf);
                //        //processbaixar.startinfo.arguments = string.format("{0} {1} {2}", @"c:\\users\\ytalo\\documents\\project\\campinasv2\\develop-pec\\distributedinterfaces\\atendtelemedicina.distributedinterfaces.services\\wwwroot\\examesestetoscopio\\baixar.py", uuid_gravacoes[i], examemodel.individuocpf);

                //        //processbaixar.startinfo.arguments = @"@""c:\\users\\ytalo\\documents\\project\\campinasv2\\develop-pec\\distributedinterfaces\\atendtelemedicina.distributedinterfaces.services\\wwwroot\\examesestetoscopio\\baixar.py";
                //        processbaixar.StartInfo.UseShellExecute = false;
                //        processbaixar.StartInfo.RedirectStandardError = true; //pega saída de erros
                //        processbaixar.StartInfo.RedirectStandardOutput = true; //pega a saída        
                //        processbaixar.Start();
                //        //processbaixar.BeginOutputReadLine();
                //        //StreamReader readerbaixar = processbaixar.StandardOutput;
                //        //string outputbaixar = readerbaixar.ReadToEnd();
                //        //processbaixar.WaitForExit();
                //        var outputbaixar = "";
                        
                //        while (!processbaixar.HasExited)
                //        {
                //            // do something with the outputLine
                //        }
                //        outputbaixar = processbaixar.StandardOutput.ReadLine();
                //        processbaixar.Close();

                //        //insert na tabela o audio
                //        exameModel.Id = Guid.NewGuid().ToString();
                //        var exame = _mapper.Map<Exames>(exameModel);

                //        //Find Count By Audio
                //        var totalAudio = _examesApplication.Count(_mapper.Map<Exames>(exameModel));
                //        if (totalAudio <= 0)
                //        {
                //            _examesApplication.Add(exame);
                //        }

                //        //insert na tabela o pdf
                //        exameModel.Id = Guid.NewGuid().ToString();
                //        exameModel.Nome = "pdf_" + UUID_gravacoes[i];
                //        exameModel.Formato = ".pdf";
                //        exameModel.Url = $"ExamesEstetoscopio/{exameModel.IndividuoCpf}/{UUID_gravacoes[i]}{exameModel.Formato}";
                //        var examepdf = _mapper.Map<Exames>(exameModel);


                //        /*Verificar se o file já existe na Pasta:
                //        string[] dir = Directory.GetDirectories(filePath + $"\\ExamesEstetoscopio\\", $"{exameModel.IndividuoId}", SearchOption.TopDirectoryOnly);
                //        if (dir.Length > 0)
                //        {
                //            string[] fileWav = Directory.GetFiles(filePath + $"\\ExamesEstetoscopio\\{exameModel.IndividuoCpf}\\", $"{exameModel.IndividuoId}{exameModel.Formato}", SearchOption.TopDirectoryOnly);
                //            if (fileWav.Length > 0)
                //            {
                //                break;
                //            }

                //        */


                //            //Find Count By Pdf Name
                //            var totalPdf = _examesApplication.Count(examepdf);

                //        if (totalPdf <= 0)
                //        {
                //            _examesApplication.Add(examepdf);
                //        }

                //    }
                //    else
                //    {
                //    }
                //};


                ExamesModel obj = new ExamesModel
                {
                    IndividuoId = IndividuoId,
                    TipoExameId = "13928",
                    Formato = ".pdf"
                };
                var (exames, totalCountExames) = await _examesApplication.GetByParam(
                                   _mapper.Map<Exames>(obj),
                                   sort,
                                   skip,
                                   take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCountExames / take);

                if (totalCountExames == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<Exames>>(exames),
                    sort,
                    skip,
                    take,
                    totalCountExames,
                    totalPages
                };

                return Ok(response);


            }
            catch (Exception e)
            {
                _logger.LogError($"MedicoesEko: {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        //DELETE api/Exames/{id}
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public async Task<IActionResult> Excluir(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("O id do exame não foi informado para exclusão do exame!");
            Exames existExam = new Exames();
            try
            {
                if (existExam == null) return BadRequest("O exame não foi encontrado!");

                await _examesApplication.Delete(id).ConfigureAwait(true);
                return Ok("O exame foi excluido com sucesso!");
            }
            catch (Exception e)
            {
                _logger.LogError($"Código para exclusão: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        //[HttpGet]
        //[Route("BaixarMedicoesEko")]
        //[AllowAnonymous]
        //public static void BaixarMedicoesEko([FromQuery] string url)
        //{

        //    try
        //    {
        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(url));
        //        request.Method = WebRequestMethods.Ftp.DownloadFile;
        //        request.UseBinary = true;
        //        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        //        {
        //            using (System.IO.Stream rs = response.GetResponseStream())
        //            {
        //                using (FileStream ws = new FileStream(url, FileMode.Create))
        //                {
        //                    byte[] buffer = new byte[2048];
        //                    int bytesRead = rs.Read(buffer, 0, buffer.Length);
        //                    while (bytesRead > 0)
        //                    {
        //                        ws.Write(buffer, 0, bytesRead);
        //                        bytesRead = rs.Read(buffer, 0, buffer.Length);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

    }
}
