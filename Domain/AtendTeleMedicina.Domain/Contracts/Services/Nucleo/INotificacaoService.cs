using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface INotificacaoService
    {
        int Add(Notificacao obj);
        int Update(string id, Notificacao obj);
        int Delete(string id);
        Notificacao GetById(string id);
        Task<(IEnumerable<Notificacao>, int)> GetByParam(Notificacao filters, string sort, int? skip, int? take);
    }
}
