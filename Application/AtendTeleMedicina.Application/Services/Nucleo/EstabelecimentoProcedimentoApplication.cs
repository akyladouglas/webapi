using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class EstabelecimentoProcedimentoApplication : IEstabelecimentoProcedimentoApplication
  {
    private readonly IEstabelecimentoProcedimentoService _estabelecimentoProcedimentoService;
    #region -- Constructor
    public EstabelecimentoProcedimentoApplication(IEstabelecimentoProcedimentoService estabelecimentoProcedimentoService)
    {
      _estabelecimentoProcedimentoService = estabelecimentoProcedimentoService;
    }
    #endregion

    public void Add(IEnumerable<EstabelecimentoProcedimento> obj)
    {
      _estabelecimentoProcedimentoService.Add(obj);
    }
    public int Update(string id, EstabelecimentoProcedimento obj)
    {
      return _estabelecimentoProcedimentoService.Update(id, obj);
    }
    public Estabelecimento GetById(string estabelecimentoId)
    {
      return _estabelecimentoProcedimentoService.GetById(estabelecimentoId);
    }
    public Task<(IEnumerable<EstabelecimentoProcedimento>, int)> GetByParam(EstabelecimentoProcedimento filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _estabelecimentoProcedimentoService.GetByParam(filters, appParams, sort, skip, take);
    }
    public void Delete(IEnumerable<EstabelecimentoProcedimento> obj)
    {
        _estabelecimentoProcedimentoService.Delete(obj);
    }
    }
}