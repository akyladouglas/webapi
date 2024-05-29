using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
  public class ProcedimentoService : IProcedimentoService
  {
    private readonly IProcedimentoRepository _procedimentoRepository;

    #region -- Constructor
    public ProcedimentoService(IProcedimentoRepository procedimentoRepository)
    {
      _procedimentoRepository = procedimentoRepository;
    }
    #endregion

    public int Add(Procedimento obj)
    {
      obj.Id = Guid.NewGuid().ToString();
      return _procedimentoRepository.Add(obj);
    }
    public int Update(string id, Procedimento obj)
    {
      return _procedimentoRepository.Update(id, obj);
    }
    public int Delete(string id)
    {
      return _procedimentoRepository.Delete(id);
    }

    public Procedimento GetById(string id)
    {
      return _procedimentoRepository.GetById(id);
    }
    public Task<(IEnumerable<Procedimento>, int)> GetByParam(Procedimento filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _procedimentoRepository.GetByParam(filters, appParams, sort, skip, take);
    }

    public void AdicionarCota(string id, int quantidade, string usuario)
    {
      _procedimentoRepository.AdicionarCota(id, quantidade, usuario);
    }

    public void DistribuirCota(string procedimentoId, string estabelecimentoId, int quantidade, string usuario)
    {
      _procedimentoRepository.DistribuirCota(procedimentoId, estabelecimentoId, quantidade, usuario);
    }

    public void DistribuirCotaExecutor(string procedimentoId, string estabelecimentoId, int quantidade, string usuario)
    {
      _procedimentoRepository.DistribuirCotaExecutor(procedimentoId, estabelecimentoId, quantidade, usuario);
    }

    public void SubtrairCota(string id, int quantidade, string usuario)
    {
      _procedimentoRepository.SubtrairCota(id, quantidade, usuario);
    }

    public void UpdateCotaProfissional(string id, int quantidade, string usuario)
    {
      _procedimentoRepository.UpdateCotaProfissional(id, quantidade, usuario);
    }

  }
}