using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class UnidadeFederativaApplication : IUnidadeFederativaApplication
    {
        private readonly IUnidadeFederativaService _cidadeService;
        #region -- Constructor
        public UnidadeFederativaApplication(IUnidadeFederativaService cidadeService)
        {
            _cidadeService = cidadeService;
        }
        #endregion

        public UnidadeFederativa GetById(string id)
        {
            return _cidadeService.GetById(id);
        }

        public Task<(IEnumerable<UnidadeFederativa>, int)> GetByParam(UnidadeFederativa cidadeFilter, string sort, int? skip, int? take)
        {
            return _cidadeService.GetByParam(cidadeFilter, sort, skip, take);
        }

    }
}
