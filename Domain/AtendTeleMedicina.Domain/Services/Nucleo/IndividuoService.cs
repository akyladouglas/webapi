using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Services.Helpers;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class IndividuoService : IIndividuoService
    {
        private readonly IIndividuoRepository _individuoRepository;

        #region -- Constructor
        public IndividuoService(IIndividuoRepository individuoRepository)
        {
            _individuoRepository = individuoRepository;
        }
        #endregion

        public int Add(Individuo obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataCadastro = DateTime.Now;
            obj.DataAlteracao = DateTime.Now;
            obj.CodigoAutenticacao = GenericHelperService.GerarCodigoAutenticacao();
            obj.Ativo = true;
            if(obj.Senha != null)
            {
                obj.Senha = CryptoHashService.GenerateSha1Hash(obj.Senha);

            }
            return _individuoRepository.Add(obj);
        }
        public int Update(string id, Individuo obj)
        {
            obj.DataAlteracao = DateTime.Now;
            return _individuoRepository.Update(id, obj);
        }
        public Task Delete(string id)
        {
            return _individuoRepository.Delete(id);
        }

        public Individuo GetById(string id)
        {
            return _individuoRepository.GetById(id);
        }
        public Task<(IEnumerable<Individuo>, int)> GetByParam(Individuo filters, string sort, int? skip, int? take)
        {
            return _individuoRepository.GetByParam(filters, sort, skip, take);
        }
        public Individuo Login(string usuario, string senha)
        {
            return _individuoRepository.Login(usuario, senha.GenerateSha1Hash());
        }
        public Individuo LoginPortal(string cpf)
        {
            return _individuoRepository.LoginPortal(cpf);
        }
        public int UpdateSenha(string individuoId, string senha, string acao)
        {
            return _individuoRepository.UpdateSenha(individuoId, senha.GenerateSha1Hash(), acao);
        }
        public string UpdateCodigoAutenticacao(string individuoId)
        {
            return _individuoRepository.UpdateCodigoAutenticacao(individuoId, GenericHelperService.GerarCodigoAutenticacao());
        }
        public int AtualizarComorbidade(string id, Individuo obj)
        {
            return _individuoRepository.AtualizarComorbidade(id, obj);
        }
        public int AtualizarComorbidadeTriagem(string id, Individuo obj)
        {
            return _individuoRepository.AtualizarComorbidadeTriagem(id, obj);
        }
        public Individuo GetByCpf(string cpf)
        {
            return _individuoRepository.GetByCpf(cpf);
        }
        public Individuo GetByCns(string cns)
        {
            return _individuoRepository.GetByCns(cns);
        }
        public int UpdateNotificacaoToken(string id, string token)
        {
            return _individuoRepository.UpdateNotificacaoToken(id, token);
        }
        public object GenerateFaceToken(string usuario)
        {
            return _individuoRepository.GenerateFaceToken(usuario);
        }
        public Task<(IEnumerable<Individuo>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            return _individuoRepository.PendentesIntegracao(mes, ano, estabelecimentoId);
        }

        public void ConfirmarIntegracao(int lote, Individuo obj)
        {
            _individuoRepository.ConfirmarIntegracao(lote, obj);
        }
    }
}