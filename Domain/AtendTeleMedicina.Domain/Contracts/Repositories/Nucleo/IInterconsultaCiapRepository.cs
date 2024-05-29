using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IInterconsultaCiapRepository
    {
       Task<string> Post(InterconsultaCiap ciap);
    }
}
