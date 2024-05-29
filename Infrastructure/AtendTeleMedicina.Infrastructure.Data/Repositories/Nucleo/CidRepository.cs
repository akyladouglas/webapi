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
    public class CidRepository : BaseRepository, ICidRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"Select Cid10.Id, Cid10.Codigo, Cid10.Descricao, Cid10.Sexo From Cid10");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(Cid10.Id) From Cid10");
        private readonly MySqlConnection _connection;

        public CidRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public Cid GetById(string id)
        {
            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" And Cid10.Id = @Id");
            param.Add("@Id", dbType: DbType.Int32, value: id, direction: ParameterDirection.Input);
            _queryAll.Append(queryFilter);
            #endregion

            return _connection.Query<Cid>(
                _queryAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text)
              .SingleOrDefault();
        }

        public async Task<(IEnumerable<Cid>, int)> GetByParam(Cid cidFilters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();
            if (cidFilters != null)
            {
                if (!string.IsNullOrEmpty(cidFilters.Codigo) && !string.IsNullOrEmpty(cidFilters.Descricao))
                {
                    queryFilter.Append(" AND Cid10.Codigo LIKE ?Codigo OR Cid10.Descricao LIKE ?Descricao ");
                    param.Add("?Descricao", dbType: DbType.String, value: "%" + cidFilters.Descricao + "%", direction: ParameterDirection.Input);
                    param.Add("?Codigo", dbType: DbType.String, value: "%" + cidFilters.Codigo + "%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(cidFilters.Sexo))
                {
                    queryFilter.Append(" AND (Cid10.Sexo = '*' OR Cid10.Sexo = ?Sexo)");
                    param.Add("?Sexo", dbType: DbType.String, value: cidFilters.Sexo, direction: ParameterDirection.Input);
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
                    var x = await _connection.QueryAsync<Cid>(_queryAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text);
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
