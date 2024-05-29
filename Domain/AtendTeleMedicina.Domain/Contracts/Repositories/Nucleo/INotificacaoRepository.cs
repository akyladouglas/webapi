using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface INotificacaoRepository
    {
        int Add(Notificacao notificacao);
        int Update(string id, Notificacao notificacao);
        int Delete(string id);
        Notificacao GetById(string id);
        Task<(IEnumerable<Notificacao>, int)> GetByParam(Notificacao notificacaoFilters, string sort,
          int? skip, int? take);
    }
}
