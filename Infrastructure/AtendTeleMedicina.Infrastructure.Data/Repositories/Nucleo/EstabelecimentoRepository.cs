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
  public class EstabelecimentoRepository : BaseRepository, IEstabelecimentoRepository
  {
    private readonly StringBuilder _queryAll = new StringBuilder(
                                                        @"SELECT
                                                        CONCAT(Estabelecimento.Id, '') As Id, 
                                                        Estabelecimento.NomeFantasia, 
                                                        Estabelecimento.Cnpj, 
                                                        Estabelecimento.Cnes, 
                                                        Estabelecimento.ResponsavelCpf, 
                                                        Estabelecimento.RamoAtividade, 
                                                        Estabelecimento.TipoEmpresa, 
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
                                                        Estabelecimento.Ativo,
                                                        Estabelecimento.Tipo,
                                                        Estabelecimento.Latitude, 
                                                        Estabelecimento.Longitude,
                                                        Estabelecimento.Deletado,
                                                        CONCAT(Profissional.Id, '') As Id,
                                                        Profissional.Nome,
                                                        Profissional.Crm,
                                                        Profissional.Email
                                                        FROM
                                                          (SELECT e.Id,
                                                          e.NomeFantasia, 
                                                          e.Cnpj, 
                                                          e.Cnes, 
                                                          e.ResponsavelCpf, 
                                                          e.RamoAtividade,
                                                          e.TipoEmpresa,
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
                                                          e.Ativo,
                                                          e.Tipo,
                                                          e.Latitude, 
                                                          e.Longitude,
                                                          e.Deletado FROM Estabelecimento e ");


    private readonly StringBuilder _queryCountAll = new StringBuilder(
      @"SELECT COUNT(e.Id) FROM Estabelecimento e ");

    private readonly MySqlConnection _connection;
    private readonly IUser _user;

    public EstabelecimentoRepository(IMySqlContext context, IUser user)
        : base(context)
    {
      _connection = context.Connection;
      _user = user;
    }

    public int Add(Estabelecimento obj)
    {
      var param = new DynamicParameters();
      param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
      param.Add("?NomeFantasia", obj.NomeFantasia, DbType.String, ParameterDirection.Input);
      param.Add("?Cnpj", obj.Cnpj, DbType.String, ParameterDirection.Input);
      param.Add("?Cnes", obj.Cnes, DbType.String, ParameterDirection.Input);
      param.Add("?CodIne", obj.CodIne, DbType.String, ParameterDirection.Input);
      param.Add("?ResponsavelCpf", obj.ResponsavelCpf, DbType.String, ParameterDirection.Input);
      param.Add("?RamoAtividade", obj.RamoAtividade, DbType.String, ParameterDirection.Input);  
      param.Add("?TipoEmpresa", obj.TipoEmpresa, DbType.String, ParameterDirection.Input);  
      param.Add("?Telefone1", obj.Telefone1, DbType.String, ParameterDirection.Input);
      param.Add("?Telefone2", obj.Telefone2, DbType.String, ParameterDirection.Input);
      param.Add("?Email", obj.Email, DbType.String, ParameterDirection.Input);
      param.Add("?Cep", obj.Cep, DbType.String, ParameterDirection.Input);
      param.Add("?Uf", obj.Uf, DbType.String, ParameterDirection.Input);
      param.Add("?CodIbgeMun", obj.CodIbgeMun, DbType.String, ParameterDirection.Input);
      param.Add("?Bairro", obj.Bairro, DbType.String, ParameterDirection.Input);
      param.Add("?Logradouro", obj.Logradouro, DbType.String, ParameterDirection.Input);
      param.Add("?Numero", obj.Numero, DbType.String, ParameterDirection.Input);
      param.Add("?Complemento", obj.Complemento, DbType.String, ParameterDirection.Input);
      param.Add("?DataCadastro", obj.DataCadastro, DbType.DateTime, ParameterDirection.Input);
      param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
      param.Add("?Tipo", obj.Tipo, DbType.String, ParameterDirection.Input);
      param.Add("?Deletado", obj.Deletado, DbType.Boolean, ParameterDirection.Input);
      param.Add("?Latitude", obj.Latitude, DbType.Decimal, ParameterDirection.Input);
      param.Add("?Longitude", obj.Longitude, DbType.Decimal, ParameterDirection.Input);

      const string insertQuery = @"INSERT INTO Estabelecimento (Id, NomeFantasia, Cnpj, 
            Cnes, CodIne, ResponsavelCpf, RamoAtividade, TipoEmpresa, Telefone1, Telefone2, Email, Cep, Uf, CodIbgeMun, 
            Bairro, Logradouro, Numero, Complemento, DataCadastro, DataAlteracao, Tipo, Deletado, Latitude, Longitude) 
            VALUES (?Id, ?NomeFantasia, ?Cnpj, 
            ?Cnes, ?CodIne, ?ResponsavelCpf, ?RamoAtividade, ?TipoEmpresa , ?Telefone1, ?Telefone2, ?Email, ?Cep, ?Uf, ?CodIbgeMun, 
            ?Bairro, ?Logradouro, ?Numero, ?Complemento, ?DataCadastro, ?DataAlteracao, ?Tipo, ?Deletado, ?Latitude, ?Longitude)";

      using (var transaction = _connection.BeginTransaction())
      {
        var ret = _connection.Execute(insertQuery, param, transaction);
        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Estabelecimento", obj.Id.ToString(), "Inseriu Estabelecimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        transaction.Commit();
        return ret;
      }
    }

    public int Update(string id, Estabelecimento obj)
    {
      if (string.IsNullOrEmpty(id)) return 0;
      if (obj == null) return 0;

      var param = new DynamicParameters();
      param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
      param.Add("?NomeFantasia", obj.NomeFantasia, DbType.String, ParameterDirection.Input);
      param.Add("?Cnpj", obj.Cnpj, DbType.String, ParameterDirection.Input);
      param.Add("?Cnes", obj.Cnes, DbType.String, ParameterDirection.Input);
      param.Add("?CodIne", obj.CodIne, DbType.String, ParameterDirection.Input);
      param.Add("?ResponsavelCpf", obj.ResponsavelCpf, DbType.String, ParameterDirection.Input);
      param.Add("?RamoAtividade", obj.RamoAtividade, DbType.String, ParameterDirection.Input);  
      param.Add("?TipoEmpresa", obj.TipoEmpresa, DbType.String, ParameterDirection.Input);  
      param.Add("?Telefone1", obj.Telefone1, DbType.String, ParameterDirection.Input);
      param.Add("?Telefone2", obj.Telefone2, DbType.String, ParameterDirection.Input);
      param.Add("?Fax", obj.Fax, DbType.String, ParameterDirection.Input);
      param.Add("?Email", obj.Email, DbType.String, ParameterDirection.Input);
      param.Add("?Cep", obj.Cep, DbType.String, ParameterDirection.Input);
      param.Add("?Uf", obj.Uf, DbType.String, ParameterDirection.Input);
      param.Add("?CodIbgeMun", obj.CodIbgeMun, DbType.String, ParameterDirection.Input);
      param.Add("?Bairro", obj.Bairro, DbType.String, ParameterDirection.Input);
      param.Add("?Logradouro", obj.Logradouro, DbType.String, ParameterDirection.Input);
      param.Add("?Numero", obj.Numero, DbType.String, ParameterDirection.Input);
      param.Add("?Complemento", obj.Complemento, DbType.String, ParameterDirection.Input);
      param.Add("?Tipo", obj.Tipo, DbType.String, ParameterDirection.Input);
      param.Add("?Ativo", obj.Ativo, DbType.Boolean, ParameterDirection.Input);
      param.Add("?Latitude", obj.Latitude, DbType.Decimal, ParameterDirection.Input);
      param.Add("?Longitude", obj.Longitude, DbType.Decimal, ParameterDirection.Input);
      param.Add("?CodIbgeMun", obj.CodIbgeMun, DbType.String, ParameterDirection.Input);
      param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);


            const string updateQuery = @"UPDATE Estabelecimento SET NomeFantasia = ?NomeFantasia, Cnpj = ?Cnpj, Cnes = ?Cnes, CodIne = ?CodIne,
                           ResponsavelCpf = ?ResponsavelCpf, RamoAtividade = ?RamoAtividade, TipoEmpresa = ?TipoEmpresa,
                           Telefone1 = ?Telefone1, Telefone2 = ?Telefone2, Fax = ?Fax, Email = ?Email, Cep = ?Cep,
                           Uf = ?Uf, CodIbgeMun = ?CodIbgeMun, Bairro = ?Bairro, Logradouro = ?Logradouro, 
                           Numero = ?Numero, Complemento = ?Complemento, Tipo = ?Tipo, Ativo = ?Ativo,
                           Latitude = ?Latitude, Longitude = ?Longitude, CodIbgeMun = ?CodIbgeMun, DataAlteracao = ?DataAlteracao
                           WHERE Id = ?Id";

      try
      {
        using (var transaction = _connection.BeginTransaction())
        {
          var ret = _connection.Execute(updateQuery, param, transaction);
          RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Estabelecimento", obj.Id.ToString(), "Editou Estabelecimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
          transaction.Commit();
          return ret;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public int Delete(string id, bool? ativo)
    {
      var query = @"UPDATE Estabelecimento SET Ativo = ?Ativo WHERE Id = ?Id";
      var param = new DynamicParameters();
      param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
      param.Add("?Ativo", dbType: DbType.Boolean, value: ativo, direction: ParameterDirection.Input);


            using (var transaction = _connection.BeginTransaction())
      {
        var ret = _connection.Execute(query, param, transaction);
        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Estabelecimento", id.ToString(), "Editou Estabelecimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
        transaction.Commit();
        return ret;
      }
    }

    public Estabelecimento GetById(string id)
    {
      var query = _queryAll;

      #region -- Filtro
      var queryFilter = new StringBuilder(" Where 1 = 1 ");
      var param = new DynamicParameters();

      if (string.IsNullOrEmpty(id)) return null;

      queryFilter.Append(" AND Estabelecimento.Id = ?Id");
      param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
      query.Append(@" ) AS Estabelecimento
                               LEFT JOIN EstabelecimentoProfissional ON Estabelecimento.Id = EstabelecimentoProfissional.EstabelecimentoId
                               LEFT JOIN Profissional ON Profissional.Id = EstabelecimentoProfissional.ProfissionalId ");
      query.Append(queryFilter);
      #endregion

      var estabelecimento = new Dictionary<string, Estabelecimento>();
      return _connection.Query<Estabelecimento, Profissional, Estabelecimento>(
          query.ToString(), (a, p) =>
          {
            if (!estabelecimento.TryGetValue(a.Id, out var entity))
            {
              estabelecimento.Add(a.Id, entity = a);
            }

            if (entity.Profissionais == null)
            {
              entity.Profissionais = new List<Profissional>();
            }

            if (p == null) return entity;
            if (entity.Profissionais.All(x => x.Id != p.Id))
            {
              entity.Profissionais.Add(p);
            }

            return entity;
          }, param,
          splitOn: "Id, Id",
          commandTimeout: 60,
          commandType: CommandType.Text).FirstOrDefault();
    }

    public async Task<(IEnumerable<Estabelecimento>, int)> GetByParam(Estabelecimento filters, string sort, int? skip, int? take)
    {
      #region -- -- Filtros / Ordenação
      var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
      var param = new DynamicParameters();
      if (filters != null)
      {
        if (!string.IsNullOrEmpty(filters?.Id))
        {
          queryFilter.Append(" AND e.Id = ?Id");
          param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters?.Cnpj))
        {
          queryFilter.Append(" AND e.Cnpj = ?Cnpj");
          param.Add("?Cnpj", dbType: DbType.String, value: filters.Cnpj, direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters?.Cnes))
        {
          queryFilter.Append(" AND e.Cnes LIKE ?Cnes");
          param.Add("?Cnes", dbType: DbType.String, value: "%" + filters.Cnes + "%", direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters?.NomeFantasia))
        {
          queryFilter.Append(" AND e.NomeFantasia LIKE ?NomeFantasia");
          param.Add("?NomeFantasia", dbType: DbType.String, value: "%" + filters.NomeFantasia + "%", direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters?.Tipo))
        {
          queryFilter.Append(" AND e.Tipo IN (" + filters.Tipo + ")");
          param.Add("?Tipo", dbType: DbType.String, value: filters.Tipo, direction: ParameterDirection.Input);
        }
        if (filters.Deletado.HasValue)
        {
          queryFilter.Append(" AND e.Deletado = ?Deletado ");
          param.Add("?Deletado", dbType: DbType.Boolean, value: filters.Deletado, direction: ParameterDirection.Input);
        }

        if (filters.Ativo.HasValue)
        {
          queryFilter.Append(" AND e.Ativo = ?Ativo ");
          param.Add("?Ativo", dbType: DbType.Boolean, value: filters.Ativo, direction: ParameterDirection.Input);
        }
        if (!string.IsNullOrEmpty(filters?.Uf))
        {
          queryFilter.Append(" AND e.Uf LIKE ?Uf");
          param.Add("?Uf", dbType: DbType.String, value: "%" + filters.Uf + "%", direction: ParameterDirection.Input);
        }

        if (!string.IsNullOrEmpty(filters.CodIbgeMun))
        {
          queryFilter.Append(" AND e.CodIbgeMun LIKE ?CodIbgeMun");
          param.Add("?CodIbgeMun", dbType: DbType.String, value: filters.CodIbgeMun, direction: ParameterDirection.Input);
        }

      }
      _queryAll.Append(queryFilter);
      _queryCountAll.Append(queryFilter);

      _queryAll.Append($" ORDER BY {sort} ");
      if (skip.HasValue && take.HasValue)
      {
        _queryAll.Append($" LIMIT ?skip, ?take ");
        skip = (skip - 1) * take;
        param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
        param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
      }
      #endregion

      _queryAll.Append(@" ) AS Estabelecimento
                               LEFT JOIN EstabelecimentoProfissional ON Estabelecimento.Id = EstabelecimentoProfissional.EstabelecimentoId
                               LEFT JOIN Profissional ON Profissional.Id = EstabelecimentoProfissional.ProfissionalId ");

      _queryAll.Append($" ORDER BY {sort.Replace("e.", "Estabelecimento.")} ");

      var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();
      var list = new Dictionary<string, Estabelecimento>();

      if (totalCount > 0)
      {
        await _connection.QueryAsync<Estabelecimento>(_queryAll.ToString(),
            new[]
            {
              typeof(Estabelecimento), typeof(Profissional)
            },
            objects =>
            {
              var estabelecimento = objects[0] as Estabelecimento;
              var profissional = objects[1] as Profissional;

              if (!list.TryGetValue(estabelecimento.Id, out var entity))
              {
                entity = estabelecimento;
                if (entity.Profissionais == null)
                  entity.Profissionais = new List<Profissional>();

                list.Add(estabelecimento.Id, entity);
              }
              if (profissional != null)
              {
                if (!entity.Profissionais.Any(x => x.Id == profissional.Id))
                {
                  entity.Profissionais.Add(profissional);
                }
              }
              return entity;
            }, param, splitOn: "Id, Id", commandTimeout: TimeOut, commandType: CommandType.Text);

        return (list.Values, totalCount);
      }
      return (list.Values, 0);
    }

    public int DistribuirCota(string estabelecimentoId, string procedimentoId,  int quantidade, string usuario)
    {
      var dtAlteracao = DateTime.Now;
      using (var transaction = _connection.BeginTransaction())
      {
        var param = new DynamicParameters();

        // Pegar total de cota atual
        var totalCotaAtual = _connection.QuerySingleOrDefault<int>("SELECT CotaTotal AS totalCotaAtual FROM Procedimento WHERE Id = ?Id",
          new { Id = procedimentoId }, commandTimeout: TimeOut, commandType: CommandType.Text);

        if (quantidade > totalCotaAtual) return 0; //adicionar exception não tem cota disponível para distribuição

        var cotaTotal = totalCotaAtual - quantidade;

        try
        {
          #region -- Atualiza CotaTotal em Procedimento

          const string queryProcedimento = @"UPDATE Procedimento SET CotaTotal = ?CotaTotal, CotaEstabelecimento = CotaEstabelecimento + ?Quantidade WHERE Id = ?Id";
          param.RemoveUnused = true;
          param.Add("?CotaTotal", dbType: DbType.Int32, value: cotaTotal, direction: ParameterDirection.Input);
          param.Add("?Quantidade", dbType: DbType.Int32, value: quantidade, direction: ParameterDirection.Input);
          param.Add("?Id", dbType: DbType.String, value: procedimentoId, direction: ParameterDirection.Input);

          _connection.Execute(queryProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);

          #endregion

          #region -- Atualiza Cota em Estabelecimento_Procedimento

          const string queryEstabelecimentoProcedimento = @"UPDATE EstabelecimentoProcedimento 
                                                        SET Cota = Cota + ?Quantidade WHERE EstabelecimentoId = ?EstabelecimentoId AND ProcedimentoId = ?ProcedimentoId";
          param.RemoveUnused = true;
          param.Add("?Quantidade", dbType: DbType.Int32, value: quantidade, direction: ParameterDirection.Input);
          param.Add("?DtAlteracao", dbType: DbType.DateTime, value: dtAlteracao, direction: ParameterDirection.Input);
          param.Add("?EstabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);
          param.Add("?ProcedimentoId", dbType: DbType.String, value: procedimentoId, direction: ParameterDirection.Input);

          _connection.Execute(queryEstabelecimentoProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);

          #endregion

          #region -- Adiciona em Procedimento_Historico

          const string queryEstabelecimentoProcedimentoHistorico =
            @"INSERT INTO EstabelecimentoProcedimentoCotaHistorico (Id, EstabelecimentoId, ProcedimentoId, Data, Quantidade, UsuarioId)
                                                            VALUES(?Id, ?EstabelecimentoId, ?ProcedimentoId, ?Data, ?Quantidade, ?UsuarioId)";
          param.Add("?Id", dbType: DbType.String, value: Guid.NewGuid().ToString(), direction: ParameterDirection.Input);
          param.Add("?EstabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);
          param.Add("?ProcedimentoId", dbType: DbType.String, value: procedimentoId, direction: ParameterDirection.Input);
          param.Add("?Data", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
          param.Add("?Quantidade", dbType: DbType.Int32, value: -quantidade, direction: ParameterDirection.Input);
          param.Add("?UsuarioId", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);

          _connection.Execute(queryEstabelecimentoProcedimentoHistorico, param, commandTimeout: TimeOut, commandType: CommandType.Text);

          #endregion

          transaction.Commit();
          return 1;
        }
        catch (Exception e)
        {
          throw e;
        }
      }
    }

  }
}
