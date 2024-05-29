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
  public class EstabelecimentoProcedimentoService : IEstabelecimentoProcedimentoService
  {
    private readonly IEstabelecimentoProcedimentoRepository _estabelecimentoProcedimentoRepository;

    #region -- Constructor
    public EstabelecimentoProcedimentoService(IEstabelecimentoProcedimentoRepository estabelecimentoProcedimentoRepository)
    {
      _estabelecimentoProcedimentoRepository = estabelecimentoProcedimentoRepository;
    }
    #endregion

    public void Add(IEnumerable<EstabelecimentoProcedimento> obj)
    {
      foreach (var x in obj)
      {
        x.Id = Guid.NewGuid().ToString();
        x.DataAlteracao = DateTime.Now;
      }
      _estabelecimentoProcedimentoRepository.Add(obj);
    }
    public int Update(string id, EstabelecimentoProcedimento obj)
    {
      return _estabelecimentoProcedimentoRepository.Update(id, obj);
    }
    public Estabelecimento GetById(string estabelecimentoId)
    {
      return _estabelecimentoProcedimentoRepository.GetById(estabelecimentoId);
    }
    public Task<(IEnumerable<EstabelecimentoProcedimento>, int)> GetByParam(EstabelecimentoProcedimento filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _estabelecimentoProcedimentoRepository.GetByParam(filters, appParams, sort, skip, take);
    }
    public void Delete(IEnumerable<EstabelecimentoProcedimento> obj)
    {
      _estabelecimentoProcedimentoRepository.Delete(obj);
    }
  }
}