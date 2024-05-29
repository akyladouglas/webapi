using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class SatisfacaoRepository : BaseRepository, ISatisfacaoRepository
    {

        private readonly MySqlConnection _connection;

        public SatisfacaoRepository(IMySqlContext context)
            : base(context)
        {
            _connection = context.Connection;
        }

        public int Add(Satisfacao satisfacao)
        {
            {
                const string insertQuery = @"
                    INSERT INTO Satisfacao (Id, Avaliacao, AplicacaoId) 
                    VALUES (@Id, @Avaliacao, @AplicacaoId)";

                var param = new DynamicParameters();
                param.Add("?Id", satisfacao.Id, DbType.String, ParameterDirection.Input);
                param.Add("?Avaliacao", satisfacao.Avaliacao, DbType.Int32, ParameterDirection.Input);
                param.Add("?AplicacaoId", satisfacao.AplicacaoId, DbType.String, ParameterDirection.Input);

                //Connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(insertQuery, satisfacao, transaction);
                    // TODO Adicionar linha para registrar no LOG
                    transaction.Commit();
                    return ret;
                }
            }
        }

       
    }
}
