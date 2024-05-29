using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class EstabelecimentoProfissionalApplication : IEstabelecimentoProfissionalApplication
  {
    private readonly IEstabelecimentoProfissionalService _estabelecimentoProfissionalService;
    #region -- Constructor
    public EstabelecimentoProfissionalApplication(IEstabelecimentoProfissionalService estabelecimentoProfissionalService)
    {
      _estabelecimentoProfissionalService = estabelecimentoProfissionalService;
    }
    #endregion

    public void Add(IEnumerable<EstabelecimentoProfissional> obj)
    {
      _estabelecimentoProfissionalService.Add(obj);
    }
    public int Update(string id, EstabelecimentoProfissional obj)
    {
      return _estabelecimentoProfissionalService.Update(id, obj);
    }
    public Estabelecimento GetById(string profissionalId)
    {
      return _estabelecimentoProfissionalService.GetById(profissionalId);
    }
    public Task<(IEnumerable<EstabelecimentoProfissional>, int)> GetByParam(EstabelecimentoProfissional filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _estabelecimentoProfissionalService.GetByParam(filters, appParams, sort, skip, take);
    }
  }
}