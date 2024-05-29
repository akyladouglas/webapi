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
  public class EstabelecimentoProfissionalService : IEstabelecimentoProfissionalService
    {
    private readonly IEstabelecimentoProfissionalRepository _estabelecimentoProfissionalRepository;

    #region -- Constructor
    public EstabelecimentoProfissionalService(IEstabelecimentoProfissionalRepository estabelecimentoProfissionalRepository)
    {
      _estabelecimentoProfissionalRepository = estabelecimentoProfissionalRepository;
    }
    #endregion

    public void Add(IEnumerable<EstabelecimentoProfissional> obj)
    {
      foreach (var x in obj)
      {
        x.Id = Guid.NewGuid().ToString();
        //x.DataAlteracao = DateTime.Now;
      }
      _estabelecimentoProfissionalRepository.Add(obj);
    }
    public int Update(string id, EstabelecimentoProfissional obj)
    {
      return _estabelecimentoProfissionalRepository.Update(id, obj);
    }
    public Estabelecimento GetById(string profissionalId)
    {
      return _estabelecimentoProfissionalRepository.GetById(profissionalId);
    }
    public Task<(IEnumerable<EstabelecimentoProfissional>, int)> GetByParam(EstabelecimentoProfissional filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _estabelecimentoProfissionalRepository.GetByParam(filters, appParams, sort, skip, take);
    }
  }
}