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
    public class ExamesF200Repository : BaseRepository, IExamesF200Repository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(e.Id AS CHAR) AS Id,
                CAST(e.IdPaciente AS CHAR) AS IdPaciente,
                CAST(e.CpfPaciente AS CHAR) AS CpfPaciente,
                e.DataExameEco,
                e.DataTransferenciaEcoPc,
                e.OperadorEco,
                e.TipoExameEco,
                e.ResultadoExameEco,
                e.UnidadeResultadoEco,
                e.ValorReferenciaResultadoEco,
                e.Url,
                e.Formato,
                CAST(i.Id AS CHAR) AS 'Id Do Individuo' 
                FROM examesF200 e 
                LEFT JOIN Individuo i ON i.Id = e.IdPaciente 
                ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(e.Id) FROM examesF200 e");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public ExamesF200Repository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public int Add(ExamesF200 obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IdPaciente", dbType: DbType.String, value: obj.IdPaciente, direction: ParameterDirection.Input);
            param.Add("?CpfPaciente", dbType: DbType.String, value: obj.CpfPaciente, direction: ParameterDirection.Input);
            param.Add("?DataExameEco", dbType: DbType.DateTime, value: obj.DataExameEco, direction: ParameterDirection.Input);
            param.Add("?DataTransferenciaEcoPc", dbType: DbType.DateTime, value: obj.DataTransferenciaEcoPc, direction: ParameterDirection.Input);
            param.Add("?OperadorEco", dbType: DbType.String, value: obj.OperadorEco, direction: ParameterDirection.Input);
            param.Add("?TipoExameEco", dbType: DbType.String, value: obj.TipoExameEco, direction: ParameterDirection.Input);
            param.Add("?ResultadoExameEco", dbType: DbType.String, value: obj.ResultadoExameEco, direction: ParameterDirection.Input);
            param.Add("?UnidadeResultadoEco", dbType: DbType.String, value: obj.UnidadeResultadoEco, direction: ParameterDirection.Input);
            param.Add("?ValorReferenciaResultadoEco", dbType: DbType.String, value: obj.ValorReferenciaResultadoEco, direction: ParameterDirection.Input);
            param.Add("?Url", dbType: DbType.String, value: obj.Url, direction: ParameterDirection.Input);
            param.Add("?Formato", dbType: DbType.String, value: obj.Formato, direction: ParameterDirection.Input);

            var queryCheck = new StringBuilder(@"SELECT CpfPaciente, DataExameEco, TipoExameEco, ResultadoExameEco, ValorReferenciaResultadoEco FROM ExamesF200 WHERE 1 = 1 
                                                AND CpfPaciente = ?CpfPaciente
                                                AND DataExameEco = ?DataExameEco
                                                AND TipoExameEco = ?TipoExameEco
                                                AND ResultadoExameEco = ?ResultadoExameEco
                                                AND ValorReferenciaResultadoEco = ?ValorReferenciaResultadoEco");

            const string insertQuery = @"INSERT INTO examesF200 (Id, IdPaciente, CpfPaciente, DataExameEco, DataTransferenciaEcoPc, OperadorEco, TipoExameEco, ResultadoExameEco, UnidadeResultadoEco, ValorReferenciaResultadoEco, Url, Formato) 
                VALUES (?Id, ?IdPaciente, ?CpfPaciente, ?DataExameEco, ?DataTransferenciaEcoPc, ?OperadorEco, ?TipoExameEco, ?ResultadoExameEco, ?UnidadeResultadoEco, ?ValorReferenciaResultadoEco, ?Url, ?Formato)";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var check = _connection.Query(queryCheck.ToString(), param, transaction);

                    if (check.Count() > 0)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException("Exame já cadastrado anteriormente");
                    }

                    var ret = _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "ExameF200", obj.Id.ToString(), "Inseriu ExameF200", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Update(string id, ExamesF200 obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IdPaciente", dbType: DbType.String, value: obj.IdPaciente, direction: ParameterDirection.Input);
            param.Add("?CpfPaciente", dbType: DbType.String, value: obj.CpfPaciente, direction: ParameterDirection.Input);
            param.Add("?DataExameEco", dbType: DbType.DateTime, value: obj.DataExameEco, direction: ParameterDirection.Input);
            param.Add("?DataTransferenciaEcoPc", dbType: DbType.DateTime, value: obj.DataTransferenciaEcoPc, direction: ParameterDirection.Input);
            param.Add("?OperadorEco", dbType: DbType.String, value: obj.OperadorEco, direction: ParameterDirection.Input);
            param.Add("?TipoExameEco", dbType: DbType.String, value: obj.TipoExameEco, direction: ParameterDirection.Input);
            param.Add("?ResultadoExameEco", dbType: DbType.String, value: obj.ResultadoExameEco, direction: ParameterDirection.Input);
            param.Add("?UnidadeResultadoEco", dbType: DbType.String, value: obj.UnidadeResultadoEco, direction: ParameterDirection.Input);
            param.Add("?ValorReferenciaResultadoEco", dbType: DbType.String, value: obj.ValorReferenciaResultadoEco, direction: ParameterDirection.Input);
            param.Add("?Url", dbType: DbType.String, value: obj.Url, direction: ParameterDirection.Input);
            param.Add("?Formato", dbType: DbType.String, value: obj.Formato, direction: ParameterDirection.Input);

            const string updateQuery = @"UPDATE ExamesF200 SET
                                Id = ?Id, 
                                IdPaciente = ?IdPaciente, 
                                CpfPaciente = ?CpfPaciente,
                                DataExameEco = ?DataExameEco,
                                DataTransferenciaEcoPc = ?DataTransferenciaEcoPc,
                                OperadorEco = ?OperadorEco,
                                TipoExameEco = ?TipoExameEco,
                                ResultadoExameEco = ?ResultadoExameEco,
                                UnidadeResultadoEco = ?UnidadeResultadoEco,
                                ValorReferenciaResultadoEco = ?ValorReferenciaResultadoEco,
                                Url = ?Url,
                                Formato = ?Formato
                                WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "ReceberExame", obj.Id.ToString(), "Atualizou Exame", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

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

        public ExamesF200 GetById(string id)
        {
            var query = new StringBuilder(@"SELECT
                e.Id,
                e.IdPaciente,
                e.CpfPaciente,
                e.DataExameEco,
                e.DataTransferenciaEcoPc,
                e.OperadorEco,
                e.TipoExameEco,
                e.ResultadoExameEco,
                e.UnidadeResultadoEco,
                e.ValorReferenciaResultadoEco,
                e.Url,
                e.Formato,
                i.Cpf   
                FROM ExamesF200 e
                INNER JOIN Individuo i ON i.Cpf = e.CpfPaciente  
                 ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND e.IdPaciente = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var dictionary = new Dictionary<string, ExamesF200>();

            try
            {
                return _connection.Query<ExamesF200>(
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

        public async Task<(IEnumerable<ExamesF200>, int)> GetByCpfIndividuo(string cpf)
        {
            var query = new StringBuilder(@"SELECT
                CAST(e.Id AS CHAR) AS Id,
                CAST(e.IdPaciente AS CHAR) AS IdPaciente,
                e.CpfPaciente,
                e.DataExameEco,
                e.DataTransferenciaEcoPc,
                e.OperadorEco,
                e.TipoExameEco,
                e.ResultadoExameEco,
                e.UnidadeResultadoEco,
                e.ValorReferenciaResultadoEco,
                e.Url,
                e.Formato,
                CAST(i.Id AS CHAR) AS IndividuoId  
                FROM ExamesF200 e
                INNER JOIN Individuo i ON i.Cpf = e.CpfPaciente  
                 ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            //if (string.IsNullOrEmpty(cpf)) return null;

            queryFilter.Append(" AND e.CpfPaciente = ?cpf");
            param.Add("?cpf", dbType: DbType.String, value: cpf, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            _queryCountAll.Append(queryFilter);
            #endregion


          

            try
            {
                var totalCount = _connection.Query<int>(
               _queryCountAll.ToString(),
               param,
               commandTimeout: 60,
               commandType: CommandType.Text
           ).SingleOrDefault();

                if (totalCount > 0)
                {
                    var x = await _connection.QueryAsync<ExamesF200>(
                        query.ToString(),
                        param,
                        commandTimeout: 60,
                        commandType: CommandType.Text
                    );

                    return (x, totalCount);
                }
                return (null, 0);
            }
            catch (Exception e)
            {

                throw e;
            }
            

        }
        public async Task<(IEnumerable<ExamesF200>, int)> GetByParam(ExamesF200 filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {

                //if (!string.IsNullOrEmpty(filters?.Id))
                //{
                //    queryFilter.Append(" AND r.Id = ?Id");
                //    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                //}
                if (!string.IsNullOrEmpty(filters?.IdPaciente))
                {
                    queryFilter.Append(" AND e.IdPaciente = ?IdPaciente");
                    param.Add("?IdPaciente", dbType: DbType.String, value: filters.IdPaciente, direction: ParameterDirection.Input);
                }
                //if (!string.IsNullOrEmpty(filters?.IndividuoCpf))
                //{
                //    queryFilter.Append(" AND r.IndividuoCpf = ?IndividuoCpf");
                //    param.Add("?IndividuoCpf", dbType: DbType.String, value: filters.IndividuoCpf, direction: ParameterDirection.Input);
                //}
                //if (!string.IsNullOrEmpty(filters?.TipoExameId))
                //{
                //    queryFilter.Append(" AND r.TipoExameId Like ?TipoExameId");
                //    param.Add("?TipoExameId", dbType: DbType.String, value: filters.TipoExameId, direction: ParameterDirection.Input);
                //}


                //if (filters.Resultado.HasValue)
                //{
                //    queryFilter.Append(" AND r.Resultado = ?Resultado ");
                //    param.Add("?Resultado", dbType: DbType.Boolean, value: filters.Resultado, direction: ParameterDirection.Input);
                //}
                //if (filters.Finalizado.HasValue)
                //{
                //    queryFilter.Append(" AND r.Finalizado = ?Finalizado ");
                //    param.Add("?Finalizado", dbType: DbType.Boolean, value: filters.Finalizado, direction: ParameterDirection.Input);
                //}

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

            var totalCount = _connection.Query<int>(
                _queryCountAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text
            ).SingleOrDefault();

            if (totalCount > 0)
            {
                var x = await _connection.QueryAsync<ExamesF200>(
                    _queryAll.ToString(),
                    param,
                    commandTimeout: 60,
                    commandType: CommandType.Text
                );

                return (x, totalCount);
            }
            return (null, 0);
        }


    }
}