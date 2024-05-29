using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class IndividuoNotificacaoApplication : IIndividuoNotificacaoApplication
    {
        private readonly IIndividuoNotificacaoService _individuoNotificacaoService;

        #region -- Constructor
        public IndividuoNotificacaoApplication(IIndividuoNotificacaoService individuoNotificacaoService)
        {
            _individuoNotificacaoService = individuoNotificacaoService;
        }
        #endregion

        public int Add(IndividuoNotificacao obj)
        {
            return _individuoNotificacaoService.Add(obj);
        }
    }
}
