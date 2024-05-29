using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class BairroApplication : IBairroApplication
    {
        private readonly IBairroService _bairroService;
        #region -- Constructor
        public BairroApplication(IBairroService bairroService)
        {
            _bairroService = bairroService;
        }
        #endregion
        public int Add(Bairro obj)
        {
            return _bairroService.Add(obj);
        }

        public int Update(string id, Bairro obj)
        {
            return _bairroService.Update(id, obj);
        }

        public int Delete(string id)
        {
            return _bairroService.Delete(id);
        }

        public Bairro GetById(string id)
        {
            return _bairroService.GetById(id);
        }

        public Task<(IEnumerable<Bairro>, int)> GetByParam(Bairro filters, string sort, int? skip, int? take)
        {
            return _bairroService.GetByParam(filters, sort, skip, take);
        }

    }
}
