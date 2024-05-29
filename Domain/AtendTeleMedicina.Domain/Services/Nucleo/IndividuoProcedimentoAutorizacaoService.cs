using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class IndividuoProcedimentoAutorizacaoService : IIndividuoProcedimentoAutorizacaoService
    {
        private readonly IIndividuoProcedimentoAutorizacaoRepository _individuoProcedimentoAutorizacaoRepository;

        #region -- Constructor
        public IndividuoProcedimentoAutorizacaoService(IIndividuoProcedimentoAutorizacaoRepository individuoProcedimentoAutorizacaoRepository)
        {
            _individuoProcedimentoAutorizacaoRepository = individuoProcedimentoAutorizacaoRepository;
        }
        #endregion

        public void Add(IEnumerable<IndividuoProcedimentoAutorizacao> obj)
        {
            foreach (var x in obj)
            {
                x.Id = Guid.NewGuid().ToString();
                x.DataCadastro = DateTime.Now;
            }
            _individuoProcedimentoAutorizacaoRepository.Add(obj);
        }
        public void Delete(IEnumerable<IndividuoProcedimentoAutorizacao> obj)
        {
            _individuoProcedimentoAutorizacaoRepository.Delete(obj);
        }

    }
}