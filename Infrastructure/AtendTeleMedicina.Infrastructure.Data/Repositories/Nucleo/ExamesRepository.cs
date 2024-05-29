using Dapper;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class ExamesRepository : BaseRepository, IExamesRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(r.Id AS CHAR) AS Id,
                CAST(r.IndividuoId AS CHAR) AS IndividuoId,
                CAST(r.TipoExameId AS CHAR) AS TipoExameId,
                r.DataDeEnvio,
                r.Url,
                r.Formato,
                r.Nome,
                r.Solicitado,
                r.Avaliado,
                r.Resultado,
                CAST(r.ProfissionalId AS CHAR) AS ProfissionalId,
                r.Descricao,
                r.Finalizado,
                r.Deletado,
                CAST(i.Id AS CHAR) AS 'Id Do Individuo'
                FROM ReceberExame r 
                LEFT JOIN Individuo i ON i.Id = r.IndividuoId         
                LEFT JOIN TipoExame t ON t.Id = r.TipoExameId
                LEFT JOIN Profissional p ON p.Id = r.ProfissionalId 
                ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(r.Id) FROM ReceberExame r");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public ExamesRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public int Add(Exames obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?TipoExameId", dbType: DbType.String, value: obj.TipoExame.Id, direction: ParameterDirection.Input);
            param.Add("?DataDeEnvio", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?Url", dbType: DbType.String, value: obj.Url, direction: ParameterDirection.Input);
            param.Add("?Formato", dbType: DbType.String, value: obj.Formato, direction: ParameterDirection.Input);
            param.Add("?Nome", dbType: DbType.String, value: obj.Nome, direction: ParameterDirection.Input);

            const string insertQuery = @"INSERT INTO ReceberExame (Id, IndividuoId, TipoExameId, DataDeEnvio, Url, Formato, Nome)
                                                     VALUES (?Id, ?IndividuoId, ?TipoExameId, ?DataDeEnvio, ?Url, ?Formato, ?Nome)";
            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Exame", obj.Id.ToString(), "Inseriu Exame", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int Update(string id, Exames obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?TipoExameId", dbType: DbType.String, value: obj.TipoExameId, direction: ParameterDirection.Input);
            param.Add("?DataDeEnvio", dbType: DbType.DateTime, value: obj.DataDeEnvio, direction: ParameterDirection.Input);
            param.Add("?Url", dbType: DbType.String, value: obj.Url, direction: ParameterDirection.Input);
            param.Add("?Formato", dbType: DbType.String, value: obj.Formato, direction: ParameterDirection.Input);
            param.Add("?Nome", dbType: DbType.String, value: obj.Nome, direction: ParameterDirection.Input);
            param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
            param.Add("?Solicitado", dbType: DbType.DateTime, value: obj.Solicitado, direction: ParameterDirection.Input);
            param.Add("?Avaliado", dbType: DbType.DateTime, value: obj.Avaliado, direction: ParameterDirection.Input);
            param.Add("?Resultado", dbType: DbType.Boolean, value: obj.Resultado, direction: ParameterDirection.Input);
            param.Add("?Descricao", dbType: DbType.String, value: obj.Descricao, direction: ParameterDirection.Input);
            param.Add("?Finalizado", dbType: DbType.Boolean, value: obj.Finalizado, direction: ParameterDirection.Input);
            param.Add("?Deletado", dbType: DbType.Boolean, value: obj.Deletado, direction: ParameterDirection.Input);

            const string updateQuery = @"UPDATE ReceberExame SET
                                IndividuoId = ?IndividuoId, 
                                TipoExameId = ?TipoExameId,
                                DataDeEnvio = ?DataDeEnvio,
                                Url = ?Url,
                                Formato = ?Formato,
                                Nome = ?Nome,
                                ProfissionalId = ?ProfissionalId,
                                Solicitado = ?Solicitado,
                                Avaliado = ?Avaliado,
                                Resultado = ?Resultado,
                                Descricao = ?Descricao,
                                Finalizado = ?Finalizado,
                                Deletado = ?Deletado
                                WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "ReceberExame", obj.Id.ToString(), "Editou Documento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task Delete(string id)
        {
            try
            {
                var query = @"UPDATE ReceberExame 
                            SET Deletado = TRUE
                            WHERE Id = ?Id";
                var param = new DynamicParameters();
                param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

                using (var transaction = _connection.BeginTransaction())
                {
                    _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", id.ToString(), "Desativou o Indivíduo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return Task.CompletedTask;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Exames GetById(string id)
        {
            var query = new StringBuilder(@"SELECT
                r.IndividuoId AS Id,
                r.TipoExameId,
                r.DataDeEnvio,
                r.Formato,
                r.Url
                FROM ReceberExame r
                 ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND r.IndividuoId = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var dictionary = new Dictionary<string, Exames>();

            try
            {
                return _connection.Query<Exames>(
                        query.ToString(),
                        param,
                        commandTimeout: 60,
                        commandType: CommandType.Text)
                        .SingleOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public async Task<(IEnumerable<Exames>, int)> GetByParam(Exames filters, string sort, int? skip, int? take)
        {
            StringBuilder queryAll = new StringBuilder(
            @"SELECT 
                CAST(r.Id AS CHAR) AS Id,
                CAST(r.IndividuoId AS CHAR) AS IndividuoId,
                CAST(r.TipoExameId AS CHAR) AS TipoExameId,
                r.DataDeEnvio,
                r.Url,
                r.Formato,
                r.Nome,
                r.Solicitado,
                r.Avaliado,
                r.Resultado,
                CAST(r.ProfissionalId AS CHAR) AS ProfissionalId,
                r.Descricao,
                r.Finalizado,
                r.Deletado,
                CAST(i.Id AS CHAR) AS 'Id Do Individuo'
                FROM ReceberExame r 
                LEFT JOIN Individuo i ON i.Id = r.IndividuoId         
                LEFT JOIN TipoExame t ON t.Id = r.TipoExameId
                LEFT JOIN Profissional p ON p.Id = r.ProfissionalId 
                ");

            StringBuilder queryCountAll = new StringBuilder(
                @"SELECT COUNT(r.Id) FROM ReceberExame r");


            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters?.Id))
                {
                    queryFilter.Append(" AND r.Id = ?Id");
                    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.IndividuoId))
                {
                    queryFilter.Append(" AND r.IndividuoId = ?IndividuoId");
                    param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.TipoExameId))
                {
                    queryFilter.Append(" AND r.TipoExameId Like ?TipoExameId");
                    param.Add("?TipoExameId", dbType: DbType.String, value: filters.TipoExameId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.Nome))
                {
                    queryFilter.Append(" AND r.Nome Like ?Nome");
                    param.Add("?Nome", dbType: DbType.String, value: $"%{filters.Nome}%", direction: ParameterDirection.Input);
                }


                if (filters.Resultado.HasValue)
                {
                    queryFilter.Append(" AND r.Resultado = ?Resultado ");
                    param.Add("?Resultado", dbType: DbType.Boolean, value: filters.Resultado, direction: ParameterDirection.Input);
                }
                if (filters.Finalizado.HasValue)
                {
                    queryFilter.Append(" AND r.Finalizado = ?Finalizado ");
                    param.Add("?Finalizado", dbType: DbType.Boolean, value: filters.Finalizado, direction: ParameterDirection.Input);
                }
                if (filters.Deletado.HasValue)
                {
                    queryFilter.Append(" AND r.Deletado = ?Deletado ");
                    param.Add("?Deletado", dbType: DbType.Boolean, value: filters.Deletado, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.Formato))
                {
                    queryFilter.Append(" AND r.Formato = ?Formato ");
                    param.Add("?Formato", dbType: DbType.String, value: filters.Formato, direction: ParameterDirection.Input);
                }
            }

            queryAll.Append(queryFilter);
            queryCountAll.Append(queryFilter);

            if (skip.HasValue && take.HasValue)
            {
                queryAll.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            var totalCount = _connection.Query<int>(
                queryCountAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text
            ).SingleOrDefault();

            if (totalCount > 0)
            {
                var x = await _connection.QueryAsync<Exames>(
                    queryAll.ToString(),
                    param,
                    commandTimeout: 60,
                    commandType: CommandType.Text
                );

                return (x, totalCount);
            }
            return (null, 0);
        }

        public Exames GetExame(Exames obj)
        {
            var query = new StringBuilder(@"SELECT 
                CAST(r.Id AS CHAR) AS Id,
                CAST(r.IndividuoId AS CHAR) AS IndividuoId,
                CAST(r.TipoExameId AS CHAR) AS TipoExameId,
                r.DataDeEnvio,
                r.Url,
                r.Formato,
                r.Nome,
                r.Solicitado,
                r.Avaliado,
                r.Resultado,
                CAST(r.ProfissionalId AS CHAR) AS ProfissionalId,
                r.Descricao,
                r.Finalizado,
                CAST(i.Id AS CHAR) AS 'Id Do Individuo'
                FROM ReceberExame r 
                LEFT JOIN Individuo i ON i.Id = r.IndividuoId         
                LEFT JOIN TipoExame t ON t.Id = r.TipoExameId
                LEFT JOIN Profissional p ON p.Id = r.ProfissionalId 
                 ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();


            if (!string.IsNullOrEmpty(obj?.Nome))
            {
                queryFilter.Append(" AND r.Nome Like ?Nome");
                param.Add("?Nome", dbType: DbType.String, value: $"%{obj.Nome}%", direction: ParameterDirection.Input);
            }


            query.Append(" LIMIT 0, 1");


        //            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
        //          query.Append(queryFilter);
            #endregion

        var dictionary = new Dictionary<string, Exames>();

            try
            {
                return _connection.Query<Exames>(
                        query.ToString(),
                        param,
                        commandTimeout: 60,
                        commandType: CommandType.Text)
                        .SingleOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int Count(Exames obj)
        {

            StringBuilder queryCount = new StringBuilder(
               @"SELECT COUNT(r.Id) FROM ReceberExame r");

            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();


            if (!string.IsNullOrEmpty(obj?.Nome))
            {
                queryFilter.Append(" AND r.Nome Like ?Nome");
                param.Add("?Nome", dbType: DbType.String, value: $"%{obj.Nome}%", direction: ParameterDirection.Input);
            }


            queryCount.Append(queryFilter);


            var totalCount = _connection.Query<int>(
                queryCount.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text
            ).SingleOrDefault();

            return totalCount;
        }
    }
}