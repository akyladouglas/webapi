using AtendTeleMedicina.Domain.Contracts.Repositories.Integracao;
using AtendTeleMedicina.Domain.Contracts.Services.Integracao;
using AtendTeleMedicina.Domain.Entities.Integracao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Integracao
{
    public class LoteService : ILoteService
    {
        private readonly ILoteRepository _loteRepository;

        #region -- Constructor
        public LoteService(ILoteRepository loteRepository)
        {
            _loteRepository = loteRepository;
        }
        #endregion
        public Task<(IEnumerable<Lote>, int)> ListarLote(int mes, int ano, string cnes, int? skip = 1, int? take = 50)
        {
            return _loteRepository.ListarLote(mes, ano, cnes, skip, take);
        }

        public Lote ExisteLote(int mes, int ano, string cnes)
        {
            return _loteRepository.ExisteLote(mes, ano, cnes);
        }

        public Lote GetById(int loteId)
        {
            return _loteRepository.GetById(loteId);
        }

        public Lote GetDetailedById(int loteId)
        {
            return _loteRepository.GetDetailedById(loteId);
        }

        public void AtualizarOuInserir(Lote entity)
        {
            if (!_loteRepository.Existe(entity.Id))
                _loteRepository.Adicionar(entity);
            else
                _loteRepository.Atualizar(entity);
        }

    }
}
