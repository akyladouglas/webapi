using AtendTeleMedicina.Domain.Entities.Integracao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Integracao
{
    public interface ILoteRepository
    {
        Lote ExisteLote(int mes, int ano, string cnes);
        Task<(IEnumerable<Lote>, int)> ListarLote(int mes, int ano, string cnes, int? skip = 1, int? take = 50);
        Lote GetById(int loteId);
        Lote GetDetailedById(int id);
        bool Existe(int id);
        void Adicionar(Lote entity);
        void Atualizar(Lote entity);
    }
}
