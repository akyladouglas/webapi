using AtendTeleMedicina.Domain.Contracts.Repositories.Security;
using AtendTeleMedicina.Domain.Contracts.Services.Security;
using AtendTeleMedicina.Domain.Entities.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Services.Helpers;

namespace AtendTeleMedicina.Domain.Services.Security
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        #region -- Constructor
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        public int Add(User cadastro, string userId)
        {
            cadastro.Id = Guid.NewGuid().ToString();
            cadastro.Senha = cadastro.Senha.GenerateSha1Hash();
            cadastro.Ativo = true;
            return _userRepository.Add(cadastro, userId);
        }

        public int Update(User cadastro, string userId)
        {
            //if (cadastro.Senha.Length < 20) cadastro.Senha = cadastro.Senha.GenerateSha1Hash();
            if (!string.IsNullOrEmpty(cadastro.Senha)) cadastro.Senha = cadastro.Senha.GenerateSha1Hash();
            return _userRepository.Update(cadastro);
        }

        public int UpdateTentativas(string userId)
        {
            //if (cadastro.Senha.Length < 20) cadastro.Senha = cadastro.Senha.GenerateSha1Hash();
            return _userRepository.UpdateTentativas(userId);
        }

        public int Delete(string id, string userId)
        {
            return _userRepository.Delete(id);
        }

        public User GetById(string id)
        {
            return _userRepository.GetById(id);
        }
        public User GetByCpf(string cpf)
        {
            return _userRepository.GetByCpf(cpf);
        }
        public User GetByUserName(string userName)
        {
            return _userRepository.GetByUserName(userName);
        }

        public Task<(IEnumerable<User>, int)> GetByParam(User filters, string sort, int? skip, int? take)
        {
            return _userRepository.GetByParam(filters, sort, skip, take);
        }

        public User Login(string usuario, string senha)
        {
            return _userRepository.Login(usuario, CryptoHashService.GenerateSha1Hash(senha));
        }

        public int AceiteTermosDoUso(string usuarioId, bool? aceite)
        {
            return _userRepository.AceiteTermosDoUso(usuarioId, aceite);
        }

        public bool ValidateCredentials(string userName, string password)
        {
            return _userRepository.ValidateCredentials(userName, password);
        }
        public int UpdateSenha(string usuarioId, string password)
        {
            return _userRepository.UpdateSenha(usuarioId, password.GenerateSha1Hash());
        }
        public string UpdateCodigoAutenticacao(string usuarioId)
        {
            return _userRepository.UpdateCodigoAutenticacao(usuarioId, GenericHelperService.GerarCodigoAutenticacao());
        }

        public int UpdateAtivo(User obj)
        {
            return _userRepository.UpdateAtivo(obj);
        }
    }
}
