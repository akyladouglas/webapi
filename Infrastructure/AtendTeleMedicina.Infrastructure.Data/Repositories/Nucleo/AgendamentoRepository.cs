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

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class AgendamentoRepository : BaseRepository, IAgendamentoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(a.Id AS CHAR) AS Id,
                CAST(a.IndividuoId AS CHAR) AS IndividuoId,
                CAST(a.ProfissionalId AS CHAR) AS ProfissionalId,
                CAST(a.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                CAST(a.ProcedimentoId AS CHAR) AS ProcedimentoId,
                a.Dia AS Dia,
                a.Interconsulta,
                a.PacientePresente,
                a.Motivo,
                a.Hora AS Hora,
                a.TipoDaConsulta,
                a.Interconsulta,
                a.PacientePresente,
                a.Motivo,
                a.Observacoes,
                a.Ativo,
                a.DataAlteracao, 
                a.DataCadastro,
                a.EmAndamento,
                a.Finalizado,
                a.Cancelado,
                a.EmAtendimentoMedico,
                a.Retorno,
                a.VinculoRetorno,
                CAST(a.RetornoAgendamentoId AS CHAR) AS RetornoAgendamentoId,
                a.PresencaConfirmada,
                a.NaoCompareceu,
                a.PressaoSanguinea,
                a.OxigenacaoSanguinea,
                a.BatimentoCardiaco,
                a.Altura,
                a.Peso,
                a.Temperatura,
                a.CorStatusTriagem,
                a.GraficoEcg,
                CAST(i.Id AS CHAR) AS Id,
                i.NomeCompleto,
                i.Cpf,
                i.NomeDaMae,
                i.RacaOuCor,
                i.DataNascimento,
                TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
                i.Cns,
                i.Email,
                i.EmailToken,
                i.CodigoAutenticacao,
                i.Senha,
                i.TelefoneCelular,
                i.GrauParentescoResponsavel,
                i.NomeResponsavel,
                i.CpfResponsavel,
                i.Sexo,
                i.RacaOuCor,
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
                i.SintomasGripais,
                i.Obesidade,
                i.Gestante,
                i.DoencaNeurologica,
                i.AnemiaFalciforme,
                i.DataInicioSintomas,
                i.Ativo,
                i.EstabelecimentoId,
                i.NotificacaoToken,
                i.Imagem,
                CAST(i.FaceToken AS CHAR) AS FaceToken,
                i.FaceTokenValidade,
                i.NomeSocial,
                i.Face,
                i.DocumentFront,
                i.DocumentBack,
                CAST(p.Id AS CHAR) AS Id,
                p.Nome,
                p.Imagem,
                p.Crm,
                p.DataAlteracao,
                CAST(e.Id AS CHAR) AS Id,
                e.NomeFantasia,
                CAST(pro.Id AS CHAR) AS Id,
                pro.Descricao
                FROM Agendamento a 
                LEFT JOIN Individuo i ON i.Id = a.IndividuoId
                LEFT JOIN Profissional p ON p.Id = a.ProfissionalId
                LEFT JOIN Estabelecimento e ON e.Id = a.EstabelecimentoId
                LEFT JOIN Procedimento pro ON pro.Id = a.ProcedimentoId
                ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(a.Id) FROM Agendamento a");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public AgendamentoRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public string Add(Agendamento obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
            param.Add("?EstabelecimentoId", dbType: DbType.String, value: obj.EstabelecimentoId, direction: ParameterDirection.Input);
            param.Add("?ProcedimentoId", dbType: DbType.String, value: obj.ProcedimentoId, direction: ParameterDirection.Input);
            param.Add("?Dia", dbType: DbType.Date, value: obj.Dia, direction: ParameterDirection.Input);
            param.Add("?Hora", dbType: DbType.Time, value: obj.Hora, direction: ParameterDirection.Input);
            param.Add("?TipoDaConsulta", dbType: DbType.String, value: obj.TipoDaConsulta, direction: ParameterDirection.Input);
            param.Add("?Observacoes", dbType: DbType.String, value: obj.Observacoes, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);
            param.Add("?Retorno", dbType: DbType.Boolean, value: obj.Retorno, direction: ParameterDirection.Input);
            param.Add("?Cancelado", dbType: DbType.Boolean, value: obj.Cancelado, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);
            param.Add("?PresencaConfirmada", dbType: DbType.Boolean, value: obj.PresencaConfirmada, direction: ParameterDirection.Input);
            param.Add("?RetornoAgendamentoId", dbType: DbType.String, value: obj.RetornoAgendamentoId, direction: ParameterDirection.Input);
            param.Add("?VinculoRetorno", dbType: DbType.Boolean, value: obj.VinculoRetorno, direction: ParameterDirection.Input);
            param.Add("?NumProntuario", dbType: DbType.String, value: obj.NumProntuario, direction: ParameterDirection.Input);
            param.Add("?Condutas", dbType: DbType.String, value: obj.Condutas, direction: ParameterDirection.Input);
            param.Add("?TipoDeAtendimento", dbType: DbType.Int32, value: obj.TipoDeAtendimento, direction: ParameterDirection.Input);
            param.Add("?GraficoEcg", dbType: DbType.String, value: obj.GraficoEcg, direction: ParameterDirection.Input);

            const string insertQuery = @"INSERT INTO Agendamento (Id, IndividuoId, ProfissionalId, EstabelecimentoId, ProcedimentoId, Dia, Hora, TipoDaConsulta, Observacoes, Ativo, Retorno, Cancelado, DataAlteracao, DataCadastro, PresencaConfirmada, RetornoAgendamentoId, VinculoRetorno, NumProntuario, TipoDeAtendimento, Condutas, GraficoEcg)
                                                     VALUES (?Id, ?IndividuoId, ?ProfissionalId, ?EstabelecimentoId, ?ProcedimentoId, ?Dia, ?Hora, ?TipoDaConsulta, ?Observacoes, ?Ativo, ?Retorno, ?Cancelado, ?DataAlteracao, ?DataCadastro, ?PresencaConfirmada, ?RetornoAgendamentoId, ?VinculoRetorno, ?NumProntuario, ?TipoDeAtendimento, ?Condutas, ?GraficoEcg)";

            using (var transaction = _connection.BeginTransaction())
            {
                _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Inseriu Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                #region Marca Horário como Utilizado
                var queryEph = @"UPDATE EstabelecimentoProcedimentoHorario 
                SET Situacao = 1
                WHERE 1 = 1
                AND ProfissionalId = ?ProfissionalId 
                AND EstabelecimentoId = ?EstabelecimentoId
                AND ProcedimentoId = ?ProcedimentoId
                AND Dia = ?Dia
                AND Hora = ?Hora ";
                _connection.Execute(queryEph, param, transaction);
                #endregion

                transaction.Commit();
                return obj.Id;
            }
        }

        public int Update(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
            param.Add("?EstabelecimentoId", dbType: DbType.String, value: obj.EstabelecimentoId, direction: ParameterDirection.Input);
            param.Add("?ProcedimentoId", dbType: DbType.String, value: obj.ProcedimentoId, direction: ParameterDirection.Input);
            param.Add("?Dia", dbType: DbType.Date, value: obj.Dia, direction: ParameterDirection.Input);
            param.Add("?Hora", dbType: DbType.Time, value: obj.Hora, direction: ParameterDirection.Input);
            param.Add("?TipoDaConsulta", dbType: DbType.String, value: obj.TipoDaConsulta, direction: ParameterDirection.Input);
            param.Add("?Observacoes", dbType: DbType.String, value: obj.Observacoes, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);
            param.Add("?Cancelado", dbType: DbType.Boolean, value: obj.Cancelado, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);
            param.Add("?BatimentoCardiaco", dbType: DbType.String, value: obj.BatimentoCardiaco, direction: ParameterDirection.Input);
            param.Add("?PressaoSanguinea", dbType: DbType.String, value: obj.PressaoSanguinea, direction: ParameterDirection.Input);
            param.Add("?OxigenacaoSanguinea", dbType: DbType.String, value: obj.OxigenacaoSanguinea, direction: ParameterDirection.Input);
            param.Add("?Altura", dbType: DbType.String, value: obj.Altura, direction: ParameterDirection.Input);
            param.Add("?Peso", dbType: DbType.String, value: obj.Peso, direction: ParameterDirection.Input);
            param.Add("?Temperatura", dbType: DbType.String, value: obj.Temperatura, direction: ParameterDirection.Input);
            param.Add("?EmAndamento", dbType: DbType.Boolean, value: obj.EmAndamento, direction: ParameterDirection.Input);
            param.Add("?RetornoAgendamentoId", dbType: DbType.String, value: obj.RetornoAgendamentoId, direction: ParameterDirection.Input);
            param.Add("?VinculoRetorno", dbType: DbType.Boolean, value: obj.VinculoRetorno, direction: ParameterDirection.Input);
            param.Add("?PresencaConfirmada", dbType: DbType.Boolean, value: obj.PresencaConfirmada, direction: ParameterDirection.Input);
            param.Add("?GraficoEcg", dbType: DbType.String, value: obj.GraficoEcg, direction: ParameterDirection.Input);



            const string updateQuery = @"UPDATE Agendamento SET
                                IndividuoId = ?IndividuoId, 
                                ProfissionalId = ?ProfissionalId, 
                                EstabelecimentoId = ?EstabelecimentoId, 
                                ProcedimentoId = ?ProcedimentoId, 
                                Dia = ?Dia, 
                                Hora = ?Hora, 
                                TipoDaConsulta = ?TipoDaConsulta, 
                                Observacoes = ?Observacoes, 
                                Ativo = ?Ativo,
                                Cancelado = ?Cancelado,
                                BatimentoCardiaco = ?BatimentoCardiaco,
                                PressaoSanguinea = ?PressaoSanguinea,
                                OxigenacaoSanguinea = ?OxigenacaoSanguinea,
                                Altura = ?Altura,
                                Peso = ?Peso,
                                Temperatura = ?Temperatura,
                                EmAndamento = ?EmAndamento,
                                RetornoAgendamentoId = ?RetornoAgendamentoId,
                                DataAlteracao = ?DataAlteracao,
                                VinculoRetorno = ?VinculoRetorno,
                                PresencaConfirmada = ?PresencaConfirmada,
                                GraficoEcg = ?GraficoEcg,
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
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateSinaisVitais(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;


            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?PressaoSanguinea", dbType: DbType.String, value: obj.PressaoSanguinea, direction: ParameterDirection.Input);
            param.Add("?BatimentoCardiaco", dbType: DbType.String, value: obj.BatimentoCardiaco, direction: ParameterDirection.Input);
            param.Add("?OxigenacaoSanguinea", dbType: DbType.String, value: obj.OxigenacaoSanguinea, direction: ParameterDirection.Input);
            param.Add("?Altura", dbType: DbType.String, value: obj.Altura, direction: ParameterDirection.Input);
            param.Add("?Peso", dbType: DbType.String, value: obj.Peso, direction: ParameterDirection.Input);
            param.Add("?Temperatura", dbType: DbType.String, value: obj.Temperatura, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
            if (id != null && obj.NaoCompareceu == true && obj.Finalizado == true)
            {
                param.Add("?NaoCompareceu", dbType: DbType.Boolean, value: obj.NaoCompareceu, direction: ParameterDirection.Input);
                param.Add("?Finalizado", dbType: DbType.Boolean, value: obj.Finalizado, direction: ParameterDirection.Input);

                const string updateQueryAusencia = @"UPDATE Agendamento SET
                                NaoCompareceu = ?NaoCompareceu,
                                Finalizado = ?Finalizado
                                WHERE Id = ?Id";

                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQueryAusencia, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
           

            const string updateQuery = @"UPDATE Agendamento SET
                                    DataAlteracao = ?DataAlteracao,
                                    BatimentoCardiaco = ?BatimentoCardiaco,
                                    PressaoSanguinea = ?PressaoSanguinea,
                                    OxigenacaoSanguinea = ?OxigenacaoSanguinea,
                                    Altura = ?Altura,
                                    Peso = ?Peso,
                                    Temperatura = ?Temperatura
                                    WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        // UTILIZADO NO FINAL DO ATENDIMENTO DA TRIAGEM QUANDO O GRAU DE RISCO É MAIOR QUE 3 E É NECESSARIO FINALIZAR O ATENDIMENTO DIRETO PELA TRIAGEM PARA QUE
        // O MEDICO DA UNIDADE CONSIGA AVALIAR COM MAIS URGENCIA

        public int UpdateFinalizarTriagem(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;


            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?PresencaConfirmada", dbType: DbType.String, value: obj.PresencaConfirmada, direction: ParameterDirection.Input);
            param.Add("?CorStatusTriagem", dbType: DbType.String, value: obj.CorStatusTriagem, direction: ParameterDirection.Input);
            param.Add("?Finalizado", dbType: DbType.Boolean, value: obj.Finalizado, direction: ParameterDirection.Input);
            if (id != null)
            {
                const string updateQueryStatus = @"UPDATE Agendamento SET
                                PresencaConfirmada = ?PresencaConfirmada,
                                CorStatusTriagem = ?CorStatusTriagem,
                                Finalizado = ?Finalizado
                                WHERE Id = ?Id";

                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQueryStatus, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                return 0;
            }
        }




        // UTILIZADO NO FINAL DO ATENDIMENTO DA TRIAGEM PARA FICAR PRESENCA CONFIRMADA 1 E O ENVIO DOS STATUS DA TRIAGEM
        public int UpdateConfirmarPresenca(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;


            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?PresencaConfirmada", dbType: DbType.Boolean, value: obj.PresencaConfirmada, direction: ParameterDirection.Input);
            param.Add("?CorStatusTriagem", dbType: DbType.String, value: obj.CorStatusTriagem, direction: ParameterDirection.Input);

            if (id != null)
            {
                const string updateQueryStatus = @"UPDATE Agendamento SET
                                PresencaConfirmada = ?PresencaConfirmada,
                                CorStatusTriagem = ?CorStatusTriagem
                                WHERE Id = ?Id";

                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQueryStatus, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                return 0;
            }
        }
        // USADO NO FINAL DO ATENDIMENTO DO MEDICO PARA O AGENDAMENTO FICAR FINALIZADO 1 & EM ATENDIMENTO MEDICO 1
        public int UpdateFinalizarMedico(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;


            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?Finalizado", dbType: DbType.Boolean, value: obj.Finalizado, direction: ParameterDirection.Input);
            param.Add("?DataFim", dbType: DbType.DateTime, value: obj.DataFim, direction: ParameterDirection.Input);
            param.Add("?EmAtendimentoMedico", dbType: DbType.Boolean, value: obj.EmAtendimentoMedico, direction: ParameterDirection.Input);

            if (id != null)
            {
                const string updateQueryStatus = @"UPDATE Agendamento SET
                                Finalizado = ?Finalizado,
                                EmAtendimentoMedico = ?EmAtendimentoMedico,     
                                DataFim = ?DataFim
                                WHERE Id = ?Id";

                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQueryStatus, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento Finalizando Atendimento Médico", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                return 0;
            }
        }

        public int UpdateEmAtendimentoMedico(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?EmAtendimentoMedico", dbType: DbType.Boolean, value: obj.EmAtendimentoMedico, direction: ParameterDirection.Input);
            param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
            param.Add("?DataInicio", dbType: DbType.DateTime, value: obj.DataInicio, direction: ParameterDirection.Input);


            if (id != null)
            {
                string updateQueryStatus = @"UPDATE Agendamento SET
                            EmAtendimentoMedico = ?EmAtendimentoMedico,
                            ProfissionalId = ?ProfissionalId,
                            DataInicio = ?DataInicio
                            WHERE Id = ?Id";
                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQueryStatus, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento para em Atendimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                return 0;
            }
        }

        public int Delete(string id)
        {
            var query = @"UPDATE Agendamento SET Cancelado = 1 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", id.ToString(), "Desativou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public Agendamento GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND a.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var agendamento = new Agendamento();
            _connection.QueryAsync<Agendamento, Individuo, Profissional, Estabelecimento, Procedimento, Agendamento>(
                _queryAll.ToString(), (a, i, p, e, pro) =>
                {
                    agendamento = a;

                    agendamento.Individuo = i;
                    agendamento.Profissional = p;
                    agendamento.Estabelecimento = e;
                    agendamento.Procedimento = pro;

                    return agendamento;
                },
                param,
                splitOn: "Id, Id, Id, Id, Id",
                commandTimeout: TimeOut,
                commandType: CommandType.Text);
            return (agendamento);
        }

        public Agendamento GetByEstabelecimentoByIndividuoId(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND i.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            queryFilter.Append(" ORDER BY a.Dia ASC ");
            query.Append(queryFilter);
            #endregion

            var agendamento = new Agendamento();
            _connection.QueryAsync<Agendamento, Individuo, Profissional, Estabelecimento, Procedimento, Agendamento>(
                _queryAll.ToString(), (a, i, p, e, pro) =>
                {
                    agendamento = a;

                    agendamento.Individuo = i;
                    agendamento.Profissional = p;
                    agendamento.Estabelecimento = e;
                    agendamento.Procedimento = pro;

                    return agendamento;
                },
                param,
                splitOn: "Id, Id, Id, Id, Id",
                commandTimeout: TimeOut,
                commandType: CommandType.Text);
            return (agendamento);
        }

        public async Task<(IEnumerable<Agendamento>, int)> GetByParam(Agendamento filters, string sort, int? skip = null, int? take = null)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {
                if (filters.Retorno == true)
                {
                    if (!string.IsNullOrEmpty(filters?.IndividuoId))
                    {
                        queryFilter.Append(" AND a.IndividuoId = ?IndividuoId");
                        queryFilter.Append(" AND a.Retorno = 0");
                        queryFilter.Append(" AND a.VinculoRetorno = 0");
                        queryFilter.Append(" AND a.Finalizado = 1");
                        queryFilter.Append(" AND a.PresencaConfirmada = 1");
                        queryFilter.Append(" AND TIMESTAMPDIFF(DAY, a.Dia, NOW()) < 30");
                        queryFilter.Append(" AND a.RetornoAgendamentoId IS NULL");
                        param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(filters?.Id))
                    {
                        queryFilter.Append(" AND a.Id = ?Id");
                        param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(filters?.IndividuoId))
                    {
                        queryFilter.Append(" And a.IndividuoId = ?IndividuoId");
                        param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                    {
                        queryFilter.Append(" And a.ProfissionalId = ?ProfissionalId");
                        param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(filters?.EstabelecimentoId))
                    {
                        queryFilter.Append(" And a.EstabelecimentoId = ?EstabelecimentoId");
                        param.Add("?EstabelecimentoId", dbType: DbType.String, value: filters.EstabelecimentoId, direction: ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(filters?.ProcedimentoId))
                    {
                        queryFilter.Append(" And a.ProcedimentoId = ?ProcedimentoId");
                        param.Add("?ProcedimentoId", dbType: DbType.String, value: filters.ProcedimentoId, direction: ParameterDirection.Input);
                    }
                    if (filters.Dia != null)
                    {
                        queryFilter.Append(" And a.Dia = ?Dia");
                        param.Add("?Dia", dbType: DbType.Date, value: filters.Dia, direction: ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(filters?.TipoDaConsulta))
                    {
                        queryFilter.Append(" And a.TipoDaConsulta = ?TipoDaConsulta");
                        param.Add("?TipoDaConsulta", dbType: DbType.String, value: filters.TipoDaConsulta, direction: ParameterDirection.Input);
                    }
                    if (filters.EmAndamento.HasValue)
                    {
                        queryFilter.Append(" AND a.EmAndamento = ?EmAndamento ");
                        param.Add("?EmAndamento", dbType: DbType.Boolean, value: filters.EmAndamento, direction: ParameterDirection.Input);
                    }
                    if (filters.Interconsulta.HasValue)
                    {
                        queryFilter.Append(" AND a.Interconsulta = ?Interconsulta ");
                        param.Add("?Interconsulta", dbType: DbType.Boolean, value: filters.Interconsulta, direction: ParameterDirection.Input);
                    }
                    if (filters.Finalizado.HasValue)
                    {
                        queryFilter.Append(" AND a.Finalizado = ?Finalizado ");
                        param.Add("?Finalizado", dbType: DbType.Boolean, value: filters.Finalizado, direction: ParameterDirection.Input);
                    }
                    if (filters.Cancelado.HasValue)
                    {
                        queryFilter.Append(" AND a.Cancelado = ?Cancelado ");
                        param.Add("?Cancelado", dbType: DbType.Boolean, value: filters.Cancelado, direction: ParameterDirection.Input);
                    }
                    if (filters.PresencaConfirmada.HasValue)
                    {
                        queryFilter.Append(" AND a.PresencaConfirmada = ?PresencaConfirmada ");
                        param.Add("?PresencaConfirmada", dbType: DbType.Boolean, value: filters.PresencaConfirmada, direction: ParameterDirection.Input);
                    }
                    if (filters.NaoCompareceu.HasValue)
                    {
                        queryFilter.Append(" AND a.NaoCompareceu = ?NaoCompareceu ");
                        param.Add("?NaoCompareceu", dbType: DbType.Boolean, value: filters.NaoCompareceu, direction: ParameterDirection.Input);
                    }
                    if (filters.Interconsulta.HasValue)
                    {
                        queryFilter.Append(" AND a.Interconsulta = ?Interconsulta ");
                        param.Add("?Interconsulta", dbType: DbType.Boolean, value: filters.Interconsulta, direction: ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(filters?.GraficoEcg))
                    {
                        queryFilter.Append(" AND a.GraficoEcg IS NOT NULL");
                    }

                    if (filters.SinaisVitaisGrafico == true)
                    {
                        // os graficos não podem conter agendamentos com sinais vitais em branco OBRIGATORIO TODOS PREENCHIDOS
                        // ou seja todos os filtros aqui terão que ser IS NOT NULL
                        //queryFilter.Append(" AND a.PressaoSanguinea IS NOT NULL ");
                        //queryFilter.Append(" AND a.OxigenacaoSanguinea IS NOT NULL ");
                        //queryFilter.Append(" AND a.BatimentoCardiaco IS NOT NULL ");
                        //queryFilter.Append(" AND a.Altura IS NOT NULL ");
                        //queryFilter.Append(" AND a.Peso IS NOT NULL ");
                        //queryFilter.Append(" AND a.Temperatura IS NOT NULL ");
                    }

                    if (filters.DataInicial != null)
                    {
                        filters.DataFinal = filters.DataFinal ?? DateTime.Now;

                        queryFilter.Append(" AND DATE(a.Dia) BETWEEN DATE(?DataInicial) AND DATE(?DataFinal)");
                        param.Add("?DataInicial", dbType: DbType.Date, value: filters.DataInicial, direction: ParameterDirection.Input);
                        param.Add("?DataFinal", dbType: DbType.Date, value: filters.DataFinal, direction: ParameterDirection.Input);
                    }

                    if (filters.VinculoRetorno.HasValue)
                    {
                        queryFilter.Append(" AND a.VinculoRetorno = ?VinculoRetorno ");
                        param.Add("?VinculoRetorno", dbType: DbType.Boolean, value: filters.VinculoRetorno, direction: ParameterDirection.Input);
                    }
                }

            }
            _queryAll.Append(queryFilter);
            _queryCountAll.Append(queryFilter);


            if(!string.IsNullOrEmpty(sort)) _queryAll.Append($" ORDER BY {sort} ");

            if (skip.HasValue && take.HasValue)
            {
                skip = (skip - 1) * take;
                _queryAll.Append($"LIMIT ?skip, ?take");
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                var agendamentos = new Dictionary<string, Agendamento>();
                await _connection.QueryAsync<Agendamento, Individuo, Profissional, Estabelecimento, Procedimento, Agendamento>(
                    _queryAll.ToString(), (a, i, p, e, pro) => {
                        if (!agendamentos.TryGetValue(a.Id, out var agendamentoEntity))
                        {
                            agendamentos.Add(a.Id, agendamentoEntity = a);
                        }
                        agendamentoEntity.Individuo = i;
                        agendamentoEntity.Profissional = p;
                        agendamentoEntity.Estabelecimento = e;
                        agendamentoEntity.Procedimento = pro;

                        return agendamentoEntity;
                    },
                    param,
                    splitOn: "Id, Id, Id, Id, Id",
                    commandTimeout: TimeOut, 
                    commandType: CommandType.Text);
                return (agendamentos.Values, totalCount);
            }
            return (null, 0);
        }


        public async Task<(IEnumerable<Agendamento>, int)> GetAusentes(Agendamento filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {


                
                queryFilter.Append(" AND (a.Cancelado = TRUE OR a.NaoCompareceu = TRUE) AND a.Finalizado = TRUE");


                if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                {
                    queryFilter.Append(" And a.ProfissionalId = ?ProfissionalId");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.TipoDaConsulta))
                {
                    queryFilter.Append(" And a.TipoDaConsulta = ?TipoDaConsulta");
                    param.Add("?TipoDaConsulta", dbType: DbType.String, value: filters.TipoDaConsulta, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.IndividuoId))
                {
                    queryFilter.Append(" And a.IndividuoId = ?IndividuoId");
                    param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
                }

                if (filters.DataInicial != null && filters.DataFinal != null)
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
                var agendamentos = new Dictionary<string, Agendamento>();
                await _connection.QueryAsync<Agendamento, Individuo, Profissional, Estabelecimento, Procedimento, Agendamento>(
                    _queryAll.ToString(), (a, i, p, e, pro) => {
                        if (!agendamentos.TryGetValue(a.Id, out var agendamentoEntity))
                        {
                            agendamentos.Add(a.Id, agendamentoEntity = a);
                        }
                        agendamentoEntity.Individuo = i;
                        agendamentoEntity.Profissional = p;
                        agendamentoEntity.Estabelecimento = e;
                        agendamentoEntity.Procedimento = pro;

                        return agendamentoEntity;
                    },
                    param,
                    splitOn: "Id, Id, Id, Id, Id",
                    commandTimeout: TimeOut,
                    commandType: CommandType.Text);
                return (agendamentos.Values, totalCount);
            }
            return (null, 0);
        }



        public int UpdateRecepcao(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;


            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?PressaoSanguinea", dbType: DbType.String, value: obj.PressaoSanguinea, direction: ParameterDirection.Input);
            param.Add("?BatimentoCardiaco", dbType: DbType.String, value: obj.BatimentoCardiaco, direction: ParameterDirection.Input);
            param.Add("?OxigenacaoSanguinea", dbType: DbType.String, value: obj.OxigenacaoSanguinea, direction: ParameterDirection.Input);
            param.Add("?Altura", dbType: DbType.String, value: obj.Altura, direction: ParameterDirection.Input);
            param.Add("?Peso", dbType: DbType.String, value: obj.Peso, direction: ParameterDirection.Input);
            param.Add("?Temperatura", dbType: DbType.String, value: obj.Temperatura, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
            if (id != null && obj.NaoCompareceu == true && obj.Finalizado == true)
            {
                param.Add("?NaoCompareceu", dbType: DbType.Boolean, value: obj.NaoCompareceu, direction: ParameterDirection.Input);
                param.Add("?Finalizado", dbType: DbType.Boolean, value: obj.Finalizado, direction: ParameterDirection.Input);

                const string updateQueryAusencia = @"UPDATE Agendamento SET
                                NaoCompareceu = ?NaoCompareceu,
                                Finalizado = ?Finalizado
                                WHERE Id = ?Id";

                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQueryAusencia, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            if (id != null && obj.PresencaConfirmada == true)
            {
                param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
                param.Add("?PresencaConfirmada", dbType: DbType.Boolean, value: obj.PresencaConfirmada, direction: ParameterDirection.Input);

                const string updateQueryPresenca = @"UPDATE Agendamento SET
                                DataAlteracao = ?DataAlteracao,
                                PresencaConfirmada = ?PresencaConfirmada
                                WHERE Id = ?Id";
                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQueryPresenca, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {

                const string updateQuery = @"UPDATE Agendamento SET
                                DataAlteracao = ?DataAlteracao,
                                BatimentoCardiaco = ?BatimentoCardiaco,
                                PressaoSanguinea = ?PressaoSanguinea,
                                OxigenacaoSanguinea = ?OxigenacaoSanguinea,
                                Altura = ?Altura,
                                Peso = ?Peso,
                                Temperatura = ?Temperatura
                                WHERE Id = ?Id";

                try
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        var user = _user.GetUserId();
                        var origem = _user.GetUserOrigem();
                        var ip = _user.GetUserIp();

                        var ret = _connection.Execute(updateQuery, param, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                        transaction.Commit();
                        return ret;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public int UpdateTriagem(string id, Agendamento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;


            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            //param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
            if (obj.NaoCompareceu != null) {
                param.Add("?NaoCompareceu", dbType: DbType.Boolean, value: obj.NaoCompareceu, direction: ParameterDirection.Input); 
            }
            if (obj.Finalizado != null) {
                param.Add("?Finalizado", dbType: DbType.Boolean, value: obj.Finalizado, direction: ParameterDirection.Input);
            }
            if (obj.Cancelado != null) {
                param.Add("?Cancelado", dbType: DbType.Boolean, value: obj.Cancelado, direction: ParameterDirection.Input);
            }

            const string updateQueryAusencia = @"UPDATE Agendamento SET
                            NaoCompareceu = ?NaoCompareceu,
                            Finalizado = ?Finalizado,
                            Cancelado = ?Cancelado
                            WHERE Id = ?Id";
            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQueryAusencia, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Agendamento", obj.Id.ToString(), "Editou Agendamento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
           
        }

        public async Task<(IEnumerable<Agendamento>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            var query = new StringBuilder($@"
            SELECT 
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
            ag.TipoDeAtendimento,
            ag.Condutas,
            ag.GraficoEcg,
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
            p.Crm
            FROM Agendamento ag
            INNER JOIN Estabelecimento e ON e.Id = ag.EstabelecimentoId
            INNER JOIN Individuo i ON i.Id = ag.IndividuoId
            INNER JOIN Profissional p ON p.Id = ag.ProfissionalId
            INNER JOIN Atendimento a ON a.AgendamentoId = ag.Id -- Obrigatório ter pelo menos um atendimento para conseguir gerar uma ficha completa.
            WHERE 1 = 1
            AND EXTRACT(MONTH FROM ag.Dia) = ?mes
            AND EXTRACT(YEAR FROM ag.Dia) = ?ano ");

            var queryCount = new StringBuilder($@"
            SELECT COUNT(1)
            FROM Agendamento ag
            INNER JOIN Estabelecimento e ON e.Id = ag.EstabelecimentoId
            INNER JOIN Individuo i ON i.Id = ag.IndividuoId
            INNER JOIN Profissional p ON p.Id = ag.ProfissionalId
            WHERE 1 = 1
            AND EXTRACT(MONTH FROM ag.Dia) = ?mes
            AND EXTRACT(YEAR FROM ag.Dia) = ?ano ");

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
                var list = new Dictionary<string, Agendamento>();

                if (totalCount > 0)
                {
                    await _connection.QueryAsync<Agendamento>(query.ToString(),
                        new[]
                        {
                    typeof(Agendamento), typeof(Estabelecimento), typeof(Individuo), typeof(Profissional)
                        },
                        objects =>
                        {
                            var agendamento = objects[0] as Agendamento;
                            var estabelecimento = objects[1] as Estabelecimento;
                            var individuo = objects[2] as Individuo;
                            var profissional = objects[3] as Profissional;

                            if (!list.TryGetValue(agendamento.Id, out var entity))
                            {
                                entity = agendamento;
                                list.Add(agendamento.Id, entity);
                            }

                            entity.Estabelecimento = estabelecimento;
                            entity.Profissional = profissional;
                            entity.Individuo = individuo;

                            return entity;
                        }, param, splitOn: "Id, Id, Id, Id", commandTimeout: 0, commandType: CommandType.Text);

                    foreach (var agendamento in list.Values)
                    {
                        param.Add("?AgendamentoId", dbType: DbType.String, value: agendamento.Id, direction: ParameterDirection.Input);

                        #region Atendimentos do Agendamento
                        var queryAtendimentos = @"
                               SELECT 
                                CAST(at.Id AS CHAR) AS Id, 
                                at.AtendidoMedico,
                                at.Subjetivo,
                                at.QueixaDoPaciente,
                                at.DataCadastro,
                                CAST(at.TempoAtendimento AS CHAR) AS TempoAtendimento,
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
                                at.Alergias,
                                at.Antecedentes
                                FROM Atendimento at
                                WHERE 
                                at.AgendamentoId = ?AgendamentoId ";

                        var atendimentoList = _connection.Query<Atendimento>(
                        queryAtendimentos.ToString(), param, commandType: CommandType.Text).ToList();

                        if (atendimentoList != null)
                            agendamento.Atendimentos = atendimentoList;
                        #endregion

                        #region CIAPS do Agendamento
                        var queryIndividuoCiap = @"
                        SELECT 
                        CAST(IndividuoCiap.Id AS CHAR) AS Id,
                        CAST(IndividuoCiap.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoCiap.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoCiap.CiapId AS CHAR) AS CiapId,
                        IndividuoCiap.DataCadastro,
                        IndividuoCiap.DataAlteracao,
                        Individuo.NomeCompleto,
                        IndividuoCiap.AtendidoTriagem,
                        IndividuoCiap.AtendidoMedico,
                        CAST(Ciap.Id AS CHAR) AS Id,
                        Ciap.Descricao,
                        Ciap.Codigo
                        FROM IndividuoCiap
                        INNER JOIN Individuo ON (IndividuoCiap.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoCiap.ProfissionalId = Profissional.Id)
                        INNER JOIN Ciap ON (IndividuoCiap.CiapId = Ciap.Id)
                        WHERE IndividuoCiap.AgendamentoId = ?AgendamentoId ";

                        var individuoCiapList = _connection.Query<IndividuoCiap, Ciap, IndividuoCiap>(
                        queryIndividuoCiap.ToString(), (i, c) => { i.Ciap = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoCiapList != null)
                            agendamento.IndividuoCiap = individuoCiapList;
                        #endregion

                        #region CID10 do Agendamento
                        var queryIndividuoCid10 = @"
                        SELECT
                        CAST(IndividuoCid10.Id AS CHAR) AS Id,
                        CAST(IndividuoCid10.IndividuoId AS CHAR) AS IndividuoId, 
                        CAST(IndividuoCid10.ProfissionalId AS CHAR) AS ProfissionalId,
                        CAST(IndividuoCid10.Cid10Id AS CHAR) AS Cid10Id,
                        IndividuoCid10.DataCadastro,
                        IndividuoCid10.DataAlteracao,
                        Individuo.NomeCompleto,
                        IndividuoCid10.AtendidoTriagem,
                        IndividuoCid10.AtendidoMedico,
                        CAST(Cid10.Id AS CHAR) AS Id,
                        Cid10.Descricao,
                        Cid10.Codigo
                        FROM IndividuoCid10
                        INNER JOIN Individuo ON (IndividuoCid10.IndividuoId = Individuo.Id) 
                        INNER JOIN Profissional ON (IndividuoCid10.ProfissionalId = Profissional.Id)
                        INNER JOIN Cid10 ON (IndividuoCid10.Cid10Id = Cid10.Id)
                        WHERE IndividuoCid10.AgendamentoId = ?AgendamentoId ";

                        var individuoCid10List = _connection.Query<IndividuoCid10, Cid, IndividuoCid10>(
                        queryIndividuoCid10.ToString(), (i, c) => { i.Cid = c; return i; },
                        param, splitOn: "Id", commandTimeout: TimeOut, commandType: CommandType.Text).ToList();

                        if (individuoCid10List != null)
                            agendamento.IndividuoCid10 = individuoCid10List;
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
                            agendamento.ExamesF200 = examesF200List;
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

        public void ConfirmarIntegracao(int lote, Agendamento obj)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();
                param.Add("?LoteIntegracaoId", dbType: DbType.Int32, value: lote, direction: ParameterDirection.Input);
                param.Add("?AgendamentoId", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
                param.Add("?DadoSerializado", dbType: DbType.String, value: obj.DadoSerializado, direction: ParameterDirection.Input);

                var query = @"UPDATE Agendamento SET
                        LoteIntegracaoId = ?LoteIntegracaoId,
                        DadoSerializado = ?DadoSerializado
                        WHERE Id = ?AgendamentoId";

                _connection.Execute(query, param, commandTimeout: 0);
                transaction.Commit();
            }
        }
    }
}
