using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Application.Services.Base;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class CidadeApplication : ICidadeApplication
    {
        private readonly ICidadeService _cidadeService;
        #region -- Constructor
        public CidadeApplication(ICidadeService cidadeService)
        {
            _cidadeService = cidadeService;
        }
        #endregion
        public int Add(Cidade cidade)
        {
            return _cidadeService.Add(cidade);
        }

        public int Update(string id, Cidade obj, string userId)
        {
            return _cidadeService.Update(id, obj, userId);
        }

        public Cidade GetById(string id)
        {
            return _cidadeService.GetById(id);
        }

        public Task<(IEnumerable<Cidade>, int)> GetByParam(Cidade filters, string sort, int? skip, int? take)
        {
            return _cidadeService.GetByParam(filters, sort, skip, take);
        }

    }
}
