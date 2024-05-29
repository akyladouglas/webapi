using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class ExamesService : IExamesService
    {
        private readonly IExamesRepository _examesRepository;

        #region -- Constructor
        public ExamesService(IExamesRepository examesRepository)
        {
            _examesRepository = examesRepository;
        }
        #endregion

        public int Add(Exames obj)
        {
            return _examesRepository.Add(obj);
        }
        public int Update(string id, Exames obj)
        {
            obj.Avaliado = DateTime.Now;
            return _examesRepository.Update(id, obj);
        }
        public Task Delete(string id)
        {
            return _examesRepository.Delete(id);
        }

        public Exames GetById(string id)
        {
            return _examesRepository.GetById(id);
        }
        public Task<(IEnumerable<Exames>, int)> GetByParam(Exames filters, string sort, int? skip, int? take)
        {
            return _examesRepository.GetByParam(filters, sort, skip, take);
        }

        public Exames GetExame(Exames obj)
        {
            return _examesRepository.GetExame(obj);
        }

        public int Count(Exames obj)
        {
            return _examesRepository.Count(obj);
        }
    }
}
