using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class IndividuoGlicemiaService : IIndividuoGlicemiaService
    {
        private readonly IIndividuoGlicemiaRepository _individuoGlicemiaRepository;

        #region -- Constructor
        public IndividuoGlicemiaService(IIndividuoGlicemiaRepository individuoGlicemiaRepository)
        {
            _individuoGlicemiaRepository = individuoGlicemiaRepository;
        }
        #endregion

        public string Add(IndividuoGlicemia obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataCadastro = DateTime.Now;
            return _individuoGlicemiaRepository.Add(obj);
        } 
        public string Update(string id, IndividuoGlicemia obj)
        {
            return _individuoGlicemiaRepository.Update(id, obj);
        }
        public IndividuoGlicemia GetById(string id)
        {
            return _individuoGlicemiaRepository.GetById(id);
        }
        public Task<(IEnumerable<IndividuoGlicemia>, int)> GetByParam(IndividuoGlicemia filters, string sort, int? skip, int? take)
        {
            return _individuoGlicemiaRepository.GetByParam(filters, sort, skip, take);
        }
    }
}
