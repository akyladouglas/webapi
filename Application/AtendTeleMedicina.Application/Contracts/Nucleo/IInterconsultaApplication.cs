using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IInterconsultaApplication
    {
        Task<string> Post(Interconsulta interconsulta);
    }
}
