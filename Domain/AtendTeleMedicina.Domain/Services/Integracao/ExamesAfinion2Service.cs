using AtendTeleMedicina.Domain.Contracts.Repositories.Integracao;
using AtendTeleMedicina.Domain.Contracts.Services.Integracao;
using AtendTeleMedicina.Domain.Entities.Integracao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Integracao
{
    public class ExamesAfinion2Service : IExamesAfinion2Service
    {
        private readonly IExamesAfinion2Repository _examesAfinion2Repository;

        #region -- Constructor
        public ExamesAfinion2Service(IExamesAfinion2Repository examesAfinion2Repository)
        {
            _examesAfinion2Repository = examesAfinion2Repository;
        }
        #endregion

        //public int Add(ExamesAfinion2 obj)
        //{
        //    obj.Id = Guid.NewGuid().ToString();
        //    return _examesAfinion2Repository.Add(obj);
        //}
        
        //public int Update(string id, ExamesAfinion2 obj)
        //{
        //    //obj.Avaliado = DateTime.Now;
        //    return _examesAfinion2Repository.Update(id, obj);
        //}
        
        //public int Delete(string id)
        //{
        //    return _documentoRepository.Delete(id);
        //}

        //public ExamesAfinion2 GetById(string id)
        //{
        //    return _examesAfinion2Repository.GetById(id);
        //}
        public Task<(IEnumerable<ExamesAfinion2>, int)> GetByParam(ExamesAfinion2 filters, string sort, int? skip, int? take)
        {
            return _examesAfinion2Repository.GetByParam(filters, sort, skip, take);
        }

        public Task<(IEnumerable<ExamesAfinion2>, int)> GetByCpfIndividuo(string cpf)
        {
            return _examesAfinion2Repository.GetByCpfIndividuo(cpf);
        }
    }
}