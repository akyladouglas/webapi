using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
  public interface IInformacoesUteisApplication
    {
    int Add(InformacoesUteis obj);
    int Update(string id, InformacoesUteis obj);
    int Delete(string id);
        InformacoesUteis GetById(string id);
    Task<(IEnumerable<InformacoesUteis>, int)> GetByParam(InformacoesUteis filters, string sort, int? skip, int? take);
  }
}
