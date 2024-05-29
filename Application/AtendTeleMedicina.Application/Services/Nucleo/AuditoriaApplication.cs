using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class AuditoriaApplication : IAuditoriaApplication
  {
    private readonly IAuditoriaService _auditoriaService;
    #region -- Constructor
    public AuditoriaApplication(IAuditoriaService auditoriaService)
    {
      _auditoriaService = auditoriaService;
    }
    #endregion

    public Auditoria GetById(string id)
    {
      return _auditoriaService.GetById(id);
    }

    public Task<(IEnumerable<Auditoria>, int)> GetByParam(Auditoria filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _auditoriaService.GetByParam(filters, appParams, sort, skip, take);
    }

  }
}
