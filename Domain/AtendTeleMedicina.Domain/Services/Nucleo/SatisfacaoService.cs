using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class SatisfacaoService : ISatisfacaoService
    {
        private ISatisfacaoRepository _satisfacaoRepository;

        #region -- Constructor
        public SatisfacaoService(ISatisfacaoRepository satisfacaoRepository)
        {
            _satisfacaoRepository = satisfacaoRepository;
        }
        #endregion

        public int Add(Satisfacao obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            return _satisfacaoRepository.Add(obj);
        }
    }
}
