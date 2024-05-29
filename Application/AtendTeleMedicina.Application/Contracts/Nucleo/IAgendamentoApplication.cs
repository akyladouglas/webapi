using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IAgendamentoApplication
    {
        string Add(Agendamento obj);
        int Update(string id, Agendamento obj);
        int UpdateSinaisVitais(string id, Agendamento obj);

        int UpdateConfirmarPresenca(string id, Agendamento obj);
        int UpdateFinalizarMedico(string id, Agendamento obj);
        int UpdateEmAtendimentoMedico(string id, Agendamento obj);
        int UpdateFinalizarTriagem(string id, Agendamento obj);
        int Delete(string id);
        Agendamento GetById(string id);
        Task<(IEnumerable<Agendamento>, int)> GetByParam(Agendamento filters, string sort, int? skip = null, int? take = null);
        Task<(IEnumerable<Agendamento>, int)> GetAusentes(Agendamento filters, string sort, int? skip, int? take);
        int UpdateRecepcao(string id, Agendamento obj);
        int UpdateTriagem(string id, Agendamento obj);
        Agendamento GetByEstabelecimentoByIndividuoId(string id);
        Task<(IEnumerable<Agendamento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId);
        void ConfirmarIntegracao(int lote, Agendamento obj);

    }
}
