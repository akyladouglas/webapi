using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class NotificacaoApplication : INotificacaoApplication
    {
        private readonly INotificacaoService _notificacaoService;

        #region -- Constructor
        public NotificacaoApplication(INotificacaoService notificacaoService)
        {
            _notificacaoService = notificacaoService;
        }

        public int Add(Notificacao notificacao)
        {
            return _notificacaoService.Add(notificacao);
        }

        public int Delete(string id)
        {
            return _notificacaoService.Delete(id);
        }

        public Notificacao GetById(string id)
        {
            return _notificacaoService.GetById(id);
        }

        public Task<(IEnumerable<Notificacao>, int)> GetByParam(Notificacao notificacaoFilters, string sort, int? skip, int? take)
        {
            return _notificacaoService.GetByParam(notificacaoFilters, sort, skip, take);
        }

        public int Update(string id, Notificacao notificacao)
        {
            return _notificacaoService.Update(id, notificacao);
        }
        #endregion
    }
}
