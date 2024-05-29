using AtendTeleMedicina.Domain.Entities.Integracao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Integracao
{
    public interface IExamesAfinion2Service
    {
        //int Add(ExamesAfinion2 obj);
        //int Update(string id, ExamesAfinion2 obj);
        //int Delete(string id);
        //ExamesAfinion2 GetById(string id);
        Task<(IEnumerable<ExamesAfinion2>, int)> GetByParam(ExamesAfinion2 filters, string sort, int? skip, int? take);
        Task<(IEnumerable<ExamesAfinion2>, int)> GetByCpfIndividuo(string cpf);
    }
}