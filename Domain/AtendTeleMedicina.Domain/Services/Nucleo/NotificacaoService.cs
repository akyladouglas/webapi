using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class NotificacaoService : INotificacaoService
    {
        private INotificacaoRepository _notificacaoRepository;

        #region -- Constructor
        public NotificacaoService(INotificacaoRepository notificacaoRepository)
        {
            _notificacaoRepository = notificacaoRepository;
        }
        #endregion

        public int Add(Notificacao obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataCadastro = DateTime.Now;
            return _notificacaoRepository.Add(obj);
        }

        public int Delete(string id)
        {
            return _notificacaoRepository.Delete(id);
        }

        public Notificacao GetById(string id)
        {
            return _notificacaoRepository.GetById(id);
        }

        public Task<(IEnumerable<Notificacao>, int)> GetByParam(Notificacao filters, string sort, int? skip, int? take)
        {
            return _notificacaoRepository.GetByParam(filters, sort, skip, take);
        }

        public int Update(string id, Notificacao obj)
        {
            return _notificacaoRepository.Update(id, obj);
        }
    }
}
