using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
  public interface IInformacoesUteisService
    {
    int Add(InformacoesUteis obj);
    int Update(string id, InformacoesUteis obj);
    int Delete(string id);
    InformacoesUteis GetById(string id);
    Task<(IEnumerable<InformacoesUteis>, int)> GetByParam(InformacoesUteis filters, string sort, int? skip, int? take);
  }
}