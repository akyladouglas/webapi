using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class IndividuoProcedimentoAutorizacaoApplication : IIndividuoProcedimentoAutorizacaoApplication
    {
        private readonly IIndividuoProcedimentoAutorizacaoService _individuoProcedimentoAutorizacaoService;
        #region -- Constructor
        public IndividuoProcedimentoAutorizacaoApplication(IIndividuoProcedimentoAutorizacaoService individuoProcedimentoAutorizacaoService)
        {
            _individuoProcedimentoAutorizacaoService = individuoProcedimentoAutorizacaoService;
        }
        #endregion
        public void Add(IEnumerable<IndividuoProcedimentoAutorizacao> obj)
        {
            _individuoProcedimentoAutorizacaoService.Add(obj);
        }
        public void Delete(IEnumerable<IndividuoProcedimentoAutorizacao> obj)
        {
            _individuoProcedimentoAutorizacaoService.Delete(obj);
        }

    }
}
