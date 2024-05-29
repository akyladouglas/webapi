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
using System.Transactions;
using System.Data.Common;
using System.Collections;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class InterconsultaCiapRepository : BaseRepository, IInterconsultaCiapRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(iCiap.Id AS CHAR) AS Id,
                CAST(iCiap.InterconsultaId AS CHAR) AS InterconsultaId,
                CAST(iCiap.CiapId AS CHAR) AS CiapId,
                iCiap.DataCadastro,
                iCiap.DataAlteracao
                FROM InterconsultaCiap iCiap
            ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(iCiap.Id) FROM InterconsultaCiap iCiap");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public InterconsultaCiapRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public async Task<string> Post(InterconsultaCiap ciap)
        {
            var paramInsert = new DynamicParameters();
            paramInsert.Add("?Id", dbType: DbType.String, value: ciap.Id, direction: ParameterDirection.Input);
            paramInsert.Add("?InterconsultaId", dbType: DbType.String, value: ciap.InterconsultaId, direction: ParameterDirection.Input);
            paramInsert.Add("?CiapId", dbType: DbType.Int32, value: ciap.CiapId, direction: ParameterDirection.Input);
            paramInsert.Add("?DataCadastro", dbType: DbType.DateTime, value: ciap.DataCadastro, direction: ParameterDirection.Input);

            const string insertQuery = @"INSERT INTO InterconsultaCiap (Id, InterconsultaId, CiapId, DataCadastro)
                                                     VALUES (?Id, ?InterconsultaId, ?CiapId, ?DataCadastro)";

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    _connection.Execute(insertQuery, paramInsert, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "InterconsultaCiap", ciap.Id.ToString(), "Inseriu a InterconsultaCiap", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ciap.Id;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message.ToString());
                }

            }
        }

        //public int Update(string id, AgendamentoParticipante obj)
        //{
        //    if (string.IsNullOrEmpty(id)) return 0;
        //    if (obj == null) return 0;

        //    var param = new DynamicParameters();
        //    param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
        //    param.Add("?AgendamentoId", dbType: DbType.String, value: obj.AgendamentoId, direction: ParameterDirection.Input);
        //    param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
        //    param.Add("?Convidado", dbType: DbType.Boolean, value: obj.Convidado, direction: ParameterDirection.Input);
        //    param.Add("?Aceitou", dbType: DbType.Boolean, value: obj.Aceitou, direction: ParameterDirection.Input);
        //    param.Add("?ParticipouAtendimento", dbType: DbType.Boolean, value: obj.ParticipouAtendimento, direction: ParameterDirection.Input);
        //    param.Add("?Avaliacao", dbType: DbType.String, value: obj.Avaliacao, direction: ParameterDirection.Input);
        //    param.Add("?DadoSerializado", dbType: DbType.String, value: obj.DadoSerializado, direction: ParameterDirection.Input);
        //    param.Add("?LoteIntegracaoId", dbType: DbType.Int32, value: obj.LoteIntegracaoId, direction: ParameterDirection.Input);
            



        //    const string updateQuery = @"UPDATE AgendamentoParticipante SET
        //                        AgendamentoId = ?AgendamentoId, 
        //                        ProfissionalId = ?ProfissionalId, 
        //                        Convidado = ?Convidado, 
        //                        Aceitou = ?Aceitou, 
        //                        ParticipouAtendimento = ?ParticipouAtendimento, 
        //                        Avaliacao = ?Avaliacao, 
        //                        DadoSerializado = ?DadoSerializado, 
        //                        LoteIntegracaoId = ?LoteIntegracaoId
        //                        WHERE Id = ?Id";

        //    try
        //    {
        //        using (var transaction = _connection.BeginTransaction())
        //        {
        //            var user = _user.GetUserId();
        //            var origem = _user.GetUserOrigem();
        //            var ip = _user.GetUserIp();

        //            var ret = _connection.Execute(updateQuery, param, transaction);
        //            RegistrarAcao(_connection, Guid.NewGuid().ToString(), "AgendamentoParticipante", obj.Id.ToString(), "Editou AgendamentoParticipante", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        //            transaction.Commit();
        //            return ret;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //public AgendamentoParticipante GetById(string id)
        //{
        //    var query = _queryAll;

        //    #region -- Filtro
        //    var queryFilter = new StringBuilder(" Where 1 = 1 ");
        //    var param = new DynamicParameters();

        //    if (string.IsNullOrEmpty(id)) return null;

        //    queryFilter.Append(" AND ap.Id = ?Id");
        //    param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
        //    query.Append(queryFilter);
        //    #endregion

        //    var agendamento = new AgendamentoParticipante();
        //    //fazendo a requisição
        //    _connection.Query<AgendamentoParticipante, Agendamento, Individuo, Profissional, AgendamentoParticipante>(
        //    query.ToString(), (ap, a, i, p) =>
        //    {
        //        agendamento = ap;
        //        agendamento.Agendamento = a;
        //        agendamento.Agendamento.Individuo = i;
        //        agendamento.Profissional = p;
        //        return agendamento;
        //    },
        //    param,
        //    splitOn: "Id, Id, Id, Id",
        //    commandTimeout: TimeOut,
        //    commandType: CommandType.Text).SingleOrDefault();
        //    return agendamento;
        //}

        //public async Task<(IEnumerable<AgendamentoParticipante>, int)> GetByParam(AgendamentoParticipante filters, string sort, int? skip, int? take)
        //{
        //    #region -- -- Filtros / Ordenação
        //    var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
        //    var param = new DynamicParameters();
        //    if (filters != null)
        //    {

        //        //filtros pela tabela AgendamentoParticipante
        //        if (!string.IsNullOrEmpty(filters?.Id))
        //        {
        //            queryFilter.Append(" And ap.Id = ?Id");
        //            param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
        //        }
        //        if (!string.IsNullOrEmpty(filters?.ProfissionalId))
        //        {
        //            queryFilter.Append(" And ap.ProfissionalId = ?ProfissionalId");
        //            param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
        //        }
        //        if (!string.IsNullOrEmpty(filters?.AgendamentoId))
        //        {
        //            queryFilter.Append(" And ap.AgendamentoId = ?AgendamentoId");
        //            param.Add("?AgendamentoId", dbType: DbType.String, value: filters.AgendamentoId, direction: ParameterDirection.Input);
        //        } 

        //        if (filters.Interconsulta.HasValue)
        //        {
        //            queryFilter.Append(" And a.Interconsulta = ?Interconsulta");
        //            param.Add("?Interconsulta", dbType: DbType.Boolean, value: filters.Interconsulta, direction: ParameterDirection.Input);
        //        }

        //        //Filtro da data esta sendo pelo Agendamento a.Dia
        //        if (filters.DataInicial != null)
        //        {
        //            filters.DataFinal = filters.DataFinal ?? DateTime.Now;

        //            queryFilter.Append(" AND DATE(a.Dia) BETWEEN DATE(?DataInicial) AND DATE(?DataFinal)");
        //            param.Add("?DataInicial", dbType: DbType.Date, value: filters.DataInicial, direction: ParameterDirection.Input);
        //            param.Add("?DataFinal", dbType: DbType.Date, value: filters.DataFinal, direction: ParameterDirection.Input);
        //        }
                

        //    }
        //    _queryAll.Append(queryFilter);
        //    _queryCountAll.Append(queryFilter);


        //    if (skip.HasValue && take.HasValue)
        //    {
        //        _queryAll.Append($" ORDER BY {sort} LIMIT ?skip, ?take ");
        //        skip = (skip - 1) * take;
        //        param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
        //        param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
        //    }
        //    #endregion

        //    var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

        //    if (totalCount > 0)
        //    {
        //        var agendamentoParticipantes = new Dictionary<string, AgendamentoParticipante>();


        //        await _connection.QueryAsync<AgendamentoParticipante, Agendamento, Individuo, Profissional, Profissional, AgendamentoParticipante>(
        //            _queryAll.ToString(), (ap, a, i, pro, p) =>
        //            {
        //                if (!agendamentoParticipantes.TryGetValue(ap.Id, out var agendamentoParticipanteEntity))
        //                {
        //                    agendamentoParticipantes.Add(ap.Id, agendamentoParticipanteEntity = ap);
        //                }
        //                agendamentoParticipanteEntity.Agendamento = a;
        //                agendamentoParticipanteEntity.Agendamento.Individuo = i;
        //                agendamentoParticipanteEntity.Agendamento.Profissional = pro;
        //                agendamentoParticipanteEntity.Profissional = p;

        //                return agendamentoParticipanteEntity;
        //            },
        //            param,
        //            splitOn: "Id, Id, Id, Id, Id",
        //            commandTimeout: TimeOut,
        //            commandType: CommandType.Text);
        //        return (agendamentoParticipantes.Values, totalCount);
        //    }
        //    return (null, 0);
        //}
    }
}
