using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class EstabelecimentoProcedimentoHorarioApplication : IEstabelecimentoProcedimentoHorarioApplication
    {
        private readonly IEstabelecimentoProcedimentoHorarioService _estabelecimentoProcedimentoHorarioService;
        #region -- Constructor
        public EstabelecimentoProcedimentoHorarioApplication(IEstabelecimentoProcedimentoHorarioService estabelecimentoProcedimentoHorarioService)
        {
            _estabelecimentoProcedimentoHorarioService = estabelecimentoProcedimentoHorarioService;
        }
        #endregion

        public Task<(IEnumerable<EstabelecimentoProcedimentoHorario>, int)> GetByParam(EstabelecimentoProcedimentoHorario filters, AppParams appParams, string sort, int? skip, int? take)
        {
            return _estabelecimentoProcedimentoHorarioService.GetByParam(filters, appParams, sort, skip, take);
        }

        public void Add(IEnumerable<EstabelecimentoProcedimentoHorario> list)
        {
            _estabelecimentoProcedimentoHorarioService.Add(list);
        }

        public int Update(string id, EstabelecimentoProcedimentoHorario obj)
        {
            return _estabelecimentoProcedimentoHorarioService.Update(id, obj);
        }
    }
}