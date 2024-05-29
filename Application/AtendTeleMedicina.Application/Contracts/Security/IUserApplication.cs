using AtendTeleMedicina.Domain.Entities.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Security
{
    public interface IUserApplication
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
        int UpdateAtivo(User obj);
        bool ValidateCredentials(string userName, string password);
        int UpdateSenha(string usuarioId, string password);
        string UpdateCodigoAutenticacao(string usuarioId);
    }
}
