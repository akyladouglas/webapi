using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
  public class AuditoriaService : IAuditoriaService
  {
    private readonly IAuditoriaRepository _auditoriaRepository;

    #region -- Constructor
    public AuditoriaService(IAuditoriaRepository auditoriaRepository)
    {
      _auditoriaRepository = auditoriaRepository;
    }
    #endregion

    public Auditoria GetById(string id)
    {
      return _auditoriaRepository.GetById(id);
    }
    public Task<(IEnumerable<Auditoria>, int)> GetByParam(Auditoria filters, AppParams appParams, string sort, int? skip, int? take)
    {
      return _auditoriaRepository.GetByParam(filters, appParams, sort, skip, take);
    }
  }
}