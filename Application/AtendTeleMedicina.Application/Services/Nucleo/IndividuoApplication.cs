using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class IndividuoApplication : IIndividuoApplication
    {
        private readonly IIndividuoService _individuoService;
        #region -- Constructor
        public IndividuoApplication(IIndividuoService individuoService)
        {
            _individuoService = individuoService;
        }
        #endregion
        public int Add(Individuo obj)
        {
            return _individuoService.Add(obj);
        }

        public int Update(string id, Individuo obj)
        {
            return _individuoService.Update(id, obj);
        }

        public Task Delete(string id)
        {
            return _individuoService.Delete(id);
        }

        public Individuo GetById(string id)
        {
            return _individuoService.GetById(id);
        }

        public Individuo GetByCpf(string cpf)
        {
            return _individuoService.GetByCpf(cpf);
        }
        public Individuo GetByCns(string cns)
        {
            return _individuoService.GetByCns(cns);
        }

        public Task<(IEnumerable<Individuo>, int)> GetByParam(Individuo filters, string sort, int? skip, int? take)
        {
            return _individuoService.GetByParam(filters, sort, skip, take);
        }
        public Individuo Login(string usuario, string senha)
        {
            return _individuoService.Login(usuario, senha);
        }
        public Individuo LoginPortal(string cpf)
        {
            return _individuoService.LoginPortal(cpf);
        }
        public int UpdateSenha(string individuoId, string senha, string acao)
        {
            return _individuoService.UpdateSenha(individuoId, senha, acao);
        }
        public string UpdateCodigoAutenticacao(string individuoId)
        {
            return _individuoService.UpdateCodigoAutenticacao(individuoId);
        }

        public int AtualizarComorbidade(string id, Individuo obj)
        {
            return _individuoService.AtualizarComorbidade(id, obj);
        }

        public int AtualizarComorbidadeTriagem(string id, Individuo obj)
        {
            return _individuoService.AtualizarComorbidadeTriagem(id, obj);
        }
        public int UpdateNotificacaoToken(string id, string token)
        {
            return _individuoService.UpdateNotificacaoToken(id, token);
        }
        public object GenerateFaceToken(string usuario)
        {
            return _individuoService.GenerateFaceToken(usuario);
        }

        public Task<(IEnumerable<Individuo>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            return _individuoService.PendentesIntegracao(mes, ano, estabelecimentoId);
        }

        public void ConfirmarIntegracao(int lote, Individuo obj)
        {
            _individuoService.ConfirmarIntegracao(lote, obj);
        }

    }
}
