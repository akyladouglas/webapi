using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IAcompanhamentoService
    {
        int Add(Acompanhamento obj);
        int Update(string id, Acompanhamento obj);
        int Delete(string id);
        Acompanhamento GetById(string id);
        Task<(IEnumerable<Acompanhamento>, int)> GetByParam(Acompanhamento filters, string sort,
            int? skip, int? take);
        
    }
}