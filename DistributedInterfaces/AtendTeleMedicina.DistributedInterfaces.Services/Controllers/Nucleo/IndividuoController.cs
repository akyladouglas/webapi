using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using Microsoft.AspNetCore.Hosting;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using AtendTeleMedicina.DistributedInterfaces.Services.Helpers;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoController : Controller
    {
        private readonly IIndividuoApplication _individuoApplication;
        private readonly IAcompanhamentoApplication _acompanhamentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private static IHostingEnvironment _environment;
        private readonly IConfiguration _config;


        public IndividuoController(IIndividuoApplication individuoApplication,
                                  IAcompanhamentoApplication acompanhamentoApplication,
                                  IMapper mapper, IConfiguration config,
                                  ILogger<IndividuoController> logger, IHostingEnvironment environment)
        {
            _individuoApplication = individuoApplication;
            _acompanhamentoApplication = acompanhamentoApplication;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
            _config = config;

        }

        /// <summary>
        /// Retorna uma lista de Indivíduos
        /// </summary>
        /// <param name="individuoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Indivíduos</returns>
        /// <response code="200">Estabelecimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Indivíduo encontrado</response>
        /// <response code="404">Erro ao procurar Indivíduo</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] IndividuoModel individuoModel, string param, string sort = "NomeCompleto",
          int skip = 1, int take = 100000)
        {


            var filePath = _config["AppSettings:FilePath"];

            var file3 = filePath;

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                individuoModel = JsonConvert.DeserializeObject<IndividuoModel>(decodeString);
            }
            individuoModel?.ValidarGet();
            try
            {
                var (estabelecimentos, totalCount) = await _individuoApplication.GetByParam(
                    _mapper.Map<Individuo>(individuoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<IndividuoModel>>(estabelecimentos),
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
                _logger.LogError($"IndividuoGet: {individuoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        private int Idade(DateTime DataNascimento, DateTime DataAtual)
        {
            return (DataAtual.Year - DataNascimento.Year - 1) +
                   (((DataAtual.Month > DataNascimento.Month) ||
                     ((DataAtual.Month == DataNascimento.Month) && (DataAtual.Day >= DataNascimento.Day))) ? 1 : 0);
        }

        /// <summary>
        /// Retorna um Indivíduo específico.
        /// </summary>
        /// <param name="id"></param>
        [ProducesResponseType(typeof(IndividuoModel), (int)HttpStatusCode.OK)]
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App, Individuo, Médico, MédicoEspecialista")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {

                var individuoModel = _mapper.Map<IndividuoModel>(_individuoApplication.GetById(id));

                if (individuoModel == null) return NotFound();

                Acompanhamento acompanhamento = new Acompanhamento();
                acompanhamento.IndividuoId = individuoModel.Id;

                var (acompanhamentoResult, totalCount) = await _acompanhamentoApplication.GetByParam(acompanhamento, "Data DESC", 1, 10).ConfigureAwait(false);

                //CASO DESCOMENTE SERÁ NECESSARIO TER PELO MENOS UM REGISTRO DE SINTOMAS PARA QUE NÃO OCORRA O ERRO NA TELA
                //if (!acompanhamentoResult.Any()) return BadRequest("Paciente não possui Acompanhamentos");

                if (acompanhamentoResult != null)
                {
                    if (acompanhamentoResult.Any(x => (DateTime.Now - x.Data).TotalHours <= 24))
                        individuoModel.RespondeuSintomasNoPeriodo = true;

                    var acompanhamentoFinalResult = acompanhamentoResult.FirstOrDefault();

                    var idade = Idade((DateTime)individuoModel.DataNascimento, DateTime.Now);
                    individuoModel.CorStatus = 1;
                    individuoModel.CorMensagem = "VerdeBaixoRisco";
                    if (individuoModel.RespondeuComorbidade == true && acompanhamentoFinalResult != null)
                    {

                        if (acompanhamentoFinalResult.DificuldadeRespirar == false
                    && (acompanhamentoFinalResult.Temperatura == true || acompanhamentoFinalResult.Tosse == true || acompanhamentoFinalResult.Coriza == true || acompanhamentoFinalResult.DorCorpo == true || acompanhamentoFinalResult.Fraqueza == true || acompanhamentoFinalResult.DorGarganta == true ||
                        acompanhamentoFinalResult.NauseaVomito == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Diarreia == true || acompanhamentoFinalResult.PerdaOlfatoPaladar == true)
                    && (individuoModel.Transplantado == false && individuoModel.DoencaComprometeImunidade == false
                        && individuoModel.Fumante == false && individuoModel.Hipertenso == false && individuoModel.Asma == false
                        && individuoModel.Diabetes == false && individuoModel.DoencaPulmao == false && individuoModel.DoencaCoracao == false
                        && individuoModel.DoencaRins == false && individuoModel.DoencaFigado == false && individuoModel.DoencaCancer == false))
                        {
                            individuoModel.CorStatus = 2;
                            individuoModel.CorMensagem = "Amarelo";
                        }

                        if (idade >= 60 && acompanhamentoFinalResult.DificuldadeRespirar == false
                    && (acompanhamentoFinalResult.Temperatura == true || acompanhamentoFinalResult.Tosse == true || acompanhamentoFinalResult.Coriza == true || acompanhamentoFinalResult.DorCorpo == true || acompanhamentoFinalResult.Fraqueza == true || acompanhamentoFinalResult.DorGarganta == true ||
                        acompanhamentoFinalResult.NauseaVomito == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Diarreia == true || acompanhamentoFinalResult.PerdaOlfatoPaladar == true))
                        {
                            individuoModel.CorStatus = 3;
                            individuoModel.CorMensagem = "Laranja";
                        }

                        if (acompanhamentoFinalResult.DificuldadeRespirar == false
                    && (acompanhamentoFinalResult.Temperatura == true || acompanhamentoFinalResult.Tosse == true || acompanhamentoFinalResult.Coriza == true || acompanhamentoFinalResult.DorCorpo == true || acompanhamentoFinalResult.Fraqueza == true || acompanhamentoFinalResult.DorGarganta == true ||
                        acompanhamentoFinalResult.NauseaVomito == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Diarreia == true || acompanhamentoFinalResult.PerdaOlfatoPaladar == true)
                    && (individuoModel.Transplantado == true || individuoModel.DoencaComprometeImunidade == true
                        || individuoModel.Fumante == true || individuoModel.Hipertenso == true || individuoModel.Asma == true
                        || individuoModel.Diabetes == true || individuoModel.DoencaPulmao == true || individuoModel.DoencaCoracao == true
                        || individuoModel.DoencaRins == true || individuoModel.DoencaFigado == true || individuoModel.DoencaCancer == true))
                        {
                            individuoModel.CorStatus = 3;
                            individuoModel.CorMensagem = "Laranja";
                        }

                        if (acompanhamentoFinalResult.DificuldadeRespirar == true)
                        {
                            individuoModel.CorStatus = 4;
                            individuoModel.CorMensagem = "VermelhoBaixoRisco";
                        }

                        if (acompanhamentoFinalResult.DificuldadeRespirar == true
                   && (acompanhamentoFinalResult.Temperatura == true || acompanhamentoFinalResult.Tosse == true || acompanhamentoFinalResult.Coriza == true || acompanhamentoFinalResult.DorCorpo == true || acompanhamentoFinalResult.Fraqueza == true || acompanhamentoFinalResult.DorGarganta == true
                       || acompanhamentoFinalResult.NauseaVomito == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Taquicardia == true || acompanhamentoFinalResult.Diarreia == true || acompanhamentoFinalResult.PerdaOlfatoPaladar == true
                       || individuoModel.Transplantado == true || individuoModel.DoencaComprometeImunidade == true
                       || individuoModel.Fumante == true || individuoModel.Hipertenso == true || individuoModel.Asma == true
                       || individuoModel.Diabetes == true || individuoModel.DoencaPulmao == true || individuoModel.DoencaCoracao == true
                       || individuoModel.DoencaRins == true || individuoModel.DoencaFigado == true || individuoModel.DoencaCancer == true))
                        {
                            individuoModel.CorStatus = 4;
                            individuoModel.CorMensagem = "VermelhoAltoRisco";
                        }
                    }
                } else
                {
                    individuoModel.CorStatus = 0;
                    individuoModel.CorMensagem = "Não Informado";
                }

                return Ok(individuoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        //POST api/individuo
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] IndividuoModel individuoModel)

        {

            try
            {
                if (individuoModel == null) return BadRequest("Necessário informar dados do Indivíduo");
                if (string.IsNullOrEmpty(individuoModel.Email)) individuoModel.Email = null;
                individuoModel.ValidarAdicionar();
                if (individuoModel.Invalid)
                {
                    return BadRequest(individuoModel);
                }



                var individuo = _mapper.Map<Individuo>(individuoModel);
                //individuo.UfAbreviado = "PB";
                //individuo.CidadeId = "2507507";
                if (individuo.UfAbreviado == null || individuo.UfAbreviado == "" || individuo.LogradouroCep == null || individuo.LogradouroCep == "")
                {
                    //Endereço não informado.
                    individuo.UfAbreviado = "NI";
                    individuo.CidadeId = "1100014";
                }
                if (individuoModel.Imagem.Length > 0)
                {
                    byte[] data = System.Convert.FromBase64String(individuoModel.Imagem);
                    SaveToFile(data, $"\\Fotos\\Foto_{individuoModel.Cpf}");
                    individuo.Imagem = $"Fotos/Foto_{individuoModel.Cpf}";
                }

                if (!string.IsNullOrEmpty(individuo.TelefoneCelular))
                {
                    bool hasMask = HasPhoneNumberMask(individuo.TelefoneCelular);
                    if (hasMask)
                    {
                        individuo.TelefoneCelular = RemovePhoneNumberMask(individuo.TelefoneCelular);
                    }
                }

                //if (individuoModel.Cns != null)
                //{
                //    var existPacient = _individuoApplication.GetByCns(individuo.Cns);
                //    if (existPacient != null) return BadRequest("Já existe um cadastro com o Cns: " + individuo.Cns);
                //}
                //if (individuoModel.Cpf != null) {
                //    var existPacient = _individuoApplication.GetByCpf(individuo.Cpf);
                //    if (existPacient != null) return BadRequest("Já existe um cadastro com o CPF: " + individuo.Cpf);
                //}

                _individuoApplication.Add(individuo);
                individuoModel = _mapper.Map<IndividuoModel>(individuo);

                return Created($"api/individuo/{individuoModel.Id}", individuoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoPost: {individuoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/individuo/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public IActionResult Put(string id, [FromBody] IndividuoModel individuoModel)
        {
            try
            {
                if (individuoModel == null) return BadRequest("Informe o registro a ser alterado");

                individuoModel.ValidarEditar();
                if (individuoModel.Invalid)
                {
                    return BadRequest(individuoModel);
                }

                var individuoExists = _individuoApplication.GetById(id);
                if (individuoExists == null)
                {
                    individuoModel.AddNotification("Individuo.Id", "Registro não encontrado");
                    return NotFound();
                }

                if (individuoModel.UfAbreviado == null || individuoModel.UfAbreviado == "" || individuoModel.LogradouroCep == null || individuoModel.LogradouroCep == "")
                {
                    //Endereço não informado.
                    individuoModel.UfAbreviado = "NI";
                    individuoModel.CidadeId = "1100014";
                }

                if (individuoModel.Imagem.Length > 50)
                {
                    try
                    {
                        byte[] data = System.Convert.FromBase64String(individuoModel.Imagem);
                        SaveToFile(data, $"\\Fotos\\Foto_{individuoModel.Cpf}");
                        individuoModel.Imagem = $"Fotos/Foto_{individuoModel.Cpf}";

                    }
                    catch (Exception e)
                    {
                        var erro = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                        return BadRequest(erro);
                    }
                }

                _individuoApplication.Update(id, _mapper.Map<Individuo>(individuoModel));

                return Ok(individuoModel);

            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoPut: {individuoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //POST api/individuo/{method}/{cpfOrCns}
        [HttpPost]
        [Route("{method}/{cpfOrCns}")]
        [AllowAnonymous]
        public IActionResult RequestCode(string method, string cpfOrCns)
        {

            if (cpfOrCns == null) return BadRequest("Cpf ou Cns não informado para o envio do código.");
            if (method == null) return BadRequest("Método não informado para o envio do código.");

            bool hasCpf = cpfOrCns.Length == 11;
            bool hasCns = cpfOrCns.Length == 15;

            Individuo existPacient = new Individuo();
            try
            {
                if (hasCpf)
                {
                    string cpf = cpfOrCns.Replace(".", "").Replace("-", "");
                    existPacient = _individuoApplication.GetByCpf(cpf);
                }
                else if (hasCns)
                {
                    string cns = cpfOrCns.Replace(".", "").Replace("-", "");
                    existPacient = _individuoApplication.GetByCns(cns);
                }
                else return BadRequest("Ocorreu um erro durante o processo de verificação se é CPF ou CNS");

                if (existPacient == null) return BadRequest("O paciente não foi encontrado.");
                if (existPacient.Ativo == false) return BadRequest("O paciente está inativo, entre em contato com o suporte.");
                existPacient.CodigoAutenticacao = _individuoApplication.UpdateCodigoAutenticacao(existPacient.Id);

                SendCode sendCode = new SendCode(_config);
                string messageReturn = sendCode.SendCodeSmsOrEmail(method, existPacient.Email, existPacient.TelefoneCelular, existPacient.CodigoAutenticacao);

                return Ok($"{messageReturn}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Código para exclusão: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //DELETE api/individuo/{code}/{cpfOrCns}
        [HttpDelete]
        [Route("{code}/{cpfOrCns}")]
        [AllowAnonymous]
        public async Task<IActionResult> Excluir(string code, string cpfOrCns)
        {
            if (string.IsNullOrEmpty(cpfOrCns)) return BadRequest("Cpf ou Cns não informado para inativação do paciente.");
            if (string.IsNullOrEmpty(code)) return BadRequest("Cpf ou Cns não informado para inativação do paciente.");

            bool hasCpf = cpfOrCns.Length == 11;
            bool hasCns = cpfOrCns.Length == 15;

            Individuo existPacient = new Individuo();
            try
            {
                if (hasCpf)
                {
                    string cpf = cpfOrCns.Replace(".", "").Replace("-", "");
                    existPacient = _individuoApplication.GetByCpf(cpf);
                    if (existPacient == null) return BadRequest("Paciente com o cpf informado não foi encontrado!");
                }

                if (hasCns)
                {
                    string cns = cpfOrCns.Replace(".", "").Replace("-", "");
                    existPacient = _individuoApplication.GetByCns(cns);
                    if (existPacient == null) return BadRequest("Paciente com o cns informado não foi encontrado!");
                }

                if (existPacient.Ativo == false) return BadRequest("O paciente informado já está inativado!");
                if (existPacient.CodigoAutenticacao != code) return BadRequest("O código informado não é o que foi enviado!");

                await _individuoApplication.Delete(existPacient.Id).ConfigureAwait(true);
                return Ok("O paciente foi inativado com sucesso!");
            }
            catch (Exception e)
            {
                _logger.LogError($"Código para exclusão: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Excluir(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest($"Dados incompletos");
            try
            {
                _individuoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpGet("ExisteCadastroCnsCpf")]
        [AllowAnonymous]
        public ActionResult<bool> ExisteCadastroCnsCpf([FromQuery] string cpf, [FromQuery] string cns)
        {
            try
            {
                bool existeCadastroCns = false;
                bool existeCadastroCpf = false;

                if (cns != null)
                {
                    if (cns.Length == 15)
                    {
                        var objPaciente = _individuoApplication.GetByCns(cns);
                        if (objPaciente != null) existeCadastroCns = true;
                        else existeCadastroCns = false;
                    }
                }

                if (cpf != null) {
                    if (cpf.Length == 11)
                    {
                        var existePacienteCPF = _individuoApplication.GetByCpf(cpf);
                        if (existePacienteCPF != null) existeCadastroCpf = true;
                        else existeCadastroCpf = false;
                    }
                }
                

                if (existeCadastroCns == true && existeCadastroCpf == true)
                {
                    return Ok(true);
                }
                else if (existeCadastroCns == false && existeCadastroCpf == true)
                {
                    return Ok(true);
                }
                else if (existeCadastroCns == true && existeCadastroCpf == false)
                {
                    return Ok(true);
                }
                else {
                    return Ok(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro: {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        private void SaveToFile(byte[] imageBytes, string url)
        {

            if (imageBytes == null)
                return;
            var path = $"{_environment.WebRootPath}";
            var filename = path + url + ".jpg";

            try
            {
                var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                var img = Image.FromStream(ms);
                img.Save(filename, ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private static bool HasPhoneNumberMask(string phoneNumber)
        {
            // Defina o padrão da máscara de telefone com expressão regular
            string maskPattern = @"\(\d{3}\) \d{3}-\d{4}";

            // Use a função Regex.IsMatch para verificar se a string corresponde ao padrão da máscara
            return Regex.IsMatch(phoneNumber, maskPattern);
        }

        private static string RemovePhoneNumberMask(string phoneNumber)
        {
            // Defina o padrão da máscara de telefone com expressão regular
            string maskPattern = @"[^\d]";

            // Use a função Regex.Replace para remover os caracteres de máscara
            return Regex.Replace(phoneNumber, maskPattern, "");
        }

        //[HttpPut("{individuoId}/Comorbidades")]
        //[Authorize(Roles = "Retaguarda, Individuo, Triagem")]
        //public IActionResult Put(string individuoId, Individuo individuoComorbidade)
        //{

        //    //return null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(individuoId)) return BadRequest();
        //        if (individuoComorbidade == null) return BadRequest();

        //        _individuoApplication.AtualizarComorbidadeTriagem(individuoId, individuoComorbidade);

        //        return Ok(individuoComorbidade);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Erro ao atualizar Comorbidade: {DateTime.Now}, {individuoId}, {e}");
        //        return BadRequest(e.Message.ToString());
        //    }
        //}

    }
}
