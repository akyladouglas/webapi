using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
  public class AuditoriaRepository : BaseRepository, IAuditoriaRepository
  {
    private readonly StringBuilder _queryAll = new StringBuilder(
        @"
        SELECT
	        CAST(Auditoria.Id AS CHAR) AS Id, 
	        Auditoria.Tabela, 
	        CAST(Auditoria.TabelaId AS CHAR)  AS TabelaId, 
	        Auditoria.Acao, 
	        Auditoria.DataHora, 
	        Auditoria.Origem, 
            CAST(Auditoria.AtorId AS CHAR) AS AtorId,
	        Auditoria.Ip,
            Auditoria.Justificativa,
			Usuario.Username AS Usuario,
			Profissional.UserName AS Profissional,
			Individuo.Cpf AS Individuo
        FROM
	        Auditoria
					LEFT JOIN Usuario ON Usuario.Id = Auditoria.AtorId
					LEFT JOIN Profissional ON Profissional.Id = Auditoria.AtorId
					LEFT JOIN Individuo ON Individuo.Id = Auditoria.AtorId ");

    private readonly StringBuilder _queryCountAll = new StringBuilder(
        @"SELECT COUNT(1) FROM Auditoria");

    private readonly MySqlConnection _connection;

    public AuditoriaRepository(IMySqlContext context)
        : base(context)
    {
      _connection = context.Connection;
    }
    public Auditoria GetById(string id)
    {
      #region -- Filtro
      var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
      var param = new DynamicParameters();

      if (string.IsNullOrEmpty(id)) return null;

      queryFilter.Append(" AND Auditoria.Id = ?Id");
      param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
      _queryAll.Append(queryFilter);
      #endregion

      return _connection.Query<Auditoria>(
          _queryAll.ToString(),
          param,
          commandTimeout: 60,
          commandType: CommandType.Text
      ).SingleOrDefault();
    }

    public async Task<(IEnumerable<Auditoria>, int)> GetByParam(Auditoria filters, AppParams appParams, string sort, int? skip, int? take)
    {
      #region -- -- Filtros / Ordenação
      var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
      var param = new DynamicParameters();
      if (filters != null || appParams != null)
      {
        if (!string.IsNullOrEmpty(filters.Tabela))
        {
          queryFilter.Append(" AND Auditoria.Tabela = ?Tabela ");
          param.Add("?Tabela", dbType: DbType.String, value: filters.Tabela, direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters.Origem))
        {
          queryFilter.Append(" AND Auditoria.Origem = ?Origem ");
          param.Add("?Origem", dbType: DbType.String, value: filters.Origem, direction: ParameterDirection.Input);
        }
        if (appParams.DataInicial.HasValue)
        {
          DateTime dataFinal = appParams.DataFinal ?? DateTime.Now;
          param.Add("?dataInicial", dbType: DbType.DateTime, value: appParams.DataInicial, direction: ParameterDirection.Input);
          param.Add("?dataFinal", dbType: DbType.DateTime, value: dataFinal, direction: ParameterDirection.Input);
          queryFilter.Append(" AND DATE(Auditoria.DataHora) BETWEEN DATE(?dataInicial) AND DATE(?dataFinal) ");
        }
      }

      _queryAll.Append(queryFilter);
      _queryCountAll.Append(queryFilter);

      if (skip.HasValue && take.HasValue)
      {
        _queryAll.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
        skip = (skip - 1) * take;
        param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
        param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
      }
      #endregion

      var totalCount = _connection.Query<int>(
          _queryCountAll.ToString(),
          param,
          commandTimeout: 60,
          commandType: CommandType.Text
      ).SingleOrDefault();

      if (totalCount > 0)
      {
        var x = await _connection.QueryAsync<Auditoria>(
            _queryAll.ToString(),
            param,
            commandTimeout: 60,
            commandType: CommandType.Text
        );

        return (x, totalCount);
      }
      return (null, 0);
    }

  }
}
