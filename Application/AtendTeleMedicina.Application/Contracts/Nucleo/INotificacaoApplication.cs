using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface INotificacaoApplication
    {
        int Add(Notificacao notificacao);
        int Update(string id, Notificacao notificacao);
        int Delete(string id);
        Notificacao GetById(string id);
        Task<(IEnumerable<Notificacao>, int)> GetByParam(Notificacao notificacaoFilters, string sort,
          int? skip, int? take);
    }
}
