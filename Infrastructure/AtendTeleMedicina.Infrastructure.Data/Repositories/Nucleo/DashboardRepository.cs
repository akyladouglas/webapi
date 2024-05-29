using Dapper;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Dashboard;
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

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class DashboardRepository : BaseRepository, IDashboardRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"");
        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public DashboardRepository(IMySqlContext context, IUser user)
                : base(context)
        {
            _user = user;
            _connection = context.Connection;
        }

        public async Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentos(DashboardParams dashParams)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" ");
            var param = new DynamicParameters();
            if (dashParams != null)
            {
                if (dashParams.DataInicial != null && dashParams.DataFinal != null)
                {
                    dashParams.DataFinal = dashParams.DataFinal ?? DateTime.Now;

                    queryFilter.Append(" WHERE DATE(a.DataCadastro) BETWEEN DATE(?DataInicial) AND DATE(?DataFinal)");
                    param.Add("?DataInicial", dbType: DbType.Date, value: dashParams.DataInicial, direction: ParameterDirection.Input);
                    param.Add("?DataFinal", dbType: DbType.Date, value: dashParams.DataFinal, direction: ParameterDirection.Input);
                }
            }
            var query = new StringBuilder($@"
                              SELECT DISTINCT COUNT(a.`Id`) AS 'Atendimentos', p.`Nome` AS 'ProfissionalNome', CAST(p.`Id` AS CHAR) AS 'Id', CAST(ag.`Id` AS CHAR) AS 'Id' FROM `Atendimento` a
                              INNER JOIN `Agendamento` ag ON a.`AgendamentoId` = ag.`Id`
                              INNER JOIN `Profissional` p ON p.`Id` = ag.`ProfissionalId`
                              {queryFilter}
                              GROUP BY p.`Id`"
            );

            var queryCount = new StringBuilder($@"
                              SELECT COUNT(a.`Id`) FROM `Atendimento` a
                              {queryFilter}
                              "
            );
            #endregion

            try
            {
                //return await _connection.QueryAsync<int>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);
                var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

                if (totalCount > 0)
                {
                    var dashboard = new Dictionary<string, DashboardParams>();
                    await _connection.QueryAsync<DashboardParams, Agendamento, Profissional, DashboardParams>(
                        query.ToString(), (a, ag, p) =>
                        {
                            if (!dashboard.TryGetValue(a.Atendimentos, out var dashboardEntity))
                            {
                                dashboard.Add(a.Atendimentos, dashboardEntity = a);
                            }
                            dashboardEntity.Agendamento = ag;
                            dashboardEntity.Profissional = p;


                            return dashboardEntity;
                        },
                        param,
                        splitOn: "Id, Id, Id",
                        commandTimeout: TimeOut,
                        commandType: CommandType.Text);
                    return (dashboard.Values, totalCount);
                }
                return (null, 0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentosPacientes(DashboardParams dashParams)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" ");
            //var param = new DynamicParameters();
            //if (dashParams != null)
            //{
            //    if (dashParams.DataInicial != null && dashParams.DataFinal != null)
            //    {
            //        dashParams.DataFinal = dashParams.DataFinal ?? DateTime.Now;

            //        queryFilter.Append(" AND DATE(a.DataCadastro) BETWEEN DATE(?DataInicial) AND DATE(?DataFinal)");
            //        param.Add("?DataInicial", dbType: DbType.Date, value: dashParams.DataInicial, direction: ParameterDirection.Input);
            //        param.Add("?DataFinal", dbType: DbType.Date, value: dashParams.DataFinal, direction: ParameterDirection.Input);
            //    }

            //}

            //tem que ser agrupado por individuo id para retornar a quantidade de atendimentos por individuo
            var query = new StringBuilder($@"
                              SELECT DISTINCT COUNT(a.Id) AS 'Atendimentos', CAST(a.Id AS CHAR) AS 'Id', i.NomeCompleto AS 'IndividuoNome', CAST(i.Id AS CHAR) AS 'Id', CAST(ag.Id AS CHAR) AS 'Id' FROM Atendimento a
                              LEFT JOIN Agendamento ag ON a.AgendamentoId = ag.Id
                              LEFT JOIN Individuo i ON i.Id = ag.IndividuoId
                              WHERE a.AtendidoMedico = TRUE {queryFilter}
                              GROUP BY ag.IndividuoId
                             "
            );

            //tem que ser agrupado por individuo id para retornar a quantidade de atendimentos por individuo
            var queryCount = new StringBuilder($@"
                              SELECT COUNT(a.Id) FROM Atendimento a
                              WHERE a.AtendidoMedico = TRUE {queryFilter}
                              "
            );
            #endregion

            try
            {
                var totalCount = _connection.Query<int>(queryCount.ToString(), commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

                if (totalCount > 0)
                {
                    var dashboard = new Dictionary<string, DashboardParams>();
                    await _connection.QueryAsync<DashboardParams, Agendamento, Individuo, DashboardParams>(
                        query.ToString(), (a, ag, i) =>
                        {
                            Console.WriteLine(a);
                            if (!dashboard.TryGetValue(a.Id, out var dashboardEntity))
                            {
                                dashboard.Add(a.Id, dashboardEntity = a);
                            }
                            dashboardEntity.Agendamento = ag;
                            dashboardEntity.Individuo = i;

                            return dashboardEntity;
                        },

                        splitOn: "Id, Id, Id",
                        commandTimeout: TimeOut,
                        commandType: CommandType.Text);
                    return (dashboard.Values, totalCount);
                }
                return (null, 0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
    }
}
