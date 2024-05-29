using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Services.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class AgendamentoParticipanteApplication : IAgendamentoParticipanteApplication
    {
        private readonly IAgendamentoParticipanteService _agendamentoParticipanteService;
        public AgendamentoParticipanteApplication(IAgendamentoParticipanteService agendamentoParticipanteService)
        {
            _agendamentoParticipanteService = agendamentoParticipanteService;
        }

        public string Post(Agendamento obj)
        {
            return _agendamentoParticipanteService.Post(obj);
        }

        public int Update(string id, AgendamentoParticipante obj)
        {
            return _agendamentoParticipanteService.Update(id, obj);
        }

        public AgendamentoParticipante GetById(string id)
        {
            return _agendamentoParticipanteService.GetById(id);
        }

        public Task<(IEnumerable<AgendamentoParticipante>, int)> GetByParam(AgendamentoParticipante filters, string sort, int? skip, int? take)
        {
            return _agendamentoParticipanteService.GetByParam(filters, sort, skip, take);
        }
    }
}
