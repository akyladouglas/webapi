using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IInterconsultaCiapService
    {
        Task<string> Post(InterconsultaCiap ciap);
    }
}
