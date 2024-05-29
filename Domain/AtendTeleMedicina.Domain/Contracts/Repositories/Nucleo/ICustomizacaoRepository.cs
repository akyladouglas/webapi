using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface ICustomizacaoRepository
    {
        int Add(Customizacao notificacao);
        int Update(string id, Customizacao notificacao);
        int Delete(string id);
        Customizacao GetById(string id);
        Task<(IEnumerable<Customizacao>, int)> GetByParam(Customizacao notificacaoFilters, string sort,
          int? skip, int? take);
    }
}
