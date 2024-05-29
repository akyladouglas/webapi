using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class AtendimentoService : IAtendimentoService
    {
        private readonly IAtendimentoRepository _atendimentoRepository;

        #region -- Constructor
        public AtendimentoService(IAtendimentoRepository atendimentoRepository)
        {
            _atendimentoRepository = atendimentoRepository;
        }
        #endregion

        public int Add(Atendimento obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataCadastro = DateTime.Now;
            obj.DataAlteracao = DateTime.Now;
            return _atendimentoRepository.Add(obj);
        }
        public int AddMedico(Atendimento obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataCadastro = DateTime.Now;
            obj.DataAlteracao = DateTime.Now;
            return _atendimentoRepository.AddMedico(obj);
        }
        public int Update(string id, Atendimento obj)
        {
            return _atendimentoRepository.Update(id, obj);
        }
        public int UpdateInativar(string id, Atendimento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            return _atendimentoRepository.UpdateInativar(id, obj);
        }
        public int Delete(string id)
        {
            return _atendimentoRepository.Delete(id);
        }

        public async Task<Atendimento> GetById(string id)
        {
            return await _atendimentoRepository.GetById(id);
        }
        public Atendimento GetAgendamentoById(string id)
        {
            return _atendimentoRepository.GetAgendamentoById(id);
        }
        public Task<(IEnumerable<Atendimento>, int)> GetByParam(Atendimento filters, string sort, int? skip = null, int? take = null)
        {
            return _atendimentoRepository.GetByParam(filters, sort, skip, take);
        }

        public Atendimento GetAtendimentoById(string atendimentoId)
        {
          return _atendimentoRepository.GetAtendimentoById(atendimentoId);
        }

        public Task<IEnumerable<DashboardTipoAtendimento>> GetTotalTipoAtendimento(DashboardParams dashParams)
        {
            return _atendimentoRepository.GetTotalTipoAtendimento(dashParams);
        }

        public Atendimento GetMaxAtendimentoByIndividuoId(string id)
        {
            return _atendimentoRepository.GetMaxAtendimentoByIndividuoId(id);
        }
        public async Task<(IEnumerable<Atendimento>, int)> ProcedimentosPendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            return await _atendimentoRepository.ProcedimentosPendentesIntegracao(mes, ano, estabelecimentoId);
        }
        public async Task<(IEnumerable<Atendimento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId, bool includeAD)
        {
            return await _atendimentoRepository.PendentesIntegracao(mes, ano, estabelecimentoId, includeAD);
        }
        public void ConfirmarIntegracao(int lote, Atendimento obj)
        {
            _atendimentoRepository.ConfirmarIntegracao(lote, obj);
        }

        public void ConfirmarIntegracaoProcedimentos(int lote, Atendimento obj)
        {
            _atendimentoRepository.ConfirmarIntegracaoProcedimentos(lote, obj);
        }
    }
}