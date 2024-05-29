using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IInterconsultaCidApplication
    {
        Task<string> Post(InterconsultaCid cid);
    }
}
