using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
  public class InformacoesUteisService : IInformacoesUteisService
    {
    private readonly IInformacoesUteisRepository _informacoesUteisRepository;

    #region -- Constructor
    public InformacoesUteisService(IInformacoesUteisRepository informacoesUteisRepository)
    {
           _informacoesUteisRepository = informacoesUteisRepository;
    }
    #endregion

    public int Add(InformacoesUteis obj)
    {
      obj.Id = Guid.NewGuid().ToString();
      obj.DataCadastro = DateTime.Now;
      return _informacoesUteisRepository.Add(obj);
    }
    public int Update(string id, InformacoesUteis obj)
    {
      obj.DataAlteracao = DateTime.Now;
      return _informacoesUteisRepository.Update(id, obj);
    }
    public int Delete(string id)
    {
      return _informacoesUteisRepository.Delete(id);
    }

    public InformacoesUteis GetById(string id)
    {
      return _informacoesUteisRepository.GetById(id);
    }
    public Task<(IEnumerable<InformacoesUteis>, int)> GetByParam(InformacoesUteis filters, string sort, int? skip, int? take)
    {
      return _informacoesUteisRepository.GetByParam(filters, sort, skip, take);
    }
  }
}