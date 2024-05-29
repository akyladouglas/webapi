using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class ExamesF200Service : IExamesF200Service
    {
        private readonly IExamesF200Repository _examesF200Repository;

        #region -- Constructor
        public ExamesF200Service(IExamesF200Repository examesF200Repository)
        {
            _examesF200Repository = examesF200Repository;
        }
        #endregion

        public int Add(ExamesF200 obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            return _examesF200Repository.Add(obj);
        }
        public int Update(string id, ExamesF200 obj)
        {
            //obj.Avaliado = DateTime.Now;
            return _examesF200Repository.Update(id, obj);
        }
        //public int Delete(string id)
        //{
        //    return _documentoRepository.Delete(id);
        //}

        public ExamesF200 GetById(string id)
        {
            return _examesF200Repository.GetById(id);
        }
        public Task<(IEnumerable<ExamesF200>, int)> GetByParam(ExamesF200 filters, string sort, int? skip, int? take)
        {
            return _examesF200Repository.GetByParam(filters, sort, skip, take);
        }

        public Task<(IEnumerable<ExamesF200>, int)> GetByCpfIndividuo(string cpf)
        {
            return _examesF200Repository.GetByCpfIndividuo(cpf);
        }
    }
}