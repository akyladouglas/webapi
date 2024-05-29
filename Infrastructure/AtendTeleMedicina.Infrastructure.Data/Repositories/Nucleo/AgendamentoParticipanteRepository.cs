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
    public class AgendamentoParticipanteRepository : BaseRepository, IAgendamentoParticipanteRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(ap.Id AS CHAR) AS Id,
                CAST(ap.AgendamentoId AS CHAR) AS AgendamentoId,
                CAST(ap.ProfissionalId AS CHAR) AS ProfissionalId,
                ap.Convidado,
                ap.Aceitou,
                ap.ParticipouAtendimento,
                ap.Avaliacao,
                ap.DadoSerializado,
                ap.LoteIntegracaoId,
                CAST(a.Id AS CHAR) AS Id,
                CAST(a.IndividuoId AS CHAR) AS IndividuoId,
                CAST(a.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                CAST(a.ProcedimentoId AS CHAR) AS ProcedimentoId,
                CAST(a.ProfissionalId AS CHAR) AS ProfissionalId,
                a.Dia,
                a.Hora,
                a.TipoDaConsulta,
                a.EmAndamento,
                a.Finalizado,
                a.Cancelado,
                a.PresencaConfirmada,
                a.NaoCompareceu,
                a.Interconsulta,
                a.PacientePresente,
                a.Motivo,
                CAST(i.Id AS CHAR) AS Id,
                i.NomeCompleto,
                i.Cpf,
                i.DataNascimento,
                CAST(pro.Id AS CHAR) AS Id,
                pro.Nome,
                pro.Crm,
                pro.Cpf,
                pro.Cns,
                pro.Email,
                CAST(p.Id AS CHAR) AS Id,
                p.Nome,
                p.Crm,
                p.Cpf
                FROM AgendamentoParticipante ap
                LEFT JOIN Agendamento a ON a.Id = ap.AgendamentoId
                LEFT JOIN Individuo i ON i.Id = a.IndividuoId
                LEFT JOIN Profissional pro ON pro.Id = a.ProfissionalId
                LEFT JOIN Profissional p ON p.Id = ap.ProfissionalId
                ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(ap.Id) FROM AgendamentoParticipante ap
            LEFT JOIN Agendamento a ON a.Id = ap.AgendamentoId
            LEFT JOIN Profissional p ON p.Id = ap.ProfissionalId");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public AgendamentoParticipanteRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public string Post(Agendamento agendamento)
        {
            var paramInsert = new DynamicParameters();
            paramInsert.Add("?Id", dbType: DbType.String, value: agendamento.Id, direction: ParameterDirection.Input);
            paramInsert.Add("?Interconsulta", dbType: DbType.Boolean, value: agendamento.Interconsulta, direction: ParameterDirection.Input);
            paramInsert.Add("?PacientePresente", dbType: DbType.Boolean, value: agendamento.PacientePresente, direction: ParameterDirection.Input);
            paramInsert.Add("?Motivo", dbType: DbType.String, value: agendamento.Motivo, direction: ParameterDirection.Input);
            paramInsert.Add("?IndividuoId", dbType: DbType.String, value: agendamento.IndividuoId, direction: ParameterDirection.Input);
            paramInsert.Add("?Estabelecimentoid", dbType: DbType.String, value: agendamento.EstabelecimentoId, direction: ParameterDirection.Input);
            paramInsert.Add("?ProfissionalId", dbType: DbType.String, value: agendamento.ProfissionalId, direction: ParameterDirection.Input);
            paramInsert.Add("?ProcedimentoId", dbType: DbType.String, value: agendamento.ProcedimentoId, direction: ParameterDirection.Input);
            paramInsert.Add("?IndividuoId", dbType: DbType.String, value: agendamento.IndividuoId, direction: ParameterDirection.Input);
            paramInsert.Add("?TipoDaConsulta", dbType: DbType.String, value: agendamento.TipoDaConsulta, direction: ParameterDirection.Input);
            paramInsert.Add("?Dia", dbType: DbType.Date, value: agendamento.Dia, direction: ParameterDirection.Input);
            paramInsert.Add("?DataCadastro", dbType: DbType.DateTime, value: agendamento.DataCadastro, direction: ParameterDirection.Input);
            paramInsert.Add("?DataAlteracao", dbType: DbType.Date, value: agendamento.DataAlteracao, direction: ParameterDirection.Input);
            paramInsert.Add("?Hora", dbType: DbType.Time, value: agendamento.Hora, direction: ParameterDirection.Input);

            const string insertQuery = @"INSERT INTO Agendamento (Id, Interconsulta, PacientePresente, Motivo, EstabelecimentoId, ProfissionalId, ProcedimentoId, IndividuoId, TipoDaConsulta, Dia, Hora, DataCadastro, DataAlteracao)
                                                     VALUES (?Id, ?Interconsulta, ?PacientePresente, ?Motivo, ?EstabelecimentoId, ?ProfissionalId, ?ProcedimentoId, ?IndividuoId, ?TipoDaConsulta, ?Dia, ?Hora, ?DataCadastro, ?DataAlteracao)";

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    _connection.Execute(insertQuery, paramInsert, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", agendamento.Id.ToString(), "Inseriu a Interconsulta", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    // ADICIONANDO OS REGISTROS DE AGENDAMENTO PARTICIPANTE E ALTERANDO O STATUS DO HORARIO PARA UTILIZADO
                    if (agendamento.ProfissionaisInterconsulta.Count > 0)
                    {
                        foreach (var profissional in agendamento.ProfissionaisInterconsulta)
                        {
                            //criando o objeto do agendamentoParticipante
                            var agendamentoParticipante = new AgendamentoParticipante();
                            agendamentoParticipante.Id = Guid.NewGuid().ToString();
                            agendamentoParticipante.AgendamentoId = agendamento.Id;
                            agendamentoParticipante.ProfissionalId = profissional.Id.ToString();
                            //Se o médico for convidado, o campo Convidado será true, se o médico for Anfitrião sera false
                            agendamentoParticipante.Convidado = profissional.Participante == "Convidado" ? true : false;
                            agendamentoParticipante.ParticipouAtendimento = false;


                            //adicionando os params do agendamentoParticipante
                            var paramAgendamentoParticipante = new DynamicParameters();
                            paramAgendamentoParticipante.Add("?Id", dbType: DbType.String, value: agendamentoParticipante.Id, direction: ParameterDirection.Input);
                            paramAgendamentoParticipante.Add("?AgendamentoId", dbType: DbType.String, value: agendamentoParticipante.AgendamentoId, direction: ParameterDirection.Input);
                            paramAgendamentoParticipante.Add("?ProfissionalId", dbType: DbType.String, value: agendamentoParticipante.ProfissionalId, direction: ParameterDirection.Input);
                            paramAgendamentoParticipante.Add("?Convidado", dbType: DbType.Boolean, value: agendamentoParticipante.Convidado, direction: ParameterDirection.Input);
                            paramAgendamentoParticipante.Add("?ParticipouAtendimento", dbType: DbType.Boolean, value: agendamentoParticipante.ParticipouAtendimento, direction: ParameterDirection.Input);
                            paramAgendamentoParticipante.Add("?Aceitou", dbType: DbType.Boolean, value: null, direction: ParameterDirection.Input);


                            var addAgendamentoParticipante = @"INSERT INTO AgendamentoParticipante 
                        (Id, AgendamentoId, ProfissionalId, Convidado, ParticipouAtendimento, Aceitou) 
                        VALUES (?Id, ?AgendamentoId, ?ProfissionalId, ?Convidado, ?ParticipouAtendimento, ?Aceitou)";

                            _connection.Execute(addAgendamentoParticipante, paramAgendamentoParticipante, transaction);
                            RegistrarAcao(_connection, Guid.NewGuid().ToString(), "AgendamentoParticipante", agendamentoParticipante.Id.ToString(), "Inseriu Agendamento Participante.", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());



                            //adicionando os params do horario
                            var paramEph = new DynamicParameters();
                            paramEph.Add("?ProfissionalId", dbType: DbType.String, value: profissional.Id.ToString(), direction: ParameterDirection.Input);
                            paramEph.Add("?Situacao", dbType: DbType.Boolean, value: true, direction: ParameterDirection.Input);
                            paramEph.Add("?Dia", dbType: DbType.Date, value: agendamento.Dia, direction: ParameterDirection.Input);
                            paramEph.Add("?Hora", dbType: DbType.Time, value: agendamento.Hora, direction: ParameterDirection.Input);

                            //alterando o status do horario para utilizado
                            var queryEph = @"UPDATE EstabelecimentoProcedimentoHorario
                        SET Situacao = 1
                        WHERE
                        ProfissionalId = ?ProfissionalId
                        AND Dia = ?Dia
                        AND Hora = ?Hora ";
                            _connection.Execute(queryEph, paramEph, transaction);
                            RegistrarAcao(_connection, Guid.NewGuid().ToString(), "EstabelecimentoProcedimentoHorario", agendamentoParticipante.Id.ToString(), "Alterou a Situação Para True Interconsulta.", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        }

                    }

                    transaction.Commit();
                    return agendamento.Id;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
        }

        public int Update(string id, AgendamentoParticipante obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?AgendamentoId", dbType: DbType.String, value: obj.AgendamentoId, direction: ParameterDirection.Input);
            param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
            param.Add("?Convidado", dbType: DbType.Boolean, value: obj.Convidado, direction: ParameterDirection.Input);
            param.Add("?Aceitou", dbType: DbType.Boolean, value: obj.Aceitou, direction: ParameterDirection.Input);
            param.Add("?ParticipouAtendimento", dbType: DbType.Boolean, value: obj.ParticipouAtendimento, direction: ParameterDirection.Input);
            param.Add("?Avaliacao", dbType: DbType.String, value: obj.Avaliacao, direction: ParameterDirection.Input);
            param.Add("?DadoSerializado", dbType: DbType.String, value: obj.DadoSerializado, direction: ParameterDirection.Input);
            param.Add("?LoteIntegracaoId", dbType: DbType.Int32, value: obj.LoteIntegracaoId, direction: ParameterDirection.Input);
            



            const string updateQuery = @"UPDATE AgendamentoParticipante SET
                                AgendamentoId = ?AgendamentoId, 
                                ProfissionalId = ?ProfissionalId, 
                                Convidado = ?Convidado, 
                                Aceitou = ?Aceitou, 
                                ParticipouAtendimento = ?ParticipouAtendimento, 
                                Avaliacao = ?Avaliacao, 
                                DadoSerializado = ?DadoSerializado, 
                                LoteIntegracaoId = ?LoteIntegracaoId
                                WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "AgendamentoParticipante", obj.Id.ToString(), "Editou AgendamentoParticipante", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AgendamentoParticipante GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND ap.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var agendamento = new AgendamentoParticipante();
            //fazendo a requisição
            _connection.Query<AgendamentoParticipante, Agendamento, Individuo, Profissional, AgendamentoParticipante>(
            query.ToString(), (ap, a, i, p) =>
            {
                agendamento = ap;
                agendamento.Agendamento = a;
                agendamento.Agendamento.Individuo = i;
                agendamento.Profissional = p;
                return agendamento;
            },
            param,
            splitOn: "Id, Id, Id, Id",
            commandTimeout: TimeOut,
            commandType: CommandType.Text).SingleOrDefault();
            return agendamento;
        }

        public async Task<(IEnumerable<AgendamentoParticipante>, int)> GetByParam(AgendamentoParticipante filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {

                //filtros pela tabela AgendamentoParticipante
                if (!string.IsNullOrEmpty(filters?.Id))
                {
                    queryFilter.Append(" And ap.Id = ?Id");
                    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                {
                    queryFilter.Append(" And ap.ProfissionalId = ?ProfissionalId");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.AgendamentoId))
                {
                    queryFilter.Append(" And ap.AgendamentoId = ?AgendamentoId");
                    param.Add("?AgendamentoId", dbType: DbType.String, value: filters.AgendamentoId, direction: ParameterDirection.Input);
                } 

                if (filters.Interconsulta.HasValue)
                {
                    queryFilter.Append(" And a.Interconsulta = ?Interconsulta");
                    param.Add("?Interconsulta", dbType: DbType.Boolean, value: filters.Interconsulta, direction: ParameterDirection.Input);
                }

                //Filtro da data esta sendo pelo Agendamento a.Dia
                if (filters.DataInicial != null)
                {
                    filters.DataFinal = filters.DataFinal ?? DateTime.Now;

                    queryFilter.Append(" AND DATE(a.Dia) BETWEEN DATE(?DataInicial) AND DATE(?DataFinal)");
                    param.Add("?DataInicial", dbType: DbType.Date, value: filters.DataInicial, direction: ParameterDirection.Input);
                    param.Add("?DataFinal", dbType: DbType.Date, value: filters.DataFinal, direction: ParameterDirection.Input);
                }
                

            }
            _queryAll.Append(queryFilter);
            _queryCountAll.Append(queryFilter);


            if (skip.HasValue && take.HasValue)
            {
                _queryAll.Append($" ORDER BY {sort} LIMIT ?skip, ?take ");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                var agendamentoParticipantes = new Dictionary<string, AgendamentoParticipante>();


                await _connection.QueryAsync<AgendamentoParticipante, Agendamento, Individuo, Profissional, Profissional, AgendamentoParticipante>(
                    _queryAll.ToString(), (ap, a, i, pro, p) =>
                    {
                        if (!agendamentoParticipantes.TryGetValue(ap.Id, out var agendamentoParticipanteEntity))
                        {
                            agendamentoParticipantes.Add(ap.Id, agendamentoParticipanteEntity = ap);
                        }
                        agendamentoParticipanteEntity.Agendamento = a;
                        agendamentoParticipanteEntity.Agendamento.Individuo = i;
                        agendamentoParticipanteEntity.Agendamento.Profissional = pro;
                        agendamentoParticipanteEntity.Profissional = p;

                        return agendamentoParticipanteEntity;
                    },
                    param,
                    splitOn: "Id, Id, Id, Id, Id",
                    commandTimeout: TimeOut,
                    commandType: CommandType.Text);
                return (agendamentoParticipantes.Values, totalCount);
            }
            return (null, 0);
        }
    }
}
