using AtendTeleMedicina.Application.Contracts.Integracao;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Services.Integracao;
using AtendTeleMedicina.Domain.Entities.Integracao;

namespace AtendTeleMedicina.Application.Services.Integracao
{
    public class ExamesAfinion2Application : IExamesAfinion2Application
    {
        private readonly IExamesAfinion2Service _examesAfinion2Service;
        public ExamesAfinion2Application(IExamesAfinion2Service examesAfinion2Service)
        {
            _examesAfinion2Service = examesAfinion2Service;
        }

        //public int Add(ExamesAfinion2 obj)
        //{
        //    return _examesAfinion2Service.Add(obj);
        //}

        //public int Update(string id, ExamesAfinion2 obj)
        //{
        //    return _examesAfinion2Service.Update(id, obj);
        //}

        //public int Delete(string id)
        //{
        //    return _documentoService.Delete(id);
        //}

        //public ExamesAfinion2 GetById(string id)
        //{
        //    return _examesAfinion2Service.GetById(id);
        //}

        public Task<(IEnumerable<ExamesAfinion2>, int)> GetByParam(ExamesAfinion2 filters, string sort, int? skip, int? take)
        {
            return _examesAfinion2Service.GetByParam(filters, sort, skip, take);
        }

       public Task<(IEnumerable<ExamesAfinion2>, int)> GetByCpfIndividuo(string cpf)
        {
            return _examesAfinion2Service.GetByCpfIndividuo(cpf);
        }
    }
}