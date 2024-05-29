using AtendTeleMedicina.Application.Contracts.Integracao;
using AtendTeleMedicina.Domain.Contracts.Services.Integracao;
using AtendTeleMedicina.Domain.Entities.Integracao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Integracao
{
    public class LoteApplication : ILoteApplication
    {
        private readonly ILoteService _loteService;
        #region -- Constructor
        public LoteApplication(ILoteService loteService)
        {
            _loteService = loteService;
        }
        #endregion
        public Task<(IEnumerable<Lote>, int)> ListarLote(int mes, int ano, string cnes, int? skip = 1, int? take = 50)
        {
            return _loteService.ListarLote(mes, ano, cnes, skip, take);
        }

        public Lote GetById(int loteId)
        {
            return _loteService.GetById(loteId);
        }
        public Lote GetDetailedById(int loteId)
        {
            return _loteService.GetDetailedById(loteId);
        }
        public Lote ExisteLote(int mes, int ano, string estabelecimentoId)
        {
            return _loteService.ExisteLote(mes, ano, estabelecimentoId);
        }
        public void AtualizarOuInserir(Lote entity)
        {
            _loteService.AtualizarOuInserir(entity);
        }

    }
}
