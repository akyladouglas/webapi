using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class AvaliacaoApplication : IAvaliacaoApplication
    {

        private readonly IAvaliacaoService _avaliacaoService;
        #region -- Constructor
        public AvaliacaoApplication(IAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }
        #endregion
        public int Add(Avaliacao obj)
        {
            return _avaliacaoService.Add(obj);
        }

    }

}