using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class IndividuoNotificacaoRepository : BaseRepository, IIndividuoNotificacaoRepository
    {
        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public IndividuoNotificacaoRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }
        public int Add(IndividuoNotificacao obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);
            param.Add("?NotificacaoId", dbType: DbType.String, value: obj.NotificacaoId, direction: ParameterDirection.Input);

            const string insertQuery = @"INSERT INTO IndividuoNotificacao (Id, IndividuoId, DataCadastro, NotificacaoId)
                                                     VALUES (?Id, ?IndividuoId, ?DataCadastro, ?NotificacaoId)";
            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoNotificacao", obj.Id.ToString(), "Inseriu IndividuoNotificacao", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
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
