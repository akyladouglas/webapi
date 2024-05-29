using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IAtendimentoService
    {
        int Add(Atendimento obj);
        int AddMedico(Atendimento obj);
        int Update(string id, Atendimento obj);
        int UpdateInativar(string id, Atendimento obj);
        int Delete(string id);
        Task<Atendimento> GetById(string id);
        Atendimento GetAgendamentoById(string id);
        Task<(IEnumerable<Atendimento>, int)> GetByParam(Atendimento filters, string sort, int? skip = null, int? take = null);
        Atendimento GetAtendimentoById(string atendimentoId);
        Task<IEnumerable<DashboardTipoAtendimento>> GetTotalTipoAtendimento(DashboardParams dashParams);
        Atendimento GetMaxAtendimentoByIndividuoId(string id);
        Task<(IEnumerable<Atendimento>, int)> ProcedimentosPendentesIntegracao(int mes, int ano, string estabelecimentoId);
        Task<(IEnumerable<Atendimento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId, bool includeAD);
        void ConfirmarIntegracao(int lote, Atendimento obj);
        void ConfirmarIntegracaoProcedimentos(int lote, Atendimento obj);

    }
}