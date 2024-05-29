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
    public class IndividuoProcedimentoRepository : BaseRepository, IIndividuoProcedimentoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(@"SELECT 
                                                                CAST(IndividuoProcedimento.Id AS CHAR) AS Id,
                                                                CAST(IndividuoProcedimento.IndividuoId AS CHAR) AS IndividuoId, 
                                                                CAST(IndividuoProcedimento.ProfissionalId AS CHAR) AS ProfissionalId,
                                                                IndividuoProcedimento.ProcedimentoCodigo,
                                                                IndividuoProcedimento.DataCadastro,
                                                                IndividuoProcedimento.DataAlteracao,
                                                                Individuo.NomeCompleto,
                                                                IndividuoProcedimento.AtendidoTriagem,
                                                                IndividuoProcedimento.AtendidoMedico
                                                                FROM IndividuoProcedimento
                                                                INNER JOIN Individuo ON (IndividuoProcedimento.IndividuoId = Individuo.Id) 
                                                                INNER JOIN Profissional ON (IndividuoProcedimento.ProfissionalId = Profissional.Id)");


        private readonly StringBuilder _queryCountAll = new StringBuilder(@"SELECT COUNT(1) FROM IndividuoProcedimento
                                                                      INNER JOIN Individuo ON (IndividuoProcedimento.IndividuoId = Individuo.Id) 
                                                                INNER JOIN Profissional ON (IndividuoProcedimento.ProfissionalId = Profissional.Id)");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public IndividuoProcedimentoRepository(IMySqlContext context, IUser user)
          : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }


        //public void Add(IEnumerable<IndividuoCid10> list)
        //{
        //    const string insertQuery = @"INSERT INTO IndividuoCid10 
        //                    (Id, IndividuoId, ProfissionalId, CId10Id, AgendamentoId, DataCadastro, DataAlteracao, AtendidoMedico) VALUES
        //                    (?Id, ?IndividuoId, ?ProfissionalId, ?Cid10Id, ?AgendamentoId, ?DataCadastro, ?DataAlteracao, ?AtendidoMedico)";
        //    var param = new DynamicParameters();

        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        foreach (var item in list)
        //        {
        //            param.Add("?Id", item.Id, DbType.String, ParameterDirection.Input);
        //            param.Add("?IndividuoId", item.IndividuoId, DbType.String, ParameterDirection.Input);
        //            param.Add("?ProfissionalId", item.ProfissionalId, DbType.String, ParameterDirection.Input);
        //            param.Add("?Cid10Id", item.Cid10Id, DbType.Int32, ParameterDirection.Input);
        //            param.Add("?AgendamentoId", item.AgendamentoId, DbType.String, ParameterDirection.Input);
        //            param.Add("?DataCadastro", item.DataCadastro, DbType.DateTime, ParameterDirection.Input);
        //            param.Add("?DataAlteracao", item.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
        //            param.Add("?AtendidoMedico", item.AtendidoTriagem, DbType.Boolean, ParameterDirection.Input);

        //            _connection.Execute(insertQuery, param, transaction);
        //            RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoCid10", item.Id,
        //              $"Inseriu o CID10 {item.Cid10Id} para o Individuo: {item.IndividuoId}",
        //              DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        //        }
        //        transaction.Commit();
        //    }
        //}
        public void AddTriagem(IEnumerable<IndividuoProcedimento> list)
        {
            const string insertQuery = @"INSERT INTO IndividuoProcedimento
                            (Id, IndividuoId, ProfissionalId, ProcedimentoCodigo, AgendamentoId, DataCadastro, DataAlteracao, AtendidoTriagem) VALUES
                            (?Id, ?IndividuoId, ?ProfissionalId, ?ProcedimentoCodigo, ?AgendamentoId, ?DataCadastro, ?DataAlteracao, ?AtendidoTriagem)";
            var param = new DynamicParameters();

            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var item in list)
                {
                    param.Add("?Id", item.Id, DbType.String, ParameterDirection.Input);
                    param.Add("?IndividuoId", item.IndividuoId, DbType.String, ParameterDirection.Input);
                    param.Add("?ProfissionalId", item.ProfissionalId, DbType.String, ParameterDirection.Input);
                    param.Add("?ProcedimentoCodigo", item.ProcedimentoCodigo, DbType.String, ParameterDirection.Input);
                    param.Add("?AgendamentoId", item.AgendamentoId, DbType.String, ParameterDirection.Input);
                    param.Add("?DataCadastro", item.DataCadastro, DbType.DateTime, ParameterDirection.Input);
                    param.Add("?DataAlteracao", item.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
                    param.Add("?AtendidoTriagem", item.AtendidoTriagem, DbType.Boolean, ParameterDirection.Input);

                    _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoCid10", item.Id,
                      $"Inseriu o ProcedimentoCodigo {item.ProcedimentoCodigo} para o Individuo: {item.IndividuoId}",
                      DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                }
                transaction.Commit();
            }
        }

        //public int Update(string id, IndividuoCid10 obj)
        //{
        //    if (string.IsNullOrEmpty(id)) return 0;
        //    if (obj == null) return 0;

        //    const string updateQuery = @"UPDATE IndividuoCid10 SET
        //                        IndividuoId = ?IndividuoId, 
        //                        ProfissionalId = ?ProfissionalId,
        //                        Cid10Id = ?Cid10Id,
        //                        DataAlteracao = ?DataAlteracao
        //                        WHERE Id = ?Id";

        //    var param = new DynamicParameters();
        //    param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
        //    param.Add("?IndividuoId", obj.IndividuoId, DbType.String, ParameterDirection.Input);
        //    param.Add("?ProfissionalId", obj.ProfissionalId, DbType.String, ParameterDirection.Input);
        //    param.Add("?Cid10Id", obj.Cid10Id, DbType.Int32, ParameterDirection.Input);
        //    param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);

        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        var ret = _connection.Execute(updateQuery, param, transaction);
        //        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoCiap", obj.Id.ToString(),
        //          $"Editou o Cid10 ${obj.Cid10Id} do Individuo {obj.IndividuoId}", DateTime.Now,
        //          _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        //        transaction.Commit();
        //        return ret;
        //    }
        //}

        public async Task<(IEnumerable<IndividuoProcedimento>, int)> GetByParam(IndividuoProcedimento filters,
          string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação

            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters?.IndividuoId))
                {
                    queryFilter.Append(" AND IndividuoId = ?IndividuoId ");
                    param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId,
                      direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                {
                    queryFilter.Append(" AND ProfissionalId = ?ProfissionalId ");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId,
                      direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.ProcedimentoCodigo))
                {
                    queryFilter.Append(" AND ProcedimentoCodigo = ?ProcedimentoCodigo ");
                    param.Add("?ProcedimentoCodigo", dbType: DbType.String, value: filters.ProcedimentoCodigo,
                    direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.AgendamentoId))
                {
                    queryFilter.Append(" AND AgendamentoId = ?AgendamentoId ");
                    param.Add("?AgendamentoId", dbType: DbType.String, value: filters.AgendamentoId,
                    direction: ParameterDirection.Input);
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

            var totalCount = _connection
              .Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text)
              .SingleOrDefault();

            if (totalCount > 0)
            {
                var individuos = await _connection.QueryAsync<IndividuoProcedimento>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);

                var individuoDict = new Dictionary<string, IndividuoProcedimento>();

                foreach (var individuo in individuos)
                {
                    if (!individuoDict.TryGetValue(individuo.Id, out var individuoEntity))
                    {
                        individuoDict.Add(individuo.Id, individuo);
                    }
                }

                return (individuoDict.Values, totalCount);
            }

            return (null, 0);
        }

        //public Individuo GetById(string individuoId)
        //{

        //    var query = new StringBuilder(@"
        //                   SELECT 
        //                    CAST(Individuo.Id AS CHAR) As Id, 
        //                    Individuo.Cpf,
        //                    Individuo.Email,
        //                    Individuo.EmailToken,
        //                    Individuo.CodigoAutenticacao,
        //                    Individuo.Senha,
        //                    Individuo.TelefoneCelular,
        //                    Individuo.Individuo.NomeCompleto,
        //                    Individuo.NomeDaMae,
        //                    Individuo.DataNascimento,
        //                    Individuo.Sexo,
        //                    Individuo.RacaOuCor,
        //                    Individuo.DataCadastro,
        //                    Individuo.DataAlteracao,
        //                    Individuo.Latitude,
        //                    Individuo.Longitude,
        //                    Individuo.Logradouro,
        //                    Individuo.LogradouroNumero,
        //                    Individuo.LogradouroComplemento,
        //                    Individuo.LogradouroCep,
        //                    Individuo.LogradouroBairro,
        //                    Individuo.UfAbreviado,
        //                    Individuo.CidadeId,
        //                    Individuo.RespondeuComorbidade,
        //                    Individuo.Comorbidades,
        //                    Individuo.Hipertenso,
        //                    Individuo.Diabetes,
        //                    Individuo.Fumante,
        //                    Individuo.Asma,
        //                    Individuo.DoencaCoracao,
        //                    Individuo.DoencaPulmao,
        //                    Individuo.DoencaRins,
        //                    Individuo.DoencaFigado,
        //                    Individuo.DoencaCancer,
        //                    Individuo.Transplantado,
        //                    Individuo.DoencaComprometeImunidade,
        //                    Individuo.LugarComCasosCorona,
        //                    Individuo.Individuo.SintomasGripais,
        //                    Individuo.Obesidade,
        //                    Individuo.Gestante,
        //                    Individuo.DoencaNeurologica,
        //                    Individuo.AnemiaFalciforme,
        //                    Individuo.DataInicioSintomas,
        //                    Individuo.Ativo,
        //                    Individuo.EstabelecimentoId,
        //                    Individuo.NotificacaoToken,
        //                    CAST(IndividuoCid10.IndividuoId AS CHAR) AS IndividuoId, 
        //                    CAST(IndividuoCid10.ProfissionalId AS CHAR) AS ProfissionalId,
        //                    CAST(IndividuoCid10.Cid10Id AS CHAR) AS Cid10Id,
        //                    IndividuoCid10.DataCadastro,
        //                    IndividuoCid10.DataAlteracao
        //                    FROM 
        //                    (SELECT 
        //                    CAST(i.Id AS CHAR) As Id, 
        //                    i.Cpf,
        //                    i.Email,
        //                    i.EmailToken,
        //                    i.CodigoAutenticacao,
        //                    i.Senha,
        //                    i.TelefoneCelular,
        //                    i.IndividuoNomeCompleto,
        //                    i.NomeDaMae,
        //                    i.DataNascimento,
        //                    i.Sexo,
        //                    i.RacaOuCor,
        //                    i.DataCadastro,
        //                    i.DataAlteracao,
        //                    i.Latitude,
        //                    i.Longitude,
        //                    i.Logradouro,
        //                    i.LogradouroNumero,
        //                    i.LogradouroComplemento,
        //                    i.LogradouroCep,
        //                    i.LogradouroBairro,
        //                    i.UfAbreviado,
        //                    i.CidadeId,
        //                    i.RespondeuComorbidade,
        //                    i.Comorbidades,
        //                    i.Hipertenso,
        //                    i.Diabetes,
        //                    i.Fumante,
        //                    i.Asma,
        //                    i.DoencaCoracao,
        //                    i.DoencaPulmao,
        //                    i.DoencaRins,
        //                    i.DoencaFigado,
        //                    i.DoencaCancer,
        //                    i.Transplantado,
        //                    i.DoencaComprometeImunidade,
        //                    i.LugarComCasosCorona,
        //                    i.IndividuoSintomasGripais,
        //                    i.Obesidade,
        //                    i.Gestante,
        //                    i.DoencaNeurologica,
        //                    i.AnemiaFalciforme,
        //                    i.DataInicioSintomas,
        //                    i.Ativo,
        //                    i.EstabelecimentoId,
        //                    i.NotificacaoToken FROM Individuo i
        //                    WHERE 1 = 1 ");

        //    var queryFilter = new StringBuilder(" ");
        //    var param = new DynamicParameters();

        //    if (string.IsNullOrEmpty(individuoId)) return null;

        //    queryFilter.Append(" AND i.Id = ?individuoId ");
        //    param.Add("@individuoId", dbType: DbType.String, value: individuoId, direction: ParameterDirection.Input);

        //    query.Append(queryFilter);

        //    query.Append(@" ) AS Individuo
        //                   LEFT JOIN IndividuoCid10 ON IndividuoCid10.IndividuoId = Individuo.Id ");

        //    var individuo = _connection
        //      .Query<Individuo>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text)
        //      .FirstOrDefault();

        //    var queryProcedimentos = @"SELECT CAST(IndividuoCid10.Id AS CHAR) AS Id,
        //                          CAST(IndividuoCid10.IndividuoId AS CHAR) AS IndividuoId,
        //                          CAST(IndividuoCid10.ProfissionalId AS CHAR) AS ProfissionalId,
        //                          CAST(IndividuoCid10.CiaplId AS CHAR) AS CiaplId,
        //                          IndividuoCid10.DataCadastro,
        //                          IndividuoCid10.DataAlteracao,
        //                          CAST(Cid10.Id AS CHAR) AS Id, 
        //                          Cid10.Codigo,
        //                          Cid10.Descricao
        //                          FROM IndividuoCid10
        //                          LEFT JOIN Cid10 ON Cid10.Id = IndividuoCid10.Cid10Id
        //                          WHERE IndividuoCid10.IndividuoId = ?individuoId ";

        //    var individuoCiaps =
        //      (IList<IndividuoCiap>)_connection.Query<IndividuoCid10, Cid, IndividuoCid10>(queryProcedimentos,
        //        (ic, c) =>
        //        {
        //            ic.Cid = c;
        //            return ic;
        //        }, param, commandTimeout: TimeOut, commandType: CommandType.Text);

        //    if (!individuoCiaps.Any()) return individuo;
        //    individuo.IndividuoCiap = individuoCiaps;

        //    return individuo;
        //}
    }

}
