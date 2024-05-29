using AtendTeleMedicina.Domain.Entities.Integracao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Integracao
{
    public interface ILoteApplication
    {
        Lote ExisteLote(int mes, int ano, string cnes);
        Lote GetById(int loteId);
        Lote GetDetailedById(int loteId);
        Task<(IEnumerable<Lote>, int)> ListarLote(int mes, int ano, string cnes, int? skip = 1, int? take = 50);
        void AtualizarOuInserir(Lote entity);
    }
}
