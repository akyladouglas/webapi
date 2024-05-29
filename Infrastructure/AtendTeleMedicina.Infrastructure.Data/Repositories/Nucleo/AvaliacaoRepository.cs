using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using AtendTeleMedicina.Infra.Data.Context;
using Dapper;
using System.Data;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class AvaliacaoRepository : BaseRepository, IAvaliacaoRepository
    {
       
        private readonly StringBuilder _queryAll = new StringBuilder(
                                                            @"SELECT
                                                                CONCAT(a.IndividuoId, '') As IndividuoId,
                                                                CONCAT(a.AtendimentoId, '') As AtendimentoId,
                                                                a.DataAvaliacao, 
                                                                a.Questao1,
                                                                a.Questao2
                                                             FROM Avaliacao a
                                                           ");


        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"SELECT COUNT(a.Id) FROM Avaliacao a ");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public AvaliacaoRepository(IMySqlContext context, IUser user) : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public int Add(Avaliacao obj)
        {
            var param = new DynamicParameters();
            param.Add("?IndividuoId", obj.IndividuoId, DbType.String, ParameterDirection.Input);
            param.Add("?ProfissionalId", obj.ProfissionalId, DbType.String, ParameterDirection.Input);
            param.Add("?AtendimentoId", obj.AtendimentoId, DbType.String, ParameterDirection.Input);
            param.Add("?DataAvaliacao", obj.DataAvaliacao, DbType.DateTime, ParameterDirection.Input);
            param.Add("?Questao1", obj.Questao1, DbType.Int32, ParameterDirection.Input);
            param.Add("?Questao2", obj.Questao2, DbType.Int32, ParameterDirection.Input);


            const string insertQuery = @"INSERT INTO Avaliacao (IndividuoId, ProfissionalId, AtendimentoId, DataAvaliacao, Questao1, Questao2)
                                                     VALUES    (?IndividuoId, ?ProfissionalId, ?AtendimentoId, ?DataAvaliacao, ?Questao1, ?Questao2);";

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Avaliacao", obj.Id.ToString(), "Inseriu Avaliacao", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }
    }
}
