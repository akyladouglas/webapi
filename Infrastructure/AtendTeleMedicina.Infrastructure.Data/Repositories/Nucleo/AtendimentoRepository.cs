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
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using MySqlX.XDevAPI.Common;
using System.Collections;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class AtendimentoRepository : BaseRepository, IAtendimentoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"SELECT
                DISTINCT CAST(Atendimento.Id AS CHAR) AS Id,
                Atendimento.Subjetivo,
                Atendimento.QueixaDoPaciente,
                DATE_FORMAT(Atendimento.DataCadastro, '%d/%m/%Y') AS DataDoAtendimento,
                CAST(Atendimento.TempoAtendimento AS CHAR) AS TempoAtendimento,
                Atendimento.InicioDoAtendimento,
                Atendimento.FimDoAtendimento,
                Atendimento.Objetivo,
                Atendimento.Avaliacao,
                Atendimento.Plano,
                Atendimento.AgendamentoId,
                Atendimento.DataCadastro,
                Atendimento.AtendidoTriagem,
                Atendimento.AtendidoMedico,
                Atendimento.Ativo,
                Atendimento.DataAlteracao,
                Atendimento.CadastroEditado,
                Atendimento.DescricaoEditado,
                Atendimento.Alergias,
                Atendimento.Antecedentes,
                Atendimento.CondicoesAvaliadas,
                Atendimento.CondutaDesfecho,
                Atendimento.ModalidadeAD,
                Atendimento.ProcedimentosRealizados,
                Atendimento.TipoAtendimento,
                CAST(Atendimento.UsuarioEditou AS CHAR) AS UsuarioEditou,

                CAST(ag.Id AS CHAR) AS Id,
	            CAST(ag.ProfissionalId AS CHAR) AS ProfissionalId,
                CAST(ag.IndividuoId AS CHAR) AS IndividuoId,
                CAST(ag.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                CAST(ag.ProcedimentoId AS CHAR) AS ProcedimentoId,
                ag.Dia AS Dia,
                ag.Hora AS Hora,
                ag.TipoDaConsulta,
                ag.Observacoes,
                ag.Ativo,
                ag.DataAlteracao, 
                ag.DataCadastro,
                ag.EmAndamento,
                ag.Finalizado,
                ag.Cancelado,
                ag.EmAtendimentoMedico,
                ag.Retorno,
                ag.VinculoRetorno,
                CAST(ag.RetornoAgendamentoId AS CHAR) AS RetornoAgendamentoId,
                ag.PresencaConfirmada,
                ag.NaoCompareceu,
                ag.PressaoSanguinea,
                ag.OxigenacaoSanguinea,
                ag.BatimentoCardiaco,
                ag.Altura,
                ag.Peso,
                ag.Temperatura,
                ag.CorStatusTriagem,

                CAST(p.Id AS CHAR) AS Id,
	            p.Crm,
                p.Nome,
                CAST(i.Id AS CHAR) AS Id,
                TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
                i.NomeCompleto,
                i.Cpf,
                i.Cns,
                i.Email,
                i.TelefoneCelular,
                i.DataNascimento,
                DATE_FORMAT(i.DataNascimento, '%d/%m/%Y') AS DataDoNascimento,
                i.Hipertenso,
                i.Diabetes,
                i.Fumante,
                i.Asma,
                i.DoencaPulmao,
                i.DoencaCoracao,
                i.DoencaCancer,
                i.Transplantado,
                i.DoencaComprometeImunidade,
                i.DoencaRins,
                i.DoencaFigado,
                i.DataAlteracao,
                TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
                i.NomeDaMae,
                CAST(ciap.Id AS CHAR) AS Id,
                ciap.Codigo,
                ciap.Descricao,
                CAST(cid10.Id AS CHAR) AS Id,
                cid10.Codigo,
                cid10.Descricao,
                CAST(proc.Id AS CHAR) AS Id,
                proc.Descricao
                FROM
                Atendimento
                LEFT JOIN Agendamento ag ON ag.Id = Atendimento.AgendamentoId
                LEFT JOIN Profissional p ON ag.ProfissionalId = p.Id
                LEFT JOIN Individuo i ON i.Id = ag.IndividuoId
                LEFT JOIN IndividuoCiap iciap ON iciap.AgendamentoId = Atendimento.AgendamentoId
                LEFT JOIN Ciap ciap ON ciap.Id = iciap.CiapId
                LEFT JOIN IndividuoCid10 icid10 ON icid10.AgendamentoId = Atendimento.AgendamentoId
                LEFT JOIN Cid10 cid10 ON cid10.Id = icid10.Cid10Id
                LEFT JOIN Procedimento proc ON proc.Id = ag.ProcedimentoId
                 ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(Atendimento.Id) From Atendimento 
        LEFT JOIN Agendamento ag ON	ag.Id = Atendimento.AgendamentoId
        LEFT JOIN Profissional p ON ag.ProfissionalId = p.Id
        LEFT JOIN Individuo i ON i.Id = ag.IndividuoId
        LEFT JOIN IndividuoCiap iciap ON iciap.AgendamentoId = Atendimento.AgendamentoId
        LEFT JOIN Ciap ciap ON ciap.Id = iciap.CiapId
        LEFT JOIN IndividuoCid10 icid10 ON icid10.AgendamentoId = Atendimento.AgendamentoId
        LEFT JOIN Cid10 cid10 ON cid10.Id = icid10.Cid10Id");
        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public AtendimentoRepository(IMySqlContext context, IUser user)
                : base(context)
        {
            _user = user;
            _connection = context.Connection;
        }

        // USADO PELA TRIAGEM / RECEPCAO
        public int Add(Atendimento obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?QueixaDoPaciente", obj.QueixaDoPaciente, DbType.String, ParameterDirection.Input);
            param.Add("?AgendamentoId", obj.AgendamentoId, DbType.String, ParameterDirection.Input);
            param.Add("?DataCadastro", obj.DataCadastro, DbType.DateTime, ParameterDirection.Input);
            param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
            param.Add("?AtendidoTriagem", dbType: DbType.Boolean, value: obj.AtendidoTriagem, direction: ParameterDirection.Input);
            param.Add("?AtendidoMedico", dbType: DbType.Boolean, value: obj.AtendidoMedico, direction: ParameterDirection.Input);
            param.Add("?LocalDeAtendimento", dbType: DbType.Int32, value: obj.LocalDeAtendimento, direction: ParameterDirection.Input);
            param.Add("?UsuarioId", dbType: DbType.String, value: obj.UsuarioId, direction: ParameterDirection.Input);

            const string insertQuery = @"
                INSERT INTO Atendimento(
                Atendimento.Id,
                Atendimento.QueixaDoPaciente,
                Atendimento.AgendamentoId,
                Atendimento.DataCadastro, 
                Atendimento.DataAlteracao,
                Atendimento.AtendidoTriagem,
                Atendimento.AtendidoMedico,
                Atendimento.LocalDeAtendimento,
                Atendimento.UsuarioId
             )
              VALUES(
                ?Id,
                ?QueixaDoPaciente,
                ?AgendamentoId,
                ?DataCadastro,
                ?DataAlteracao,
                ?AtendidoTriagem,
                ?AtendidoMedico,
                ?LocalDeAtendimento,
                ?UsuarioId
             )";

            using (var transaction = _connection.BeginTransaction())
            {
                #region Marca Agendamento como Presenca Confirmada
                var queryAgendamento = @"UPDATE Agendamento 
        SET PresencaConfirmada = 1
        WHERE 1 = 1
        AND Id = ?AgendamentoId 
        ";
                _connection.Execute(queryAgendamento, param, transaction);
                #endregion

                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Atendimento", obj.Id.ToString(), "Inseriu Atendimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        // USADO PELO MEDICO
        public int AddMedico(Atendimento obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?Subjetivo", obj.Subjetivo, DbType.String, ParameterDirection.Input);
            param.Add("?InicioDoAtendimento", obj.InicioDoAtendimento, DbType.DateTime, ParameterDirection.Input);
            param.Add("?FimDoAtendimento", obj.FimDoAtendimento, DbType.DateTime, ParameterDirection.Input);
            param.Add("?TempoAtendimento", obj.TempoAtendimento, DbType.String, ParameterDirection.Input);
            param.Add("?Objetivo", obj.Objetivo, DbType.String, ParameterDirection.Input);
            param.Add("?Avaliacao", obj.Avaliacao, DbType.String, ParameterDirection.Input);
            param.Add("?Plano", obj.Plano, DbType.String, ParameterDirection.Input);
            param.Add("?AgendamentoId", obj.AgendamentoId, DbType.String, ParameterDirection.Input);
            param.Add("?DataCadastro", obj.DataCadastro, DbType.DateTime, ParameterDirection.Input);
            param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
            param.Add("?AtendidoMedico", dbType: DbType.Boolean, value: obj.AtendidoMedico, direction: ParameterDirection.Input);
            param.Add("?Alergias", dbType: DbType.String, value: obj.Alergias, direction: ParameterDirection.Input);
            param.Add("?Antecedentes", dbType: DbType.String, value: obj.Antecedentes, direction: ParameterDirection.Input);
            param.Add("?CondicoesAvaliadas", dbType: DbType.String, value: obj.CondicoesAvaliadas, direction: ParameterDirection.Input);
            param.Add("?CondutaDesfecho", dbType: DbType.String, value: obj.CondutaDesfecho, direction: ParameterDirection.Input);
            param.Add("?ModalidadeAD", dbType: DbType.String, value: obj.ModalidadeAD, direction: ParameterDirection.Input);
            param.Add("?ProcedimentosRealizados", dbType: DbType.String, value: obj.ProcedimentosRealizados, direction: ParameterDirection.Input);
            param.Add("?TipoAtendimento", dbType: DbType.String, value: obj.TipoAtendimento, direction: ParameterDirection.Input);
            param.Add("?LocalDeAtendimento", dbType: DbType.Int32, value: obj.LocalDeAtendimento, direction: ParameterDirection.Input);


            const string insertQuery = @"
            INSERT INTO Atendimento(
            Atendimento.Id,
            Atendimento.Subjetivo,
            Atendimento.InicioDoAtendimento,
            Atendimento.FimDoAtendimento,  
            Atendimento.TempoAtendimento,
            Atendimento.Objetivo,
            Atendimento.Avaliacao,
            Atendimento.Plano,
            Atendimento.AgendamentoId,
            Atendimento.DataCadastro,
            Atendimento.DataAlteracao,
            Atendimento.AtendidoMedico,
            Atendimento.Alergias,
            Atendimento.Antecedentes,
            Atendimento.CondicoesAvaliadas,
            Atendimento.CondutaDesfecho,
            Atendimento.ModalidadeAD,
            Atendimento.ProcedimentosRealizados,
            Atendimento.TipoAtendimento,
            Atendimento.LocalDeAtendimento
            )
            VALUES(
            ?Id,
            ?Subjetivo,
            ?InicioDoAtendimento,
            ?FimDoAtendimento,
            ?TempoAtendimento,
            ?Objetivo,
            ?Avaliacao,
            ?Plano,
            ?AgendamentoId,
            ?DataCadastro,
            ?DataAlteracao,
            ?AtendidoMedico,
            ?Alergias,
            ?Antecedentes,
            ?CondicoesAvaliadas,
            ?CondutaDesfecho,
            ?ModalidadeAD,
            ?ProcedimentosRealizados,
            ?TipoAtendimento,
            ?LocalDeAtendimento
            )";

            using (var transaction = _connection.BeginTransaction())
            {

                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Atendimento", obj.Id.ToString(), "Inseriu Atendimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Atendimento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?Subjetivo", obj.Subjetivo, DbType.String, ParameterDirection.Input);
            param.Add("?InicioDoAtendimento", obj.InicioDoAtendimento, DbType.DateTime, ParameterDirection.Input);
            param.Add("?FimDoAtendimento", obj.FimDoAtendimento, DbType.DateTime, ParameterDirection.Input);
            param.Add("?TempoAtendimento", obj.TempoAtendimento, DbType.String, ParameterDirection.Input);
            param.Add("?Objetivo", obj.Objetivo, DbType.String, ParameterDirection.Input);
            param.Add("?Avaliacao", obj.Avaliacao, DbType.String, ParameterDirection.Input);
            param.Add("?Plano", obj.Plano, DbType.String, ParameterDirection.Input);
            param.Add("?AgendamentoId", obj.AgendamentoId, DbType.String, ParameterDirection.Input);
            param.Add("?DataCadastro", obj.DataCadastro, DbType.DateTime, ParameterDirection.Input);
            param.Add("?DataAlteracao", DateTime.Now, DbType.DateTime, ParameterDirection.Input);
            param.Add("?Ativo", obj.Ativo, DbType.DateTime, ParameterDirection.Input);
            param.Add("?AtendidoMedico", dbType: DbType.Boolean, value: obj.AtendidoMedico, direction: ParameterDirection.Input);
            param.Add("?AtendidoTriagem", dbType: DbType.Boolean, value: obj.AtendidoTriagem, direction: ParameterDirection.Input);
            param.Add("?Alergias", dbType: DbType.String, value: obj.Alergias, direction: ParameterDirection.Input);
            param.Add("?Antecedentes", dbType: DbType.String, value: obj.Antecedentes, direction: ParameterDirection.Input);
            param.Add("?CondicoesAvaliadas", dbType: DbType.String, value: obj.CondicoesAvaliadas, direction: ParameterDirection.Input);
            param.Add("?CondutaDesfecho", dbType: DbType.String, value: obj.CondutaDesfecho, direction: ParameterDirection.Input);
            param.Add("?ModalidadeAD", dbType: DbType.String, value: obj.ModalidadeAD, direction: ParameterDirection.Input);
            param.Add("?ProcedimentosRealizados", dbType: DbType.String, value: obj.ProcedimentosRealizados, direction: ParameterDirection.Input);
            param.Add("?TipoAtendimento", dbType: DbType.String, value: obj.TipoAtendimento, direction: ParameterDirection.Input);


            const string updateQuery = @"
              UPDATE Atendimento SET
              Subjetivo = ?Subjetivo,
              Objetivo = ?Objetivo,
              Avaliacao = ?Avaliacao,
              Plano = ?Plano,
              InicioDoAtendimento = ?InicioDoAtendimento,
              FimDoAtendimento = ?FimDoAtendimento,
              DataCadastro = ?DataCadastro,
              DataAlteracao = ?DataAlteracao,
              AtendidoMedico = ?AtendidoMedico,
              AtendidoTriagem = ?AtendidoTriagem,
              Alergias = ?Alergias,
              Antecedentes = ?Antecedentes,
              CondicoesAvaliadas = ?CondicoesAvaliadas,
              CondutaDesfecho = ?CondutaDesfecho,
              ModalidadeAD = ?ModalidadeAD,
              ProcedimentosRealizados = ?ProcedimentosRealizados,
              TipoAtendimento = ?TipoAtendimento,
              DadoSerializado = NULL
              WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Atendimento", obj.Id.ToString(), "Editou Atendimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int UpdateInativar(string id, Atendimento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?DescricaoEditado", obj.DescricaoEditado, DbType.String, ParameterDirection.Input);
            param.Add("?CadastroEditado", obj.CadastroEditado, DbType.Boolean, ParameterDirection.Input);
            param.Add("?UsuarioEditou", obj.UsuarioEditou, DbType.String, ParameterDirection.Input);
            param.Add("?Ativo", obj.Ativo, DbType.Boolean, ParameterDirection.Input);
            param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);

            const string updateQuery = @"
              UPDATE Atendimento SET
              DescricaoEditado = ?DescricaoEditado,
              CadastroEditado = ?CadastroEditado,
              UsuarioEditou = ?UsuarioEditou,
              DataAlteracao = ?DataAlteracao,
              Ativo = ?Ativo
              WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Atendimento", obj.Id.ToString(), "Editou Atendimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Atendimento> GetById(string id)
        {
            /* 06/09/2024
             * Assim como no metodo GetByParam, se fez necessario realizar a insercao
             * dos procedimentos em IndividuoProcedimentos de maneira separada da busca
             * original, pela mesma razao apontada no metodo GetByParam.
             */
            
            if (string.IsNullOrEmpty(id)) return null;

            #region - queryAll

            var queryAll = new StringBuilder(@"
            SELECT DISTINCT
	            CAST( Atendimento.Id AS CHAR ) AS Id,
	            Atendimento.Subjetivo,
	            Atendimento.QueixaDoPaciente,
	            DATE_FORMAT( Atendimento.DataCadastro, '%d/%m/%Y' ) AS DataDoAtendimento,
	            CAST( Atendimento.TempoAtendimento AS CHAR ) AS TempoAtendimento,
	            Atendimento.InicioDoAtendimento,
	            Atendimento.FimDoAtendimento,
	            Atendimento.Objetivo,
	            Atendimento.Avaliacao,
	            Atendimento.Plano,
	            Atendimento.AgendamentoId,
	            Atendimento.DataCadastro,
	            Atendimento.AtendidoTriagem,
	            Atendimento.AtendidoMedico,
	            Atendimento.Ativo,
	            Atendimento.DataAlteracao,
	            Atendimento.CadastroEditado,
	            Atendimento.DescricaoEditado,
	            Atendimento.Alergias,
	            Atendimento.Antecedentes,
	            Atendimento.CondicoesAvaliadas,
	            Atendimento.CondutaDesfecho,
	            Atendimento.ModalidadeAD,
	            Atendimento.ProcedimentosRealizados,
	            Atendimento.TipoAtendimento,
	            CAST( Atendimento.UsuarioEditou AS CHAR ) AS UsuarioEditou,
	            CAST( ag.Id AS CHAR ) AS Id,
	            CAST( ag.ProfissionalId AS CHAR ) AS ProfissionalId,
	            CAST( ag.IndividuoId AS CHAR ) AS IndividuoId,
	            CAST( ag.EstabelecimentoId AS CHAR ) AS EstabelecimentoId,
	            CAST( ag.ProcedimentoId AS CHAR ) AS ProcedimentoId,
	            ag.Dia AS Dia,
	            ag.Hora AS Hora,
	            ag.TipoDaConsulta,
	            ag.Observacoes,
	            ag.Ativo,
	            ag.DataAlteracao,
	            ag.DataCadastro,
	            ag.EmAndamento,
	            ag.Finalizado,
	            ag.Cancelado,
	            ag.EmAtendimentoMedico,
	            ag.Retorno,
	            ag.VinculoRetorno,
	            CAST( ag.RetornoAgendamentoId AS CHAR ) AS RetornoAgendamentoId,
	            ag.PresencaConfirmada,
	            ag.NaoCompareceu,
	            ag.PressaoSanguinea,
	            ag.OxigenacaoSanguinea,
	            ag.BatimentoCardiaco,
	            ag.Altura,
	            ag.Peso,
	            ag.Temperatura,
	            ag.CorStatusTriagem,
	            CAST( p.Id AS CHAR ) AS Id,
	            p.Crm,
	            p.Nome,
	            CAST( i.Id AS CHAR ) AS Id,
	            TIMESTAMPDIFF(
		            YEAR,
		            i.DataNascimento,
	            NOW()) AS Idade,
	            i.NomeCompleto,
	            i.Cpf,
	            i.Cns,
	            i.Email,
	            i.TelefoneCelular,
	            i.DataNascimento,
	            DATE_FORMAT( i.DataNascimento, '%d/%m/%Y' ) AS DataDoNascimento,
	            i.Hipertenso,
	            i.Diabetes,
	            i.Fumante,
	            i.Asma,
	            i.DoencaPulmao,
	            i.DoencaCoracao,
	            i.DoencaCancer,
	            i.Transplantado,
	            i.DoencaComprometeImunidade,
	            i.DoencaRins,
	            i.DoencaFigado,
	            i.DataAlteracao,
	            TIMESTAMPDIFF(
		            YEAR,
		            i.DataNascimento,
	            NOW()) AS Idade,
	            i.NomeDaMae,
	            CAST( proc.Id AS CHAR ) AS Id,
	            proc.Descricao
                FROM
                Atendimento
                LEFT JOIN Agendamento ag ON ag.Id = Atendimento.AgendamentoId
                LEFT JOIN Profissional p ON ag.ProfissionalId = p.Id
                LEFT JOIN Individuo i ON i.Id = ag.IndividuoId
                LEFT JOIN Procedimento proc ON proc.Id = ag.ProcedimentoId
                 ");
            
            #endregion

            #region - Filtro: permite a busca de um atendimento específico

            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            queryFilter.Append(" AND Atendimento.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            queryAll.Append(queryFilter);

            #endregion

            #region - Busca e mapeamento de atendimentos


            var atendimentoDict = new Dictionary<string, Atendimento>();

            await _connection.QueryAsync<Atendimento, Agendamento, Profissional, Individuo, Procedimento, Atendimento>(
                queryAll.ToString(), (a, ag, p, i, proc) =>
                {
                    if (!atendimentoDict.TryGetValue(a.Id, out var atendimentoEntity))
                    {
                        atendimentoDict.Add(a.Id, atendimentoEntity = a);
                    }
                    atendimentoEntity.Agendamento = ag;
                    atendimentoEntity.Profissional = p;
                    atendimentoEntity.Individuo = i;
                    atendimentoEntity.Procedimento = proc;

                    return atendimentoEntity;
                },
                param,
                splitOn: "Id, Id, Id, Id",
                commandTimeout: TimeOut,
                commandType: CommandType.Text);

            #endregion

            #region - IndividuoCiap

            param.Add("?IdIndividuo", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            var queryCiap = new StringBuilder(@"
                    SELECT
	                    CAST( iciap.Id AS CHAR ) AS Id,
	                    CAST( iciap.IndividuoId AS CHAR ) AS IndividuoId,
	                    CAST( iciap.ProfissionalId AS CHAR ) AS ProfissionalId,
	                    CAST( iciap.AgendamentoId AS CHAR ) AS AgendamentoId,
	                    CAST( iciap.CiapId AS CHAR ) AS CiapId,
	                    iciap.DataCadastro,
	                    iciap.DataAlteracao,
	                    iciap.AtendidoTriagem,
	                    iciap.AtendidoMedico,
	                    CAST( ciap.Id AS CHAR ) AS Id,
	                    ciap.Codigo,
	                    ciap.Descricao 
                    FROM
	                    Ciap
	                    INNER JOIN IndividuoCiap iciap ON iciap.CiapId = ciap.Id 
                    -- WHERE
	                   -- iciap.IndividuoId = ? IdIndividuo 
	                   -- AND iciap.AgendamentoId = ? AgendamentoId");

            var individuoCiap = _connection.Query<IndividuoCiap, Ciap, IndividuoCiap>(queryCiap.ToString(), (i, c) =>
            {
                i.Ciap = c;
                return i;
            }, param, commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

            foreach (var atend in atendimentoDict.Values)
            {
                atend.IndividuoCiap = individuoCiap.Where(x => x.IndividuoId == atend.Individuo.Id && x.AgendamentoId == atend.AgendamentoId).ToList();
            }

            #endregion

            #region - IndividuoCid10

            param.Add("?IdIndividuo", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            var queryCid10 = new StringBuilder(@"
                    SELECT
                        CAST( icid10.Id AS CHAR ) AS Id,
                        CAST( icid10.IndividuoId AS CHAR ) AS IndividuoId,
                        CAST( icid10.ProfissionalId AS CHAR ) AS ProfissionalId,
                        CAST( icid10.AgendamentoId AS CHAR ) AS AgendamentoId,
                        CAST( icid10.Cid10Id AS CHAR ) AS Cid10Id,
                        icid10.DataCadastro,
                        icid10.DataAlteracao,
                        icid10.AtendidoTriagem,
                        icid10.AtendidoMedico,
                        CAST( cid10.Id AS CHAR ) AS Id,
                        cid10.Codigo,
                        cid10.Descricao 
                    FROM
                        Cid10
                        INNER JOIN IndividuoCid10 icid10 ON icid10.Cid10Id = cid10.Id 
                    -- WHERE
                       -- icid10.IndividuoId = ? IdIndividuo 
                       -- AND icid10.AgendamentoId = ? AgendamentoId");

            var individuoCid10 = _connection.Query<IndividuoCid10, Cid, IndividuoCid10>(queryCid10.ToString(),
                (i, c) =>
                {
                    i.Cid = c;
                    return i;
                }, param, commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

            try
            {
                foreach (var atend in atendimentoDict.Values)
                {
                    atend.IndividuoCid10 = individuoCid10.Where(x => x.IndividuoId == atend.Individuo.Id && x.AgendamentoId == atend.AgendamentoId).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            #endregion

            #region - Procedimento (SIGTAP)

            param.Add("?IdIndividuo", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            var queryProcedimento = new StringBuilder(@"
                    SELECT
	                    CAST( iproc.Id AS CHAR ) AS Id,
	                    CAST( iproc.IndividuoId AS CHAR ) AS IndividuoId,
	                    CAST( iproc.ProfissionalId AS CHAR ) AS ProfissionalId,
	                    CAST( iproc.AgendamentoId AS CHAR ) AS AgendamentoId,
	                    iproc.ProcedimentoCodigo,
	                    iproc.DataCadastro,
	                    iproc.DataAlteracao,
	                    iproc.AtendidoTriagem,
	                    iproc.AtendidoMedico,
	                    CAST( procedimento.Id AS CHAR ) AS Id,
	                    procedimento.Codigo,
	                    procedimento.Descricao 
                    FROM
	                    Procedimento
	                    INNER JOIN IndividuoProcedimento iproc ON iproc.ProcedimentoCodigo = procedimento.Codigo
                        ");

            var individuoProcedimento = _connection.Query<IndividuoProcedimento, Procedimento, IndividuoProcedimento>(queryProcedimento.ToString(), (i, c) =>
            {
                i.Procedimento = c;
                return i;
            }, param, commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

            foreach (var atend in atendimentoDict.Values)
            {
                atend.IndividuoProcedimentos = individuoProcedimento.Where(x => x.IndividuoId == atend.Individuo.Id && x.AgendamentoId == atend.AgendamentoId).ToList();
            }

            #endregion

            return atendimentoDict.Values.SingleOrDefault();
        }

        public Atendimento GetMaxAtendimentoByIndividuoId(string id)
        {

            var query = new StringBuilder(@"SELECT i.NomeCompleto, d.Id, d.DataCadastro, d.Alergias, d.Antecedentes  
                                            FROM Agendamento a
                                            INNER JOIN Individuo i ON i.Id = a.IndividuoId
                                            INNER JOIN Atendimento d ON d.AgendamentoId = a.Id ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 AND d.AtendidoTriagem = 0 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND i.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            query.Append(" ORDER BY d.DataCadastro DESC LIMIT 1 ");

            #endregion

            try
            {
                return _connection.Query<Atendimento>(
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

        public Atendimento GetAgendamentoById(string id)
        {
            var query = new StringBuilder(@"SELECT
                DISTINCT CAST(Atendimento.Id AS CHAR) AS Id,
                Atendimento.QueixaDoPaciente,
                Atendimento.Subjetivo,
                Atendimento.InicioDoAtendimento,
                Atendimento.FimDoAtendimento,
                Atendimento.TempoAtendimento,
                Atendimento.Objetivo,
                Atendimento.Avaliacao,
                Atendimento.Plano,
                Atendimento.DataCadastro,
                Atendimento.AtendidoTriagem,
                Atendimento.AtendidoMedico,
                Atendimento.Ativo,
                Atendimento.DataAlteracao,
                Atendimento.CadastroEditado,
                CAST(Atendimento.AgendamentoId AS CHAR) AS AgendamentoId,
                Atendimento.DescricaoEditado,
                CAST(Atendimento.UsuarioEditou AS CHAR) AS UsuarioEditou,
                Atendimento.CondicoesAvaliadas,
                Atendimento.CondutaDesfecho,
                Atendimento.ModalidadeAD,
                Atendimento.ProcedimentosRealizados,
                Atendimento.TipoAtendimento
                FROM Atendimento
                 ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND Atendimento.AgendamentoId = ?AgendamentoId");
            param.Add("?AgendamentoId", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            try
            {
                return _connection.Query<Atendimento>(
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


        public async Task<(IEnumerable<Atendimento>, int)> GetByParam(Atendimento atendimentoFilters, string sort, int? skip = null, int? take = null)
        {
            /* 04/03/2024
             * Devido a maneira como o back-end foi estruturado (em algum momento),
             * precisou-se realizar alteracoes para correcao do retorno esperado neste
             * metodo. Assim, Ciap e Cid10 sao inseridos em IndividuoCid e IndividuoCiap,
             * respectivamente, apos a busca dos atendimentos.
             * Esta e' uma solucao paliativa que foi necessaria para resolver este problema
             * durante o desenvolvimento do Meeds for Doctors.
             */

            #region - queryAll e queryCountAll
            var queryAll = new StringBuilder(@"
            SELECT DISTINCT
	            CAST( Atendimento.Id AS CHAR ) AS Id,
	            Atendimento.Subjetivo,
	            Atendimento.QueixaDoPaciente,
	            DATE_FORMAT( Atendimento.DataCadastro, '%d/%m/%Y' ) AS DataDoAtendimento,
	            CAST( Atendimento.TempoAtendimento AS CHAR ) AS TempoAtendimento,
	            Atendimento.InicioDoAtendimento,
	            Atendimento.FimDoAtendimento,
	            Atendimento.Objetivo,
	            Atendimento.Avaliacao,
	            Atendimento.Plano,
	            Atendimento.AgendamentoId,
	            Atendimento.DataCadastro,
	            Atendimento.AtendidoTriagem,
	            Atendimento.AtendidoMedico,
	            Atendimento.Ativo,
	            Atendimento.DataAlteracao,
	            Atendimento.CadastroEditado,
	            Atendimento.DescricaoEditado,
	            Atendimento.Alergias,
	            Atendimento.Antecedentes,
	            Atendimento.CondicoesAvaliadas,
	            Atendimento.CondutaDesfecho,
	            Atendimento.ModalidadeAD,
	            Atendimento.ProcedimentosRealizados,
	            Atendimento.TipoAtendimento,
	            CAST( Atendimento.UsuarioEditou AS CHAR ) AS UsuarioEditou,
	            CAST( ag.Id AS CHAR ) AS Id,
	            CAST( ag.ProfissionalId AS CHAR ) AS ProfissionalId,
	            CAST( ag.IndividuoId AS CHAR ) AS IndividuoId,
	            CAST( ag.EstabelecimentoId AS CHAR ) AS EstabelecimentoId,
	            CAST( ag.ProcedimentoId AS CHAR ) AS ProcedimentoId,
	            ag.Dia AS Dia,
	            ag.Hora AS Hora,
	            ag.TipoDaConsulta,
	            ag.Observacoes,
	            ag.Ativo,
	            ag.DataAlteracao,
	            ag.DataCadastro,
	            ag.EmAndamento,
	            ag.Finalizado,
	            ag.Cancelado,
	            ag.EmAtendimentoMedico,
	            ag.Retorno,
	            ag.VinculoRetorno,
	            CAST( ag.RetornoAgendamentoId AS CHAR ) AS RetornoAgendamentoId,
	            ag.PresencaConfirmada,
	            ag.NaoCompareceu,
	            ag.PressaoSanguinea,
	            ag.OxigenacaoSanguinea,
	            ag.BatimentoCardiaco,
	            ag.Altura,
	            ag.Peso,
	            ag.Temperatura,
	            ag.CorStatusTriagem,
	            CAST( p.Id AS CHAR ) AS Id,
	            p.Crm,
	            p.Nome,
	            CAST( i.Id AS CHAR ) AS Id,
	            TIMESTAMPDIFF(
		            YEAR,
		            i.DataNascimento,
	            NOW()) AS Idade,
	            i.NomeCompleto,
	            i.Cpf,
	            i.Cns,
	            i.Email,
	            i.TelefoneCelular,
	            i.DataNascimento,
	            DATE_FORMAT( i.DataNascimento, '%d/%m/%Y' ) AS DataDoNascimento,
	            i.Hipertenso,
	            i.Diabetes,
	            i.Fumante,
	            i.Asma,
	            i.DoencaPulmao,
	            i.DoencaCoracao,
	            i.DoencaCancer,
	            i.Transplantado,
	            i.DoencaComprometeImunidade,
	            i.DoencaRins,
	            i.DoencaFigado,
	            i.DataAlteracao,
	            TIMESTAMPDIFF(
		            YEAR,
		            i.DataNascimento,
	            NOW()) AS Idade,
	            i.NomeDaMae,
	            CAST( proc.Id AS CHAR ) AS Id,
	            proc.Descricao
                FROM
                Atendimento
                LEFT JOIN Agendamento ag ON ag.Id = Atendimento.AgendamentoId
                LEFT JOIN Profissional p ON ag.ProfissionalId = p.Id
                LEFT JOIN Individuo i ON i.Id = ag.IndividuoId
                LEFT JOIN Procedimento proc ON proc.Id = ag.ProcedimentoId
                 ");

            var queryCountAll = new StringBuilder(@"
                SELECT COUNT(Atendimento.Id) 
                FROM 
                    Atendimento 
                    LEFT JOIN Agendamento ag ON	ag.Id = Atendimento.AgendamentoId
                    LEFT JOIN Profissional p ON ag.ProfissionalId = p.Id
                    LEFT JOIN Individuo i ON i.Id = ag.IndividuoId");
            #endregion

            #region - Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (atendimentoFilters != null)
            {
                if (atendimentoFilters.Retorno.HasValue)
                {
                    queryFilter.Append(" AND ag.Retorno = ?Retorno");
                    param.Add("?Retorno", dbType: DbType.Boolean, value: atendimentoFilters.Retorno, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(atendimentoFilters?.TipoDaConsulta))
                {
                    queryFilter.Append(" And ag.TipoDaConsulta Like ?TipoDaConsulta");
                    param.Add("?TipoDaConsulta", dbType: DbType.String, value: atendimentoFilters.TipoDaConsulta, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(atendimentoFilters.Avaliacao))
                {
                    queryFilter.Append(" AND Atendimento.Avaliacao LIKE ?Avaliacao ");
                    param.Add("?Avaliacao", dbType: DbType.String, value: $"%{atendimentoFilters.Avaliacao}%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(atendimentoFilters.Objetivo))
                {
                    queryFilter.Append(" AND Atendimento.Objetivo LIKE ?Objetivo ");
                    param.Add("?Objetivo", dbType: DbType.String, value: $"%{atendimentoFilters.Objetivo}%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(atendimentoFilters.AgendamentoId))
                {
                    queryFilter.Append(" AND Atendimento.AgendamentoId LIKE ?AgendamentoId ");
                    param.Add("?AgendamentoId", dbType: DbType.String, value: $"%{atendimentoFilters.AgendamentoId}%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(atendimentoFilters.Plano))
                {
                    queryFilter.Append(" AND Atendimento.Plano LIKE ?Plano ");
                    param.Add("?Plano", dbType: DbType.String, value: $"%{atendimentoFilters.Plano}%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(atendimentoFilters?.ProfissionalId))
                {
                    queryFilter.Append(" And ag.ProfissionalId Like ?ProfissionalId");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: atendimentoFilters.ProfissionalId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(atendimentoFilters?.IndividuoId))
                {
                    queryFilter.Append(" And ag.IndividuoId Like ?IndividuoId");
                    param.Add("?IndividuoId", dbType: DbType.String, value: atendimentoFilters.IndividuoId, direction: ParameterDirection.Input);
                }
                if (atendimentoFilters.AtendidoTriagem)
                {
                    queryFilter.Append(" AND Atendimento.AtendidoTriagem = ?AtendidoTriagem ");
                    param.Add("?AtendidoTriagem", dbType: DbType.Boolean, value: atendimentoFilters.AtendidoTriagem, direction: ParameterDirection.Input);
                }
                if (atendimentoFilters.PresencaConfirmada.HasValue)
                {
                    queryFilter.Append(" AND ag.PresencaConfirmada = ?PresencaConfirmada ");
                    param.Add("?PresencaConfirmada", dbType: DbType.Boolean, value: atendimentoFilters.PresencaConfirmada, direction: ParameterDirection.Input);
                }
                if (atendimentoFilters.Finalizado.HasValue)
                {
                    queryFilter.Append(" AND ag.Finalizado = ?Finalizado ");
                    param.Add("?Finalizado", dbType: DbType.Boolean, value: atendimentoFilters.Finalizado, direction: ParameterDirection.Input);
                }
                if (atendimentoFilters.Cancelado.HasValue)
                {
                    queryFilter.Append(" AND ag.Cancelado = ?Cancelado ");
                    param.Add("?Cancelado", dbType: DbType.Boolean, value: atendimentoFilters.Cancelado, direction: ParameterDirection.Input);
                }
                if (atendimentoFilters.NaoCompareceu.HasValue)
                {
                    queryFilter.Append(" AND ag.NaoCompareceu = ?NaoCompareceu ");
                    param.Add("?NaoCompareceu", dbType: DbType.Boolean, value: atendimentoFilters.NaoCompareceu, direction: ParameterDirection.Input);
                }
                if (atendimentoFilters.AtendidoMedico)
                {
                    queryFilter.Append(" AND Atendimento.AtendidoMedico = ?AtendidoMedico ");
                    param.Add("?AtendidoMedico", dbType: DbType.Boolean, value: atendimentoFilters.AtendidoMedico, direction: ParameterDirection.Input);
                }
                if (atendimentoFilters.Ativo.HasValue)
                {
                    queryFilter.Append(" AND Atendimento.Ativo = ?Ativo ");
                    param.Add("?Ativo", dbType: DbType.Boolean, value: atendimentoFilters.Ativo, direction: ParameterDirection.Input);
                }

                if (atendimentoFilters.DataCadastro != DateTime.MinValue)
                {
                    DateTime dateOnly = atendimentoFilters.DataCadastro.Date;
                    var x = dateOnly.ToString("yyyy-MM-dd");

                    queryFilter.Append(" And DATE(Atendimento.DataCadastro) = ?DataCadastro");
                    param.Add("?DataCadastro", dbType: DbType.Date, value: x, direction: ParameterDirection.Input);
                }

                if (atendimentoFilters.DataInicial != null && atendimentoFilters.DataFinal != null)
                {
                    atendimentoFilters.DataFinal = atendimentoFilters.DataFinal ?? DateTime.Now;

                    queryFilter.Append(" AND DATE(Atendimento.DataCadastro) BETWEEN DATE(?DataInicial) AND DATE(?DataFinal)");
                    param.Add("?DataInicial", dbType: DbType.Date, value: atendimentoFilters.DataInicial, direction: ParameterDirection.Input);
                    param.Add("?DataFinal", dbType: DbType.Date, value: atendimentoFilters.DataFinal, direction: ParameterDirection.Input);
                }
            }
            queryAll.Append(queryFilter);
            queryCountAll.Append(queryFilter);

            if (!string.IsNullOrEmpty(sort)) queryAll.Append($" ORDER BY Atendimento.{sort} ");

            if (skip.HasValue && take.HasValue)
            {
                skip = (skip - 1) * take;
                queryAll.Append($"LIMIT ?skip, ?take");
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            #region - Busca e mapeamento de atendimentos

            var totalCount = _connection.Query<int>(queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                var atendimentos = new Dictionary<string, Atendimento>();
                await _connection.QueryAsync<Atendimento, Agendamento, Profissional, Individuo, Procedimento, Atendimento>(
                    queryAll.ToString(), (a, ag, p, i, proc) =>
                    {
                        if (!atendimentos.TryGetValue(a.Id, out var atendimentoEntity))
                        {
                            atendimentos.Add(a.Id, atendimentoEntity = a);
                        }
                        atendimentoEntity.Agendamento = ag;
                        atendimentoEntity.Profissional = p;
                        atendimentoEntity.Individuo = i;
                        atendimentoEntity.Procedimento = proc;

                        return atendimentoEntity;
                    },
                    param,
                    splitOn: "Id, Id, Id, Id",
                    commandTimeout: TimeOut,
                    commandType: CommandType.Text);

                #endregion

                #region - IndividuoCiap

                param.Add("?IdIndividuo", dbType: DbType.String, value: atendimentoFilters.IndividuoId, direction: ParameterDirection.Input);

                var queryCiap = new StringBuilder(@"
                    SELECT
	                    CAST( iciap.Id AS CHAR ) AS Id,
	                    CAST( iciap.IndividuoId AS CHAR ) AS IndividuoId,
	                    CAST( iciap.ProfissionalId AS CHAR ) AS ProfissionalId,
	                    CAST( iciap.AgendamentoId AS CHAR ) AS AgendamentoId,
	                    CAST( iciap.CiapId AS CHAR ) AS CiapId,
	                    iciap.DataCadastro,
	                    iciap.DataAlteracao,
	                    iciap.AtendidoTriagem,
	                    iciap.AtendidoMedico,
	                    CAST( ciap.Id AS CHAR ) AS Id,
	                    ciap.Codigo,
	                    ciap.Descricao 
                    FROM
	                    Ciap
	                    INNER JOIN IndividuoCiap iciap ON iciap.CiapId = ciap.Id 
                    -- WHERE
	                   -- iciap.IndividuoId = ? IdIndividuo 
	                   -- AND iciap.AgendamentoId = ? AgendamentoId");

                var individuoCiap = _connection.Query<IndividuoCiap, Ciap, IndividuoCiap>(queryCiap.ToString(), (i, c) =>
                        {
                            i.Ciap = c;
                            return i;
                        }, param, commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                foreach (var atendimento in atendimentos.Values)
                {
                    atendimento.IndividuoCiap = individuoCiap.Where(x => x.IndividuoId == atendimento.Individuo.Id && x.AgendamentoId == atendimento.AgendamentoId).ToList();
                };

                #endregion

                #region - IndividuoCid10

                param.Add("?IdIndividuo", dbType: DbType.String, value: atendimentoFilters.IndividuoId, direction: ParameterDirection.Input);

                var queryCid10 = new StringBuilder(@"
                    SELECT
                        CAST( icid10.Id AS CHAR ) AS Id,
                        CAST( icid10.IndividuoId AS CHAR ) AS IndividuoId,
                        CAST( icid10.ProfissionalId AS CHAR ) AS ProfissionalId,
                        CAST( icid10.AgendamentoId AS CHAR ) AS AgendamentoId,
                        CAST( icid10.Cid10Id AS CHAR ) AS Cid10Id,
                        icid10.DataCadastro,
                        icid10.DataAlteracao,
                        icid10.AtendidoTriagem,
                        icid10.AtendidoMedico,
                        CAST( cid10.Id AS CHAR ) AS Id,
                        cid10.Codigo,
                        cid10.Descricao 
                    FROM
                        Cid10
                        INNER JOIN IndividuoCid10 icid10 ON icid10.Cid10Id = cid10.Id 
                    -- WHERE
                       -- icid10.IndividuoId = ? IdIndividuo 
                       -- AND icid10.AgendamentoId = ? AgendamentoId");

                var individuoCid10 = _connection.Query<IndividuoCid10, Cid, IndividuoCid10>(queryCid10.ToString(),
                    (i, c) =>
                    {
                        i.Cid = c;
                        return i;
                    }, param, commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                try
                {
                    foreach (var atendimento in atendimentos.Values)
                    {
                        atendimento.IndividuoCid10 = individuoCid10.Where(x => x.IndividuoId == atendimento.Individuo.Id && x.AgendamentoId == atendimento.AgendamentoId).ToList();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                #endregion

                #region - Procedimento (SIGTAP)

                param.Add("?IdIndividuo", dbType: DbType.String, value: atendimentoFilters.IndividuoId, direction: ParameterDirection.Input);

                var queryProcedimento = new StringBuilder(@"
                    SELECT
	                    CAST( iproc.Id AS CHAR ) AS Id,
	                    CAST( iproc.IndividuoId AS CHAR ) AS IndividuoId,
	                    CAST( iproc.ProfissionalId AS CHAR ) AS ProfissionalId,
	                    CAST( iproc.AgendamentoId AS CHAR ) AS AgendamentoId,
	                    iproc.ProcedimentoCodigo,
	                    iproc.DataCadastro,
	                    iproc.DataAlteracao,
	                    iproc.AtendidoTriagem,
	                    iproc.AtendidoMedico,
	                    CAST( procedimento.Id AS CHAR ) AS Id,
	                    procedimento.Codigo,
	                    procedimento.Descricao 
                    FROM
	                    Procedimento
	                    INNER JOIN IndividuoProcedimento iproc ON iproc.ProcedimentoCodigo = procedimento.Codigo
                        ");

                var individuoProcedimento = _connection.Query<IndividuoProcedimento, Procedimento, IndividuoProcedimento>(queryProcedimento.ToString(), (i, c) =>
                {
                    i.Procedimento = c;
                    return i;
                }, param, commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                foreach (var atend in atendimentos.Values)
                {
                    atend.IndividuoProcedimentos = individuoProcedimento.Where(x => x.IndividuoId == atend.Individuo.Id && x.AgendamentoId == atend.AgendamentoId).ToList();
                }

                #endregion

                return (atendimentos.Values, totalCount);
            }
            return (null, 0);
        }

        public int Delete(string id)
        {
            var query = @"UPDATE Atendimento SET Ativo = 0 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Atendimento", id, "Desativou Atendimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public Atendimento GetAtendimentoById(string atendimentoId)
        {
            var query = new StringBuilder(@"
                           DISTINCT CAST(Atendimento.Id AS CHAR) AS Id,
                Atendimento.Subjetivo,
                Atendimento.TempoAtendimento,
                Atendimento.Objetivo,
                Atendimento.Avaliacao,
                Atendimento.Plano,
                Atendimento.DataCadastro,
                Atendimento.AtendidoTriagem,
                Atendimento.AtendidoMedico
                FROM
                Atendimento
                WHERE 1 = 1");

            query.Append(@" ) AS Atendimento ");

            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(atendimentoId)) return null;

            query.Append(" AND Atendimento.Id = ?atendimentoId ");
            param.Add("?atendimentoId", dbType: DbType.String, value: atendimentoId, direction: ParameterDirection.Input);

            var result = new Dictionary<string, Atendimento>();

            try
            {
                return _connection.Query<Atendimento>(
                    query.ToString(),
                    param,
                    commandTimeout: TimeOut,
                    commandType: CommandType.Text)
                .SingleOrDefault();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IEnumerable<DashboardTipoAtendimento>> GetTotalTipoAtendimento(DashboardParams dashParams)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" ");
            var param = new DynamicParameters();
            if (dashParams != null)
            {
                if (dashParams.DataInicial != null && dashParams.DataFinal != null)
                {
                    dashParams.DataFinal = dashParams.DataFinal ?? DateTime.Now;

                    queryFilter.Append(" AND DATE(b.Dia) BETWEEN DATE(@DataInicial) AND DATE(@DataFinal)");
                    param.Add("@DataInicial", dbType: DbType.Date, value: dashParams.DataInicial, direction: ParameterDirection.Input);
                    param.Add("@DataFinal", dbType: DbType.Date, value: dashParams.DataFinal, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(dashParams?.TipoDaConsulta))
                {
                    queryFilter.Append(" AND b.TipoDaConsulta = ?TipoDaConsulta");
                    param.Add("?TipoDaConsulta", dbType: DbType.String, value: dashParams.TipoDaConsulta, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(dashParams?.EstabelecimentoId))
                {
                    queryFilter.Append(" AND b.EstabelecimentoId = ?EstabelecimentoId");
                    param.Add("?EstabelecimentoId", dbType: DbType.String, value: dashParams.EstabelecimentoId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(dashParams?.ProfissionalId))
                {
                    queryFilter.Append(" AND b.ProfissionalId = ?ProfissionalId");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: dashParams.ProfissionalId, direction: ParameterDirection.Input);
                }
            }


            var query = new StringBuilder($@"
                            SELECT
                              b.TipoDaConsulta AS TipoDaConsulta,
                              DATE_FORMAT(a.DataCadastro, '%m-%Y') AS Periodo,
                              COUNT(a.Id) AS Total
                            FROM Atendimento a
                            INNER JOIN Agendamento b ON (a.AgendamentoId = b.Id)
                            WHERE 1 = 1
                            AND a.AtendidoMedico = TRUE
                            {queryFilter}
                            GROUP BY
                            b.TipoDaConsulta,
                            MONTH(b.Dia),
                            YEAR(b.Dia);
                            "
            );



            #endregion

            try
            {
                return await _connection.QueryAsync<DashboardTipoAtendimento>(
                query.ToString(),
                param,
                commandTimeout: TimeOut,
                commandType: CommandType.Text);


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Ficha de Procedimento
        public async Task<(IEnumerable<Atendimento>, int)> ProcedimentosPendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {

            var query = new StringBuilder($@"
        SELECT 
        CAST(at.Id AS CHAR) AS Id, at.AtendidoMedico,
        at.Subjetivo,
        at.QueixaDoPaciente,
        at.DataCadastro,
        CAST(at.TempoAtendimento AS CHAR) AS TempoAtendimento,
        at.InicioDoAtendimento,
        at.FimDoAtendimento,
        at.LocalDeAtendimento,
        at.Objetivo,
        at.Avaliacao,
        at.Plano,
        CAST(at.AgendamentoId AS CHAR) AgendamentoId,
        at.DataCadastro,
        at.AtendidoTriagem,
        at.AtendidoMedico,
        at.Ativo,
        at.DataAlteracao,
        at.DadoSerializado,
        at.LoteIntegracaoId,
        at.Alergias,
        at.Antecedentes,
        at.CondicoesAvaliadas,
        at.CondutaDesfecho,
        at.ModalidadeAD,
        at.ProcedimentosRealizados,
        at.TipoAtendimento,

        CAST(ag.Id AS CHAR) AS Id,
        CAST(ag.IndividuoId AS CHAR) AS IndividuoId,
        CAST(ag.ProfissionalId AS CHAR) AS ProfissionalId,
        CAST(ag.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
        CAST(ag.ProcedimentoId AS CHAR) AS ProcedimentoId,
        ag.Dia,
        ag.Hora,
        ag.TipoDaConsulta,
        ag.Observacoes,
        ag.Ativo,
        ag.DataAlteracao, 
        ag.DataCadastro,
        ag.EmAndamento,
        ag.Finalizado,
        ag.Cancelado,
        ag.EmAtendimentoMedico,
        ag.Retorno,
        ag.PresencaConfirmada,
        ag.NaoCompareceu,
        ag.PressaoSanguinea,
        ag.OxigenacaoSanguinea,
        ag.BatimentoCardiaco,
        ag.Altura,
        ag.Peso,
        ag.Temperatura,
        ag.CorStatusTriagem,
        ag.DataInicio,
        ag.DataFim,
        ag.NumProntuario,
        CAST(e.Id AS CHAR) AS Id, e.NomeFantasia, e.Cnes, e.CodIne, e.CodIbgeMun,
        CAST(i.Id AS CHAR) AS Id,
        CAST(i.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
        i.Cpf,
        i.Cns,
        i.Email,
        i.TelefoneCelular,
        i.NomeCompleto,
        i.NomeDaMae,
        i.NomeSocial,
        i.DataNascimento,
        TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
        i.Sexo,
        i.RacaOuCor,
        i.DataCadastro,
        i.DataAlteracao,
        i.Latitude,
        i.Longitude,
        i.Logradouro,
        i.LogradouroNumero,
        i.LogradouroComplemento,
        i.LogradouroCep,
        i.LogradouroBairro,
        i.UfAbreviado,
        i.CidadeId,
        i.Ativo,
        i.RespondeuComorbidade,
        i.Comorbidades,
        i.Hipertenso,
        i.Diabetes,
        i.Fumante,
        i.Asma,
        i.DoencaCoracao,
        i.DoencaPulmao,
        i.DoencaRins,
        i.DoencaFigado,
        i.DoencaCancer,
        i.Transplantado,
        i.DoencaComprometeImunidade,
        i.LugarComCasosCorona,
        i.DataInicioSintomas,
        i.Obesidade,
        i.Gestante,
        i.DoencaNeurologica,
        i.AnemiaFalciforme,
        i.CidadeDeNascimentoIbge,
        i.TemMaeDesconhecida,
        i.Nacionalidade,
        i.NomeDoPai,
        i.PisPasep,
        i.PaisDeNascimento,
        i.TemPaiDesconhecido,
        i.GrauDeInstrucao,
        i.FrequentaEscola,
        CAST(p.Id AS CHAR) AS Id,
        p.Nome,
        p.Cpf,
        p.Cns,
        p.DataNascimento,
        p.Crm,
        o1.Id,
        o1.Codigo,
        o1.Descricao,
        CAST(u.Id AS CHAR) AS Id,
        u.Cpf,
        u.Cns,
        o2.Id,
        o2.Codigo,
        o2.Descricao
        FROM Atendimento at
        INNER JOIN Agendamento ag ON ag.Id = at.AgendamentoId
        INNER JOIN Estabelecimento e ON e.Id = ag.EstabelecimentoId
        INNER JOIN Individuo i ON i.Id = ag.IndividuoId
        LEFT JOIN Profissional p ON p.Id = ag.ProfissionalId
        LEFT JOIN Ocupacao o1 ON o1.Id = p.OcupacaoId
        LEFT JOIN Usuario u ON u.Id = at.UsuarioId
        LEFT JOIN Ocupacao o2 ON o2.Id = u.OcupacaoId
        WHERE 1 = 1
        AND at.Ativo = 1
        AND ag.PresencaConfirmada = TRUE
        AND at.AtendidoMedico = FALSE
        -- AND at.DadoSerializado IS NULL
        AND EXTRACT(MONTH FROM at.DataAlteracao) = ?mes
        AND EXTRACT(YEAR FROM at.DataAlteracao) = ?ano ");

            var queryCount = new StringBuilder($@"
        SELECT COUNT(1)
        FROM Atendimento at
        INNER JOIN Agendamento ag ON ag.Id = at.AgendamentoId
        INNER JOIN Estabelecimento e ON e.Id = ag.EstabelecimentoId
        INNER JOIN Individuo i ON i.Id = ag.IndividuoId
        LEFT JOIN Profissional p ON p.Id = ag.ProfissionalId
        LEFT JOIN Ocupacao o1 ON o1.Id = p.OcupacaoId
        LEFT JOIN Usuario u ON u.Id = at.UsuarioId
        LEFT JOIN Ocupacao o2 ON o2.Id = u.OcupacaoId
        WHERE 1 = 1
        AND at.Ativo = 1
        AND ag.PresencaConfirmada = TRUE
        AND at.AtendidoMedico = FALSE
        -- AND at.DadoSerializado IS NULL
        AND EXTRACT(MONTH FROM at.DataAlteracao) = ?mes
        AND EXTRACT(YEAR FROM at.DataAlteracao) = ?ano ");

            #region -- -- Filtros / Ordenação
            var param = new DynamicParameters();

            param.Add("?estabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);
            param.Add("?mes", dbType: DbType.Int32, value: mes, direction: ParameterDirection.Input);
            param.Add("?ano", dbType: DbType.Int32, value: ano, direction: ParameterDirection.Input);

            if (!string.IsNullOrEmpty(estabelecimentoId))
            {
                query.Append(" AND e.Id = ?estabelecimentoId ");
                queryCount.Append(" AND e.Id = ?estabelecimentoId ");
            }

            #endregion

            try
            {
                var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();
                var list = new Dictionary<string, Atendimento>();

                if (totalCount > 0)
                {
                    await _connection.QueryAsync<Atendimento>(query.ToString(),
                        new[]
                        {
                    typeof(Atendimento), typeof(Agendamento), typeof(Estabelecimento), typeof(Individuo), typeof(Profissional), typeof(Ocupacao), typeof(User), typeof(Ocupacao),
                        },
                        objects =>
                        {
                            var atendimento = objects[0] as Atendimento;
                            var agendamento = objects[1] as Agendamento;
                            var estabelecimento = objects[2] as Estabelecimento;
                            var individuo = objects[3] as Individuo;
                            var profissional = objects[4] as Profissional;
                            var ocupacaoMedico = objects[5] as Ocupacao;
                            var usuario = objects[6] as User;
                            var ocupacaoRecepcao = objects[7] as Ocupacao;

                            if (!list.TryGetValue(atendimento.Id, out var entity))
                            {
                                entity = atendimento;
                                list.Add(atendimento.Id, entity);
                            }

                            if (agendamento != null)
                            {
                                entity.Agendamento = agendamento;
                                if (estabelecimento != null)
                                    entity.Agendamento.Estabelecimento = estabelecimento;
                                if (profissional != null)
                                {
                                    entity.Agendamento.Profissional = profissional;
                                    if (ocupacaoMedico != null)
                                        entity.Agendamento.Profissional.Ocupacao = ocupacaoMedico;
                                }
                            }

                            if (usuario != null)
                                entity.Usuario = usuario;
                            if (ocupacaoRecepcao != null)
                                entity.Usuario.Ocupacao = ocupacaoRecepcao;

                            if (entity.Individuo == null)
                                if (individuo != null)
                                    entity.Individuo = individuo;

                            return entity;
                        }, param, splitOn: "Id, Id, Id, Id, Id, Id, Id", commandTimeout: 0, commandType: CommandType.Text);

                    foreach (var atendimento in list.Values)
                    {
                        param.Add("?AgendamentoId", dbType: DbType.String, value: atendimento.AgendamentoId, direction: ParameterDirection.Input);

                        #region CIAPS do Atendimento
                        var queryIndividuoCiap = @"
                        SELECT 
                        CAST(IndividuoCiap.Id AS CHAR) AS Id,
                        CAST(IndividuoCiap.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoCiap.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoCiap.CiapId AS CHAR) AS CiapId,  
                        CAST(IndividuoCiap.AgendamentoId AS CHAR) AS AgendamentoId,
                        IndividuoCiap.DataCadastro,
                        IndividuoCiap.DataAlteracao,
                        Individuo.NomeCompleto,
                        IndividuoCiap.AtendidoTriagem,
                        IndividuoCiap.AtendidoMedico,
                        IndividuoCiap.DadoSerializado,
                        CAST(Ciap.Id AS CHAR) AS Id,
                        Ciap.Descricao,
                        Ciap.Codigo,
                        Ciap.Sexo
                        FROM IndividuoCiap
                        INNER JOIN Individuo ON (IndividuoCiap.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoCiap.ProfissionalId = Profissional.Id)
                        INNER JOIN Ciap ON (IndividuoCiap.CiapId = Ciap.Id)
                        WHERE IndividuoCiap.AgendamentoId = ?AgendamentoId 
                        -- AND IndividuoCiap.DadoSerializado IS NULL ";

                        var individuoCiapList = _connection.Query<IndividuoCiap, Ciap, IndividuoCiap>(
                        queryIndividuoCiap.ToString(), (i, c) => { i.Ciap = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoCiapList != null)
                            atendimento.IndividuoCiap = individuoCiapList;
                        #endregion

                        #region CID10 do Atendimento
                        var queryIndividuoCid10 = @"
                        SELECT
                        CAST(IndividuoCid10.Id AS CHAR) AS Id,
                        CAST(IndividuoCid10.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoCid10.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoCid10.Cid10Id AS CHAR) AS Cid10Id,
                        CAST(IndividuoCid10.AgendamentoId AS CHAR) AS AgendamentoId,
                        IndividuoCid10.DataCadastro,
                        IndividuoCid10.DataAlteracao,
                        Individuo.NomeCompleto,
                        IndividuoCid10.AtendidoTriagem,
                        IndividuoCid10.AtendidoMedico,
                        IndividuoCid10.DadoSerializado,
                        CAST(Cid10.Id AS CHAR) AS Id,
                        Cid10.Descricao,
                        Cid10.Codigo,
                        Cid10.Sexo
                        FROM IndividuoCid10
                        INNER JOIN Individuo ON (IndividuoCid10.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoCid10.ProfissionalId = Profissional.Id)
                        INNER JOIN Cid10 ON (IndividuoCid10.Cid10Id = Cid10.Id)
                        WHERE IndividuoCid10.AgendamentoId = ?AgendamentoId
                        -- AND IndividuoCid10.DadoSerializado IS NULL
                        ";

                        var individuoCid10List = _connection.Query<IndividuoCid10, Cid, IndividuoCid10>(
                        queryIndividuoCid10.ToString(), (i, c) => { i.Cid = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoCid10List != null)
                            atendimento.IndividuoCid10 = individuoCid10List;
                        #endregion

                        #region Procedimentos do Atendimento
                        var queryIndividuoProcedimentos = @"
                        SELECT
                        CAST(IndividuoProcedimento.Id AS CHAR) AS Id,
                        CAST(IndividuoProcedimento.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoProcedimento.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoProcedimento.AgendamentoId AS CHAR) AS AgendamentoId,
                        IndividuoProcedimento.ProcedimentoCodigo,
                        IndividuoProcedimento.DataCadastro,
                        IndividuoProcedimento.DataAlteracao,
                        IndividuoProcedimento.AtendidoTriagem,
                        IndividuoProcedimento.AtendidoMedico,
                        CAST(Procedimento.Id AS CHAR) AS Id,
                        Procedimento.Descricao,
                        Procedimento.Tipo,
                        Procedimento.Codigo,
                        Procedimento.Sexo,
                        Procedimento.Grupo
                        FROM IndividuoProcedimento
                        INNER JOIN Individuo ON (IndividuoProcedimento.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoProcedimento.ProfissionalId = Profissional.Id)
                        INNER JOIN Procedimento ON (IndividuoProcedimento.ProcedimentoCodigo = Procedimento.Codigo)
                        WHERE IndividuoProcedimento.AgendamentoId = ?AgendamentoId ";

                        var individuoProcedimentoList = _connection.Query<IndividuoProcedimento, Procedimento, IndividuoProcedimento>(
                        queryIndividuoProcedimentos.ToString(), (i, c) => { i.Procedimento = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoProcedimentoList != null)
                            atendimento.IndividuoProcedimentos = individuoProcedimentoList;
                        #endregion

                        #region ExamesF200 do Agendamento
                        var queryExamesF200 = @"
                        SELECT
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
                        e.Formato
                        FROM ExamesF200 e 
                        INNER JOIN Agendamento a ON a.Id = e.AgendamentoId
                        AND e.AgendamentoId = ?AgendamentoId ";

                        var examesF200List = _connection.Query<ExamesF200>(
                        queryExamesF200.ToString(), param, commandType: CommandType.Text).ToList();

                        if (examesF200List != null)
                            atendimento.Agendamento.ExamesF200 = examesF200List;
                        #endregion

                    }

                    return (list.Values, totalCount);
                }
                return (list.Values, 0);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<(IEnumerable<Atendimento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId, bool includeAD)
        {
            var query = new StringBuilder($@"
        SELECT 
        CAST(at.Id AS CHAR) AS Id, at.AtendidoMedico,
        at.Subjetivo,
        at.QueixaDoPaciente,
        at.DataCadastro,
        CAST(at.TempoAtendimento AS CHAR) AS TempoAtendimento,
        at.InicioDoAtendimento,
        at.FimDoAtendimento,
        at.LocalDeAtendimento,
        at.Objetivo,
        at.Avaliacao,
        at.Plano,
        CAST(at.AgendamentoId AS CHAR) AgendamentoId,
        at.DataCadastro,
        at.AtendidoTriagem,
        at.AtendidoMedico,
        at.Ativo,
        at.DataAlteracao,
        at.DadoSerializado,
        at.LoteIntegracaoId,
        at.Alergias,
        at.Antecedentes,
        at.CondicoesAvaliadas,
        at.CondutaDesfecho,
        at.ModalidadeAD,
        at.ProcedimentosRealizados,
        at.TipoAtendimento,
        CAST(ag.Id AS CHAR) AS Id,
        CAST(ag.IndividuoId AS CHAR) AS IndividuoId,
        CAST(ag.ProfissionalId AS CHAR) AS ProfissionalId,
        CAST(ag.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
        CAST(ag.ProcedimentoId AS CHAR) AS ProcedimentoId,
        ag.Dia,
        ag.Hora,
        ag.TipoDaConsulta,
        ag.Observacoes,
        ag.Ativo,
        ag.DataAlteracao, 
        ag.DataCadastro,
        ag.EmAndamento,
        ag.Finalizado,
        ag.Cancelado,
        ag.EmAtendimentoMedico,
        ag.Retorno,
        ag.PresencaConfirmada,
        ag.NaoCompareceu,
        ag.PressaoSanguinea,
        ag.OxigenacaoSanguinea,
        ag.BatimentoCardiaco,
        ag.Altura,
        ag.Peso,
        ag.Temperatura,
        ag.CorStatusTriagem,
        ag.DataInicio,
        ag.DataFim,
        ag.NumProntuario,
        ag.Condutas,
        CAST(e.Id AS CHAR) AS Id, e.NomeFantasia, e.Cnes, e.CodIne, e.CodIbgeMun,
        CAST(i.Id AS CHAR) AS Id,
        CAST(i.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
        i.Cpf,
        i.Cns,
        i.Email,
        i.TelefoneCelular,
        i.NomeCompleto,
        i.NomeDaMae,
        i.NomeSocial,
        i.DataNascimento,
        TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
        i.Sexo,
        i.RacaOuCor,
        i.DataCadastro,
        i.DataAlteracao,
        i.Latitude,
        i.Longitude,
        i.Logradouro,
        i.LogradouroNumero,
        i.LogradouroComplemento,
        i.LogradouroCep,
        i.LogradouroBairro,
        i.UfAbreviado,
        i.CidadeId,
        i.Ativo,
        i.RespondeuComorbidade,
        i.Comorbidades,
        i.Hipertenso,
        i.Diabetes,
        i.Fumante,
        i.Asma,
        i.DoencaCoracao,
        i.DoencaPulmao,
        i.DoencaRins,
        i.DoencaFigado,
        i.DoencaCancer,
        i.Transplantado,
        i.DoencaComprometeImunidade,
        i.LugarComCasosCorona,
        i.DataInicioSintomas,
        i.Obesidade,
        i.Gestante,
        i.DoencaNeurologica,
        i.AnemiaFalciforme,
        i.CidadeDeNascimentoIbge,
        i.TemMaeDesconhecida,
        i.Nacionalidade,
        i.NomeDoPai,
        i.PisPasep,
        i.PaisDeNascimento,
        i.TemPaiDesconhecido,
        i.GrauDeInstrucao,
        i.FrequentaEscola,
        CAST(p.Id AS CHAR) AS Id,
        p.Nome,
        p.Cpf,
        p.Cns,
        p.DataNascimento,
        p.Crm,
        o1.Id,
        o1.Codigo,
        o1.Descricao
        FROM Atendimento at
        INNER JOIN Agendamento ag ON ag.Id = at.AgendamentoId
        INNER JOIN Estabelecimento e ON e.Id = ag.EstabelecimentoId
        INNER JOIN Individuo i ON i.Id = ag.IndividuoId
        LEFT JOIN Profissional p ON p.Id = ag.ProfissionalId
        LEFT JOIN Ocupacao o1 ON o1.Id = p.OcupacaoId
        WHERE 1 = 1
        AND at.Ativo = 1
        AND at.AtendidoMedico = TRUE
        AND ag.Finalizado = TRUE
        -- AND at.DadoSerializado IS NULL
        AND EXTRACT(MONTH FROM at.DataAlteracao) = ?mes
        AND EXTRACT(YEAR FROM at.DataAlteracao) = ?ano ");

            var queryCount = new StringBuilder($@"
        SELECT COUNT(1)
        FROM Atendimento at
        INNER JOIN Agendamento ag ON ag.Id = at.AgendamentoId
        INNER JOIN Estabelecimento e ON e.Id = ag.EstabelecimentoId
        INNER JOIN Individuo i ON i.Id = ag.IndividuoId
        LEFT JOIN Profissional p ON p.Id = ag.ProfissionalId
        LEFT JOIN Ocupacao o1 ON o1.Id = p.OcupacaoId
        WHERE 1 = 1
        AND at.Ativo = 1
        AND at.AtendidoMedico = TRUE
        AND ag.Finalizado = TRUE
        -- AND at.DadoSerializado IS NULL
        AND EXTRACT(MONTH FROM at.DataAlteracao) = ?mes
        AND EXTRACT(YEAR FROM at.DataAlteracao) = ?ano ");

            #region -- -- Filtros / Ordenação
            var param = new DynamicParameters();

            param.Add("?estabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);
            param.Add("?mes", dbType: DbType.Int32, value: mes, direction: ParameterDirection.Input);
            param.Add("?ano", dbType: DbType.Int32, value: ano, direction: ParameterDirection.Input);

            if (!string.IsNullOrEmpty(estabelecimentoId))
            {
                query.Append(" AND e.Id = ?estabelecimentoId ");
                queryCount.Append(" AND e.Id = ?estabelecimentoId ");
            }

            // exclui atendimento domiciliar
            if (includeAD == false)
            {
                query.Append(" AND at.ModalidadeAD IS NULL ");
                queryCount.Append(" AND at.ModalidadeAD IS NULL ");
            }
            // apenas atendimento domiciliar
            if (includeAD == true)
            {
                query.Append(" AND at.ModalidadeAD IS NOT NULL ");
                queryCount.Append(" AND at.ModalidadeAD IS NOT NULL ");
            }

            #endregion

            try
            {
                var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();
                var list = new Dictionary<string, Atendimento>();

                if (totalCount > 0)
                {
                    await _connection.QueryAsync<Atendimento>(query.ToString(),
                        new[]
                        {
                    typeof(Atendimento), typeof(Agendamento), typeof(Estabelecimento), typeof(Individuo), typeof(Profissional), typeof(Ocupacao)
                        },
                        objects =>
                        {
                            var atendimento = objects[0] as Atendimento;
                            var agendamento = objects[1] as Agendamento;
                            var estabelecimento = objects[2] as Estabelecimento;
                            var individuo = objects[3] as Individuo;
                            var profissional = objects[4] as Profissional;
                            var ocupacao = objects[5] as Ocupacao;

                            if (!list.TryGetValue(atendimento.Id, out var entity))
                            {
                                entity = atendimento;
                                list.Add(atendimento.Id, entity);
                            }

                            if (agendamento != null)
                            {
                                entity.Agendamento = agendamento;
                                if (estabelecimento != null)
                                    entity.Agendamento.Estabelecimento = estabelecimento;
                                if (profissional != null)
                                {
                                    entity.Agendamento.Profissional = profissional;
                                    if (ocupacao != null)
                                        entity.Agendamento.Profissional.Ocupacao = ocupacao;
                                }
                            }

                            if (entity.Individuo == null)
                                if (individuo != null)
                                    entity.Individuo = individuo;

                            return entity;
                        }, param, splitOn: "Id, Id, Id, Id", commandTimeout: 0, commandType: CommandType.Text);

                    foreach (var atendimento in list.Values)
                    {
                        param.Add("?AgendamentoId", dbType: DbType.String, value: atendimento.AgendamentoId, direction: ParameterDirection.Input);

                        #region CIAPS do Atendimento
                        var queryIndividuoCiap = @"
                        SELECT 
                        CAST(IndividuoCiap.Id AS CHAR) AS Id,
                        CAST(IndividuoCiap.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoCiap.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoCiap.CiapId AS CHAR) AS CiapId,
                        CAST(IndividuoCiap.AgendamentoId AS CHAR) AS AgendamentoId,
                        IndividuoCiap.DataCadastro,
                        IndividuoCiap.DataAlteracao,
                        Individuo.NomeCompleto,
                        IndividuoCiap.AtendidoTriagem,
                        IndividuoCiap.AtendidoMedico,
                        CAST(Ciap.Id AS CHAR) AS Id,
                        Ciap.Descricao,
                        Ciap.Codigo,
                        Ciap.Sexo
                        FROM IndividuoCiap
                        INNER JOIN Individuo ON (IndividuoCiap.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoCiap.ProfissionalId = Profissional.Id)
                        INNER JOIN Ciap ON (IndividuoCiap.CiapId = Ciap.Id)
                        WHERE IndividuoCiap.AgendamentoId = ?AgendamentoId ";

                        var individuoCiapList = _connection.Query<IndividuoCiap, Ciap, IndividuoCiap>(
                        queryIndividuoCiap.ToString(), (i, c) => { i.Ciap = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoCiapList != null)
                            atendimento.IndividuoCiap = individuoCiapList;
                        #endregion

                        #region CID10 do Atendimento
                        var queryIndividuoCid10 = @"
                        SELECT
                        CAST(IndividuoCid10.Id AS CHAR) AS Id,
                        CAST(IndividuoCid10.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoCid10.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoCid10.Cid10Id AS CHAR) AS Cid10Id,
                        CAST(IndividuoCid10.AgendamentoId AS CHAR) AS AgendamentoId,
                        IndividuoCid10.DataCadastro,
                        IndividuoCid10.DataAlteracao,
                        Individuo.NomeCompleto,
                        IndividuoCid10.AtendidoTriagem,
                        IndividuoCid10.AtendidoMedico,
                        CAST(Cid10.Id AS CHAR) AS Id,
                        Cid10.Descricao,
                        Cid10.Codigo,
                        Cid10.Sexo
                        FROM IndividuoCid10
                        INNER JOIN Individuo ON (IndividuoCid10.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoCid10.ProfissionalId = Profissional.Id)
                        INNER JOIN Cid10 ON (IndividuoCid10.Cid10Id = Cid10.Id)
                        WHERE IndividuoCid10.AgendamentoId = ?AgendamentoId ";

                        var individuoCid10List = _connection.Query<IndividuoCid10, Cid, IndividuoCid10>(
                        queryIndividuoCid10.ToString(), (i, c) => { i.Cid = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoCid10List != null)
                            atendimento.IndividuoCid10 = individuoCid10List;
                        #endregion

                        #region Procedimentos do Atendimento
                        var queryIndividuoProcedimentos = @"
                        SELECT
                        CAST(IndividuoProcedimento.Id AS CHAR) AS Id,
                        CAST(IndividuoProcedimento.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoProcedimento.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoProcedimento.AgendamentoId AS CHAR) AS AgendamentoId,
                        IndividuoProcedimento.ProcedimentoCodigo,
                        IndividuoProcedimento.DataCadastro,
                        IndividuoProcedimento.DataAlteracao,
                        IndividuoProcedimento.AtendidoTriagem,
                        IndividuoProcedimento.AtendidoMedico,
                        CAST(Procedimento.Id AS CHAR) AS Id,
                        Procedimento.Descricao,
                        Procedimento.Tipo,
                        Procedimento.Codigo,
                        Procedimento.Sexo,
                        Procedimento.Grupo
                        FROM IndividuoProcedimento
                        INNER JOIN Individuo ON (IndividuoProcedimento.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoProcedimento.ProfissionalId = Profissional.Id)
                        INNER JOIN Procedimento ON (IndividuoProcedimento.ProcedimentoCodigo = Procedimento.Codigo)
                        WHERE IndividuoProcedimento.AgendamentoId = ?AgendamentoId ";

                        var individuoProcedimentoList = _connection.Query<IndividuoProcedimento, Procedimento, IndividuoProcedimento>(
                        queryIndividuoProcedimentos.ToString(), (i, c) => { i.Procedimento = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoProcedimentoList != null)
                            atendimento.IndividuoProcedimentos = individuoProcedimentoList;
                        #endregion

                        #region ExamesF200 do Agendamento
                        var queryExamesF200 = @"
                        SELECT
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
                        e.Formato
                        FROM ExamesF200 e 
                        INNER JOIN Agendamento a ON a.Id = e.AgendamentoId
                        AND e.AgendamentoId = ?AgendamentoId ";

                        var examesF200List = _connection.Query<ExamesF200>(
                        queryExamesF200.ToString(), param, commandType: CommandType.Text).ToList();

                        if (examesF200List != null)
                            atendimento.Agendamento.ExamesF200 = examesF200List;
                        #endregion

                    }

                    return (list.Values, totalCount);
                }
                return (list.Values, 0);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public void ConfirmarIntegracao(int lote, Atendimento obj)
        {

            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();
                param.Add("?LoteIntegracaoId", dbType: DbType.Int32, value: lote, direction: ParameterDirection.Input);
                param.Add("?AtendimentoId", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
                param.Add("?DadoSerializado", dbType: DbType.String, value: obj.DadoSerializado, direction: ParameterDirection.Input);

                var query = @"UPDATE Atendimento SET
                        LoteIntegracaoId = ?LoteIntegracaoId,
                        DadoSerializado = ?DadoSerializado
                        WHERE Id = ?AtendimentoId";

                _connection.Execute(query, param, commandTimeout: 0);
                transaction.Commit();
            }
        }

        public void ConfirmarIntegracaoProcedimentos(int lote, Atendimento obj)
        {
            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("?LoteIntegracaoId", dbType: DbType.Int32, value: lote, direction: ParameterDirection.Input);
                    param.Add("?AtendimentoId", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
                    param.Add("?DadoSerializado", dbType: DbType.String, value: obj.DadoSerializado, direction: ParameterDirection.Input);

                    var query = @"UPDATE Atendimento SET
                        LoteIntegracaoId = ?LoteIntegracaoId,
                        DadoSerializado = ?DadoSerializado
                        WHERE Id = ?AtendimentoId";
                    _connection.Execute(query, param, commandTimeout: 0);

                    //var queryUpdateCid10 = @"UPDATE IndividuoCid10 SET
                    //    DadoSerializado = ?DadoSerializado
                    //    WHERE AgendamentoId = ?AgendamentoId";
                    //_connection.Execute(queryUpdateCid10, param, commandTimeout: 0);

                    //var queryUpdateCiap = @"UPDATE IndividuoCiap SET
                    //    DadoSerializado = ?DadoSerializado
                    //    WHERE AgendamentoId = ?AgendamentoId";
                    //_connection.Execute(queryUpdateCiap, param, commandTimeout: 0);


                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
