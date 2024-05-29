using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class OcupacaoRepository : BaseRepository, IOcupacaoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"Select * From Ocupacao");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(Ocupacao.Id) From Ocupacao");
        private readonly MySqlConnection _connection;

        public OcupacaoRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public async Task<Ocupacao> GetByCbo(string cbo)
        {
            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(cbo)) return null;

            queryFilter.Append(" And Ocupacao.Codigo = @Codigo");
            param.Add("@Codigo", dbType: DbType.Int32, value: cbo, direction: ParameterDirection.Input);
            _queryAll.Append(queryFilter);
            #endregion

            return await _connection.QueryAsync<Ocupacao>(
                _queryAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text)
                .ContinueWith(task => task.Result.FirstOrDefault());
        }

        public async Task<(IEnumerable<Ocupacao>, int)> GetByParam(Ocupacao filters, bool cboEsus, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Codigo))
                {
                    queryFilter.Append(" And Ocupacao.Codigo = ?Codigo)");
                    param.Add("?Codigo", dbType: DbType.String, value: filters.Codigo, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Descricao))
                {
                    queryFilter.Append(" And Ocupacao.Descricao = ?Descricao)");
                    param.Add("?Descricao", dbType: DbType.String, value: filters.Descricao, direction: ParameterDirection.Input);
                }

                if (cboEsus) queryFilter.Append(" And Ocupacao.CboEsus = TRUE");
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
                    var x = await _connection.QueryAsync<Ocupacao>(_queryAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text);
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