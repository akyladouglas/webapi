using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class AgendamentoParticipanteService : IAgendamentoParticipanteService
    {
        private readonly IAgendamentoParticipanteRepository _agendamentoParticipanteRepository;

        #region -- Constructor
        public AgendamentoParticipanteService(IAgendamentoParticipanteRepository agendamentoParticipanteRepository)
        {
            _agendamentoParticipanteRepository = agendamentoParticipanteRepository;
        }
        #endregion
        public string Post(Agendamento obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataAlteracao = DateTime.Now;
            obj.DataCadastro = DateTime.Now;
            return _agendamentoParticipanteRepository.Post(obj);
        }
        public int Update(string id, AgendamentoParticipante obj)
        {
            return _agendamentoParticipanteRepository.Update(id, obj);
        }
        public AgendamentoParticipante GetById(string id)
        {
            return _agendamentoParticipanteRepository.GetById(id);
        }
        public Task<(IEnumerable<AgendamentoParticipante>, int)> GetByParam(AgendamentoParticipante filters, string sort, int? skip, int? take)
        {
            return _agendamentoParticipanteRepository.GetByParam(filters, sort, skip, take);
        }
    }
}
