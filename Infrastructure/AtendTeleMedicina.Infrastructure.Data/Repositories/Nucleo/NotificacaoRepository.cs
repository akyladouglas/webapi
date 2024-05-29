using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class NotificacaoRepository : BaseRepository, INotificacaoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"Select 
                CONCAT(Notificacao.Id, '') As Id, 
                Notificacao.Titulo, 
                Notificacao.Descricao, 
                Notificacao.DataCadastro, 
                Notificacao.DataAlteracao, 
                Notificacao.Deletado,
                CAST(IndividuoNotificacao.IndividuoId AS CHAR)
                FROM  Notificacao
                LEFT JOIN IndividuoNotificacao
                ON IndividuoNotificacao.NotificacaoId = Notificacao.Id 
     
");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
             @"Select Count(Notificacao.Id) FROM Notificacao
                LEFT JOIN IndividuoNotificacao
                ON IndividuoNotificacao.NotificacaoId = Notificacao.Id 
");

        private readonly MySqlConnection _connection;

        public NotificacaoRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public int Add(Notificacao notificacao)
        {
            {
                const string insertQuery = @"
                    INSERT INTO Notificacao (Id, Titulo, Descricao, DataCadastro, DataAlteracao, Deletado) 
                    VALUES (@Id, @Titulo, @Descricao, @DataCadastro, @DataAlteracao, @Deletado)";

                //Connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(insertQuery, notificacao, transaction);
                    // TODO Adicionar linha para registrar no LOG
                    transaction.Commit();
                    return ret;
                }
            }
        }

        public int Delete(string id)
        {
            var query = @"UPDATE Notificacao SET Deletado = 1 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                // TODO Adicionar linha para registrar no LOG
                transaction.Commit();
                return ret;
            }
        }

        public Notificacao GetById(string id)
        {
            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND Notificacao.Id = @Id");
            param.Add("@Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            _queryAll.Append(queryFilter);
            #endregion

            return _connection.Query<Notificacao>(
                _queryAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text
            ).SingleOrDefault();
        }

        public async Task<(IEnumerable<Notificacao>, int)> GetByParam(Notificacao notificacaoFilters, string sort, int? skip, int? take)
        {
            var queryFilter = new StringBuilder("");
            var param = new DynamicParameters();
            if (notificacaoFilters != null)
            {


                #region -- -- Filtros / Ordenação
                queryFilter.Append(" WHERE 1 = 1 ");


                if (!string.IsNullOrEmpty(notificacaoFilters.Titulo))
                {
                    queryFilter.Append(" AND Notificacao.Titulo LIKE ?Titulo ");
                    param.Add("?Titulo", dbType: DbType.String, value: $"%{notificacaoFilters.Titulo}", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(notificacaoFilters.Descricao))
                {
                    queryFilter.Append(" AND Notificacao.Descricao LIKE ?Descricao ");
                    param.Add("?Descricao", dbType: DbType.String, value: $"%{notificacaoFilters.Descricao}", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(notificacaoFilters.IndividuoId))
                {
                    queryFilter.Append(" AND IndividuoNotificacao.IndividuoId = ?IndividuoId");
                    queryFilter.Append(" OR IndividuoNotificacao.IndividuoId IS NULL");
                    param.Add("?IndividuoId", dbType: DbType.String, value: notificacaoFilters.IndividuoId, direction: ParameterDirection.Input);
                }

                if (notificacaoFilters.Deletado.HasValue)
                {
                    queryFilter.Append(" AND Deletado = ?Deletado ");
                    param.Add("?Deletado", dbType: DbType.Boolean, value: notificacaoFilters.Deletado, direction: ParameterDirection.Input);
                }
            }

            _queryAll.Append(queryFilter);
            _queryCountAll.Append(queryFilter);

            if (skip.HasValue && take.HasValue)
            {
                _queryAll.Append(" GROUP BY Notificacao.Id");
                _queryAll.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            var totalCount = _connection.Query<int>(
                _queryCountAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text
            ).SingleOrDefault();

            if (totalCount > 0)
            {
                var x = await _connection.QueryAsync<Notificacao>(
                    _queryAll.ToString(),
                    param,
                    commandTimeout: 60,
                    commandType: CommandType.Text
                );

                return (x, totalCount);
            }
            return (null, 0);
        }

        public int Update(string id, Notificacao notificacao)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (notificacao == null) return 0;

            const string updateQuery = @"
                UPDATE Notificacao SET
                    Titulo = ?Titulo,
                    Descricao = ?Descricao,
                    DataAlteracao = ?DataAlteracao,
                    Deletado = ?Deletado
                WHERE Id = ?Id";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?Titulo", dbType: DbType.String, value: notificacao.Titulo, direction: ParameterDirection.Input);
            param.Add("?Descricao", dbType: DbType.String, value: notificacao.Descricao, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: notificacao.DataAlteracao, direction: ParameterDirection.Input);
            param.Add("?Deletado", dbType: DbType.Boolean, value: notificacao.Deletado, direction: ParameterDirection.Input);


            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(updateQuery, param, transaction);
                // TODO Adicionar linha para registrar no LOG
                transaction.Commit();
                return ret;
            }
        }
    }
}
