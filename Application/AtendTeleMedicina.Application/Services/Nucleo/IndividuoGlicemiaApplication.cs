using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Services.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class IndividuoGlicemiaApplication : IIndividuoGlicemiaApplication
    {
        private readonly IIndividuoGlicemiaService _individuoGlicemiaService;
        public IndividuoGlicemiaApplication(IIndividuoGlicemiaService individuoGlicemiaService)
        {
            _individuoGlicemiaService = individuoGlicemiaService;
        }

        public string Add(IndividuoGlicemia obj)
        {
            return _individuoGlicemiaService.Add(obj);
        }

        public string Update(string id, IndividuoGlicemia obj)
        {
            return _individuoGlicemiaService.Update(id, obj);
        }


        public IndividuoGlicemia GetById(string id)
        {
            return _individuoGlicemiaService.GetById(id);
        }

        public Task<(IEnumerable<IndividuoGlicemia>, int)> GetByParam(IndividuoGlicemia filters, string sort, int? skip, int? take)
        {
            return _individuoGlicemiaService.GetByParam(filters, sort, skip, take);
        }
    }
}
