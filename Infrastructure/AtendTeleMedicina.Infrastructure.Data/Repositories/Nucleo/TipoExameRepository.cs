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
    public class TipoExameRepository : BaseRepository, ITipoExameRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(t.Id AS CHAR) AS Id,
                CAST(t.Nome AS CHAR) AS Nome
                FROM TipoExame t
                ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(TipoExame.Id) FROM TipoExame");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public TipoExameRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public async Task<(IEnumerable<TipoExame>, int)> GetByParam(TipoExame filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {

                if (filters.Id != null)
                {
                    queryFilter.Append(" AND t.Id = ?Id ");
                    param.Add("?Id", dbType: DbType.Int32, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Nome))
                {
                    queryFilter.Append(" AND t.Nome LIKE ?Nome ");
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
                var x = await _connection.QueryAsync<TipoExame>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);
                return (x, totalCount);
            }
            return (null, 0);
        }

        //public int Update(string id, Documento obj)
        //{
        //    if (string.IsNullOrEmpty(id)) return 0;
        //    if (obj == null) return 0;

        //    var param = new DynamicParameters();
        //    param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
        //    param.Add("?ProcedimentoId", dbType: DbType.String, value: obj.ProcedimentoId, direction: ParameterDirection.Input);
        //    param.Add("?AgendamentoId", dbType: DbType.String, value: obj.AgendamentoId, direction: ParameterDirection.Input);
        //    param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
        //    param.Add("?Url", dbType: DbType.String, value: obj.Url, direction: ParameterDirection.Input);
        //    param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);

        //    const string updateQuery = @"UPDATE Documento SET
        //                        IndividuoId = ?IndividuoId, 
        //                        ProcedimentolId = ?ProcedimentolId, 
        //                        AgendamentoId = ?AgendamentoId,
        //                        Profissionalid = ?Profissionalid,
        //                        Url = ?Url";

        //    try
        //    {
        //        using (var transaction = _connection.BeginTransaction())
        //        {
        //            var user = _user.GetUserId();
        //            var origem = _user.GetUserOrigem();
        //            var ip = _user.GetUserIp();

        //            var ret = _connection.Execute(updateQuery, param, transaction);
        //            RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Documento", obj.Id.ToString(), "Editou Documento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        //            transaction.Commit();
        //            return ret;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //public int Delete(string id)
        //{
        //    var query = @"UPDATE Documento SET Ativo = 0 WHERE Id = ?Id";
        //    var param = new DynamicParameters();
        //    param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        var ret = _connection.Execute(query, param, transaction);
        //        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Documento", id.ToString(), "Desativou Documento", DateTime.Now, AtorAuditoria.Origem, AtorAuditoria.Id, AtorAuditoria.Ip);
        //        transaction.Commit();
        //        return ret;
        //    }
        //}

        public TipoExame GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND t.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var dictionary = new Dictionary<string, TipoExame>();

            try
            {
                return _connection.Query<TipoExame>(
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

        //public async Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take)
        //{
        //    #region -- -- Filtros / Ordenação
        //    var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
        //    var param = new DynamicParameters();
        //    if (filters != null)
        //    {
        //        if (!string.IsNullOrEmpty(filters?.Id))
        //        {
        //            queryFilter.Append(" AND Id = ?Id");
        //            param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
        //        }
        //        if (!string.IsNullOrEmpty(filters?.IndividuoId))
        //        {
        //            queryFilter.Append(" And IndividuoId = ?IndividuoId");
        //            param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
        //        }
        //        if (!string.IsNullOrEmpty(filters?.ProfissionalId))
        //        {
        //            queryFilter.Append(" And ProfissionalId = ?ProfissionalId");
        //            param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
        //        }
        //        if (!string.IsNullOrEmpty(filters?.ProcedimentoId))
        //        {
        //            queryFilter.Append(" And ProcedimentoId Like ?ProcedimentoId");
        //            param.Add("?ProcedimentoId", dbType: DbType.String, value: filters.ProcedimentoId, direction: ParameterDirection.Input);
        //        }

        //        if (!string.IsNullOrEmpty(filters?.AgendamentoId))
        //        {
        //            queryFilter.Append(" And AgendamentoId = ?AgendamentoId");
        //            param.Add("?AgendamentoId", dbType: DbType.String, value: filters.AgendamentoId, direction: ParameterDirection.Input);
        //        }

        //    }

        //    _queryAll.Append(queryFilter);
        //    _queryCountAll.Append(queryFilter);

        //    if (skip.HasValue && take.HasValue)
        //    {
        //        _queryAll.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
        //        skip = (skip - 1) * take;
        //        param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
        //        param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
        //    }
        //    #endregion

        //    var totalCount = _connection.Query<int>(
        //        _queryCountAll.ToString(),
        //        param,
        //        commandTimeout: 60,
        //        commandType: CommandType.Text
        //    ).SingleOrDefault();

        //    if (totalCount > 0)
        //    {
        //        var x = await _connection.QueryAsync<Documento>(
        //            _queryAll.ToString(),
        //            param,
        //            commandTimeout: 60,
        //            commandType: CommandType.Text
        //        );

        //        return (x, totalCount);
        //    }
        //    return (null, 0);
        //}
    }
}
