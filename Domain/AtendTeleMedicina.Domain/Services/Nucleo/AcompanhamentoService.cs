using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
  public class AcompanhamentoService : IAcompanhamentoService
  {
    private readonly IAcompanhamentoRepository _acompanhamentoRepository;

    #region -- Constructor
    public AcompanhamentoService(IAcompanhamentoRepository acompanhamentoRepository)
    {
            _acompanhamentoRepository = acompanhamentoRepository;
    }
    #endregion

    public int Add(Acompanhamento obj)
    {
      obj.Id = Guid.NewGuid().ToString();
      obj.Data = DateTime.Now;
      return _acompanhamentoRepository.Add(obj);
    }
    public int Update(string id, Acompanhamento obj)
    {
      //obj.DataAlteracao = DateTime.Now;
      return _acompanhamentoRepository.Update(id, obj);
    }
    public int Delete(string id)
    {
      return _acompanhamentoRepository.Delete(id);
    }

    public Acompanhamento GetById(string id)
    {
      return _acompanhamentoRepository.GetById(id);
    }
    public Task<(IEnumerable<Acompanhamento>, int)> GetByParam(Acompanhamento filters, string sort, int? skip, int? take)
    {
      return _acompanhamentoRepository.GetByParam(filters, sort, skip, take);
    }
  }
}