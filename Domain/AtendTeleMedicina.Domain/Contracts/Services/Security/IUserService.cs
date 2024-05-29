using AtendTeleMedicina.Domain.Contracts.Services.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Security
{
    public interface IUserService : IBaseService
    {
        int Add(User cadastro, string userId);
        int Update(User cadastro, string userId);
        int UpdateTentativas(string userId);
        int Delete(string id, string userId);
        User GetById(string id);
        User GetByCpf(string cpf);
        User GetByUserName(string userName);
        Task<(IEnumerable<User>, int)> GetByParam(User filters, string sort, int? skip, int? take);
        User Login(string usuario, string senha);
        int AceiteTermosDoUso(string usuarioId, bool? aceite);
        bool ValidateCredentials(string userName, string password);
        int UpdateSenha(string usuarioId, string password);
        int UpdateAtivo(User obj);
        string UpdateCodigoAutenticacao(string usuarioId);
    }
}
