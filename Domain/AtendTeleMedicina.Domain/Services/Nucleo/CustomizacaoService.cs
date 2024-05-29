using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class CustomizacaoService : ICustomizacaoService
    {
        private ICustomizacaoRepository _customizacaoRepository;

        #region -- Constructor
        public CustomizacaoService(ICustomizacaoRepository customizacaoRepository)
        {
            _customizacaoRepository = customizacaoRepository;
        }
        #endregion

        public int Add(Customizacao obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            return _customizacaoRepository.Add(obj);
        }

        public int Delete(string id)
        {
            return _customizacaoRepository.Delete(id);
        }

        public Customizacao GetById(string id)
        {
            return _customizacaoRepository.GetById(id);
        }

        public Task<(IEnumerable<Customizacao>, int)> GetByParam(Customizacao filters, string sort, int? skip, int? take)
        {
            return _customizacaoRepository.GetByParam(filters, sort, skip, take);
        }

        public int Update(string id, Customizacao obj)
        {
            return _customizacaoRepository.Update(id, obj);
        }
    }
}
