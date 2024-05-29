using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class IndividuoCid10Application : IIndividuoCid10Application
    {
    private readonly IIndividuoCid10Service _individuoCid10Service;
    #region -- Constructor
    public IndividuoCid10Application(IIndividuoCid10Service individuoCid10Service)
    {
            _individuoCid10Service = individuoCid10Service;
    }
    #endregion

    public void Add(IEnumerable<IndividuoCid10> obj)
    {
            _individuoCid10Service.Add(obj);
    }
    public void AddTriagem(IEnumerable<IndividuoCid10> obj)
    {
        _individuoCid10Service.AddTriagem(obj);
    }
        public int Update(string id, IndividuoCid10 obj)
    {
      return _individuoCid10Service.Update(id, obj);
    }
    public Individuo GetById(string individuoId)
    {
      return _individuoCid10Service.GetById(individuoId);
    }
    public Task<(IEnumerable<IndividuoCid10>, int)> GetByParam(IndividuoCid10 filters, string sort, int? skip, int? take)
    {
      return _individuoCid10Service.GetByParam(filters, sort, skip, take);
    }
  }
}