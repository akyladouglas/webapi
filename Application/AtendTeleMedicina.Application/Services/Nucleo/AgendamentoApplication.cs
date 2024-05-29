using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Services.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class AgendamentoApplication : IAgendamentoApplication
    {
        private readonly IAgendamentoService _agendamentoService;
        public AgendamentoApplication(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        public string Add(Agendamento obj)
        {
            return _agendamentoService.Add(obj);
        }

        public int Update(string id, Agendamento obj)
        {
            return _agendamentoService.Update(id, obj);
        }

        public int UpdateSinaisVitais(string id, Agendamento obj)
        {
            return _agendamentoService.UpdateSinaisVitais(id, obj);
        }

        public int UpdateConfirmarPresenca(string id, Agendamento obj)
        {
            return _agendamentoService.UpdateConfirmarPresenca(id, obj);
        }
        public int UpdateFinalizarMedico(string id, Agendamento obj)
        {
            return _agendamentoService.UpdateFinalizarMedico(id, obj);
        }

        public int UpdateEmAtendimentoMedico(string id, Agendamento obj)
        {
            return _agendamentoService.UpdateEmAtendimentoMedico(id, obj);
        }

        public int UpdateFinalizarTriagem(string id, Agendamento obj)
        {
            return _agendamentoService.UpdateFinalizarTriagem(id, obj);
        }


        public int Delete(string id)
        {
            return _agendamentoService.Delete(id);
        }

        public Agendamento GetById(string id)
        {
            return _agendamentoService.GetById(id);
        }

        public Task<(IEnumerable<Agendamento>, int)> GetByParam(Agendamento filters, string sort, int? skip = null, int? take = null)
        {
            return _agendamentoService.GetByParam(filters, sort, skip, take);
        }
        public Task<(IEnumerable<Agendamento>, int)> GetAusentes(Agendamento filters, string sort, int? skip, int? take)
        {
            return _agendamentoService.GetAusentes(filters, sort, skip, take);
        }
        public int UpdateRecepcao(string id, Agendamento obj)
        {
            return _agendamentoService.UpdateRecepcao(id, obj);
        }
        public int UpdateTriagem(string id, Agendamento obj)
        {
            return _agendamentoService.UpdateTriagem(id, obj);
        }

        public Agendamento GetByEstabelecimentoByIndividuoId(string id)
        {
            return _agendamentoService.GetByEstabelecimentoByIndividuoId(id);
        }

        public Task<(IEnumerable<Agendamento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            return _agendamentoService.PendentesIntegracao(mes, ano, estabelecimentoId);
        }

        public void ConfirmarIntegracao(int lote, Agendamento obj)
        {
            _agendamentoService.ConfirmarIntegracao(lote, obj);
        }
    }
}
