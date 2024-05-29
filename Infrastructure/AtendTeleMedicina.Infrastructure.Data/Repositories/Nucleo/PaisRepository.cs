using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using Dapper;
using MySql.Data.MySqlClient;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class PaisRepository : BaseRepository, IPaisRepository
    {
        private readonly IUser _user;

        private readonly StringBuilder _queryAll = new StringBuilder(
       @"SELECT
            p.Id,
            p.Nome
        From Pais p");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
      @"SELECT COUNT(p.Id) FROM Pais p");
        private readonly MySqlConnection _connection;

        public PaisRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _user = user;
            _connection = context.Connection;
        }

        public async Task<(IEnumerable<Pais>, int)> GetByParam(Pais filters, string sort, int? skip, int? take)
        {

            var query = _queryAll;

            var queryCount = new StringBuilder();
            queryCount.Append(_queryCountAll);


            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Nome))
                {
                    queryFilter.Append(" AND p.Nome = ?Nome ");
                    param.Add("?Nome", dbType: DbType.String, value: filters.Nome, direction: ParameterDirection.Input);
                }
            }
            query.Append(queryFilter);
            queryCount.Append(queryFilter);


            if (skip.HasValue && take.HasValue)
            {
                query.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            try
            {
                var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

                if (totalCount > 0)
                {
                    var x = await _connection.QueryAsync<Pais>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);
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
