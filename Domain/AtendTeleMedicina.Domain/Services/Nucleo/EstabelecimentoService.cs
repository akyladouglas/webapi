using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
  public class EstabelecimentoService : IEstabelecimentoService
  {
    private readonly IEstabelecimentoRepository _estabelecimentoRepository;

    #region -- Constructor
    public EstabelecimentoService(IEstabelecimentoRepository estabelecimentoRepository)
    {
      _estabelecimentoRepository = estabelecimentoRepository;
    }
    #endregion

    public int Add(Estabelecimento obj)
    {
      obj.Id = Guid.NewGuid().ToString();
      obj.DataCadastro = DateTime.Now;
      obj.DataAlteracao = DateTime.Now;
      return _estabelecimentoRepository.Add(obj);
    }
    public int Update(string id, Estabelecimento obj)
    {
      return _estabelecimentoRepository.Update(id, obj);
    }
    public int Delete(string id, bool? ativo)
    {
      return _estabelecimentoRepository.Delete(id, ativo);
    }

    public Estabelecimento GetById(string id)
    {
      return _estabelecimentoRepository.GetById(id);
    }
    public Task<(IEnumerable<Estabelecimento>, int)> GetByParam(Estabelecimento filters, string sort, int? skip, int? take)
    {
      return _estabelecimentoRepository.GetByParam(filters, sort, skip, take);
    }

    public int DistribuirCota(string estabelecimentoId, string procedimentoId, int quantidade, string usuario)
    {
      return _estabelecimentoRepository.DistribuirCota(estabelecimentoId, procedimentoId, quantidade, usuario);
    }

  }
}