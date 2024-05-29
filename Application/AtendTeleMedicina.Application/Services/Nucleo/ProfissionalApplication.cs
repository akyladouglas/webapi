using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class ProfissionalApplication : IProfissionalApplication
    {
        private readonly IProfissionalService _profissionalService;
        #region -- Constructor
        public ProfissionalApplication(IProfissionalService profissionalService)
        {
            _profissionalService = profissionalService;
        }
        #endregion
        public int Add(Profissional obj)
        {
            return _profissionalService.Add(obj);
        }

        public int Update(string id, Profissional obj)
        {
            return _profissionalService.Update(id, obj);
        }

        public int UpdateFoto(string id, Profissional obj)
        {
            return _profissionalService.UpdateFoto(id, obj);
        }

        public int UpdateTentativas(string userId)
        {
            return _profissionalService.UpdateTentativas(userId);
        }

        public int Delete(string id)
        {
            return _profissionalService.Delete(id);
        }

        public Profissional GetById(string id)
        {
            return _profissionalService.GetById(id);
        }
        public Profissional GetByUserName(string userName)
        {
            return _profissionalService.GetByUserName(userName);
        }
        public Profissional GetByCpf(string cpf)
        {
            return _profissionalService.GetByCpf(cpf);
        }

        public Task<(IEnumerable<Profissional>, int)> GetByParam(Profissional filters, string sort, int? skip, int? take)
        {
            return _profissionalService.GetByParam(filters, sort, skip, take);
        }

        public Profissional GetEstabelecimentosById(string profissionalId)
        {
            return _profissionalService.GetEstabelecimentosById(profissionalId);
        }

        public Profissional Login(string username, string password)
        {
            return _profissionalService.Login(username, password);
        }
        public int UpdateSenha(string profissionalId, string password)
        {
            return _profissionalService.UpdateSenha(profissionalId, password);
        }
        public Task<IEnumerable<RelatorioProfissionalEscala>> GetProfissionalHorarios(AppParams filters)
        {
            return _profissionalService.GetProfissionalHorarios(filters);
        }

        public Task<IEnumerable<Profissional>> GetProfissionalProcedimentos(AppParams filters, string sort, int? skip, int? take)
        {
            return _profissionalService.GetProfissionalProcedimentos(filters, sort, skip, take);
        }

        public int AddToken(string id, string token)
        {
            return _profissionalService.AddToken(id, token);
        }
        public int UpdateToken(string id, string token)
        {
            return _profissionalService.UpdateToken(id, token);
        }
        public int AceiteTermosDoUso(string profissionalId, bool? aceite)
        {
            return _profissionalService.AceiteTermosDoUso(profissionalId, aceite);
        }
        public string UpdateCodigoAutenticacao(string profissionalId)
        {
            return _profissionalService.UpdateCodigoAutenticacao(profissionalId);
        }

        public int UpdateAtivarDesativar(Profissional obj)
        {
            return _profissionalService.UpdateAtivarDesativar(obj);
        }
    }
}
