using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IInterconsultaCidService
    {
        Task<string> Post(InterconsultaCid cid);
    }
}
