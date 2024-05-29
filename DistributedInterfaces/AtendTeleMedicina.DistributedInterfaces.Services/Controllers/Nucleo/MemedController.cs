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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]


    public class MemedController : Controller
    {
        private readonly IProfissionalApplication _profissionalApplication;
        private readonly IUser _user;
        private readonly IDocumentoApplication _documentoApplication;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public MemedController(IUser user,
            IProfissionalApplication profissionalApplication,
            IDocumentoApplication documentoApplication,
            IMapper mapper,
            IConfiguration config
            )
        {
            _user = user;
            _profissionalApplication = profissionalApplication;
            _documentoApplication = documentoApplication;
            _mapper = mapper;
            _config = config;
        }

        string URLMemed = $"https://api.memed.com.br/v1/sinapse-prescricao/usuarios";
        string urlCadastroProfissional = "https://api.memed.com.br/v1/sinapse-prescricao/usuarios";
        string urlDocumentos = "https://api.memed.com.br/v1/prescricoes/";
        string urlRecuperarMedico = "https://api.memed.com.br/v1/sinapse-prescricao/usuarios/";
        string urlToken = "https://api.memed.com.br/v1/sinapse-prescricao/usuarios/";

        //public string Get(string uri)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    using (Stream stream = response.GetResponseStream())
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        return reader.ReadToEnd();
        //    }
        //}

        [HttpGet]
        [Authorize(Roles = "Retaguarda, App, Médico, MédicoEspecialista, MédicoAD")]
        public String GetInfo()
        {
            var medicoId = _user.GetUserId();
            var finalUrl = $"{URLMemed}/{medicoId}/?api-key={_config["Memed:ApiKey"]}&secret-key={_config["Memed:SecretKey"]}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(finalUrl);
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/vnd.api+json";
            request.ContentType = "application/json; charset=utf-8";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        [HttpPost]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public string AddProfissional([FromBody] ProfissionalMemedModel profissionalModel)
        {
            try
            {
                var finalUrl = $"{urlCadastroProfissional}?api-key={_config["Memed:ApiKey"]}&secret-key={_config["Memed:SecretKey"]}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(finalUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.Accept = "application/json";
                request.ContentType = "application/json; charset=utf-8";

                var bodyObject = new
                {
                    data = new
                    {
                        type = "usuarios",
                        attributes = new
                        {
                            external_id = profissionalModel.Id,
                            nome = profissionalModel.Nome,
                            sobrenome = profissionalModel.Sobrenome,
                            data_nascimento = profissionalModel.DataNascimento.ToString("dd/MM/yyyy"),
                            board = new
                            {
                                board_code = profissionalModel.Conselho,
                                board_number = profissionalModel.Crm,
                                board_state = profissionalModel.CrmUF
                            }
                        }
                    }
                };

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var dataNascimento = profissionalModel.DataNascimento.ToString("dd/MM/yyyy");
                    string json = JsonConvert.SerializeObject(bodyObject);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        [Route("enfermeiro")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public string AddEnfermeiro([FromBody] ProfissionalMemedModel profissionalModel)
        {
            try
            {
                var finalUrl = $"{urlCadastroProfissional}?api-key={_config["Memed:ApiKey"]}&secret-key={_config["Memed:SecretKey"]}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(finalUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.Accept = "application/json";
                request.ContentType = "application/json; charset=utf-8";

                var bodyObject = new
                {
                    data = new
                    {
                        type = "usuarios",
                        attributes = new
                        {
                            external_id = profissionalModel.Id,
                            nome = profissionalModel.Nome,
                            sobrenome = profissionalModel.Sobrenome,
                            data_nascimento = profissionalModel.DataNascimento.ToString("dd/MM/yyyy"),
                            board = new
                            {
                                board_code = "COREN",
                                board_number = profissionalModel.Crm,
                                board_state = profissionalModel.CrmUF
                            }
                        }
                    }
                };

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var dataNascimento = profissionalModel.DataNascimento.ToString("dd/MM/yyyy");
                    string json = JsonConvert.SerializeObject(bodyObject);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }

                //return Created($"api/atendimento/{atendimentoModel.Id}", atendimentoModel);
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        //--------------------------------------------------------------------------------------------
        [HttpPost]
        [Route("RetornoDocumentos/{id}/{token}")]
        [Authorize(Roles = "Retaguarda, App, Médico, MédicoEspecialista, MédicoAD")]
        public object RetornoDocumentos([FromBody] DocumentoModel documentoModel, string id, string token)
        {

            try
            {
                if (documentoModel == null) return null;

                var urlDocumentosId = urlDocumentos + id + "/url-document/full" + "?token=" + token;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlDocumentosId);
                request.Method = WebRequestMethods.Http.Get;


                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseServer = reader.ReadToEnd();
                    dynamic data = JObject.Parse(responseServer);
                    string link = data.data[0].attributes.link;

                    documentoModel.Url = link;

                    var documento = _mapper.Map<Documento>(documentoModel);
                    _documentoApplication.Add(documento);


                    return data;
                }




                return null;

            }
            catch (Exception e)
            {
                return e.Message.ToString();

            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        [Route("GetToken")]
        [Authorize(Roles = "Retaguarda, App, Médico, MédicoEspecialista, MédicoAD")]
        public object RecoverTokenMedico()
        {
            try
            {

                var medicoId = _user.GetUserId();
                var urlMedico = urlRecuperarMedico + medicoId;
                var keySecret = $"?api-key={_config["Memed:ApiKey"]}&secret-key={_config["Memed:SecretKey"]}";
                var finalUrl = urlMedico + keySecret;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(finalUrl);
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = "application/vnd.api+json";
                request.ContentType = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseServer = reader.ReadToEnd();
                    dynamic data = JObject.Parse(responseServer);
                    return data;
                }



                return null;

            }
            catch (Exception e)
            {
                return e.Message.ToString();

            }
        }

        [HttpPost]
        [Route("ConfigImpressao")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public string ConfigImpressao()
        {

            try
            {
                var teste = RecoverTokenMedico();
                return teste.ToString();
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create();
                //request.Method = WebRequestMethods.Http.Post;
                //request.Accept = "application/vnd.api+json";
                //request.ContentType = "application/json; charset=utf-8";




                //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                //{
                //    string json = "{\"data\":{\"type\":\"string\",\"attributes\":{\"medicos_id\":\0,\"indice\":\0,\"mostrar_label_nome_paciente\":\true,\"mostrar_label_paciente_especial\":\0,\"mostrar_data\":\0,\"mostrar_data_controle_especial\":\0,\"fonte\":\"string\",\"tamanho_fonte\":\0,\"espacamento\":\0,\"mostrar_unidades\":\true,\"mostrar_unidades_especial\":\true,\"separar_por_uso\":\true,\"mostrar_nome_fabricante\":\true,\"separador_uso\":\0,\"separador_medicamento\":\0,\"largura_papel\":\0,\"altura_papel\":\0,\"margem_esquerda\":\0,\"margem_direita\":\0,\"margem_superior\":\0,\"margem_inferior\":\0,\"titulo_fonte\":\"string\",\"titulo_tamanho_fonte\":\0,\"titulo\":\"string\",\"titulo_cor\":\"string\",\"subtitulo_fonte\":\"string\",\"subtitulo_tamanho_fonte\":\0,\"subtitulo\":\"string\",\"subtitulo_cor\":\"string\",\"tamanho_cabecalho\":\0,\"rodape_fonte\":\"string\",\"rodape_tamanho_fonte\":\0,\"rodape\":\"string\",\"rodape_cor\":\"string\",\"tamanho_rodape\":\0,\"modelo_cabecalho_rodape\":\0,\"ativo\":\true,\"imprimir_controle_especial\":\true,\"imprimir_controle_especial_antibioticos\":\true,\"imprimir_controle_especial_c4\":\true,\"imprimir_lme\":\true,\"nome_medico\":\"string\",\"endereco_medico\":\"string\",\"cidade_medico\":\"string\",\"telefone_medico\":\"string\",\"mostrar_cabecalho_rodape_simples\":\0,\"modelo_rodape\":\0,\"width_logo\":\0,\"height_logo\":\0,\"mostrar_cabecalho_rodape_especial\":\0,\"logo_nome\":\"string\",\"logo_src\":\"string\",\"zoom_logo\":\0,\"header_image\":\"string\",\"footer_image\":\"string\",\"number_of_lme_copies\":\0},\"links\":{\"self\":\"string\"},\"id\":\0}}";
                //    streamWriter.Write(json);
                //    streamWriter.Flush();
                //    streamWriter.Close();


                //}



                //using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())

                //using (Stream stream = response.GetResponseStream())
                //using (StreamReader reader = new StreamReader(stream))
                //{
                //    return reader.ReadToEnd();
                //}

                //return Created($"api/atendimento/{atendimentoModel.Id}", atendimentoModel);
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }


    }
}
