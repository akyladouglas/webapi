using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using AtendTeleMedicina.Domain.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class ProfissionalService : IProfissionalService
    {
        private readonly IProfissionalRepository _profissionalRepository;

        #region -- Constructor
        public ProfissionalService(IProfissionalRepository profissionalRepository)
        {
            _profissionalRepository = profissionalRepository;
        }
        #endregion

        public int Add(Profissional obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.Senha = CryptoHashService.GenerateSha1Hash(obj.Senha);
            obj.Nome = obj.Nome.ToUpper();
            obj.DataCadastro = DateTime.Now;
            obj.DataAlteracao = DateTime.Now;
            return _profissionalRepository.Add(obj);
        }
        public int Update(string id, Profissional obj)
        {
            if(obj.Senha == null)
            {
                return _profissionalRepository.Update(id, obj);
            }
            if (obj.Senha.Length < 20) {
                obj.Senha = obj.Senha.GenerateSha1Hash();
            }
            return _profissionalRepository.Update(id, obj);
        }

        public int UpdateFoto(string id, Profissional obj)
        {
            return _profissionalRepository.UpdateFoto(id, obj);
        }
        public int UpdateTentativas(string userId)
        {
            return _profissionalRepository.UpdateTentativas(userId);
        }
        public int Delete(string id)
        {
            return _profissionalRepository.Delete(id);
        }

        public Profissional GetById(string id)
        {
            return _profissionalRepository.GetById(id);
        }
        public Profissional GetByUserName(string userName)
        {
            return _profissionalRepository.GetByUserName(userName);
        }
        public Profissional GetByCpf(string cpf)
        {
            return _profissionalRepository.GetByCpf(cpf);
        }
        public Task<(IEnumerable<Profissional>, int)> GetByParam(Profissional filters, string sort, int? skip, int? take)
        {
            return _profissionalRepository.GetByParam(filters, sort, skip, take);
        }

        public Profissional GetEstabelecimentosById(string profissionalId)
        {
            return _profissionalRepository.GetEstabelecimentosById(profissionalId);
        }

        public Profissional Login(string username, string password)
        {
            return _profissionalRepository.Login(username, CryptoHashService.GenerateSha1Hash(password));
        }
        public int UpdateSenha(string profissionalId, string password)
        {
            return _profissionalRepository.UpdateSenha(profissionalId, password.GenerateSha1Hash());
        }

        public Task<IEnumerable<RelatorioProfissionalEscala>> GetProfissionalHorarios(AppParams filters)
        {
            return _profissionalRepository.GetProfissionalHorarios(filters);
        }

        public Task<IEnumerable<Profissional>> GetProfissionalProcedimentos(AppParams filters, string sort, int? skip, int? take)
        {
            return _profissionalRepository.GetProfissionalProcedimentos(filters, sort, skip, take);
        }
        public int AddToken(string id, string token)
        {
            return _profissionalRepository.AddToken(id, token);
        }
        public int UpdateToken(string id, string token)
        {
            return _profissionalRepository.UpdateToken(id, token);
        }
        public int AceiteTermosDoUso(string profissionalId, bool? aceite)
        {
            return _profissionalRepository.AceiteTermosDoUso(profissionalId, aceite);
        }
        public string UpdateCodigoAutenticacao(string profissionalId)
        {
            return _profissionalRepository.UpdateCodigoAutenticacao(profissionalId, GenericHelperService.GerarCodigoAutenticacao());
        }

        public int UpdateAtivarDesativar(Profissional obj)
        {
            return _profissionalRepository.UpdateAtivarDesativar(obj);
        }
    }
}