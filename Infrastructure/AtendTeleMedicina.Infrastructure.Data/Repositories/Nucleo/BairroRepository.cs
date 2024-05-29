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
    public class BairroRepository : BaseRepository, IBairroRepository
    {
        private readonly IUser _user;
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"SELECT
                CAST(Bairro.Id AS CHAR) AS Id,
                Bairro.Nome,
                Bairro.CidadeId,
                Bairro.Ativo
                FROM
                Bairro ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(Bairro.Id) From Bairro");
        private readonly MySqlConnection _connection;

        public BairroRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public int Add(Bairro obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?Nome", dbType: DbType.String, value: obj.Nome, direction: ParameterDirection.Input);
            param.Add("?CidadeId", dbType: DbType.Int32, value: obj.CidadeId, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);

            const string insertQuery = @"
              INSERT INTO Bairro (Id,
                                            Id,
                                            Nome,
                                            CidadeId,
                                            Ativo) 
                                        VALUES (
                                            ?Id,
                                            ?Nome,
                                            ?CidadeId,
                                            ?Ativo)";

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Bairro", obj.Id.ToString(), "Inseriu Bairro", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Bairro obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?Nome", dbType: DbType.String, value: obj.Nome, direction: ParameterDirection.Input);
            param.Add("?CidadeId", dbType: DbType.Int32, value: obj.CidadeId, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);

            const string updateQuery = @"
              UPDATE Bairro SET
                    Id = ?Id,
                    Nome = ?Nome,
                    CidadeId = ?CidadeId,
                    Ativo = ?Ativo
                    WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Bairro", obj.Id.ToString(), "Editou Bairro", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Bairro GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND Bairro.id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            return _connection.Query<Bairro>(
                query.ToString(),
                param,
                commandTimeout: TimeOut,
                commandType: CommandType.Text)
              .SingleOrDefault();
        }

        public async Task<(IEnumerable<Bairro>, int)> GetByParam(Bairro filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {
                
                if (filters.CidadeId != 0)
                {
                    queryFilter.Append(" AND Bairro.CidadeId = ?CidadeId ");
                    param.Add("?CidadeId", dbType: DbType.Int32, value: filters.CidadeId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Nome))
                {
                    queryFilter.Append(" AND Bairro.Nome LIKE ?Nome ");
                    param.Add("?Nome", dbType: DbType.String, value: $"%{filters.Nome}%", direction: ParameterDirection.Input);
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

            var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                var x = await _connection.QueryAsync<Bairro>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);
                return (x, totalCount);
            }
            return (null, 0);
        }

        public int Delete(string id)
        {
            var query = @"UPDATE Bairro SET Ativo = 0 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Bairro", id.ToString(), "Desativou Agente de Fiscalização", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

    }
}
