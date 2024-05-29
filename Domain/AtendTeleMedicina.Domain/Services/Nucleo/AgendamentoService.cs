using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        #region -- Constructor
        public AgendamentoService(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }
        #endregion

        public string Add(Agendamento obj)
        {
            if (obj.Dia == null)
            {
                obj.Dia = DateTime.Now;
            }
            if(obj.Hora == null)
            {
                string h = DateTime.Now.ToString("HH:mm:ss");
                var hora = TimeSpan.Parse(h);
                obj.Hora = hora;
            }
            if (obj.Retorno == null) obj.Retorno = false;
            if (obj.VinculoRetorno == null) obj.VinculoRetorno = false;
            obj.Id = Guid.NewGuid().ToString();
            obj.Cancelado = false;
            obj.DataAlteracao = DateTime.Now;
            obj.DataCadastro = DateTime.Now;
            return _agendamentoRepository.Add(obj);
        } 
        public int Update(string id, Agendamento obj)
        {
            return _agendamentoRepository.Update(id, obj);
        }
        public int UpdateSinaisVitais(string id, Agendamento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            return _agendamentoRepository.UpdateSinaisVitais(id, obj);
        }

        public int UpdateConfirmarPresenca(string id, Agendamento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            return _agendamentoRepository.UpdateConfirmarPresenca(id, obj);
        }
        public int UpdateFinalizarMedico(string id, Agendamento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            obj.DataFim = DateTime.Now;
            return _agendamentoRepository.UpdateFinalizarMedico(id, obj);
        }
        public int UpdateEmAtendimentoMedico(string id, Agendamento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            obj.DataInicio = DateTime.Now;
            return _agendamentoRepository.UpdateEmAtendimentoMedico(id, obj);
        }
        public int UpdateFinalizarTriagem(string id, Agendamento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            return _agendamentoRepository.UpdateFinalizarTriagem(id, obj);
        }
        public int Delete(string id)
        {
            return _agendamentoRepository.Delete(id);
        }

        public Agendamento GetById(string id)
        {
            return _agendamentoRepository.GetById(id);
        }
        public Task<(IEnumerable<Agendamento>, int)> GetByParam(Agendamento filters, string sort, int? skip = null, int? take = null)
        {
            return _agendamentoRepository.GetByParam(filters, sort, skip, take);
        }
        public Task<(IEnumerable<Agendamento>, int)> GetAusentes(Agendamento filters, string sort, int? skip, int? take)
        {
            return _agendamentoRepository.GetAusentes(filters, sort, skip, take);
        }
        public int UpdateRecepcao(string id, Agendamento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            return _agendamentoRepository.UpdateRecepcao(id, obj);
        }
        public int UpdateTriagem(string id, Agendamento obj)
        {
            obj.DataAlteracao = DateTime.Now;
            return _agendamentoRepository.UpdateTriagem(id, obj);
        }

        public Agendamento GetByEstabelecimentoByIndividuoId(string id)
        {
            return _agendamentoRepository.GetByEstabelecimentoByIndividuoId(id);
        }

        public Task<(IEnumerable<Agendamento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            return _agendamentoRepository.PendentesIntegracao(mes, ano, estabelecimentoId);
        }
        public void ConfirmarIntegracao(int lote, Agendamento obj)
        {
            _agendamentoRepository.ConfirmarIntegracao(lote, obj);
        }
    }
}
