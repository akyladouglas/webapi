using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;

        #region -- Constructor
        public AvaliacaoService(IAvaliacaoRepository avaliacaoRepository)
        {
            _avaliacaoRepository = avaliacaoRepository;
        }
        #endregion

        public int Add(Avaliacao obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataAvaliacao = DateTime.Now;
             return _avaliacaoRepository.Add(obj);
        }
    }
}
