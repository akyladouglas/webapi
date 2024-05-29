using Dapper;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class CidadeRepository : BaseRepository, ICidadeRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"Select Cidade.Ibge, Cidade.Nome, Cidade.UfAbreviado, Cidade.UfExtenso, Cidade.Ativo FROM Cidade");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(Cidade.Ibge) FROM Cidade");
        private readonly MySqlConnection _connection;

        public CidadeRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public int Add(Cidade obj)
        {
            const string insertQuery = @"
              INSERT INTO Cidade (Id, Ibge, Nome, UfAbreviado, UfExtenso, Ativo) 
              VALUES (@Id, @Ibge, @Nome, @UfAbreviado, @UfExtenso, @Ativo)";

            //Connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, obj, transaction);
                // TODO Adicionar linha para registrar no LOG
                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Cidade obj, string userId)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            const string updateQuery = @"
              UPDATE Cidade SET
               Ibge = @Ibge,
               Nome = @Nome, 
               UfAbreviado = @UfAbreviado, 
               UfExtenso = @UfExtenso,
               Ativo = @Ativo
              WHERE Id = @Id";

            var param = new DynamicParameters();

            if (obj.Ibge != 0)
            {
                param.Add("@Ibge", dbType: DbType.Int32, value: obj.Ibge, direction: ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(obj.Nome))
            {
                param.Add("@Nome", dbType: DbType.String, value: obj.Nome, direction: ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(obj.UfAbreviado))
            {
                param.Add("@UfAbreviado", dbType: DbType.String, value: obj.UfAbreviado, direction: ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(obj.UfExtenso))
            {
                param.Add("@UfExtenso", dbType: DbType.String, value: obj.UfExtenso, direction: ParameterDirection.Input);
            }
            param.Add("@Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(updateQuery, param, transaction);
                // TODO Adicionar linha para registrar no LOG
                transaction.Commit();
                return ret;
            }

        }

        public Cidade GetById(string id)
        {
            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" And Cidade.Ibge = @Id");
            param.Add("@Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            _queryAll.Append(queryFilter);
            #endregion

            return _connection.Query<Cidade>(
                _queryAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text)
              .SingleOrDefault();
        }

        public async Task<(IEnumerable<Cidade>, int)> GetByParam(Cidade filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {

                if (filters.Ibge != 0)
                {
                    queryFilter.Append(" And Cidade.Ibge = ?Ibge");
                    param.Add("?Ibge", dbType: DbType.Int32, value: filters.Ibge, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Nome))
                {
                    queryFilter.Append(" And Cidade.Nome LIKE ?Nome ");
                    param.Add("?Nome", dbType: DbType.String, value: $"%{filters.Nome}", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.UfAbreviado))
                {
                    queryFilter.Append(" And Cidade.UfAbreviado = ?UfAbreviado");
                    param.Add("?UfAbreviado", dbType: DbType.String, value: filters.UfAbreviado, direction: ParameterDirection.Input);
                }
                if (filters.Capital)
                {
                    queryFilter.Append(" And Cidade.Capital = ?Capital");
                    param.Add("?Capital", dbType: DbType.Boolean, value: filters.Capital, direction: ParameterDirection.Input);
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

            var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                var x = await _connection.QueryAsync<Cidade>(_queryAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text);
                return (x, totalCount);
            }
            return (null, 0);
        }

    }
}
