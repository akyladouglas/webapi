using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class AcompanhamentoApplication : IAcompanhamentoApplication
  {
    private readonly IAcompanhamentoService _acompanhamentoService;
    #region -- Constructor
    public AcompanhamentoApplication(IAcompanhamentoService acompanhamentoService)
    {
            _acompanhamentoService = acompanhamentoService;
    }
    #endregion
    public int Add(Acompanhamento obj)
    {
      return _acompanhamentoService.Add(obj);
    }

    public int Update(string id, Acompanhamento obj)
    {
      return _acompanhamentoService.Update(id, obj);
    }

    public int Delete(string id)
    {
      return _acompanhamentoService.Delete(id);
    }

    public Acompanhamento GetById(string id)
    {
      return _acompanhamentoService.GetById(id);
    }

    public Task<(IEnumerable<Acompanhamento>, int)> GetByParam(Acompanhamento filters, string sort, int? skip, int? take)
    {
      return _acompanhamentoService.GetByParam(filters, sort, skip, take);
    }

  }
}
