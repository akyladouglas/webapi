using Dapper;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class UnidadeFederativaRepository : BaseRepository, IUnidadeFederativaRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"Select UnidadeFederativa.UfAbreviado, UnidadeFederativa.UfExtenso From UnidadeFederativa");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(UnidadeFederativa.Id) From UnidadeFederativa");
        private readonly MySqlConnection _connection;

        public UnidadeFederativaRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public UnidadeFederativa GetById(string id)
        {
            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" And UnidadeFederativa.UfAbreviado = @UfAbreviado");
            param.Add("@UfAbreviado", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            _queryAll.Append(queryFilter);
            #endregion

            return _connection.Query<UnidadeFederativa>(
                _queryAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text)
              .SingleOrDefault();
        }

        public async Task<(IEnumerable<UnidadeFederativa>, int)> GetByParam(UnidadeFederativa cidadeFilters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();
            if (cidadeFilters != null)
            {
                if (!string.IsNullOrEmpty(cidadeFilters.UfExtenso))
                {
                    queryFilter.Append(" And UnidadeFederativa.UfExtenso = ?UfExtenso)");
                    param.Add("?UfExtenso", dbType: DbType.String, value: cidadeFilters.UfExtenso, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(cidadeFilters.UfAbreviado))
                {
                    queryFilter.Append(" And UnidadeFederativa.UfAbreviado = ?UfAbreviado");
                    param.Add("?UfAbreviado", dbType: DbType.String, value: cidadeFilters.UfAbreviado, direction: ParameterDirection.Input);
                }
            }
            _queryAll.Append(queryFilter);
            _queryCountAll.Append(queryFilter);

            if (skip.HasValue && take.HasValue)
            {
                _queryAll.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            try
            {
                var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text).SingleOrDefault();

                if (totalCount > 0)
                {
                    var x = await _connection.QueryAsync<UnidadeFederativa>(_queryAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text);
                    return (x, totalCount);
                }
                return (null, 0);
            }
            catch (System.Exception e)
            {

                throw e;
            }

        }

    }
}
