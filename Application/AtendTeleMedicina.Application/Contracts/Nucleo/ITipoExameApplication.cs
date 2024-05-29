using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface ITipoExameApplication
    {
        Task<(IEnumerable<TipoExame>, int)> GetByParam(TipoExame filters, string sort, int? skip, int? take);
        //int Update(string id, Documento obj);
        //int Delete(string id);
        TipoExame GetById(string id);
        //Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take);
    }
}
