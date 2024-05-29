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
using System;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class CiapRepository : BaseRepository, ICiapRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"Select Ciap.Id, Ciap.Codigo, Ciap.Descricao, Ciap.Sexo, Ciap.AD, Ciap.AI From Ciap");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(Ciap.Id) From Ciap");
        private readonly MySqlConnection _connection;

        public CiapRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public Ciap GetById(string id)
        {
            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" And Ciap.Id = @Id");
            param.Add("@Id", dbType: DbType.Int32, value: id, direction: ParameterDirection.Input);
            _queryAll.Append(queryFilter);
            #endregion

            return _connection.Query<Ciap>(
                _queryAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text)
              .SingleOrDefault();
        }

        public async Task<(IEnumerable<Ciap>, int)> GetByParam(Ciap ciapFilters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();
            if (ciapFilters != null)
            {
                if (!string.IsNullOrEmpty(ciapFilters.Codigo) && !string.IsNullOrEmpty(ciapFilters.Descricao))
                {
                    queryFilter.Append(" AND Ciap.Codigo LIKE ?Codigo OR Ciap.Descricao LIKE ?Descricao ");
                    param.Add("?Descricao", dbType: DbType.String, value: "%" + ciapFilters.Descricao + "%", direction: ParameterDirection.Input);
                    param.Add("?Codigo", dbType: DbType.String, value: "%" + ciapFilters.Codigo + "%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(ciapFilters.Sexo))
                {
                    queryFilter.Append(" AND (Ciap.Sexo = '*' OR Ciap.Sexo = ?Sexo)");
                    param.Add("?Sexo", dbType: DbType.String, value: ciapFilters.Sexo, direction: ParameterDirection.Input);
                }
                if (ciapFilters.AD.HasValue)
                {
                    queryFilter.Append(" AND Ciap.AD = ?AD ");
                    param.Add("?AD", dbType: DbType.Boolean, value: ciapFilters.AD, direction: ParameterDirection.Input);
                }
                if (ciapFilters.AI.HasValue)
                {
                    queryFilter.Append(" AND Ciap.AI = ?AI ");
                    param.Add("?AI", dbType: DbType.Boolean, value: ciapFilters.AI, direction: ParameterDirection.Input);
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
                    var x = await _connection.QueryAsync<Ciap>(_queryAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text);
                    return (x, totalCount);
                }
                return (null, 0);
            }
            catch (System.Exception e)
            {

                throw e;
            }

        }

        public Task<IEnumerable<Ciap>> GetByManyIds(string[] ciapIds)
        {
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();
            if (ciapIds != null && ciapIds.Length > 0)
            {
                var ids = ciapIds.Select((id, index) => new { id, index });
                foreach (var id in ids)
                {
                    param.Add($"@Id{id.index}", id.id);
                }
                queryFilter.Append(" AND Ciap.Codigo IN (" + string.Join(",", ids.Select(id => $"@Id{id.index}")) + ")");
            }
            _queryAll.Append(queryFilter);

            return _connection.QueryAsync<Ciap>(_queryAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text);
        }

    }
}
