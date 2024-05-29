using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class ProcedimentoApplication : IProcedimentoApplication
  {
    private readonly IProcedimentoService _procedimentoService;
    #region -- Constructor
    public ProcedimentoApplication(IProcedimentoService procedimentoService)
    {
      _procedimentoService = procedimentoService;
    }
    #endregion
    public int Add(Procedimento obj)
    {
      return _procedimentoService.Add(obj);
    }

    public int Update(string id, Procedimento obj)
    {
      return _procedimentoService.Update(id, obj);
    }

    public int Delete(string id)
    {
      return _procedimentoService.Delete(id);
    }

    public Procedimento GetById(string id)
    {
      return _procedimentoService.GetById(id);
    }

    public Task<(IEnumerable<Procedimento>, int)> GetByParam(Procedimento filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _procedimentoService.GetByParam(filters, appParams, sort, skip, take);
    }

    public void AdicionarCota(string id, int quantidade, string usuario)
    {
      _procedimentoService.AdicionarCota(id, quantidade, usuario);
    }

    public void DistribuirCota(string procedimentoId, string estabelecimentoId, int quantidade, string usuario)
    {
      _procedimentoService.DistribuirCota(procedimentoId, estabelecimentoId, quantidade, usuario);
    }

    public void DistribuirCotaExecutor(string procedimentoId, string estabelecimentoId, int quantidade, string usuario)
    {
      _procedimentoService.DistribuirCotaExecutor(procedimentoId, estabelecimentoId, quantidade, usuario);
    }

    public void SubtrairCota(string id, int quantidade, string usuario)
    {
      _procedimentoService.SubtrairCota(id, quantidade, usuario);
    }

    public void UpdateCotaProfissional(string id, int quantidade, string usuario)
    {
      _procedimentoService.UpdateCotaProfissional(id, quantidade, usuario);
    }

  }
}
