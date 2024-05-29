using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IInterconsultaCidRepository
    {
       Task<string> Post(InterconsultaCid cid);
    }
}
