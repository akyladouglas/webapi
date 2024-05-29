using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Chat.V2.Service;
using Twilio.Rest.Conversations.V1;
using Twilio.Rest.Video.V1;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class TeleConsultaController : Controller
    {
        private readonly IUser _user;
        private readonly IConfiguration _config;

        public TeleConsultaController(IUser user, IConfiguration config)
        {
            _user = user;
            _config = config;

            TwilioClient.Init(_config["Twilio:AccountSid"], _config["Twilio:AuthToken"]);
        }


        /// <summary>
        /// Retornar salas de atendimento abertas.
        /// </summary>
        /// <returns>Retorna uma lista de Salas de atendimento</returns>
        /// <response code="200">Sala(s) de atendimento encontrado(s)</response>
        /// <response code="404">Erro ao procurar Sala(s) de atendimento</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, Individuo, MédicoEspecialista, Medico")]
        public IActionResult Get()
        {
            try
            {
                var AccountSid = _config["Twilio:AccountSid"];
                var rooms = RoomResource.Read();

                return Ok(rooms);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retornar sala de atendimento aberta.
        /// </summary>
        /// <param name="id">Id do agendamento</param>
        /// <returns>Retorna uma de Sala de atendimento</returns>
        /// <response code="200">Sala de atendimento e o token para conexão com Twilio</response>
        /// <response code="400">Sala de atendimento não encontrado</response>
        /// <response code="404">Erro ao procurar Sala(s) de atendimento</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo, MédicoEspecialista, Medico")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var videoRoom = RoomResource.Fetch(
                    pathSid: id
                );

                var chatRoom = ChannelResource.Fetch(
                   pathServiceSid: _config["Twilio:ServiceChatSid"],
                   pathSid: id
               );

                string identity = _user.GetUserId();

                var videoGrant = new VideoGrant();
                videoGrant.Room = id;

                var chatGrant = new ChatGrant
                { ServiceSid = _config["Twilio:ServiceChatSid"], };

                var grants = new HashSet<IGrant> { { videoGrant }, { chatGrant } };

                var token = new Token(
                    _config["Twilio:AccountSid"],
                    _config["Twilio:ApiKey"],
                    _config["Twilio:ApiSecret"],
                    identity: identity,
                    grants: grants
                ).ToJwt();

                return Ok(new { token, videoRoom, chatRoom });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retornar o token do participante do chat
        /// </summary>
        /// <param name="participantId">Id do participante</param>
        /// <returns>Retorna um token JWT</returns>
        /// <response code="200">Sala de atendimento e o token para conexão com Twilio</response>
        /// <response code="400">Sala de atendimento não encontrado</response>
        /// <response code="404">Erro ao procurar Sala(s) de atendimento</response>
        [HttpGet("GetUserChatToken/{participantId}")]
        [Authorize(Roles = "Retaguarda, Individuo, MédicoEspecialista, Medico")]
        public async Task<IActionResult> GetUserChatToken(string participantId)
        {
            try
            {
                var chatGrant = new ChatGrant
                {
                    ServiceSid = _config["Twilio:ServiceChatSid"]
                };

                var grants = new HashSet<IGrant> { { chatGrant } };

                var token = new Token(
                _config["Twilio:AccountSid"],
                _config["Twilio:ApiKey"],
                _config["Twilio:ApiSecret"],
                identity: participantId,
                grants: grants
                ).ToJwt();

                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Cria uma sala de atendimento.
        /// </summary>
        /// <param name="agendamentoId">Id do agendamento</param>
        /// <returns>Retorna a de Sala de atendimento criada</returns>
        /// <response code="200">Sala de atendimento</response>
        /// <response code="400">Sala de atendimento já foi criada</response>
        /// <response code="404">Erro ao criar Sala de atendimento</response>
        [HttpPost]
        [Authorize(Roles = "Retaguarda, Individuo, MédicoEspecialista, Medico")]
        public async Task<IActionResult> Post(string agendamentoId)
        {
            try
            {
                var roomChat = checkAndCreateRoomChat(agendamentoId, _config["Twilio:ServiceChatSid"]);
                var roomVideo = checkAndCreateRoomVideo(agendamentoId);

                if (roomVideo == null)
                {
                    throw new Exception("Sala de atendimento ja criada.");
                }

                return Ok(new { roomVideo, roomChat });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public static RoomResource checkAndCreateRoomVideo(string id)
        {

            try
            {
                var room = RoomResource.Fetch(
                    pathSid: id
                );

                return null;
            }
            catch (Exception e)
            {

                try
                {
                    var room = RoomResource.Create(
                      type: RoomResource.RoomTypeEnum.PeerToPeer,
                      uniqueName: id
                    );

                    return room;
                }
                catch (Exception error)
                {
                    throw error;
                }

            }
        }

        public static ChannelResource checkAndCreateRoomChat(string id, string twilioServiceChatSid)
        {

            try
            {
                var room = ChannelResource.Fetch(
                    pathServiceSid: twilioServiceChatSid,
                    pathSid: id
                );

                return room;
            }
            catch (Exception e)
            {
                var room = ChannelResource.Create(
                    pathServiceSid: twilioServiceChatSid,
                    friendlyName: id,
                    uniqueName: id
                );
                return room;
            }
        }
    }
}
