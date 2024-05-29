using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class CustomizacaoApplication : ICustomizacaoApplication
    {
        private readonly ICustomizacaoService _customizacaoService;

        #region -- Constructor
        public CustomizacaoApplication(ICustomizacaoService customizacaoService)
        {
            _customizacaoService = customizacaoService;
        }

        public int Add(Customizacao customizacao)
        {
            return _customizacaoService.Add(customizacao);
        }

        public int Delete(string id)
        {
            return _customizacaoService.Delete(id);
        }

        public Customizacao GetById(string id)
        {
            return _customizacaoService.GetById(id);
        }

        public Task<(IEnumerable<Customizacao>, int)> GetByParam(Customizacao customizacaoFilters, string sort, int? skip, int? take)
        {
            return _customizacaoService.GetByParam(customizacaoFilters, sort, skip, take);
        }

        public int Update(string id, Customizacao customizacao)
        {
            return _customizacaoService.Update(id, customizacao);
        }
        #endregion
    }
}
