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
    public class IndividuoCiapRepository : BaseRepository, IIndividuoCiapRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(@"SELECT 
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
                                                                INNER JOIN Ciap ON (IndividuoCiap.CiapId = Ciap.Id) ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(@"SELECT COUNT(IndividuoCiap.Id) FROM IndividuoCiap
                                                                      INNER JOIN Individuo ON (IndividuoCiap.IndividuoId = Individuo.Id) 
                                                                INNER JOIN Profissional ON (IndividuoCiap.ProfissionalId = Profissional.Id)
                                                                INNER JOIN Ciap ON (IndividuoCiap.CiapId = Ciap.Id) ");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public IndividuoCiapRepository(IMySqlContext context, IUser user)
          : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }


        public void Add(IEnumerable<IndividuoCiap> list)
        {
            const string insertQuery = @"INSERT INTO IndividuoCiap 
                            (Id, IndividuoId, ProfissionalId, CiapId, AgendamentoId, DataCadastro, DataAlteracao, AtendidoMedico) VALUES
                            (?Id, ?IndividuoId, ?ProfissionalId, ?CiapId, ?AgendamentoId, ?DataCadastro, ?DataAlteracao, ?AtendidoMedico)";
            var param = new DynamicParameters();


            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var item in list)
                {
                    param.Add("?Id", item.Id, DbType.String, ParameterDirection.Input);
                    param.Add("?IndividuoId", item.IndividuoId, DbType.String, ParameterDirection.Input);
                    param.Add("?ProfissionalId", item.ProfissionalId, DbType.String, ParameterDirection.Input);
                    param.Add("?CiapId", item.CiapId, DbType.Int32, ParameterDirection.Input);
                    param.Add("?AgendamentoId", item.AgendamentoId, DbType.String, ParameterDirection.Input);
                    param.Add("?DataCadastro", item.DataCadastro, DbType.DateTime, ParameterDirection.Input);
                    param.Add("?DataAlteracao", item.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
                    param.Add("?AtendidoMedico", item.AtendidoTriagem, DbType.Boolean, ParameterDirection.Input);

                    _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoCiap", item.Id, $"Inseriu o CIAP {item.CiapId} para o Individuo: {item.IndividuoId}", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                }
                transaction.Commit();
            }
        }
        public void AddTriagem(IEnumerable<IndividuoCiap> list)
        {
            const string insertQuery = @"INSERT INTO IndividuoCiap 
                            (Id, IndividuoId, ProfissionalId, CiapId, AgendamentoId, DataCadastro, DataAlteracao, AtendidoTriagem) VALUES
                            (?Id, ?IndividuoId, ?ProfissionalId, ?CiapId, ?AgendamentoId, ?DataCadastro, ?DataAlteracao, ?AtendidoTriagem)";
            var param = new DynamicParameters();


            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var item in list)
                {
                    param.Add("?Id", item.Id, DbType.String, ParameterDirection.Input);
                    param.Add("?IndividuoId", item.IndividuoId, DbType.String, ParameterDirection.Input);
                    param.Add("?ProfissionalId", item.ProfissionalId, DbType.String, ParameterDirection.Input);
                    param.Add("?CiapId", item.CiapId, DbType.Int32, ParameterDirection.Input);
                    param.Add("?AgendamentoId", item.AgendamentoId, DbType.String, ParameterDirection.Input);
                    param.Add("?DataCadastro", item.DataCadastro, DbType.DateTime, ParameterDirection.Input);
                    param.Add("?DataAlteracao", item.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
                    param.Add("?AtendidoTriagem", item.AtendidoTriagem, DbType.Boolean, ParameterDirection.Input);

                    _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoCiap", item.Id, $"Inseriu o CIAP {item.CiapId} para o Individuo: {item.IndividuoId}", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                }
                transaction.Commit();
            }
        }

        public int Update(string id, IndividuoCiap obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            const string updateQuery = @"UPDATE IndividuoCiap SET
                                IndividuoId = ?IndividuoId, 
                                ProfissionalId = ?ProfissionalId,
                                CiapId = ?CiapId,
                                DataAlteracao = ?DataAlteracao
                                WHERE Id = ?Id";

            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?IndividuoId", obj.IndividuoId, DbType.String, ParameterDirection.Input);
            param.Add("?ProfissionalId", obj.ProfissionalId, DbType.String, ParameterDirection.Input);
            param.Add("?CiapId", obj.CiapId, DbType.Int32, ParameterDirection.Input);
            param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(updateQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoCiap", obj.Id.ToString(),
                  $"Editou o Ciap ${obj.CiapId} do Individuo {obj.IndividuoId}", DateTime.Now,
                  _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public async Task<(IEnumerable<IndividuoCiap>, int)> GetByParam(IndividuoCiap filters,
          string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação

            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters?.IndividuoId))
                {
                    queryFilter.Append(" AND IndividuoCiap.IndividuoId = ?IndividuoId ");
                    param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId,
                      direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                {
                    queryFilter.Append(" AND IndividuoCiap.ProfissionalId = ?ProfissionalId ");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId,
                      direction: ParameterDirection.Input);
                }

                if (filters?.CiapId != 0)
                {
                    queryFilter.Append(" AND IndividuoCiap.CiapId = ?CiapId ");
                    param.Add("?CiapId", dbType: DbType.String, value: filters.CiapId,
                        direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.AgendamentoId))
                {
                    queryFilter.Append(" AND IndividuoCiap.AgendamentoId = ?AgendamentoId ");
                    param.Add("?AgendamentoId", dbType: DbType.String, value: filters.AgendamentoId,
                        direction: ParameterDirection.Input);
                }

                if (filters.AtendidoTriagem)
                {
                    queryFilter.Append(" AND IndividuoCiap.AtendidoTriagem = ?AtendidoTriagem ");
                    param.Add("?AtendidoTriagem", dbType: DbType.Boolean, value: filters.AtendidoTriagem,
                        direction: ParameterDirection.Input);
                }

                if (filters.AtendidoMedico)
                {
                    queryFilter.Append(" AND IndividuoCiap.AtendidoMedico = ?AtendidoMedico ");
                    param.Add("?AtendidoMedico", dbType: DbType.Boolean, value: filters.AtendidoMedico,
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

            //if (totalCount > 0)
            //{
            //    var x = await _connection.QueryAsync<IndividuoCiap>(_queryAll.ToString(), param,
            //      commandTimeout: TimeOut, commandType: CommandType.Text);
            //    return (x, totalCount);
            //}

            if (totalCount > 0)
            {
                var individuoCiap = new Dictionary<string, IndividuoCiap>();
                await _connection.QueryAsync<IndividuoCiap, Ciap, IndividuoCiap>(
                    _queryAll.ToString(), (a, c) => {
                        if (!individuoCiap.TryGetValue(a.Id, out var ciapEntity))
                        {
                            individuoCiap.Add(a.Id, ciapEntity = a);
                        }
                        ciapEntity.Ciap = c;

                        return ciapEntity;
                    },
                    param,
                    splitOn: "Id",
                    commandTimeout: TimeOut,
                    commandType: CommandType.Text);
                return (individuoCiap.Values, totalCount);
            }

            return (null, 0);
        }

        public Individuo GetById(string individuoId)
        {

            var query = new StringBuilder(@"
                           SELECT 
                            CAST(Individuo.Id AS CHAR) As Id, 
                            Individuo.Cpf,
                            Individuo.Email,
                            Individuo.EmailToken,
                            Individuo.CodigoAutenticacao,
                            Individuo.Senha,
                            Individuo.TelefoneCelular,
                            Individuo.NomeCompleto,
                            Individuo.NomeDaMae,
                            Individuo.DataNascimento,
                            Individuo.Sexo,
                            Individuo.RacaOuCor,
                            Individuo.DataCadastro,
                            Individuo.DataAlteracao,
                            Individuo.Latitude,
                            Individuo.Longitude,
                            Individuo.Logradouro,
                            Individuo.LogradouroNumero,
                            Individuo.LogradouroComplemento,
                            Individuo.LogradouroCep,
                            Individuo.LogradouroBairro,
                            Individuo.UfAbreviado,
                            Individuo.CidadeId,
                            Individuo.RespondeuComorbidade,
                            Individuo.Comorbidades,
                            Individuo.Hipertenso,
                            Individuo.Diabetes,
                            Individuo.Fumante,
                            Individuo.Asma,
                            Individuo.DoencaCoracao,
                            Individuo.DoencaPulmao,
                            Individuo.DoencaRins,
                            Individuo.DoencaFigado,
                            Individuo.DoencaCancer,
                            Individuo.Transplantado,
                            Individuo.DoencaComprometeImunidade,
                            Individuo.LugarComCasosCorona,
                            Individuo.SintomasGripais,
                            Individuo.Obesidade,
                            Individuo.Gestante,
                            Individuo.DoencaNeurologica,
                            Individuo.AnemiaFalciforme,
                            Individuo.DataInicioSintomas,
                            Individuo.Ativo,
                            Individuo.EstabelecimentoId,
                            Individuo.NotificacaoToken,
                            CAST(IndividuoCiap.IndividuoId AS CHAR) AS IndividuoId, 
                            CAST(IndividuoCiap.ProfissionalId AS CHAR) AS ProfissionalId,
                            CAST(IndividuoCiap.CiapId AS CHAR) AS CiapId,
                            IndividuoCiap.DataCadastro,
                            IndividuoCiap.DataAlteracao
                            FROM 
                            (SELECT 
                            CAST(i.Id AS CHAR) As Id, 
                            i.Cpf,
                            i.Email,
                            i.EmailToken,
                            i.CodigoAutenticacao,
                            i.Senha,
                            i.TelefoneCelular,
                            i.NomeCompleto,
                            i.NomeDaMae,
                            i.DataNascimento,
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
                            i.NotificacaoToken FROM Individuo i
                            WHERE 1 = 1 ");

            var queryFilter = new StringBuilder(" ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(individuoId)) return null;

            queryFilter.Append(" AND i.Id = ?individuoId ");
            param.Add("@individuoId", dbType: DbType.String, value: individuoId, direction: ParameterDirection.Input);

            query.Append(queryFilter);

            query.Append(@" ) AS Individuo
                           LEFT JOIN IndividuoCiap ON IndividuoCiap.IndividuoId = Individuo.Id ");

            var individuo = _connection
              .Query<Individuo>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text)
              .FirstOrDefault();

            var queryProcedimentos = @"SELECT CAST(IndividuoCiap.Id AS CHAR) AS Id,
                                  CAST(IndividuoCiap.IndividuoId AS CHAR) AS IndividuoId,
                                  CAST(IndividuoCiap.ProfissionalId AS CHAR) AS ProfissionalId,
                                  CAST(IndividuoCiap.CiapId AS CHAR) AS CiapId,
                                  IndividuoCiap.DataCadastro,
                                  IndividuoCiap.DataAlteracao,
                                  CAST(Ciap.Id AS CHAR) AS Id, 
                                  Ciap.Codigo,
                                  Ciap.Descricao
                                  FROM IndividuoCiap
                                  LEFT JOIN Ciap ON Ciap.Id = IndividuoCiap.CiapId
                                  WHERE IndividuoCiap.IndividuoId = ?individuoId ";

            var individuoCiaps =
              (IList<IndividuoCiap>)_connection.Query<IndividuoCiap, Ciap, IndividuoCiap>(queryProcedimentos,
                (ic, c) =>
                {
                    ic.Ciap = c;
                    return ic;
                }, param, commandTimeout: TimeOut, commandType: CommandType.Text);

            if (!individuoCiaps.Any()) return individuo;
            individuo.IndividuoCiap = individuoCiaps;

            return individuo;
        }
    }

}
