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
  public class EstabelecimentoProcedimentoRepository : BaseRepository, IEstabelecimentoProcedimentoRepository
  {
    private readonly StringBuilder _queryAll = new StringBuilder(@"SELECT 
                                                                CAST(EstabelecimentoProcedimento.Id AS CHAR) AS Id,
                                                                CAST(EstabelecimentoProcedimento.ProcedimentoId AS CHAR) AS ProcedimentoId, 
                                                                CAST(EstabelecimentoProcedimento.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                                                                EstabelecimentoProcedimento.Cota, 
                                                                EstabelecimentoProcedimento.CotaExecutor,
                                                                EstabelecimentoProcedimento.DataAlteracao, 
                                                                Procedimento.Descricao, 
                                                                Procedimento.Codigo,
                                                                Procedimento.CotaTotal, 
                                                                Procedimento.CotaTotalExecutor, 
                                                                Estabelecimento.NomeFantasia
                                                                FROM EstabelecimentoProcedimento
                                                                INNER JOIN Procedimento ON (EstabelecimentoProcedimento.ProcedimentoId = Procedimento.Id) -- VOLTAR PARA INNER APOS TESTES 
                                                                INNER JOIN Estabelecimento ON (EstabelecimentoProcedimento.EstabelecimentoId = Estabelecimento.Id)  ");

    private readonly StringBuilder _queryCountAll = new StringBuilder(@"SELECT COUNT(1) FROM EstabelecimentoProcedimento
                                                                      INNER JOIN Procedimento ON (EstabelecimentoProcedimento.ProcedimentoId = Procedimento.Id) 
                                                                      INNER JOIN Estabelecimento ON (EstabelecimentoProcedimento.EstabelecimentoId = Estabelecimento.Id) ");

    private readonly MySqlConnection _connection;
    private readonly IUser _user;

    public EstabelecimentoProcedimentoRepository(IMySqlContext context, IUser user)
      : base(context)
    {
      _connection = context.Connection;
      _user = user;
    }


    public void Add(IEnumerable<EstabelecimentoProcedimento> list)
    {
      const string insertQuery = @"INSERT INTO EstabelecimentoProcedimento 
                            (Id, EstabelecimentoId, ProcedimentoId, Cota, CotaExecutor, DataAlteracao) VALUES
                            (?Id, ?EstabelecimentoId, ?ProcedimentoId, ?Cota, ?CotaExecutor, ?DataAlteracao)";
      var param = new DynamicParameters();

      using (var transaction = _connection.BeginTransaction())
      {
        foreach (var item in list)
        {
          param.Add("?Id", item.Id, DbType.String, ParameterDirection.Input);
          param.Add("?EstabelecimentoId", item.EstabelecimentoId, DbType.String, ParameterDirection.Input);
          param.Add("?ProcedimentoId", item.ProcedimentoId, DbType.String, ParameterDirection.Input);
          param.Add("?Cota", item.Cota, DbType.Int32, ParameterDirection.Input);
          param.Add("?CotaExecutor", item.CotaExecutor, DbType.Int32, ParameterDirection.Input);
          param.Add("?DataAlteracao", item.DataAlteracao, DbType.DateTime, ParameterDirection.Input);

          _connection.Execute(insertQuery, param, transaction);
          RegistrarAcao(_connection, Guid.NewGuid().ToString(), "EstabelecimentoProcedimento", item.Id,
            $"Inseriu o Procedimento {item.ProcedimentoId} para o Estabelecimento: {item.EstabelecimentoId}",
            DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        }
        transaction.Commit();
      }
    }

    public int Update(string id, EstabelecimentoProcedimento obj)
    {
      if (string.IsNullOrEmpty(id)) return 0;
      if (obj == null) return 0;

      const string updateQuery = @"UPDATE EstabelecimentoProcedimento SET
                                EstabelecimentoId = ?EstabelecimentoId, 
                                ProcedimentoId = ?ProcedimentoId,
                                Cota = @Cota,
                                CotaExecutor = @CotaExecutor,
                                DataAlteracao = @DataAlteracao
                                WHERE Id = @Id";

      var param = new DynamicParameters();
      param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
      param.Add("?EstabelecimentoId", obj.EstabelecimentoId, DbType.String, ParameterDirection.Input);
      param.Add("?ProcedimentoId", obj.ProcedimentoId, DbType.String, ParameterDirection.Input);
      param.Add("?Cota", obj.Cota, DbType.Int32, ParameterDirection.Input);
      param.Add("?CotaExecutor", obj.Cota, DbType.Int32, ParameterDirection.Input);
      param.Add("?DataAlteracao", DateTime.Now, DbType.DateTime, ParameterDirection.Input);

      using (var transaction = _connection.BeginTransaction())
      {
        var ret = _connection.Execute(updateQuery, param, transaction);
        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "EstabelecimentoProcedimento", obj.Id.ToString(),
          $"Editou o Procedimento ${obj.ProcedimentoId} do Estabelecimento {obj.EstabelecimentoId}", DateTime.Now,
          _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        transaction.Commit();
        return ret;
      }
    }

    public async Task<(IEnumerable<EstabelecimentoProcedimento>, int)> GetByParam(EstabelecimentoProcedimento filters,
      AppParams appParams, string sort, int? skip, int? take)
    {
      #region -- -- Filtros / Ordenação

      var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
      var param = new DynamicParameters();

      if (filters != null)
      {
        if (!string.IsNullOrEmpty(filters?.ProcedimentoId))
        {
          queryFilter.Append(" AND ProcedimentoId = ?ProcedimentoId ");
          param.Add("?ProcedimentoId", dbType: DbType.String, value: filters.ProcedimentoId,
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
        var x = await _connection.QueryAsync<EstabelecimentoProcedimento>(_queryAll.ToString(), param,
          commandTimeout: TimeOut, commandType: CommandType.Text);
        return (x, totalCount);
      }

      return (null, 0);
    }

    public Estabelecimento GetById(string estabelecimentoId)
    {

      var query = new StringBuilder(@"
                           SELECT 
                            CAST(Estabelecimento.Id AS CHAR) As Id, 
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
                            CAST(EstabelecimentoProcedimento.Id AS CHAR) AS Id,
                            CAST(EstabelecimentoProcedimento.ProcedimentoId AS CHAR) AS ProcedimentoId, 
                            CAST(EstabelecimentoProcedimento.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                            EstabelecimentoProcedimento.Cota, 
                            EstabelecimentoProcedimento.CotaExecutor,
                            EstabelecimentoProcedimento.DataAlteracao
                            FROM 
                            (SELECT 
                            CAST(e.Id AS CHAR) As Id, 
                            e.NomeFantasia, 
                            e.Cnpj, 
                            e.Cnes, 
                            e.CodEsfAdm, 
                            e.TpUnidId,
                            e.Telefone1,
                            e.Telefone2,
                            e.Fax, 
                            e.Email,
                            e.Cep,
                            e.Uf, 
                            e.CodIbgeMun, 
                            e.Bairro, 
                            e.Logradouro, 
                            e.Numero,
                            e.Complemento, 
                            e.PontoReferencia, 
                            e.SgComplexidade1, 
                            e.SgComplexidade2,
                            e.TpEquipe,
                            e.SgEquipe,
                            e.DsEquipe,
                            e.CodIne, 
                            e.CodArea, 
                            e.DsArea, 
                            e.NmReferencia, 
                            e.DataDesativacao, 
                            e.Tipo, 
                            e.Deletado FROM Estabelecimento e
                            WHERE e.Deletado = FALSE ");

      var queryFilter = new StringBuilder(" ");
      var param = new DynamicParameters();

      if (string.IsNullOrEmpty(estabelecimentoId)) return null;

      queryFilter.Append(" AND e.Id = ?estabelecimentoId ");
      param.Add("@estabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);

      query.Append(queryFilter);

      query.Append(@" ) AS Estabelecimento
                           LEFT JOIN EstabelecimentoProcedimento ON EstabelecimentoProcedimento.EstabelecimentoId = Estabelecimento.Id ");

      var estabelecimento = _connection
        .Query<Estabelecimento>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text)
        .FirstOrDefault();

      var queryProcedimentos = @"SELECT CAST(EstabelecimentoProcedimento.Id AS CHAR) AS Id,
                                  CAST(EstabelecimentoProcedimento.ProcedimentoId AS CHAR) AS ProcedimentoId,
                                  CAST(EstabelecimentoProcedimento.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                                  EstabelecimentoProcedimento.Cota, 
                                  EstabelecimentoProcedimento.CotaExecutor,
                                  EstabelecimentoProcedimento.DataAlteracao,
                                  CAST(Procedimento.Id AS CHAR) AS Id, 
                                  Procedimento.Tipo, 
                                  Procedimento.Codigo, 
                                  Procedimento.Descricao,
                                  Procedimento.Sexo, 
                                  Procedimento.CotaTotal,
                                  Procedimento.CotaTotalExecutor, 
                                  Procedimento.CotaEstabelecimento, 
                                  Procedimento.CotaProfissional, 
                                  Procedimento.CotaExecutor,
                                  Procedimento.Valor 
                                  FROM EstabelecimentoProcedimento
                                  LEFT JOIN Procedimento ON Procedimento.Id = EstabelecimentoProcedimento.ProcedimentoId
                                  WHERE EstabelecimentoProcedimento.EstabelecimentoId = ?estabelecimentoId ";

      var estabelecimentoProcedimentos =
        (IList<EstabelecimentoProcedimento>)_connection.Query<EstabelecimentoProcedimento, Procedimento, EstabelecimentoProcedimento>(queryProcedimentos,
          (ep, p) =>
          {
            ep.Procedimento = p;
            return ep;
          }, param, commandTimeout: TimeOut, commandType: CommandType.Text);

      if (!estabelecimentoProcedimentos.Any()) return estabelecimento;
      estabelecimento.Procedimentos = estabelecimentoProcedimentos;

      return estabelecimento;
    }

    public void Delete(IEnumerable<EstabelecimentoProcedimento> list)
    {
            
        try {
            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var item in list)
                {
                    var query = @"DELETE FROM EstabelecimentoProcedimento WHERE ProcedimentoId = ?ProcedimentoId AND EstabelecimentoId = ?EstabelecimentoId";
                    var param = new DynamicParameters();
                    param.Add("?ProcedimentoId", dbType: DbType.String, value: item.ProcedimentoId, direction: ParameterDirection.Input);
                    param.Add("?EstabelecimentoId", dbType: DbType.String, value: item.EstabelecimentoId, direction: ParameterDirection.Input);
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
