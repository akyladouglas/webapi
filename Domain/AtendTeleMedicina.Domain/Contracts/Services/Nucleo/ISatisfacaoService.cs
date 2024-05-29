using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface ISatisfacaoService
    {
        int Add(Satisfacao satisfacao);
 
    }
}
