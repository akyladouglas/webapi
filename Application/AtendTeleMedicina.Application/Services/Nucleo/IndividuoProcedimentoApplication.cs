using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class IndividuoProcedimentoApplication : IIndividuoProcedimentoApplication
    {
    private readonly IIndividuoProcedimentoService _individuoProcedimentoService;
    #region -- Constructor
    public IndividuoProcedimentoApplication(IIndividuoProcedimentoService individuoProcedimentoService)
    {
            _individuoProcedimentoService = individuoProcedimentoService;
    }
    #endregion

    //public void Add(IEnumerable<IndividuoProcedimento> obj)
    //{
    //        _individuoProcedimentoService.Add(obj);
    //}
    public void AddTriagem(IEnumerable<IndividuoProcedimento> obj)
    {
        _individuoProcedimentoService.AddTriagem(obj);
    }
        //public int Update(string id, IndividuoProcedimento obj)
        //{
        //  return _individuoProcedimentoService.Update(id, obj);
        //}
        //public Individuo GetById(string individuoId)
        //{
        //  return _individuoProcedimentoService.GetById(individuoId);
        //}
    public Task<(IEnumerable<IndividuoProcedimento>, int)> GetByParam(IndividuoProcedimento filters, string sort, int? skip, int? take)
    {
        return _individuoProcedimentoService.GetByParam(filters, sort, skip, take);
    }
  }
}