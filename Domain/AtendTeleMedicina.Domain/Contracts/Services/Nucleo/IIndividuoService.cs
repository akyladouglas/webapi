using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IIndividuoService
    {
        int Add(Individuo obj);
        int Update(string id, Individuo obj);
        Task Delete(string id);
        Individuo GetById(string id);
        Individuo GetByCpf(string cpf);
        Individuo GetByCns(string cpf);
        Task<(IEnumerable<Individuo>, int)> GetByParam(Individuo filters, string sort, int? skip, int? take);
        Individuo Login(string usuario, string senha);
        Individuo LoginPortal(string cpf);
        int UpdateSenha(string individuoId, string senha, string acao);
        string UpdateCodigoAutenticacao(string individuoId);
        int AtualizarComorbidade(string id, Individuo obj);
        int AtualizarComorbidadeTriagem(string id, Individuo obj);
        int UpdateNotificacaoToken(string id, string token);
        object GenerateFaceToken(string usuario);
        Task<(IEnumerable<Individuo>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId);
        void ConfirmarIntegracao(int lote, Individuo obj);
    }
}