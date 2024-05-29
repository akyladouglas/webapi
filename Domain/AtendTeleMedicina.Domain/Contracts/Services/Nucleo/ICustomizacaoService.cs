using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface ICustomizacaoService
    {
        int Add(Customizacao obj);
        int Update(string id, Customizacao obj);
        int Delete(string id);
        Customizacao GetById(string id);
        Task<(IEnumerable<Customizacao>, int)> GetByParam(Customizacao filters, string sort, int? skip, int? take);
    }
}
