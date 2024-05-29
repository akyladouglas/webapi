using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
  public class InformacoesUteisApplication : IInformacoesUteisApplication
    {
    private readonly IInformacoesUteisService _informacoesUteisService;
    #region -- Constructor
    public InformacoesUteisApplication(IInformacoesUteisService informacoesUteisService)
    {
            _informacoesUteisService = informacoesUteisService;
    }
    #endregion
    public int Add(InformacoesUteis obj)
    {
      return _informacoesUteisService.Add(obj);
    }

    public int Update(string id, InformacoesUteis obj)
    {
      return _informacoesUteisService.Update(id, obj);
    }

    public int Delete(string id)
    {
      return _informacoesUteisService.Delete(id);
    }

    public InformacoesUteis GetById(string id)
    {
      return _informacoesUteisService.GetById(id);
    }

    public Task<(IEnumerable<InformacoesUteis>, int)> GetByParam(InformacoesUteis filters, string sort, int? skip, int? take)
    {
      return _informacoesUteisService.GetByParam(filters, sort, skip, take);
    }
  }
}
