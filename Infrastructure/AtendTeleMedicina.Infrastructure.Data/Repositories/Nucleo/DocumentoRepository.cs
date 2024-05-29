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
    public class DocumentoRepository : BaseRepository, IDocumentoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(d.Id AS CHAR) AS Id,
                CAST(d.IndividuoId AS CHAR) AS IndividuoId,
                CAST(d.ProcedimentoId AS CHAR) AS ProcedimentoId,
                CAST(d.AgendamentoId AS CHAR) AS AgendamentoId,
                CAST(d.ProfissionalId AS CHAR) AS ProfissionalId,
                d.Url,
                d.DataCadastro,         
                CAST(i.Id AS CHAR) AS Id,
                CAST(pro.Id AS CHAR) AS Id,
                pro.Descricao,
                CAST(a.Id AS CHAR) AS Id,
                a.Dia,
                a.Hora,
                CAST(p.Id AS CHAR) AS Id,
                p.Nome
                FROM Documento d 
                LEFT JOIN Individuo i ON i.Id = d.IndividuoId
                LEFT JOIN Procedimento pro ON pro.Id = d.ProcedimentoId
                LEFT JOIN Agendamento a ON a.Id = d.AgendamentoId
                LEFT JOIN Profissional p ON p.Id = d.ProfissionalId
                
                
                ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(d.Id) FROM Documento d");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public DocumentoRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public int Add(Documento obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?ProcedimentoId", dbType: DbType.String, value: obj.ProcedimentoId, direction: ParameterDirection.Input);
            param.Add("?AgendamentoId", dbType: DbType.String, value: obj.AgendamentoId, direction: ParameterDirection.Input);
            param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
            param.Add("?Url", dbType: DbType.String, value: obj.Url, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);

            const string insertQuery = @"INSERT INTO Documento (Id, IndividuoId, AgendamentoId, ProfissionalId, DataCadastro, Url, ProcedimentoId)
                                                     VALUES (?Id, ?IndividuoId, ?AgendamentoId, ?ProfissionalId, ?DataCadastro, ?Url, ?ProcedimentoId)";

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Documento", obj.Id.ToString(), "Inseriu Documento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Documento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?ProcedimentoId", dbType: DbType.String, value: obj.ProcedimentoId, direction: ParameterDirection.Input);
            param.Add("?AgendamentoId", dbType: DbType.String, value: obj.AgendamentoId, direction: ParameterDirection.Input);
            param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
            param.Add("?Url", dbType: DbType.String, value: obj.Url, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);

            const string updateQuery = @"UPDATE Documento SET
                                IndividuoId = ?IndividuoId, 
                                ProcedimentolId = ?ProcedimentolId, 
                                AgendamentoId = ?AgendamentoId,
                                Profissionalid = ?Profissionalid,
                                Url = ?Url";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Documento", obj.Id.ToString(), "Editou Documento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Delete(string id)
        {
            var query = @"UPDATE Documento SET Ativo = 0 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Documento", id.ToString(), "Desativou Documento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public Documento GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND d.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var documento = new Documento();
            _connection.QueryAsync<Documento, Individuo, Agendamento, Profissional, Procedimento, Documento>(
                _queryAll.ToString(), (d, i, a, p, pro) =>
                {
                    documento = d;

                    documento.Individuo = i;
                    documento.Agendamento = a;
                    documento.Profissional = p;
                    documento.Procedimento = pro;

                    return documento;
                },
                param,
                splitOn: "Id, Id, Id, Id, Id",
                commandTimeout: TimeOut,
                commandType: CommandType.Text);
            return (documento);
        }

        public async Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters?.Id))
                {
                    queryFilter.Append(" AND d.Id = ?Id");
                    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.IndividuoId))
                {
                    queryFilter.Append(" And d.IndividuoId = ?IndividuoId");
                    param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
                }
                if (filters.DataCadastro != null)
                {
                    queryFilter.Append(" And DATE(d.DataCadastro) = DATE(?DataCadastro)");
                    param.Add("?DataCadastro", dbType: DbType.DateTime, value: filters.DataCadastro, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                {
                    queryFilter.Append(" And d.ProfissionalId = ?ProfissionalId");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.ProcedimentoId))
                {
                    queryFilter.Append(" And d.ProcedimentoId Like ?ProcedimentoId");
                    param.Add("?ProcedimentoId", dbType: DbType.String, value: filters.ProcedimentoId, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.AgendamentoId))
                {
                    queryFilter.Append(" And d.AgendamentoId = ?AgendamentoId");
                    param.Add("?AgendamentoId", dbType: DbType.String, value: filters.AgendamentoId, direction: ParameterDirection.Input);
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

            var totalCount = _connection.Query<int>(
                _queryCountAll.ToString(),
                param,
                commandTimeout: 60,
                commandType: CommandType.Text
            ).SingleOrDefault();


            if (totalCount > 0)
            {

                //var x = await _connection.QueryAsync<Documento>(
                //    _queryAll.ToString(),
                //    param,
                //    commandTimeout: 60,
                //    commandType: CommandType.Text
                //);

                //return (x, totalCount);





                var documentos = new Dictionary<string, Documento>();
                await _connection.QueryAsync<Documento, Individuo, Procedimento, Agendamento, Profissional, Documento>(
                    _queryAll.ToString(), (d, i, pro, a, p) =>
                    {
                        if (!documentos.TryGetValue(d.Id, out var documentoEntity))
                        {
                            documentos.Add(d.Id, documentoEntity = d);
                        }
                        documentoEntity.Individuo = i;
                        documentoEntity.Profissional = p;
                        documentoEntity.Agendamento = a;
                        documentoEntity.Procedimento = pro;

                        return documentoEntity;
                    },
                    param,
                    splitOn: "Id, Id, Id, Id, Id",
                    commandTimeout: TimeOut,
                    commandType: CommandType.Text);
                return (documentos.Values, totalCount);






                //var documentos = new Dictionary<string, Documento>();
                //await _connection.QueryAsync<Documento, Individuo, Agendamento, Profissional, Procedimento, Documento>(
                //    _queryAll.ToString(), (d, i, a, p, pro) =>
                //    {
                //        documentos = d;

                //        documentos.Individuo = i;
                //        documentos.Agendamento = a;
                //        documentos.Profissional = p;
                //        documentos.Procedimento = pro;

                //        return documentos;
                //    },
                //    param,
                //    splitOn: "Id, Id, Id, Id, Id",
                //    commandTimeout: TimeOut,
                //    commandType: CommandType.Text);
                //return (documentos);





            }
            return (null, 0);
        }
    }
}
