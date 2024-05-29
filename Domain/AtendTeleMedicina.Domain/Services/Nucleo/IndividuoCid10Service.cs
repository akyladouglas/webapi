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
  public class IndividuoCid10Service : IIndividuoCid10Service
    {
    private readonly IIndividuoCid10Repository _individuoCid10Repository;

    #region -- Constructor
    public IndividuoCid10Service(IIndividuoCid10Repository individuoCid10Repository)
    {
            _individuoCid10Repository = individuoCid10Repository;
    }
    #endregion

    public void Add(IEnumerable<IndividuoCid10> obj)
    {
      foreach (var x in obj)
      {
        x.Id = Guid.NewGuid().ToString();
        x.DataAlteracao = DateTime.Now;
        x.DataCadastro = DateTime.Now;
      }
      _individuoCid10Repository.Add(obj);
    }
    public void AddTriagem(IEnumerable<IndividuoCid10> obj)
    {
        foreach (var x in obj)
        {
            x.Id = Guid.NewGuid().ToString();
            x.DataAlteracao = DateTime.Now;
            x.DataCadastro = DateTime.Now;
        }
        _individuoCid10Repository.AddTriagem(obj);
    }
    public int Update(string id, IndividuoCid10 obj)
    {
      return _individuoCid10Repository.Update(id, obj);
    }
    public Individuo GetById(string individuoId)
    {
      return _individuoCid10Repository.GetById(individuoId);
    }
    public Task<(IEnumerable<IndividuoCid10>, int)> GetByParam(IndividuoCid10 filters, string sort, int? skip, int? take)
    {
      return _individuoCid10Repository.GetByParam(filters, sort, skip, take);
    }
  }
}