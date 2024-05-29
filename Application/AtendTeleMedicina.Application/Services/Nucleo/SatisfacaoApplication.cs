using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class SatisfacaoApplication : ISatisfacaoApplication
    {
        private readonly ISatisfacaoService _satisfacaoService;

        #region -- Constructor
        public SatisfacaoApplication(ISatisfacaoService satisfacaoService)
        {
            _satisfacaoService = satisfacaoService;
        }

        public int Add(Satisfacao satisfacao)
        {
            return _satisfacaoService.Add(satisfacao);
        }
        #endregion
    }
}
