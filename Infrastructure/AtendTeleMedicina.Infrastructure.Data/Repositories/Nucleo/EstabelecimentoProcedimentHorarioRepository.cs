using Dapper;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Params;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using System;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class EstabelecimentoProcedimentoHorarioRepository : BaseRepository, IEstabelecimentoProcedimentoHorarioRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(@"SELECT
                                                                CAST(EstabelecimentoProcedimentoHorario.Id AS CHAR) AS Id,
                                                                EstabelecimentoProcedimentoHorario.Dia,
                                                                EstabelecimentoProcedimentoHorario.Hora,
                                                                EstabelecimentoProcedimentoHorario.DataHora,
                                                                EstabelecimentoProcedimentoHorario.Situacao,
                                                                CAST(EstabelecimentoProcedimentoHorario.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                                                                CAST(EstabelecimentoProcedimentoHorario.ProcedimentoId AS CHAR) AS ProcedimentoId,
                                                                CAST(Estabelecimento.Id AS CHAR) AS Id,
                                                                Estabelecimento.NomeFantasia,
                                                                Estabelecimento.Cnes,
                                                                CAST(Procedimento.Id AS CHAR) AS Id,
                                                                Procedimento.Tipo,
                                                                Procedimento.Codigo,
                                                                Procedimento.Descricao
                                                                FROM EstabelecimentoProcedimentoHorario
                                                                LEFT JOIN Estabelecimento ON EstabelecimentoProcedimentoHorario.EstabelecimentoId = Estabelecimento.Id
                                                                LEFT JOIN Procedimento ON EstabelecimentoProcedimentoHorario.ProcedimentoId = Procedimento.Id
");

        private readonly StringBuilder _queryCountAll = new StringBuilder(@"SELECT COUNT(1) FROM EstabelecimentoProcedimentoHorario
                                                                        LEFT JOIN Estabelecimento ON EstabelecimentoProcedimentoHorario.EstabelecimentoId = Estabelecimento.Id
                                                                        LEFT JOIN Procedimento ON EstabelecimentoProcedimentoHorario.ProcedimentoId = Procedimento.Id
");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public EstabelecimentoProcedimentoHorarioRepository(IMySqlContext context, IUser user)
          : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }


        public async Task<(IEnumerable<EstabelecimentoProcedimentoHorario>, int)> GetByParam(EstabelecimentoProcedimentoHorario filters, AppParams appParams, string sort, int? skip, int? take)
        {

            var query = new StringBuilder(@"
                  SELECT
                  CAST(EstabelecimentoProcedimentoHorario.Id AS CHAR) AS Id,
                  EstabelecimentoProcedimentoHorario.Dia,
                  EstabelecimentoProcedimentoHorario.Hora,
                  EstabelecimentoProcedimentoHorario.DataHora,
                  EstabelecimentoProcedimentoHorario.TipoDaConsulta,
                  EstabelecimentoProcedimentoHorario.Situacao,
                  CAST(EstabelecimentoProcedimentoHorario.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                  CAST(EstabelecimentoProcedimentoHorario.ProcedimentoId AS CHAR) AS ProcedimentoId,
                  CAST(EstabelecimentoProcedimentoHorario.ProfissionalId AS CHAR) AS ProfissionalId,
                  CAST(Estabelecimento.Id AS CHAR) AS Id,
                  Estabelecimento.NomeFantasia,
                  Estabelecimento.Cnes,
                  CAST(Procedimento.Id AS CHAR) AS Id,
                  Procedimento.Tipo,
                  Procedimento.Codigo,
                  Procedimento.Descricao,
                  CAST(Profissional.Id AS CHAR) AS Id,
                  Profissional.Nome,
                  Profissional.Cpf,
                  Profissional.Cns,
                  Profissional.Sexo,
                  Profissional.Crm
                  FROM 
                    (SELECT
                    eph.Id,
                    eph.Dia,
                    eph.Hora,
                    eph.DataHora,
                    eph.Situacao,
                    eph.EstabelecimentoId,
                    eph.ProcedimentoId,
                    eph.ProfissionalId,
                    eph.TipoDaConsulta
                    FROM EstabelecimentoProcedimentoHorario eph ");

            var queryCount = new StringBuilder(@"
                    SELECT
                      COUNT(1)
                      FROM 
                      (SELECT
                      eph.Id,
                      eph.Dia,
                      eph.Hora,
                      eph.DataHora,
                      eph.Situacao,
                      eph.EstabelecimentoId,
                      eph.ProcedimentoId,
                      eph.ProfissionalId
                      FROM EstabelecimentoProcedimentoHorario eph ");

            #region -- -- Filtros / Ordenação

            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters?.ProcedimentoId))
                {
                    queryFilter.Append(" AND eph.ProcedimentoId = ?ProcedimentoId ");
                    param.Add("?ProcedimentoId", dbType: DbType.String, value: filters.ProcedimentoId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.EstabelecimentoId))
                {
                    queryFilter.Append(" AND eph.EstabelecimentoId = ?EstabelecimentoId ");
                    param.Add("?EstabelecimentoId", dbType: DbType.String, value: filters.EstabelecimentoId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                {
                    queryFilter.Append(" AND eph.ProfissionalId = ?ProfissionalId ");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
                }
                if (!appParams.DataInicial.HasValue && !appParams.DataFinal.HasValue)
                {
                    queryFilter.Append(" AND eph.DataHora >= NOW() ");
                }
                if (appParams.DataInicial.HasValue && appParams.DataFinal.HasValue)
                {
                    queryFilter.Append(" AND DATE(eph.DataHora) BETWEEN DATE(?dataInicial) AND  DATE(?dataFinal)");
                    param.Add("?dataInicial", dbType: DbType.DateTime, value: appParams.DataInicial, direction: ParameterDirection.Input);
                    param.Add("?dataFinal", dbType: DbType.DateTime, value: appParams.DataFinal, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.TipoDaConsulta))
                {
                    queryFilter.Append(" And eph.TipoDaConsulta = ?TipoDaConsulta");
                    param.Add("?TipoDaConsulta", dbType: DbType.String, value: filters.TipoDaConsulta, direction: ParameterDirection.Input);
                }
                if (!filters.Situacao.HasValue)
                {
                    filters.Situacao = 0;
                    queryFilter.Append(" AND eph.Situacao = ?Situacao ");
                    param.Add("?Situacao", dbType: DbType.Int32, value: filters.Situacao, direction: ParameterDirection.Input);
                }
            }

            query.Append(queryFilter);
            queryCount.Append(queryFilter);

            query.Append($" ORDER BY {sort} ");
            if (skip.HasValue && take.HasValue)
            {
                query.Append($" LIMIT ?skip, ?take ");
                if (skip <= 0) skip = 1;
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }

            #endregion

            query.Append(@" ) AS EstabelecimentoProcedimentoHorario
            LEFT JOIN Estabelecimento ON EstabelecimentoProcedimentoHorario.EstabelecimentoId = Estabelecimento.Id
            LEFT JOIN Procedimento ON EstabelecimentoProcedimentoHorario.ProcedimentoId = Procedimento.Id
            LEFT JOIN Profissional ON EstabelecimentoProcedimentoHorario.ProfissionalId = Profissional.Id
            ");

            queryCount.Append(@" ) AS EstabelecimentoProcedimentoHorario ");

            query.Append($" ORDER BY {sort.Replace("eph.", "EstabelecimentoProcedimentoHorario.")} ");

            var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            var individuos = new Dictionary<string, EstabelecimentoProcedimentoHorario>();

            if (totalCount > 0)
            {
                await _connection.QueryAsync<EstabelecimentoProcedimentoHorario>(query.ToString(),
                  new[]
                    { typeof(EstabelecimentoProcedimentoHorario), typeof(Estabelecimento), typeof(Procedimento), typeof(Profissional) },
                  objects =>
                  {
                      var eph = objects[0] as EstabelecimentoProcedimentoHorario;
                      var e = objects[1] as Estabelecimento;
                      var pc = objects[2] as Procedimento;
                      var pf = objects[3] as Profissional;

                      if (!individuos.TryGetValue(eph.Id, out var ephEntity))
                      {
                          ephEntity = eph;

                          individuos.Add(eph.Id, ephEntity);
                      }

                      if (ephEntity.Estabelecimento == null)
                          if (e != null)
                              ephEntity.Estabelecimento = e;
                      if (ephEntity.Procedimento == null)
                          if (pc != null)
                              ephEntity.Procedimento = pc;
                      if (ephEntity.Profissional == null)
                          if (pf != null)
                              ephEntity.Profissional = pf;

                      return ephEntity;
                  }, param, splitOn: "Id, Id, Id", commandType: CommandType.Text);
                return (individuos.Values, totalCount);
            }

            return (individuos.Values, 0);
        }

        public void Add(IEnumerable<EstabelecimentoProcedimentoHorario> list)
        {
            const string query = @"INSERT INTO EstabelecimentoProcedimentoHorario (Id, EstabelecimentoId, ProcedimentoId, ProfissionalId, DataHora, Dia, Hora, TipoDaConsulta, Situacao)
                             VALUES(@Id, @EstabelecimentoId, @ProcedimentoId, @ProfissionalId, @DataHora,  @Dia, @Hora, @TipoDaConsulta, @Situacao)";

            var estProcHorList = new List<EstabelecimentoProcedimentoHorario>();

            foreach (var item in list)
            {
                var estProcHor = item;
  
                if (GetByDateTime(estProcHor) == null)
                {
                    estProcHor.Id = Guid.NewGuid().ToString();
                    estProcHor.Situacao = 0;

                    estProcHorList.Add(estProcHor);
                }
                
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    _connection.Execute(query, estProcHorList, transaction, commandTimeout: TimeOut, commandType: CommandType.Text);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    throw;
                }
            }
        }

        public EstabelecimentoProcedimentoHorario GetByDateTime(EstabelecimentoProcedimentoHorario filters)
        {

            var query = @"SELECT CONCAT(EstabelecimentoId, '') AS EstabelecimentoId, CONCAT(ProcedimentoId, '') AS ProcedimentoId, CONCAT(ProfissionalId, '') AS ProfissionalId, DataHora FROM EstabelecimentoProcedimentoHorario 
                    WHERE EstabelecimentoId = ?EstabelecimentoId 
                    AND ProcedimentoId = ?ProcedimentoId
                    AND ProfissionalId = ?ProfissionalId
                    AND TipoDaConsulta = ?TipoDaConsulta
                    AND DataHora = ?DataHora";

            var param = new DynamicParameters();
            param.Add("?EstabelecimentoId", filters.EstabelecimentoId, DbType.String, ParameterDirection.Input);
            param.Add("?ProcedimentoId", filters.ProcedimentoId, DbType.String, ParameterDirection.Input);
            param.Add("?ProfissionalId", filters.ProfissionalId, DbType.String, ParameterDirection.Input);
            param.Add("?TipoDaConsulta", filters.TipoDaConsulta, DbType.String, ParameterDirection.Input);
            param.Add("?DataHora", filters.DataHora, DbType.DateTime, ParameterDirection.Input);

            var estProcHor = _connection.Query<EstabelecimentoProcedimentoHorario>(query, param).FirstOrDefault();

            return estProcHor;

        }

        public int Update(string id, EstabelecimentoProcedimentoHorario obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;
            var situacao = false;
            try
            {
                var param = new DynamicParameters();
                param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
                //param.Add("?EstabelecimentoId", dbType: DbType.String, value: obj.EstabelecimentoId, direction: ParameterDirection.Input);
                //param.Add("?ProcedimentoId", dbType: DbType.String, value: obj.ProcedimentoId, direction: ParameterDirection.Input);
                //param.Add("?ProfissionalId", dbType: DbType.String, value: obj.ProfissionalId, direction: ParameterDirection.Input);
                //param.Add("?DataHora", dbType: DbType.DateTime, value: obj.DataHora, direction: ParameterDirection.Input);
                //param.Add("?Dia", dbType: DbType.Date, value: obj.Dia, direction: ParameterDirection.Input);
                //param.Add("?Hora", dbType: DbType.Time, value: obj.Hora, direction: ParameterDirection.Input);
                //param.Add("?Situacao", dbType: DbType.Boolean, value: situacao, direction: ParameterDirection.Input);
                //param.Add("?TipoDaConsulta", dbType: DbType.String, value: obj.TipoDaConsulta, direction: ParameterDirection.Input);

                const string updateQuery = @"UPDATE EstabelecimentoProcedimentoHorario E SET                        
                                    E.Situacao = False
                                    WHERE E.Id = ?Id";

            
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(updateQuery, param, transaction);
                    // RegistrarAcao(_connection, Guid.NewGuid().ToString(), "EstabelecimentoProcedimentoHorario", obj.Id.ToString(), "Editou EstabelecimentoProcedimentoHorario", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
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
