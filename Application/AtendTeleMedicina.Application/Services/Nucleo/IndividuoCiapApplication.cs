using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class IndividuoCiapApplication : IIndividuoCiapApplication
    {
    private readonly IIndividuoCiapService _individuoCiapService;
    #region -- Constructor
    public IndividuoCiapApplication(IIndividuoCiapService individuoCiapService)
    {
            _individuoCiapService = individuoCiapService;
    }
    #endregion

    public void Add(IEnumerable<IndividuoCiap> obj)
    {
            _individuoCiapService.Add(obj);
    }
    public void AddTriagem(IEnumerable<IndividuoCiap> obj)
    {
        _individuoCiapService.AddTriagem(obj);
    }
    public int Update(string id, IndividuoCiap obj)
    {
      return _individuoCiapService.Update(id, obj);
    }
    public Individuo GetById(string individuoId)
    {
      return _individuoCiapService.GetById(individuoId);
    }
    public Task<(IEnumerable<IndividuoCiap>, int)> GetByParam(IndividuoCiap filters, string sort, int? skip, int? take)
    {
      return _individuoCiapService.GetByParam(filters, sort, skip, take);
    }
  }
}