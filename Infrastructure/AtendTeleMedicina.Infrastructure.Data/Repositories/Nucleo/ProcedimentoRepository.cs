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
using AtendTeleMedicina.Domain.Params;
using System.Text.RegularExpressions;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class ProcedimentoRepository : BaseRepository, IProcedimentoRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"SELECT CAST(Id AS CHAR) AS Id, 
              Tipo, 
              Codigo, 
              Descricao,
              Sexo,
              IdadeMin,
              IdadeMax,
              Grupo,
              CotaTotal,
              CotaTotalExecutor, 
              CotaEstabelecimento, 
              CotaProfissional, 
              CotaExecutor,
              Valor 
              FROM Procedimento ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"SELECT COUNT(Procedimento.Id) FROM Procedimento");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public ProcedimentoRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public int Add(Procedimento obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?Tipo", dbType: DbType.String, value: obj.Tipo, direction: ParameterDirection.Input);
            param.Add("?Codigo", dbType: DbType.String, value: obj.Codigo, direction: ParameterDirection.Input);
            param.Add("?Descricao", dbType: DbType.String, value: obj.Descricao, direction: ParameterDirection.Input);
            param.Add("?Sexo", dbType: DbType.String, value: obj.Sexo, direction: ParameterDirection.Input);
            param.Add("?CotaTotal", dbType: DbType.Int32, value: 0, direction: ParameterDirection.Input);
            param.Add("?CotaTotalExecutor", dbType: DbType.Int32, value: 0, direction: ParameterDirection.Input);
            param.Add("?CotaEstabelecimento", dbType: DbType.Int32, value: 0, direction: ParameterDirection.Input);
            param.Add("?CotaProfissional", dbType: DbType.Int32, value: 0, direction: ParameterDirection.Input);
            param.Add("?CotaExecutor", dbType: DbType.Int32, value: 0, direction: ParameterDirection.Input);
            param.Add("?Valor", dbType: DbType.Double, value: obj.Valor, direction: ParameterDirection.Input);

            const string insertQuery = @"INSERT INTO Procedimento (Id, Tipo, Codigo, Descricao, Sexo, CotaTotal, CotaTotalExecutor, CotaEstabelecimento, CotaProfissional, CotaExecutor, Valor)
                                                     VALUES (?Id, ?Tipo, ?Codigo, ?Descricao, ?Sexo, ?CotaTotal, ?CotaTotalExecutor, ?CotaEstabelecimento, ?CotaProfissional, ?CotaExecutor, ?Valor)";

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Procedimento", obj.Id.ToString(), "Inseriu Procedimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Procedimento obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Descricao", dbType: DbType.String, value: obj.Descricao, direction: ParameterDirection.Input);
            param.Add("?Sexo", dbType: DbType.String, value: obj.Sexo, direction: ParameterDirection.Input);
            param.Add("?Valor", dbType: DbType.Double, value: obj.Valor, direction: ParameterDirection.Input);
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);

            const string updateQuery = @"UPDATE Procedimento SET
                                  Descricao = ?Descricao, 
                                  Sexo = ?Sexo, 
                                  Valor = ?Valor
                                  WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Procedimento", obj.Id.ToString(), "Editou Procedimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
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
            var query = @"UPDATE Procedimento SET Ativo = 0 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Procedimento", id.ToString(), "Desativou Procedimento", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public Procedimento GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND Procedimento.id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            return _connection.Query<Procedimento>(
                query.ToString(),
                param,
                commandTimeout: TimeOut,
                commandType: CommandType.Text)
              .SingleOrDefault();
        }

        public async Task<(IEnumerable<Procedimento>, int)> GetByParam(Procedimento filters, AppParams appParams, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters?.Id))
                {
                    queryFilter.Append(" AND Id = ?Id");
                    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters?.Tipo))
                {
                    queryFilter.Append(" And Tipo = ?Tipo");
                    param.Add("?Tipo", dbType: DbType.String, value: filters.Tipo, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.Descricao))
                {
                    queryFilter.Append(" AND Descricao LIKE ?Descricao ");
                    param.Add("?Descricao", dbType: DbType.String, value: "%" + filters.Descricao + "%", direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.Codigo))
                {
                    queryFilter.Append(" AND Codigo LIKE ?Codigo");
                    param.Add("?Codigo", dbType: DbType.String, value: "%" + filters.Codigo + "%", direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(appParams?.GruposProcedimento))
                {
                        queryFilter.Append(" AND Grupo IN (" + appParams.GruposProcedimento + ") ");
                }
                if (!string.IsNullOrEmpty(filters?.Sexo) && !filters.Sexo.Equals("*"))
                {
                    var sexoOposto = "F";
                    if (filters.Sexo == "F") sexoOposto = "M";
                    queryFilter.Append(" And Sexo != ?Sexo");
                    param.Add("?Sexo", dbType: DbType.String, value: sexoOposto, direction: ParameterDirection.Input);
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
                var x = await _connection.QueryAsync<Procedimento>(_queryAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);
                return (x, totalCount);
            }
            return (null, 0);
        }

        public void AdicionarCota(string id, int quantidade, string usuario)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();

                // Pegar total de cota atual
                var totalCotaAtual = _connection.QuerySingleOrDefault<int>("SELECT CotaTotal AS TotalCotaAtual FROM Procedimento WHERE Id = ?Id",
                 new { Id = id }, commandTimeout: TimeOut, commandType: CommandType.Text);

                var totalCotaExecutorAtual = _connection.QuerySingleOrDefault<int>("SELECT CotaTotalExecutor AS TotalCotaExecutorAtual FROM Procedimento WHERE Id = ?Id",
                  new { Id = id }, commandTimeout: TimeOut, commandType: CommandType.Text);

                var cotaTotal = totalCotaAtual + quantidade;
                var cotaTotalExecutor = totalCotaExecutorAtual + quantidade;

                #region -- Atualiza CotaTotal em Procedimento
                const string queryProcedimento = @"UPDATE Procedimento SET CotaTotal = ?CotaTotal, CotaTotalExecutor = ?CotaTotalExecutor WHERE Id = ?Id";
                param.RemoveUnused = true;
                param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
                param.Add("?CotaTotal", dbType: DbType.Int32, value: cotaTotal, direction: ParameterDirection.Input);
                param.Add("?CotaTotalExecutor", dbType: DbType.Int32, value: cotaTotalExecutor, direction: ParameterDirection.Input);

                _connection.Execute(queryProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);
                #endregion

                #region -- Adiciona em Procedimento_Historico
                const string queryProcedimentoHistorico = @"INSERT INTO Procedimento_Historico (Id, ProcedimentoId, Data, Quantidade, UsuarioId)
                                                  VALUES(?Id, ?ProcedimentoId, ?Data, ?Quantidade, ?UsuarioId)";
                param.Add("?Id", dbType: DbType.String, value: Guid.NewGuid().ToString(), direction: ParameterDirection.Input);
                param.Add("?ProcedimentoId", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
                param.Add("?Data", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
                param.Add("?Quantidade", dbType: DbType.Int32, value: quantidade, direction: ParameterDirection.Input);
                param.Add("?UsuarioId", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);

                _connection.Execute(queryProcedimentoHistorico, param, commandTimeout: TimeOut, commandType: CommandType.Text);
                #endregion

                transaction.Commit();
            }
        }

        // TODO Movido para EstabelecimentoRepository
        public void DistribuirCota(string procedimentoId, string estabelecimentoId, int quantidade, string usuario)
        {
            var dtAlteracao = DateTime.Now;
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();

                // Pegar total de cota atual
                var totalCotaAtual = _connection.QuerySingleOrDefault<int>("SELECT CotaTotal AS totalCotaAtual FROM Procedimento WHERE Id = ?Id",
                  new { Id = procedimentoId }, commandTimeout: TimeOut, commandType: CommandType.Text);

                if (quantidade > totalCotaAtual) return; //adicionar exception não tem cota disponível para distribuição

                var cotaTotal = totalCotaAtual - quantidade;

                #region -- Atualiza CotaTotal em Procedimento

                const string queryProcedimento = @"UPDATE Procedimento SET CotaTotal = ?CotaTotal, CotaEstabelecimento = CotaEstabelecimento + ?Quantidade WHERE Id = ?Id";
                param.RemoveUnused = true;
                param.Add("?CotaTotal", dbType: DbType.Int32, value: cotaTotal, direction: ParameterDirection.Input);
                param.Add("?Quantidade", dbType: DbType.Int32, value: quantidade, direction: ParameterDirection.Input);
                param.Add("?Id", dbType: DbType.String, value: procedimentoId, direction: ParameterDirection.Input);

                _connection.Execute(queryProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);

                #endregion

                #region -- Atualiza Cota em Estabelecimento_Procedimento

                const string queryEstabelecimentoProcedimento = @"UPDATE Estabelecimento_Procedimento 
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
                  @"INSERT INTO Estabelecimento_Procedimento_Cota_Historico (Id, EstabelecimentoId, ProcedimentoId, Data, Quantidade, UsuarioId)
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
            }
        }

        public void SubtrairCota(string id, int quantidade, string usuario)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();

                // Pegar total de cota atual
                var totalCotaAtual = _connection.QuerySingleOrDefault<int>("SELECT (CotaTotal - CotaEstabelecimento) AS totalCotaAtual FROM Procedimento WHERE Id = ?Id",
                 new { Id = id }, commandTimeout: TimeOut, commandType: CommandType.Text);

                if (quantidade > totalCotaAtual) return; //adicionar exception

                var cotaTotal = totalCotaAtual + quantidade;

                #region -- Atualiza CotaTotal em Procedimento
                const string queryProcedimento = @"UPDATE Procedimento SET CotaTotal = ?CotaTotal Where Id = ?Id";
                param.RemoveUnused = true;
                param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
                param.Add("?CotaTotal", dbType: DbType.Int32, value: cotaTotal, direction: ParameterDirection.Input);

                _connection.Execute(queryProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);

                #endregion
                #region -- Adiciona em Procedimento_Historico
                const string queryProcedimentoHistorico = @"INSERT INTO Procedimento_Historico (Id, ProcedimentoId, Data, Quantidade, UsuarioId)
                                                            VALUES(?Id, ?ProcedimentoId, ?Data, ?Quantidade, ?UsuarioId)";
                param.Add("?Id", dbType: DbType.String, value: Guid.NewGuid().ToString(), direction: ParameterDirection.Input);
                param.Add("?ProcedimentoId", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
                param.Add("?Data", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
                param.Add("?Quantidade", dbType: DbType.Int32, value: -quantidade, direction: ParameterDirection.Input);
                param.Add("?UsuarioId", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);

                _connection.Execute(queryProcedimentoHistorico, param, commandTimeout: TimeOut, commandType: CommandType.Text);
                #endregion

                transaction.Commit();
            }
        }

        public void DistribuirCotaExecutor(string procedimentoId, string estabelecimentoId, int quantidade, string usuario)
        {
            var dtAlteracao = DateTime.Now;
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();
                // Pegar total de cota atual
                var totalCotaExecutorAtual = _connection.QuerySingleOrDefault<int>("SELECT CotaTotalExecutor AS totalCotaExecutorAtual FROM Procedimento WHERE Id = ?Id",
                  new { Id = procedimentoId }, commandTimeout: TimeOut, commandType: CommandType.Text);

                if (quantidade > totalCotaExecutorAtual) return; //adicionar exception não tem cota executor disponível para distribuição

                var cotaTotalExecutor = totalCotaExecutorAtual - quantidade;

                #region -- Atualiza CotaTotal em Procedimento

                const string queryProcedimento = @"UPDATE Procedimento SET CotaTotalExecutor = ?CotaTotalExecutor, CotaExecutor = CotaExecutor + ?Quantidade WHERE Id = ?Id";

                param.RemoveUnused = true;
                param.Add("?CotaTotalExecutor", dbType: DbType.Int32, value: cotaTotalExecutor, direction: ParameterDirection.Input);
                param.Add("?Quantidade", dbType: DbType.Int32, value: quantidade, direction: ParameterDirection.Input);
                param.Add("?Id", dbType: DbType.String, value: procedimentoId, direction: ParameterDirection.Input);

                _connection.Execute(queryProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);

                #endregion

                #region -- Atualiza Cota em Estabelecimento_Procedimento

                const string queryEstabelecimentoProcedimento = @"UPDATE Estabelecimento_Procedimento 
                                                        SET CotaExecutor = CotaExecutor + ?Quantidade
                                                        WHERE EstabelecimentoId = ?EstabelecimentoId AND ProcedimentoId = ?ProcedimentoId";

                param.Add("?Quantidade", dbType: DbType.Int32, value: quantidade, direction: ParameterDirection.Input);
                param.Add("?DtAlteracao", dbType: DbType.DateTime, value: dtAlteracao, direction: ParameterDirection.Input);
                param.Add("?EstabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);
                param.Add("?ProcedimentoId", dbType: DbType.String, value: procedimentoId, direction: ParameterDirection.Input);

                _connection.Execute(queryEstabelecimentoProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);

                #endregion

                #region -- Adiciona em Procedimento_Historico

                //const string queryEstabelecimentoProcedimentoHistorico =
                //  @"INSERT INTO estabelecimento_procedimento_cota_historico (Id, EstabelecimentoId, ProcedimentoId, Data, Quantidade, UserSub)
                //                                                    VALUES(?Id, ?EstabelecimentoId, ?ProcedimentoId, ?Data, ?Quantidade, ?UserSub)";
                //param.Add("?Id", dbType: DbType.String, value: Guid.NewGuid().ToString(), direction: ParameterDirection.Input);
                //param.Add("?EstabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);
                //param.Add("?ProcedimentoId", dbType: DbType.String, value: procedimentoId, direction: ParameterDirection.Input);
                //param.Add("?Data", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
                //param.Add("?Quantidade", dbType: DbType.Int32, value: -quantidade, direction: ParameterDirection.Input);
                //param.Add("?UserSub", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);

                //db.Execute(queryEstabelecimentoProcedimentoHistorico, param, commandType: CommandType.Text);

                #endregion

                transaction.Commit();
            }
        }

        /// <summary>
        /// Atualiza a CotaProfissional do Procedimento
        /// </summary>
        /// <param name="id">Id do Procedimento a ser atualizado</param>
        /// <param name="quantidade">Quantidade positiva ou negativa de cotas</param>
        /// <param name="usuario">Usuário que está fazendo a alteração para ser salvo no histórico</param>
        public void UpdateCotaProfissional(string id, int quantidade, string usuario)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();

                #region -- Atualiza CotaTotal em Procedimento
                const string queryProcedimento = @"UPDATE Procedimento SET CotaProfissional = CotaProfissional + ?Quantidade Where Id = ?Id";
                param.RemoveUnused = true;
                param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
                param.Add("?Quantidade", dbType: DbType.Int32, value: quantidade, direction: ParameterDirection.Input);

                _connection.Execute(queryProcedimento, param, commandTimeout: TimeOut, commandType: CommandType.Text);

                #endregion
                #region -- Adiciona em Procedimento_Historico
                //const string queryProcedimentoHistorico = @"INSERT INTO procedimento_historico (Id, ProcedimentoId, Data, Quantidade, UserSub)
                //                                                    VALUES(?Id, ?ProcedimentoId, ?Data, ?Quantidade, ?UserSub)";
                //param.Add("?Id", dbType: DbType.String, value: Guid.NewGuid().ToString(), direction: ParameterDirection.Input);
                //param.Add("?ProcedimentoId", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
                //param.Add("?Data", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
                //param.Add("?Quantidade", dbType: DbType.Int32, value: -quantidade, direction: ParameterDirection.Input);
                //param.Add("?UserSub", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);

                //db.Execute(queryProcedimentoHistorico, param, commandType: CommandType.Text);
                #endregion
            }
        }

    }
}
