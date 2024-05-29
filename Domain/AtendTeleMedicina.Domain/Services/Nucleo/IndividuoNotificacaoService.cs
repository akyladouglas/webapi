using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class IndividuoNotificacaoService : IIndividuoNotificacaoService
    {
        private readonly IIndividuoNotificacaoRepository _individuoNotificacaoRepository;

        #region -- Constructor
        public IndividuoNotificacaoService(IIndividuoNotificacaoRepository individuoNotificacaoRepository)
        {
            _individuoNotificacaoRepository = individuoNotificacaoRepository;
        }
        #endregion

        public int Add(IndividuoNotificacao obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataCadastro = DateTime.Now;
            return _individuoNotificacaoRepository.Add(obj);

        }
    }
}
