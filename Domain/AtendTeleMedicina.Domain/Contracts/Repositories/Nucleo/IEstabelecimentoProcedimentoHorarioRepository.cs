using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IEstabelecimentoProcedimentoHorarioRepository
    {
        int Update(string id, EstabelecimentoProcedimentoHorario obj);
        Task<(IEnumerable<EstabelecimentoProcedimentoHorario>, int)> GetByParam(EstabelecimentoProcedimentoHorario filters, AppParams appParams, string sort, int? skip, int? take);
        void Add(IEnumerable<EstabelecimentoProcedimentoHorario> list);
    }
}