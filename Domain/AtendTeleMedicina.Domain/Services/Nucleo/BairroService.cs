using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class BairroService : IBairroService
    {
        private readonly IBairroRepository _bairroRepository;

        #region -- Constructor
        public BairroService(IBairroRepository bairroRepository)
        {
            _bairroRepository = bairroRepository;
        }
        #endregion

        public int Add(Bairro obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            return _bairroRepository.Add(obj);
        }
        public int Update(string id, Bairro obj)
        {
            return _bairroRepository.Update(id, obj);
        }
        public int Delete(string id)
        {
            return _bairroRepository.Delete(id);
        }

        public Bairro GetById(string id)
        {
            return _bairroRepository.GetById(id);
        }
        public Task<(IEnumerable<Bairro>, int)> GetByParam(Bairro filters, string sort, int? skip, int? take)
        {
            return _bairroRepository.GetByParam(filters, sort, skip, take);
        }


    }
}