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
    public class EstabelecimentoProfissionalRepository : BaseRepository, IEstabelecimentoProfissionalRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(@" SELECT
                                                    CAST( EstabelecimentoProfissional.Id AS CHAR ) AS Id,
                                                    CAST( EstabelecimentoProfissional.ProfissionalId AS CHAR ) AS ProfissionalId,
                                                    CAST( EstabelecimentoProfissional.EstabelecimentoId AS CHAR ) AS EstabelecimentoId,
                                                    CAST( Estabelecimento.Id AS CHAR ) AS Id,
                                                     Estabelecimento.NomeFantasia, 
                                                    Estabelecimento.Cnpj, 
                                                    Estabelecimento.Cnes, 
                                                    Estabelecimento.CodEsfAdm, 
                                                    Estabelecimento.TpUnidId,
                                                    Estabelecimento.Telefone1,
                                                    Estabelecimento.Telefone2,
                                                    Estabelecimento.Fax, 
                                                    Estabelecimento.Email,
                                                    Estabelecimento.Cep,
                                                    Estabelecimento.Uf, 
                                                    Estabelecimento.CodIbgeMun, 
                                                    Estabelecimento.Bairro, 
                                                    Estabelecimento.Logradouro, 
                                                    Estabelecimento.Numero,
                                                    Estabelecimento.Complemento, 
                                                    Estabelecimento.PontoReferencia, 
                                                    Estabelecimento.SgComplexidade1, 
                                                    Estabelecimento.SgComplexidade2,
                                                    Estabelecimento.TpEquipe,
                                                    Estabelecimento.SgEquipe,
                                                    Estabelecimento.DsEquipe,
                                                    Estabelecimento.CodIne, 
                                                    Estabelecimento.CodArea, 
                                                    Estabelecimento.DsArea, 
                                                    Estabelecimento.NmReferencia, 
                                                    Estabelecimento.DataDesativacao, 
                                                    Estabelecimento.Tipo, 
                                                    Estabelecimento.Deletado,
                                                    Profissional.Nome

                                                    FROM
	                                                    EstabelecimentoProfissional
	                                                    INNER JOIN Profissional ON ( EstabelecimentoProfissional.ProfissionalId = Profissional.Id ) -- VOLTAR PARA INNER APOS TESTES
	                                                    INNER JOIN Estabelecimento ON ( EstabelecimentoProfissional.EstabelecimentoId = Estabelecimento.Id )  ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(@"SELECT COUNT(1) FROM EstabelecimentoProfissional
                                                                      INNER JOIN Profissional ON (EstabelecimentoProfissional.ProfissionalId = Profissional.Id) 
                                                                      INNER JOIN Estabelecimento ON (EstabelecimentoProfissional.EstabelecimentoId = Estabelecimento.Id) ");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public EstabelecimentoProfissionalRepository(IMySqlContext context, IUser user)
          : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }


        public void Add(IEnumerable<EstabelecimentoProfissional> list)
        {
            const string insertQuery = @"INSERT INTO EstabelecimentoProfissional
                            (Id, EstabelecimentoId, ProfissionalId) VALUES
                            (?Id, ?EstabelecimentoId, ?ProfissionalId)";
            var param = new DynamicParameters();

            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var item in list)
                {
                    param.Add("?Id", item.Id, DbType.String, ParameterDirection.Input);
                    param.Add("?EstabelecimentoId", item.EstabelecimentoId, DbType.String, ParameterDirection.Input);
                    param.Add("?ProfissionalId", item.ProfissionalId, DbType.String, ParameterDirection.Input);
                    //param.Add("?Cota", item.Cota, DbType.Int32, ParameterDirection.Input);
                    //param.Add("?CotaExecutor", item.CotaExecutor, DbType.Int32, ParameterDirection.Input);
                    //param.Add("?DataAlteracao", item.DataAlteracao, DbType.DateTime, ParameterDirection.Input);

                    _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "EstabelecimentoProfissional", item.Id,
                      $"Inseriu o Profissional {item.ProfissionalId} para o Estabelecimento: {item.EstabelecimentoId}",
                      DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                }
                transaction.Commit();
            }
        }

        public int Update(string id, EstabelecimentoProfissional obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            const string updateQuery = @"UPDATE EstabelecimentoProfissional SET
                                EstabelecimentoId = ?EstabelecimentoId, 
                                ProfissionalId = ?ProfissionalId
                                WHERE Id = @Id";

            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?EstabelecimentoId", obj.EstabelecimentoId, DbType.String, ParameterDirection.Input);
            param.Add("?ProfissionalId", obj.ProfissionalId, DbType.String, ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(updateQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "EstabelecimentoProfissional", obj.Id.ToString(),
                  $"Editou o Profissional ${obj.ProfissionalId} do Estabelecimento {obj.EstabelecimentoId}", DateTime.Now,
                  _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public async Task<(IEnumerable<EstabelecimentoProfissional>, int)> GetByParam(EstabelecimentoProfissional filters,
          AppParams appParams, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação

            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters?.ProfissionalId))
                {
                    queryFilter.Append(" AND ProfissionalId = ?ProfissionalId ");
                    param.Add("?ProfissionalId", dbType: DbType.String, value: filters.ProfissionalId,
                      direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters?.EstabelecimentoId))
                {
                    queryFilter.Append(" AND EstabelecimentoId = ?EstabelecimentoId ");
                    param.Add("?EstabelecimentoId", dbType: DbType.String, value: filters.EstabelecimentoId,
                      direction: ParameterDirection.Input);
                }

                //if (!string.IsNullOrEmpty(filters?.Descricao))
                //{
                //  queryFilter.Append(" AND Descricao LIKE ?Descricao ");
                //  param.Add("?Descricao", dbType: DbType.String, value: $"%{filters.Descricao}%", direction: ParameterDirection.Input);
                //}

                //if (!string.IsNullOrEmpty(filters?.Codigo))
                //{
                //  queryFilter.Append(" AND Codigo LIKE ?Codigo ");
                //  param.Add("?Codigo", dbType: DbType.String, value: $"%{filters.Codigo}%", direction: ParameterDirection.Input);
                //}

                if (appParams.Positivo == true)
                {
                    queryFilter.Append(" AND Cota > 0 ");
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
                var x = await _connection.QueryAsync<EstabelecimentoProfissional>(_queryAll.ToString(), param,
                  commandTimeout: TimeOut, commandType: CommandType.Text);
                return (x, totalCount);
            }

            return (null, 0);
        }

        //public Estabelecimento GetById(string profissionalId)
        //{


        //  var queryFilter = new StringBuilder(" ");
        //  var param = new DynamicParameters();

        //  if (string.IsNullOrEmpty(profissionalId)) return null;

        //  queryFilter.Append(" AND EstabelecimentoProfissional.ProfissionalId = ?profissionalId ");
        //  param.Add("?ProfissionalId", dbType: DbType.String, value: profissionalId, direction: ParameterDirection.Input);

        //  _queryAll.Append(queryFilter);



        //  var estabelecimento = _connection.Query<Estabelecimento>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();

        //  return estabelecimento;

        //}

        public Estabelecimento GetById(string profissionalId)
        {

            var query = new StringBuilder(@"
                           SELECT
	                        DISTINCT CAST( p.Id AS CHAR ) AS Id,
	                        p.Nome,
	                        p.Cpf,
	                        p.Cns,
	                        p.DataNascimento,
	                        p.Sexo,
	                        p.Crm,
	                        p.Email,
	                        p.Telefone,
	                        p.Logradouro,
	                        p.LogradouroNumero,
	                        p.LogradouroComplemento,
	                        p.LogradouroBairro,
	                        p.LogradouroCep,
	                        p.CidadeId,
	                        p.UfAbreviado,
	                        p.DataAlteracao,
	                        p.DataCadastro,
	                        p.Ativo,
	                        p.Perfil,
	                        CAST( EstabelecimentoProfissional.Id AS CHAR ) AS EstabelecimentoProfissionalId,
	                        CAST( EstabelecimentoProfissional.ProfissionalId AS CHAR ) AS ProfissionalId,
	                        CAST( EstabelecimentoProfissional.EstabelecimentoId AS CHAR ) AS EstabelecimentoId 
                        FROM
	                        (
                        SELECT
	                        CAST( prof.Id AS CHAR ) AS Id,
	                        prof.Nome,
	                        prof.Cpf,
	                        prof.Cns,
	                        prof.DataNascimento,
	                        prof.Sexo,
	                        prof.Crm,
	                        prof.Email,
	                        prof.Telefone,
	                        prof.Logradouro,
	                        prof.LogradouroNumero,
	                        prof.LogradouroComplemento,
	                        prof.LogradouroBairro,
	                        prof.LogradouroCep,
	                        prof.CidadeId,
	                        prof.UfAbreviado,
	                        prof.DataAlteracao,
	                        prof.DataCadastro,
	                        prof.Ativo,
	                        prof.Perfil
                        FROM
	                        Profissional prof
	                        LEFT JOIN EstabelecimentoProfissional ON EstabelecimentoProfissional.ProfissionalId = prof.Id
	                        -- LEFT JOIN Profissional pro ON prof.Id = EstabelecimentoProfissional.ProfissionalId 
                        WHERE
	                        prof.Ativo = TRUE ");

            var queryFilter = new StringBuilder(" ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(profissionalId)) return null;

            queryFilter.Append(" AND prof.Id = ?profissionalId ");
            param.Add("@profissionalId", dbType: DbType.String, value: profissionalId, direction: ParameterDirection.Input);

            query.Append(queryFilter);

            query.Append(@" ) AS p
	                    LEFT JOIN EstabelecimentoProfissional ON EstabelecimentoProfissional.ProfissionalId = p.Id ");

            var estabelecimento = _connection
              .Query<Estabelecimento>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text)
              .FirstOrDefault();

            var queryProfissionais = @"SELECT
	                        CAST( EstabelecimentoProfissional.Id AS CHAR ) AS Id,
	                        CAST( EstabelecimentoProfissional.ProfissionalId AS CHAR ) AS ProfissionalId,
	                        CAST( EstabelecimentoProfissional.EstabelecimentoId AS CHAR ) AS EstabelecimentoId,
	                        CAST( Profissional.Id AS CHAR ) AS Id,
	                        Profissional.Nome,
	                        Profissional.Cpf,
	                        Profissional.Cns,
	                        Profissional.DataNascimento,
	                        Profissional.Sexo,
	                        Profissional.Crm,
	                        Profissional.Email,
	                        Profissional.Telefone,
	                        Profissional.Logradouro,
	                        Profissional.LogradouroNumero,
	                        Profissional.LogradouroComplemento,
	                        Profissional.LogradouroBairro,
	                        Profissional.LogradouroCep,
	                        Profissional.CidadeId,
	                        Profissional.UfAbreviado,
	                        Profissional.DataAlteracao,
	                        Profissional.DataCadastro,
	                        Profissional.Ativo,
	                        Profissional.Perfil 
                        FROM
	                        EstabelecimentoProfissional
	                        LEFT JOIN Profissional ON Profissional.Id = EstabelecimentoProfissional.ProfissionalId 
                        WHERE
	                        EstabelecimentoProfissional.ProfissionalId = ?profissionalId ";

            var estabelecimentoProfissionais =
              (IList<EstabelecimentoProfissional>)_connection.Query<EstabelecimentoProfissional, Profissional, EstabelecimentoProfissional>(queryProfissionais,
                (ep, p) =>
                {
                    ep.Profissional = p;
                    return ep;
                }, param, commandTimeout: TimeOut, commandType: CommandType.Text);

            if (!estabelecimentoProfissionais.Any()) return estabelecimento;
            estabelecimento.ProfissionaisEstabelecimento = estabelecimentoProfissionais;

            return estabelecimento;
        }
    }

}
