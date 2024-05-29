using Dapper;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Params;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class IndividuoProcedimentoAutorizacaoRepository : BaseRepository, IIndividuoProcedimentoAutorizacaoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(@" SELECT
                                                    CAST( IndividuoProcedimentoAutorizacao.Id AS CHAR ) AS Id,
                                                    CAST( IndividuoProcedimentoAutorizacao.IndividuoId AS CHAR ) AS IndividuoId,
                                                    CAST( IndividuoProcedimentoAutorizacao.ProcedimentoId AS CHAR ) AS ProcedimentoId,
                                                    IndividuoProcedimentoAutorizacao.DataCadastro,
                                                    IndividuoProcedimentoAutorizacao.Autorizado
                                                    FROM
                                                    IndividuoProcedimentoAutorizacao
                                                        INNER JOIN Individuo ON (IndividuoProcedimentoAutorizacao.IndividuoId = Individuo.Id)
                                                        INNER JOIN Procedimento ON (IndividuoProcedimentoAutorizacao.ProcedimentoId = Procedimento.Id) ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(@"SELECT COUNT(1) FROM IndividuoProcedimentoAutorizacao
                                                                      INNER JOIN Individuo ON (IndividuoProcedimentoAutorizacao.IndividuoId = Individuo.Id) 
                                                                      INNER JOIN Procedimento ON (IndividuoProcedimentoAutorizacao.ProcedimentoId = Procedimento.Id) ");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public IndividuoProcedimentoAutorizacaoRepository(IMySqlContext context, IUser user)
          : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }


        public void Add(IEnumerable<IndividuoProcedimentoAutorizacao> list)
        {
            const string insertQuery = @"INSERT INTO IndividuoProcedimentoAutorizacao
                            (Id, IndividuoId, ProcedimentoId, DataCadastro, Autorizado) VALUES
                            (?Id, ?IndividuoId, ?ProcedimentoId, ?DataCadastro, ?Autorizado)";
            var param = new DynamicParameters();

            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var item in list)
                {
                    param.Add("?Id", item.Id, DbType.String, ParameterDirection.Input);
                    param.Add("?IndividuoId", item.IndividuoId, DbType.String, ParameterDirection.Input);
                    param.Add("?ProcedimentoId", item.ProcedimentoId, DbType.String, ParameterDirection.Input);
                    param.Add("?DataCadastro", item.DataCadastro, DbType.DateTime, ParameterDirection.Input);
                    param.Add("?Autorizado", item.Autorizado, DbType.Boolean, ParameterDirection.Input);

                    _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoProcedimentoAutorizacao", item.Id,
                      $"Inseriu o Individuo {item.IndividuoId} para o Procedimento: {item.ProcedimentoId}",
                      DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                }
                transaction.Commit();
            }
        }
        public void Delete(IEnumerable<IndividuoProcedimentoAutorizacao> list)
        {
            
            try {
                using (var transaction = _connection.BeginTransaction())
                {
                    foreach (var item in list)
                    {
                        var query = @"DELETE FROM IndividuoProcedimentoAutorizacao WHERE ProcedimentoId = ?ProcedimentoId AND IndividuoId = ?IndividuoId";
                        var param = new DynamicParameters();
                        param.Add("?ProcedimentoId", dbType: DbType.String, value: item.ProcedimentoId, direction: ParameterDirection.Input);
                        param.Add("?IndividuoId", dbType: DbType.String, value: item.IndividuoId, direction: ParameterDirection.Input);
                        var ret = _connection.Execute(query, param, transaction);
                    } 
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
