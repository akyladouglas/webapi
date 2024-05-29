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
  public class IndividuoCiapService : IIndividuoCiapService
    {
    private readonly IIndividuoCiapRepository _individuoCiapRepository;

    #region -- Constructor
    public IndividuoCiapService(IIndividuoCiapRepository individuoCiapRepository)
    {
            _individuoCiapRepository = individuoCiapRepository;
    }
    #endregion

    public void Add(IEnumerable<IndividuoCiap> obj)
    {
      foreach (var x in obj)
      {
        x.Id = Guid.NewGuid().ToString();
        x.DataAlteracao = DateTime.Now;
        x.DataCadastro = DateTime.Now;
      }
      _individuoCiapRepository.Add(obj);
    }
    public void AddTriagem(IEnumerable<IndividuoCiap> obj)
    {
        foreach (var x in obj)
        {
            x.Id = Guid.NewGuid().ToString();
            x.DataAlteracao = DateTime.Now;
            x.DataCadastro = DateTime.Now;
        }
        _individuoCiapRepository.AddTriagem(obj);
    }
    public int Update(string id, IndividuoCiap obj)
    {
      return _individuoCiapRepository.Update(id, obj);
    }
    public Individuo GetById(string individuoId)
    {
      return _individuoCiapRepository.GetById(individuoId);
    }
    public Task<(IEnumerable<IndividuoCiap>, int)> GetByParam(IndividuoCiap filters, string sort, int? skip, int? take)
    {
      return _individuoCiapRepository.GetByParam(filters, sort, skip, take);
    }
  }
}