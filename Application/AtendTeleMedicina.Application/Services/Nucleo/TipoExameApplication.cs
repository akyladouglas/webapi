using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class TipoExameApplication : ITipoExameApplication
    {
        private readonly ITipoExameService _tipoExameService;
        public TipoExameApplication(ITipoExameService tipoExameService)
        {
            _tipoExameService = tipoExameService;
        }

        public Task<(IEnumerable<TipoExame>, int)> GetByParam(TipoExame filters, string sort, int? skip, int? take)
        {
            return _tipoExameService.GetByParam(filters, sort, skip, take);
        }

        //public int Update(string id, Documento obj)
        //{
        //    return _documentoService.Update(id, obj);
        //}

        //public int Delete(string id)
        //{
        //    return _documentoService.Delete(id);
        //}

        public TipoExame GetById(string id)
        {
            return _tipoExameService.GetById(id);
        }

        //public Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take)
        //{
        //    return _documentoService.GetByParam(filters, sort, skip, take);
        //}
    }
}
