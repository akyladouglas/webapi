using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IProfissionalRepository
    {
        int Add(Profissional obj);
        int Update(string id, Profissional obj);
        int UpdateAtivarDesativar(Profissional obj);
        int UpdateFoto(string id, Profissional obj);
        int UpdateTentativas(string id);
        int Delete(string id);
        Profissional GetById(string id);
        Profissional GetByUserName(string userName);
        Profissional GetByCpf(string cpf);
        Task<(IEnumerable<Profissional>, int)> GetByParam(Profissional filters, string sort, int? skip, int? take);
        Profissional GetEstabelecimentosById(string profissionalId);
        Profissional Login(string username, string password);
        int UpdateSenha(string profissionalId, string password);
        Task<IEnumerable<RelatorioProfissionalEscala>> GetProfissionalHorarios(AppParams filters);
        Task<IEnumerable<Profissional>> GetProfissionalProcedimentos(AppParams filters, string sort, int? skip, int? take);
        int AddToken(string id, string token);
        int UpdateToken(string id, string token);
        int AceiteTermosDoUso(string profissionalId, bool? aceite);
        string UpdateCodigoAutenticacao(string profissionalId, string codigoAutenticacao);
    }
}