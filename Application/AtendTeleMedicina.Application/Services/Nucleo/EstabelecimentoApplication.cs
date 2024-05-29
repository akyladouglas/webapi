using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class EstabelecimentoApplication : IEstabelecimentoApplication
  {
    private readonly IEstabelecimentoService _estabelecimentoService;
    #region -- Constructor
    public EstabelecimentoApplication(IEstabelecimentoService estabelecimentoService)
    {
      _estabelecimentoService = estabelecimentoService;
    }
    #endregion
    public int Add(Estabelecimento obj)
    {
      return _estabelecimentoService.Add(obj);
    }

    public int Update(string id, Estabelecimento obj)
    {
      return _estabelecimentoService.Update(id, obj);
    }

    public int Delete(string id, bool? ativo)
    {
      return _estabelecimentoService.Delete(id, ativo);
    }

    public Estabelecimento GetById(string id)
    {
      return _estabelecimentoService.GetById(id);
    }

    public Task<(IEnumerable<Estabelecimento>, int)> GetByParam(Estabelecimento filters, string sort, int? skip, int? take)
    {
      return _estabelecimentoService.GetByParam(filters, sort, skip, take);
    }

    public int DistribuirCota(string estabelecimentoId, string procedimentoId, int quantidade, string usuario)
    {
      return _estabelecimentoService.DistribuirCota(estabelecimentoId, procedimentoId, quantidade, usuario);
    }
  }
}
