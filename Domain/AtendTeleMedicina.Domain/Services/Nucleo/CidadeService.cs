using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class CidadeService : ICidadeService
    {
        private readonly ICidadeRepository _cidadeRepository;

        #region -- Constructor
        public CidadeService(ICidadeRepository cidadeRepository)
        {
            _cidadeRepository = cidadeRepository;
        }
        #endregion

        public int Add(Cidade obj)
        {
            return _cidadeRepository.Add(obj);
        }

        public int Update(string id, Cidade obj, string userId)
        {
            return _cidadeRepository.Update(id, obj, userId);
        }

        public Cidade GetById(string id)
        {
            return _cidadeRepository.GetById(id);
        }
        public Task<(IEnumerable<Cidade>, int)> GetByParam(Cidade filters, string sort, int? skip, int? take)
        {
            return _cidadeRepository.GetByParam(filters, sort, skip, take);
        }


    }
}