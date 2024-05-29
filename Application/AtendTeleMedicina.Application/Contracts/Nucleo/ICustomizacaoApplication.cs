using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface ICustomizacaoApplication
    {
        int Add(Customizacao notificacao);
        int Update(string id, Customizacao notificacao);
        int Delete(string id);
        Customizacao GetById(string id);
        Task<(IEnumerable<Customizacao>, int)> GetByParam(Customizacao notificacaoFilters, string sort,
          int? skip, int? take);
    }
}
