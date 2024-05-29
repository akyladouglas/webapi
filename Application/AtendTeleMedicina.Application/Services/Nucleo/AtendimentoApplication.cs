using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Services.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class AtendimentoApplication : IAtendimentoApplication
    {
        private readonly IAtendimentoService _atendimentoService;
        #region -- Constructor
        public AtendimentoApplication(IAtendimentoService atendimentoService)
        {
            _atendimentoService = atendimentoService;
        }
        #endregion
        public int Add(Atendimento obj)
        {
            return _atendimentoService.Add(obj);
        }
        public int AddMedico(Atendimento obj)
        {
            return _atendimentoService.AddMedico(obj);
        }

        public int Update(string id, Atendimento obj)
        {
            return _atendimentoService.Update(id, obj);
        }
        public int UpdateInativar(string id, Atendimento obj)
        {
            return _atendimentoService.UpdateInativar(id, obj);
        }

        public int Delete(string id)
        {
            return _atendimentoService.Delete(id);
        }

        public async Task<Atendimento> GetById(string id)
        {
            return await _atendimentoService.GetById(id);
        }
        public Atendimento GetAgendamentoById(string id)
        {
            return _atendimentoService.GetAgendamentoById(id);
        }

        public Task<(IEnumerable<Atendimento>, int)> GetByParam(Atendimento filters, string sort, int? skip = null, int? take = null)
        {
            return _atendimentoService.GetByParam(filters, sort, skip, take);
        }

        public Atendimento GetAtendimentoById(string atendimentolId)
        {
          return _atendimentoService.GetAtendimentoById(atendimentolId);
        }
        public Task<IEnumerable<DashboardTipoAtendimento>> GetTotalTipoAtendimento(DashboardParams dashParams)
        {
            return _atendimentoService.GetTotalTipoAtendimento(dashParams);
        }

        public Atendimento GetMaxAtendimentoByIndividuoId(string id)
        {
            return _atendimentoService.GetMaxAtendimentoByIndividuoId(id);
        }

        public async Task<(IEnumerable<Atendimento>, int)> ProcedimentosPendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            return await _atendimentoService.ProcedimentosPendentesIntegracao(mes, ano, estabelecimentoId);
        }

        public async Task<(IEnumerable<Atendimento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId, bool includeAD)
        {
            return await _atendimentoService.PendentesIntegracao(mes, ano, estabelecimentoId, includeAD);
        }

        public void ConfirmarIntegracao(int lote, Atendimento obj)
        {
            _atendimentoService.ConfirmarIntegracao(lote, obj);
        }

        public void ConfirmarIntegracaoProcedimentos(int lote, Atendimento obj)
        {
            _atendimentoService.ConfirmarIntegracaoProcedimentos(lote, obj);
        }
    }
}
