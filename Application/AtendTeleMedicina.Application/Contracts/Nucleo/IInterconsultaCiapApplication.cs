using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IInterconsultaCiapApplication
    {
        Task<string> Post(InterconsultaCiap ciap);
    }
}
