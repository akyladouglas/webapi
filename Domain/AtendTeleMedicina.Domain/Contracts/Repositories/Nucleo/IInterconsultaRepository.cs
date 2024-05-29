using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IInterconsultaRepository
    {
       Task<string> Post(Interconsulta interconsulta);
    }
}
