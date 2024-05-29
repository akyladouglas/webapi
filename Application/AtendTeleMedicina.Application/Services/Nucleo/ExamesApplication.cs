using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class ExamesApplication : IExamesApplication
    {
        private readonly IExamesService _examesService;
        public ExamesApplication(IExamesService examesService)
        {
            _examesService = examesService;
        }

        public int Add(Exames obj)
        {
            return _examesService.Add(obj);
        }

        public int Update(string id, Exames obj)
        {
            return _examesService.Update(id, obj);
        }

        public Task Delete(string id)
        {
            return _examesService.Delete(id);
        }

        public Exames GetById(string id)
        {
            return _examesService.GetById(id);
        }

        public Task<(IEnumerable<Exames>, int)> GetByParam(Exames filters, string sort, int? skip, int? take)
        {
            return _examesService.GetByParam(filters, sort, skip, take);
        }

        public Exames GetExame(Exames obj)
        {
            return _examesService.GetExame(obj);
        }

        public int Count(Exames obj)
        {
            return _examesService.Count(obj);
        }
    }
}
