using Dapper;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class ConfiguracaoRepository : BaseRepository, IConfiguracaoRepository
    {
        private readonly StringBuilder _query = new StringBuilder(
                @"SELECT 
                Configuracao.Id, 
                Configuracao.Modulo,
                Configuracao.DemandaEspontanea,
                Configuracao.UrlAtend,
                Configuracao.VersaoApp,
                Configuracao.DataAlteracao, 
                Configuracao.LoginComSenha,
                Configuracao.IntegraPec,
                Configuracao.TipoConsulta,
                CAST(Configuracao.UsuarioAlterouId AS CHAR) AS UsuarioAlterouId,
                CAST(Usuario.Id AS CHAR) AS Id,
                Usuario.Nome 
                FROM Configuracao
                LEFT JOIN Usuario ON Configuracao.UsuarioAlterouId = Usuario.Id
                ");

        private readonly MySqlConnection _connection;

        public ConfiguracaoRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public Configuracao Get(Configuracao configFilters)
        {
            var config = _connection.Query<Configuracao, User, Configuracao>(
                _query.ToString(), (c, u) =>
                {
                    c.UsuarioAlterou = u;
                    return c;
                }, 
                splitOn: "Id",
                commandTimeout: 60,
                commandType: CommandType.Text).SingleOrDefault();

            return config;
        }

        public int Update(Configuracao configuracao, string userId)
        {
            configuracao.DataAlteracao = DateTime.Now;
            var param = new DynamicParameters();
            param.Add("@Id", configuracao.Id, DbType.String, ParameterDirection.Input);
            param.Add("@DataAlteracao", configuracao.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
            param.Add("@DemandaEspontanea", configuracao.DemandaEspontanea, DbType.Boolean, ParameterDirection.Input);
            param.Add("@LoginComSenha", configuracao.LoginComSenha, DbType.Boolean, ParameterDirection.Input);
            param.Add("@IntegraPec", configuracao.IntegraPec, DbType.Boolean, ParameterDirection.Input);
            param.Add("@VersaoApp", configuracao.VersaoApp, DbType.String, ParameterDirection.Input);
            param.Add("@Modulo", configuracao.Modulo, DbType.String, ParameterDirection.Input);
            param.Add("@UsuarioAlterouId", configuracao.UsuarioAlterouId, DbType.String, ParameterDirection.Input);
            param.Add("@UrlAtend", configuracao.UrlAtend, DbType.String, ParameterDirection.Input);
            param.Add("@TipoConsulta", configuracao.TipoConsulta, DbType.String, ParameterDirection.Input);

            const string updateQuery = @"
                    UPDATE Configuracao SET
                    DemandaEspontanea = @DemandaEspontanea,
                    LoginComSenha = @LoginComSenha,
                    IntegraPec = @IntegraPec,
                    VersaoApp = @VersaoApp,
                    Modulo = @Modulo,
                    DataAlteracao = @DataAlteracao,
                    UsuarioAlterouId = @UsuarioAlterouId,
                    UrlAtend = @UrlAtend,
                    TipoConsulta = @TipoConsulta
                    WHERE Id = @Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(updateQuery, param, transaction);
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
}
