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
  public class IndividuoProcedimentoService : IIndividuoProcedimentoService
    {
    private readonly IIndividuoProcedimentoRepository _individuoProcedimentoRepository;

    #region -- Constructor
    public IndividuoProcedimentoService(IIndividuoProcedimentoRepository individuoProcedimentoRepository)
    {
            _individuoProcedimentoRepository = individuoProcedimentoRepository;
    }
    #endregion

    //public void Add(IEnumerable<IndividuoProcedimento> obj)
    //{
    //  foreach (var x in obj)
    //  {
    //    x.Id = Guid.NewGuid().ToString();
    //    x.DataAlteracao = DateTime.Now;
    //    x.DataCadastro = DateTime.Now;
    //  }
    //  _individuoProcedimentoRepository.Add(obj);
    //}
    public void AddTriagem(IEnumerable<IndividuoProcedimento> obj)
    {
        foreach (var x in obj)
        {
            x.Id = Guid.NewGuid().ToString();
            x.DataAlteracao = DateTime.Now;
            x.DataCadastro = DateTime.Now;
        }
        _individuoProcedimentoRepository.AddTriagem(obj);
    }
        //public int Update(string id, IndividuoProcedimento obj)
        //{
        //  return _individuoProcedimentoRepository.Update(id, obj);
        //}
        //public Individuo GetById(string individuoId)
        //{
        //  return _individuoProcedimentoRepository.GetById(individuoId);
        //}
    public Task<(IEnumerable<IndividuoProcedimento>, int)> GetByParam(IndividuoProcedimento filters, string sort, int? skip, int? take)
    {
        return _individuoProcedimentoRepository.GetByParam(filters, sort, skip, take);
    }
  }
}