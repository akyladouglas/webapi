using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class TipoExameService : ITipoExameService
    {
        private readonly ITipoExameRepository _tipoExameRepository;

        #region -- Constructor
        public TipoExameService(ITipoExameRepository tipoExameRepository)
        {
            _tipoExameRepository = tipoExameRepository;
        }
        #endregion

        public Task<(IEnumerable<TipoExame>, int)> GetByParam(TipoExame filters, string sort, int? skip, int? take)
        {
            return _tipoExameRepository.GetByParam(filters, sort, skip, take);
        }
        //public int Update(string id, Documento obj)
        //{
        //    return _documentoRepository.Update(id, obj);
        //}
        //public int Delete(string id)
        //{
        //    return _documentoRepository.Delete(id);
        //}

        public TipoExame GetById(string id)
        {
            return _tipoExameRepository.GetById(id);
        }
        //public Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take)
        //{
        //    return _documentoRepository.GetByParam(filters, sort, skip, take);
        //}
    }
}
