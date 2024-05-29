using System.Collections.Generic;
using System.Security.Claims;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
  public interface IUser
  {
    string Name { get; }
    string GetUserId();
    string GetUserEmail();
    string GetUserOrigem();
    string GetUserIp();
    bool IsAuthenticated();
    bool IsInRole(string role);
    IEnumerable<Claim> GetClaimsIdentity();
  }
}
