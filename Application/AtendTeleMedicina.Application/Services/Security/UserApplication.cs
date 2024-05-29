using AtendTeleMedicina.Application.Contracts.Security;
using AtendTeleMedicina.Domain.Contracts.Services.Security;
using AtendTeleMedicina.Domain.Entities.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Security
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserService _userService;
        #region -- Constructor
        public UserApplication(IUserService userService)
        {
            _userService = userService;
        }
        #endregion
        public int Add(User cadastro, string userId)
        {
            return _userService.Add(cadastro, userId);
        }

        public int Update(User cadastro, string userId)
        {
            return _userService.Update(cadastro, userId);
        }

        public int UpdateTentativas(string userId)
        {
            return _userService.UpdateTentativas(userId);
        }
        public int Delete(string id, string userId)
        {
            return _userService.Delete(id, userId);
        }
        public User GetById(string id)
        {
            return _userService.GetById(id);
        }
        public User GetByCpf(string cpf)
        {
            return _userService.GetByCpf(cpf);
        }
        public User GetByUserName(string userName)
        {
            return _userService.GetByUserName(userName);
        }

        public Task<(IEnumerable<User>, int)> GetByParam(User filters, string sort, int? skip, int? take)
        {
            return _userService.GetByParam(filters, sort, skip, take);
        }

        public User Login(string usuario, string senha)
        {
            return _userService.Login(usuario, senha);
        }
        public int AceiteTermosDoUso(string usuarioId, bool? aceite)
        {
            return _userService.AceiteTermosDoUso(usuarioId, aceite);
        }

        public bool ValidateCredentials(string userName, string password)
        {
            return _userService.ValidateCredentials(userName, password);
        }

        public int UpdateSenha(string usuarioId, string password)
        {
            return _userService.UpdateSenha(usuarioId, password);
        }
        public string UpdateCodigoAutenticacao(string usuarioId)
        {
            return _userService.UpdateCodigoAutenticacao(usuarioId);
        }

        public int UpdateAtivo(User obj)
        {
            return _userService.UpdateAtivo(obj);
        }
    }
}
