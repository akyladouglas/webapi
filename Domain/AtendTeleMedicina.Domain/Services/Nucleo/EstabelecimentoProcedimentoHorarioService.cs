using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class EstabelecimentoProcedimentoHorarioService : IEstabelecimentoProcedimentoHorarioService
    {
        private readonly IEstabelecimentoProcedimentoHorarioRepository _estabelecimentoProcedimentoHorarioRepository;

        #region -- Constructor
        public EstabelecimentoProcedimentoHorarioService(IEstabelecimentoProcedimentoHorarioRepository estabelecimentoProcedimentoHorarioRepository)
        {
            _estabelecimentoProcedimentoHorarioRepository = estabelecimentoProcedimentoHorarioRepository;
        }
        #endregion

        public Task<(IEnumerable<EstabelecimentoProcedimentoHorario>, int)> GetByParam(EstabelecimentoProcedimentoHorario filters, AppParams appParams, string sort, int? skip, int? take)
        {
            return _estabelecimentoProcedimentoHorarioRepository.GetByParam(filters, appParams, sort, skip, take);
        }

        public void Add(IEnumerable<EstabelecimentoProcedimentoHorario> list)
        {
            _estabelecimentoProcedimentoHorarioRepository.Add(list);
        }

        public int Update(string id, EstabelecimentoProcedimentoHorario obj)
        {
            return _estabelecimentoProcedimentoHorarioRepository.Update(id, obj);

        }
    }
}