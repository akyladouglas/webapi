using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using Dapper;
using MySql.Data.MySqlClient;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class AcompanhamentoRepository : BaseRepository, IAcompanhamentoRepository
    {
        private readonly IUser _user;

        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT
            CAST(a.Id AS CHAR) AS Id,
            CAST(a.IndividuoId AS CHAR) AS IndividuoId,
            a.Data,
            a.Temperatura,
            a.Tosse,
            a.Coriza,
            a.Outros,
            a.DorCorpo,
            a.DorAbdominal,
            a.Fraqueza,
            a.DorGarganta,
            a.NauseaVomito,
            a.DorCabeca,
            a.SairCasa,
            a.ContatoPessoas,
            a.DificuldadeRespirar,
            a.Taquicardia,
            a.PerdaOlfatoPaladar,
            a.Diarreia,
            a.TemperaturaRetornou,
            a.AtendidoServicoSaude,
            a.DataAtendidoServicoSaude,
            a.PerdaOlfatoPaladar,
            a.CorStatus,
            a.Latitude,
            a.Longitude,
            a.Deletado,
            a.PressaoSanguinea,
            a.BatimentoCardiaco,
            a.OxigenacaoSanguinea,
            a.DataInicioTemperatura,
            a.DataMedicao,
            a.SessaoPassos,
            a.Passos,
            a.Calorias,
            a.Data,
            a.SintomasGripais,
            CAST(a.AtendimentoId AS CHAR) AS AtendimentoId,
            Atendimento.DataCadastro,
            Atendimento.DataAlteracao,
            Atendimento.QueixaDoPaciente,
            Atendimento.AtendidoTriagem,
            Atendimento.AtendidoMedico,
            Atendimento.TempoAtendimento,
            Atendimento.Subjetivo,
            Atendimento.Objetivo,
            Atendimento.Avaliacao,
            Atendimento.Plano,
            Atendimento.AgendamentoId,
            Atendimento.Ativo,
            Atendimento.CadastroEditado,
            Atendimento.DescricaoEditado,
            Atendimento.UsuarioEditou
            FROM Acompanhamento a
            LEFT JOIN Atendimento ON Atendimento.Id = a.AtendimentoId

            ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"SELECT COUNT(a.Id) FROM Acompanhamento a");
        private readonly MySqlConnection _connection;

        public AcompanhamentoRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _user = user;
            _connection = context.Connection;
        }

        public int Add(Acompanhamento obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?Data", dbType: DbType.DateTime, value: obj.Data, direction: ParameterDirection.Input);
            param.Add("?Temperatura", dbType: DbType.Boolean, value: obj.Temperatura, direction: ParameterDirection.Input);
            param.Add("?Tosse", dbType: DbType.Boolean, value: obj.Tosse, direction: ParameterDirection.Input);
            param.Add("?Coriza", dbType: DbType.Boolean, value: obj.Coriza, direction: ParameterDirection.Input);
            param.Add("?DorCorpo", dbType: DbType.Boolean, value: obj.DorCorpo, direction: ParameterDirection.Input);
            param.Add("?DorAbdominal", dbType: DbType.Boolean, value: obj.DorAbdominal, direction: ParameterDirection.Input);
            param.Add("?Fraqueza", dbType: DbType.Boolean, value: obj.Fraqueza, direction: ParameterDirection.Input);
            param.Add("?DorGarganta", dbType: DbType.Boolean, value: obj.DorGarganta, direction: ParameterDirection.Input);
            param.Add("?NauseaVomito", dbType: DbType.Boolean, value: obj.NauseaVomito, direction: ParameterDirection.Input);
            param.Add("?DorCabeca", dbType: DbType.Boolean, value: obj.DorCabeca, direction: ParameterDirection.Input);
            param.Add("?SairCasa", dbType: DbType.Boolean, value: obj.SairCasa, direction: ParameterDirection.Input);
            param.Add("?ContatoPessoas", dbType: DbType.Boolean, value: obj.ContatoPessoas, direction: ParameterDirection.Input);
            param.Add("?DificuldadeRespirar", dbType: DbType.Boolean, value: obj.DificuldadeRespirar, direction: ParameterDirection.Input);
            param.Add("?Diarreia", dbType: DbType.Boolean, value: obj.Diarreia, direction: ParameterDirection.Input);
            param.Add("?TemperaturaRetornou", dbType: DbType.Boolean, value: obj.TemperaturaRetornou, direction: ParameterDirection.Input);
            param.Add("?AtendidoServicoSaude", dbType: DbType.Boolean, value: obj.AtendidoServicoSaude, direction: ParameterDirection.Input);
            param.Add("?CorStatus", dbType: DbType.Boolean, value: obj.CorStatus, direction: ParameterDirection.Input);
            param.Add("?Taquicardia", dbType: DbType.Boolean, value: obj.Taquicardia, direction: ParameterDirection.Input);
            param.Add("?PerdaOlfatoPaladar", dbType: DbType.Boolean, value: obj.PerdaOlfatoPaladar, direction: ParameterDirection.Input);
            param.Add("?Latitude", dbType: DbType.Double, value: obj.Latitude, direction: ParameterDirection.Input);
            param.Add("?Longitude", dbType: DbType.Double, value: obj.Longitude, direction: ParameterDirection.Input);
            param.Add("?DataAtendidoServicoSaude", dbType: DbType.DateTime, value: obj.DataAtendidoServicoSaude, direction: ParameterDirection.Input);
            param.Add("?SintomasGripais", dbType: DbType.Boolean, value: obj.SintomasGripais, direction: ParameterDirection.Input);
            param.Add("?AtendimentoId", dbType: DbType.String, value: obj.AtendimentoId, direction: ParameterDirection.Input);
            param.Add("?Outros", dbType: DbType.String, value: obj.Outros, direction: ParameterDirection.Input);


            const string insertQuery = @"
              INSERT INTO Acompanhamento (
                                    Id,                                 
                                    Data,
                                    Temperatura,
                                    Tosse,
                                    Coriza,
                                    DorCorpo,
                                    DorAbdominal,
                                    Fraqueza,
                                    DorGarganta,
                                    NauseaVomito,
                                    DorCabeca,
                                    SairCasa,
                                    ContatoPessoas,
                                    DificuldadeRespirar,
                                    Diarreia,
                                    TemperaturaRetornou,
                                    AtendidoServicoSaude,
                                    CorStatus,
                                    Taquicardia,
                                    PerdaOlfatoPaladar,
                                    IndividuoId,
                                    Latitude,
                                    Longitude,
                                    DataAtendidoServicoSaude,
                                    SintomasGripais,
                                    AtendimentoId,
                                    Outros
                                    ) 
                             VALUES (
                                    ?Id,
                                    ?Data,
                                    ?Temperatura,
                                    ?Tosse,
                                    ?Coriza,
                                    ?DorCorpo,
                                    ?DorAbdominal,
                                    ?Fraqueza,
                                    ?DorGarganta,
                                    ?NauseaVomito,
                                    ?DorCabeca,
                                    ?SairCasa,
                                    ?ContatoPessoas,
                                    ?DificuldadeRespirar,
                                    ?Diarreia,
                                    ?TemperaturaRetornou,
                                    ?AtendidoServicoSaude,
                                    ?CorStatus,
                                    ?Taquicardia,
                                    ?PerdaOlfatoPaladar,
                                    ?IndividuoId,
                                    ?Latitude,
                                    ?Longitude,
                                    ?DataAtendidoServicoSaude,
                                    ?SintomasGripais,
                                    ?AtendimentoId,
                                    ?Outros
                                    )";

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Acompanhamento", obj.Id.ToString(), "Inseriu Acompanhamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Acompanhamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?Data", dbType: DbType.DateTime, value: obj.Data, direction: ParameterDirection.Input);
            param.Add("?Temperatura", dbType: DbType.Boolean, value: obj.Temperatura, direction: ParameterDirection.Input);
            param.Add("?Tosse", dbType: DbType.Boolean, value: obj.Tosse, direction: ParameterDirection.Input);
            param.Add("?Coriza", dbType: DbType.Boolean, value: obj.Coriza, direction: ParameterDirection.Input);
            param.Add("?DorCorpo", dbType: DbType.Boolean, value: obj.DorCorpo, direction: ParameterDirection.Input);
            param.Add("?DorAbdominal", dbType: DbType.Boolean, value: obj.DorAbdominal, direction: ParameterDirection.Input);
            param.Add("?Fraqueza", dbType: DbType.Boolean, value: obj.Fraqueza, direction: ParameterDirection.Input);
            param.Add("?DorGarganta", dbType: DbType.Boolean, value: obj.DorGarganta, direction: ParameterDirection.Input);
            param.Add("?NauseaVomito", dbType: DbType.Boolean, value: obj.NauseaVomito, direction: ParameterDirection.Input);
            param.Add("?DorCabeca", dbType: DbType.Boolean, value: obj.DorCabeca, direction: ParameterDirection.Input);
            param.Add("?SairCasa", dbType: DbType.Boolean, value: obj.SairCasa, direction: ParameterDirection.Input);
            param.Add("?ContatoPessoas", dbType: DbType.Boolean, value: obj.ContatoPessoas, direction: ParameterDirection.Input);
            param.Add("?DificuldadeRespirar", dbType: DbType.Boolean, value: obj.DificuldadeRespirar, direction: ParameterDirection.Input);
            param.Add("?Diarreia", dbType: DbType.Boolean, value: obj.Diarreia, direction: ParameterDirection.Input);
            param.Add("?TemperaturaRetornou", dbType: DbType.Boolean, value: obj.TemperaturaRetornou, direction: ParameterDirection.Input);
            param.Add("?AtendidoServicoSaude", dbType: DbType.Boolean, value: obj.AtendidoServicoSaude, direction: ParameterDirection.Input);
            param.Add("?CorStatus", dbType: DbType.Boolean, value: obj.CorStatus, direction: ParameterDirection.Input);
            param.Add("?Taquicardia", dbType: DbType.Boolean, value: obj.Taquicardia, direction: ParameterDirection.Input);
            param.Add("?PerdaOlfatoPaladar", dbType: DbType.Boolean, value: obj.PerdaOlfatoPaladar, direction: ParameterDirection.Input);
            param.Add("?Latitude", dbType: DbType.Double, value: obj.Latitude, direction: ParameterDirection.Input);
            param.Add("?Longitude", dbType: DbType.Double, value: obj.Longitude, direction: ParameterDirection.Input);
            param.Add("?DataAtendidoServicoSaude", dbType: DbType.DateTime, value: obj.DataAtendidoServicoSaude, direction: ParameterDirection.Input);
            param.Add("?SintomasGripais", dbType: DbType.Boolean, value: obj.SintomasGripais, direction: ParameterDirection.Input);
            param.Add("?AtendimentoId", dbType: DbType.String, value: obj.AtendimentoId, direction: ParameterDirection.Input);
            param.Add("?Outros", dbType: DbType.String, value: obj.Outros, direction: ParameterDirection.Input);

            const string updateQuery = @"
              UPDATE Acompanhamento SET
                    IndividuoId = ?IndividuoId,
                    Temperatura = ?Temperatura,
                    Tosse = ?Tosse,
                    Coriza = ?Coriza,
                    DorCorpo = ?DorCorpo,
                    DorAbdominal = ?DorAbdominal,
                    Fraqueza = ?Fraqueza,
                    DorGarganta = ?DorGarganta,
                    NauseaVomito = ?NauseaVomito,
                    DorCabeca = ?DorCabeca,
                    SairCasa = ?SairCasa,
                    ContatoPessoas = ?ContatoPessoas,
                    DificuldadeRespirar = ?DificuldadeRespirar,
                    Diarreia = ?Diarreia,
                    TemperaturaRetornou = ?TemperaturaRetornou,
                    AtendidoServicoSaude = ?AtendidoServicoSaude,
                    CorStatus = ?CorStatus,
                    Taquicardia = ?Taquicardia,
                    PerdaOlfatoPaladar = ?PerdaOlfatoPaladar,
                    Latitude = ?Latitude,
                    Longitude = ?Longitude,
                    DataAtendidoServicoSaude = ?DataAtendidoServicoSaude,
                    SintomasGripais = ?SintomasGripais,
                    AtendimentoId = ?AtendimentoId,
                    Outros = ?Outros
                    WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Acompanhamento", obj.Id.ToString(), "Editou Acompanhamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Acompanhamento GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND a.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            return _connection.Query<Acompanhamento>(
                query.ToString(),
                param,
                commandTimeout: TimeOut,
                commandType: CommandType.Text)
              .SingleOrDefault();
        }

        public async Task<(IEnumerable<Acompanhamento>, int)> GetByParam(Acompanhamento filters, string sort, int? skip, int? take)
        {
            var query = _queryAll;

            var queryCount = new StringBuilder();
            queryCount.Append(_queryCountAll);


            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Id))
                {
                    queryFilter.Append(" AND a.Id = ?Id ");
                    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.IndividuoId))
                {
                    queryFilter.Append(" AND a.IndividuoId = ?IndividuoId ");
                    param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.AtendimentoId))
                {
                    queryFilter.Append(" AND a.AtendimentoId = ?AtendimentoId ");
                    param.Add("?AtendimentoId", dbType: DbType.String, value: filters.AtendimentoId, direction: ParameterDirection.Input);
                }
            }
            query.Append(queryFilter);
            queryCount.Append(queryFilter);

            if (skip.HasValue && take.HasValue)
            {
                query.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                var x = await _connection.QueryAsync<Acompanhamento>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);
                return (x, totalCount);
            }
            return (null, 0);

        }

        public int Delete(string id)
        {
            var query = @"UPDATE Acompanhamento SET Ativo = 0 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Acompanhamento", id.ToString(), "Desativou o Acompanhamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

    }
}
