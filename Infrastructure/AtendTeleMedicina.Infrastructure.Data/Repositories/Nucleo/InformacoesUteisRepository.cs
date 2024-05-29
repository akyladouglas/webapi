using Dapper;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
  public class InformacoesUteisRepository : BaseRepository, IInformacoesUteisRepository
    {
    private readonly StringBuilder _queryAll = new StringBuilder(
                                                        @"SELECT CONCAT(Id, '') As Id, i.Titulo, i.Descricao, i.DataCadastro
                                                         FROM InformacoesUteis i ");

    private readonly StringBuilder _queryCountAll = new StringBuilder(
      @"SELECT COUNT(i.Id) FROM InformacoesUteis i ");

    private readonly MySqlConnection _connection;
    private readonly IUser _user;

    public InformacoesUteisRepository(IMySqlContext context, IUser user)
        : base(context)
    {
      _connection = context.Connection;
      _user = user;
    }

    public int Add(InformacoesUteis obj)
    {
      var param = new DynamicParameters();
      param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
      param.Add("?Titulo", obj.Titulo, DbType.String, ParameterDirection.Input);
      param.Add("?Descricao", obj.Descricao, DbType.String, ParameterDirection.Input);
      param.Add("?Ativo", dbType: DbType.Boolean, value: true, direction: ParameterDirection.Input);
      param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);


            const string insertQuery = @"
              INSERT INTO InformacoesUteis (Id, Titulo, Descricao, Ativo, DataCadastro)
                             VALUES (
                                    ?Id,
                                    ?Titulo,
                                    ?Descricao,
                                    ?Ativo,
                                    ?DataCadastro
                                    )";

            using (var transaction = _connection.BeginTransaction())
      {
        var ret = _connection.Execute(insertQuery, param, transaction);
        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "InformacoesUteis", obj.Id.ToString(), "Inseriu Informação Útil", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        transaction.Commit();
        return ret;
      }
    }

    public int Update(string id, InformacoesUteis obj)
    {
      if (string.IsNullOrEmpty(id)) return 0;
      if (obj == null) return 0;

      var param = new DynamicParameters();
      param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
      param.Add("?Titulo", obj.Titulo, DbType.String, ParameterDirection.Input);
      param.Add("?Descricao", obj.Descricao, DbType.String, ParameterDirection.Input);
      param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
      param.Add("?Ativo", obj.Ativo, DbType.Boolean, ParameterDirection.Input);
        


            const string updateQuery = @"UPDATE InformacoesUteis SET 
                            Titulo = ?Titulo, 
                            Descricao = ?Descricao, 
                            Ativo = ?Ativo, 
                            DataAlteracao = ?DataAlteracao
                           WHERE Id = ?Id";

      try
      {
        using (var transaction = _connection.BeginTransaction())
        {
          var user = _user.GetUserId();
          var origem = _user.GetUserOrigem();
          var ip = _user.GetUserIp();

          var ret = _connection.Execute(updateQuery, param, transaction);
          RegistrarAcao(_connection, Guid.NewGuid().ToString(), "InformacoesUteis", obj.Id.ToString(), "Editou InformacoesUteis", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
          transaction.Commit();
          return ret;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public int Delete(string id)
    {

      var query = @"UPDATE InformacoesUteis SET Ativo = 0 WHERE Id = ?Id";
      var param = new DynamicParameters();
      param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

      using (var transaction = _connection.BeginTransaction())
      {
        var ret = _connection.Execute(query, param, transaction);
        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "InformacoesUteis", id.ToString(), "Desativou InformacoesUteis", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        transaction.Commit();
        return ret;
      }
    }

    public InformacoesUteis GetById(string id)
    {
      var query = _queryAll;

      #region -- Filtro
      var queryFilter = new StringBuilder(" Where 1 = 1 ");
      var param = new DynamicParameters();

      if (string.IsNullOrEmpty(id)) return null;

      queryFilter.Append(" AND i.Id = ?Id");
      param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
      query.Append(queryFilter);
      #endregion

      return _connection.Query<InformacoesUteis>(
          query.ToString(),
          param,
          commandTimeout: TimeOut,
          commandType: CommandType.Text)
        .SingleOrDefault();
    }

    public async Task<(IEnumerable<InformacoesUteis>, int)> GetByParam(InformacoesUteis filters, string sort, int? skip, int? take)
    {
      #region -- -- Filtros / Ordenação
      var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
      var param = new DynamicParameters();
      if (filters != null)
      {
        queryFilter.Append(" AND i.Ativo = ?Ativo");
        param.Add("?Ativo", dbType: DbType.Boolean, value: filters.Ativo, direction: ParameterDirection.Input);

        if (!string.IsNullOrEmpty(filters?.Id))
        {
          queryFilter.Append(" AND i.Id = ?Id");
          param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters?.Titulo))
        {
          queryFilter.Append(" AND i.Titulo = ?Titulo");
          param.Add("?Titulo", dbType: DbType.String, value: filters.Titulo, direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters?.Descricao))
        {
          queryFilter.Append(" AND i.Descricao LIKE ?Descricao");
          param.Add("?Descricao", dbType: DbType.String, value: "%" + filters.Descricao + "%", direction: ParameterDirection.Input);
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

      var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

      if (totalCount > 0)
      {
        var x = await _connection.QueryAsync<InformacoesUteis>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);
        return (x, totalCount);
      }
      return (null, 0);
    }
  }
}
