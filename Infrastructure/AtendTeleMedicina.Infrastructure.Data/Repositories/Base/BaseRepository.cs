using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using AtendTeleMedicina.Infra.Data.Context;

namespace AtendTeleMedicina.Infra.Data.Repositories.Base
{
    public class BaseRepository : IDisposable
    {
        private MySqlConnection Connection { get; set; }
        protected readonly int TimeOut = 120;

        protected BaseRepository(IMySqlContext context)
        {
            Connection = context.Connection;
            if (Connection.State == ConnectionState.Closed) Connection.Open();
        }

        public int RegistrarAcao(IDbConnection db, string id, string tabela, string tabelaId, string acao, DateTime dataHora, string origem, string atorId, string ip, string justificativa = "")
        {
            const string insertQuery = @"
                                 INSERT INTO Auditoria (Id, Tabela, TabelaId, Acao, DataHora, Origem, AtorId, Ip, Justificativa) 
                                 VALUES (?Id, ?Tabela, ?TabelaId, ?Acao, ?DataHora, ?Origem, ?AtorId, ?Ip, ?Justicativa)";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?Tabela", dbType: DbType.String, value: tabela, direction: ParameterDirection.Input);
            param.Add("?TabelaId", dbType: DbType.String, value: tabelaId, direction: ParameterDirection.Input);
            param.Add("?Acao", dbType: DbType.String, value: acao, direction: ParameterDirection.Input);
            param.Add("?Origem", dbType: DbType.String, value: origem, direction: ParameterDirection.Input);
            param.Add("?AtorId", dbType: DbType.String, value: atorId, direction: ParameterDirection.Input);
            param.Add("?DataHora", dbType: DbType.DateTime, value: dataHora.AddHours(0), direction: ParameterDirection.Input);
            param.Add("?Ip", dbType: DbType.String, value: ip, direction: ParameterDirection.Input);
            param.Add("?Justicativa", dbType: DbType.String, value: justificativa == "" ? null : justificativa, direction: ParameterDirection.Input);


            return db.Execute(insertQuery, param);
        }

        public void Dispose()
        {
            if (Connection == null) return;
            Connection.Close();
            Connection.Dispose();
            Connection = null;
            GC.SuppressFinalize(this);
        }

    }
}

