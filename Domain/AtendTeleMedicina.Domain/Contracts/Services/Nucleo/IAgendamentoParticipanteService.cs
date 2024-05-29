using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IAgendamentoParticipanteService
    {
        string Post(Agendamento obj);
        int Update(string id, AgendamentoParticipante obj);
        AgendamentoParticipante GetById(string id);
        Task<(IEnumerable<AgendamentoParticipante>, int)> GetByParam(AgendamentoParticipante filters, string sort, int? skip, int? take);
    }
}
