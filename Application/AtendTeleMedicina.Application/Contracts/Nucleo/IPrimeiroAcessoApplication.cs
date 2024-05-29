using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
   public interface IPrimeiroAcessoApplication
    {
        Task<(IEnumerable<Individuo>, int)> FindUserExternalService(Individuo individuo);
        Task<(IEnumerable<Individuo>, int)> ConfirmData(Individuo individuo);
        Task<(IEnumerable<Individuo>, int)> Add(Individuo individuo);
    }
}
