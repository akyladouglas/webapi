using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Text;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using AtendTeleMedicina.Domain.Contracts.Repositories.Integracao;
using AtendTeleMedicina.Domain.Entities.Integracao;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Infra.Data.Repositories.Integracao
{
    public class LoteRepository : BaseRepository, ILoteRepository
    {
        private readonly StringBuilder _query = new StringBuilder(@"SELECT  
                        Lote.Id, Lote.Mes, Lote.Ano, CAST(Lote.Usuario_Id AS CHAR) AS Usuario_Id, Lote.Data, CAST(Lote.Estabelecimento_Id AS CHAR) AS Estabelecimento_Id,
                        CAST(Estabelecimento.Id AS CHAR) AS Id, Estabelecimento.NomeFantasia, Estabelecimento.Cnes
                        FROM Lote
                        LEFT JOIN Estabelecimento ON(Estabelecimento.Id = Lote.Estabelecimento_Id)
                        WHERE 0 = 0 ");

        private readonly IUser _user;
        private readonly MySqlConnection _connection;

        public LoteRepository(IMySqlContext context, IUser user) : base(context)
        {
            _user = user;
            _connection = context.Connection;
        }

        public Lote ExisteLote(int mes, int ano, string estabelecimentoId)
        {
            var query = new StringBuilder("");
            query.Append(_query);
            var param = new DynamicParameters();

            query.Append(" AND Lote.Mes = @Mes AND Lote.Ano = @Ano ");
            param.Add("?Mes", dbType: DbType.Int32, value: mes, direction: ParameterDirection.Input);
            param.Add("?Ano", dbType: DbType.Int32, value: ano, direction: ParameterDirection.Input);

            query.Append("AND Lote.Estabelecimento_Id = ?EstabelecimentoId ");
            param.Add("?EstabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);

            var item = _connection.Query<Lote, Estabelecimento, Lote>(query.ToString(),
                    (l, e) =>
                    {
                        l.Estabelecimento = e;
                        return l;
                    }, param, commandTimeout: 0).SingleOrDefault();
            return item ?? BuscarProximoLote();
        }

        public Lote GetById(int id)
        {
            var query = new StringBuilder("");
            query.Append(_query);
            var param = new DynamicParameters();

            query.Append(" AND Lote.Id = ?Id ");
                param.Add("?Id", dbType: DbType.Int32, value: id, direction: ParameterDirection.Input);
                var obj = _connection.Query<Lote, Estabelecimento, Lote>(query.ToString(),
                    (l, e) =>
                    {
                        l.Estabelecimento = e;
                        return l;
                    }, param, commandTimeout: 0).SingleOrDefault();
                ListarItem(obj);
                return obj;
        }

        public Lote GetDetailedById(int id)
        {
            //var query = new StringBuilder(@"
            //            SELECT  
            //            Lote.Id, Lote.Mes, Lote.Ano, CAST(Lote.Usuario_Id AS CHAR) AS Usuario_Id, Lote.Data, CAST(Lote.Estabelecimento_Id AS CHAR) AS Estabelecimento_Id,
            //            CAST(LoteIntegracao.Cadastro_Id AS CHAR) Cadastro_Id, CAST(LoteIntegracao.Lote_Id AS CHAR) AS Lote_Id, LoteIntegracao.DataGeracao, LoteIntegracao.Erros, LoteIntegracao.Tipo,                         
            //            CAST(Estabelecimento.Id AS CHAR) AS Id, Estabelecimento.NomeFantasia, Estabelecimento.Cnes
            //            FROM Lote
            //            INNER JOIN LoteIntegracao ON LoteIntegracao.Lote_Id = Lote.Id
            //            LEFT JOIN Estabelecimento ON Estabelecimento.Id = Lote.Estabelecimento_Id
            //            WHERE 0 = 0 ");
            //var param = new DynamicParameters();

            //query.Append(" AND Lote.Id = ?Id ");
            //param.Add("?Id", dbType: DbType.Int32, value: id, direction: ParameterDirection.Input);
            //var lote = _connection.Query<Lote, List<LoteIntegracao>, Estabelecimento, Lote>(query.ToString(),
            //    (l, li, e) =>
            //    {
            //        l.Estabelecimento = e;
            //        l.LoteIntegracao = li;
            //        return l;
            //    }, param, splitOn: "Lote_Id, Id", commandTimeout: 0).FirstOrDefault();
            //return lote;


            var query = new StringBuilder(@"
                        SELECT  
                        Lote.Id, Lote.Mes, Lote.Ano, CAST(Lote.Usuario_Id AS CHAR) AS Usuario_Id, Lote.Data, CAST(Lote.Estabelecimento_Id AS CHAR) AS Estabelecimento_Id,                   
                        CAST(Estabelecimento.Id AS CHAR) AS Id, Estabelecimento.NomeFantasia, Estabelecimento.Cnes
                        FROM Lote
                        LEFT JOIN Estabelecimento ON Estabelecimento.Id = Lote.Estabelecimento_Id
                        WHERE 0 = 0 ");
            var param = new DynamicParameters();

            query.Append(" AND Lote.Id = ?Id ");
            param.Add("?Id", dbType: DbType.Int32, value: id, direction: ParameterDirection.Input);
            var lote = _connection.Query<Lote, Estabelecimento, Lote>(query.ToString(),
                (l, e) =>
                {
                    l.Estabelecimento = e;
                    return l;
                }, param, commandTimeout: 0).FirstOrDefault();

            if (lote != null)
                ListarItem(lote);

            return lote;

        }

        private void ListarItem(Lote lote)
        {
            if (lote == null)
                return;
            
                var param = new DynamicParameters();
                const string _query = "SELECT * FROM LoteIntegracao WHERE Lote_Id = @Lote_Id";
                param.Add("?Lote_Id", dbType: DbType.Int32, value: lote.Id, direction: ParameterDirection.Input);

                var loteIntegracao = _connection.Query<LoteIntegracao>(_query, param, commandTimeout: 0).ToList();

                lote.AdicionarItems(loteIntegracao);
        }

        public async Task<(IEnumerable<Lote>, int)> ListarLote(int mes, int ano, string cnes, int? skip = 1, int? take = 50)
        {
            var queryCount = new StringBuilder(@"
                SELECT  COUNT(1)   
                FROM Lote
                LEFT JOIN Estabelecimento ON (Estabelecimento.Id = Lote.Estabelecimento_Id)
                WHERE 0 = 0 ");

            var query = new StringBuilder(@"
                SELECT  
                Lote.Id, Lote.Mes, Lote.Ano, CAST(Lote.Usuario_Id AS CHAR) AS Usuario_Id, Lote.Data, CAST(Lote.Estabelecimento_Id AS CHAR) Estabelecimento_Id,
                (SELECT COUNT(1) FROM LoteIntegracao WHERE LoteIntegracao.Lote_Id = Lote.Id AND LoteIntegracao.Status = 1) AS Validos,
                (SELECT COUNT(1) FROM LoteIntegracao WHERE LoteIntegracao.Lote_Id = Lote.Id AND LoteIntegracao.Status = 0) AS Erros,
                CAST(Estabelecimento.Id AS CHAR) AS Id, Estabelecimento.NomeFantasia, Estabelecimento.Cnes     
                FROM Lote
                LEFT JOIN Estabelecimento ON (Estabelecimento.Id = Lote.Estabelecimento_Id)
                WHERE 0 = 0 ");

            var param = new DynamicParameters();
            if (!string.IsNullOrEmpty(cnes))
            {
                query.Append(" AND Lote.Estabelecimento_Id = ?cnes ");
                param.Add("?cnes", dbType: DbType.Int32, value: cnes, direction: ParameterDirection.Input);
            }

            query.Append(" AND Lote.Mes = @Mes AND Lote.Ano = @Ano ");
            param.Add("?mes", dbType: DbType.Int32, value: mes, direction: ParameterDirection.Input);
            param.Add("?ano", dbType: DbType.Int32, value: ano, direction: ParameterDirection.Input);

            query.Append(" ORDER BY Lote.Id DESC ");

            if (skip.HasValue && take.HasValue)
            {
                query.Append($" LIMIT ?skip, ?take ");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }

            var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                try
                {
                    var lotes = new Dictionary<int, Lote>();
                    await _connection.QueryAsync<Lote, Estabelecimento, Lote>(
                        query.ToString(), (lote, cidade) =>
                        {
                            if (!lotes.TryGetValue(lote.Id, out var entity))
                            {
                                lotes.Add(lote.Id, entity = lote);
                            }

                            entity.Estabelecimento = cidade;

                            return entity;
                        }, param,
                        commandTimeout: 0,
                        commandType: CommandType.Text);

                    return (lotes.Values, totalCount);
                }
                catch (System.Exception e)
                {
                    throw e;
                }
            }
            return (null, 0);
        }

        public void AtualizarOuInserir(Lote entity)
        {
            try
            {
                const string query = " REPLACE INTO Lote " +
                "(Id, Data, Usuario_Id, Mes, Ano, Estabelecimento_Id) VALUES (@Id, @Data, @Usuario_Id, @Mes, @Ano, @Estabelecimento_Id) ";
                _connection.Execute(query, entity, commandTimeout: 0);
                RegistrarLoteIntegracao(entity.Id, entity.LoteIntegracao);
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao Salvar Lote", e);
            }
        }

        public void Adicionar(Lote entity)
        {
            try
            {
                const string query = " INSERT INTO Lote " +
                "(Id, Data, Usuario_Id, Mes, Ano, Estabelecimento_Id) VALUES (@Id, @Data, @Usuario_Id, @Mes, @Ano, @Estabelecimento_Id) ";
                _connection.Execute(query, entity, commandTimeout: 0);
                RegistrarLoteIntegracao(entity.Id, entity.LoteIntegracao);
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao Salvar Lote", e);
            }
        }

        public void Atualizar(Lote entity)
        {
            try
            {
                const string query = " UPDATE Lote " +
                " SET Id = @Id, Data = @Data, Usuario_Id = @Usuario_Id, Mes = @Mes, Ano = @Ano, Estabelecimento_Id = @Estabelecimento_Id WHERE Id = @Id ";
                _connection.Execute(query, entity, commandTimeout: 0);
                RegistrarLoteIntegracao(entity.Id, entity.LoteIntegracao);
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao Salvar Lote", e);
            }
        }

        public bool Existe(int id)
        {
            const string query = "SELECT COUNT(1) FROM Lote WHERE Id = @Id";
            var lote = _connection.ExecuteScalar<int>(query, new { Id = id }, commandTimeout: 0);
            var loteExiste = lote > 0;
            return loteExiste;
        }

        #region Private Methods
        private Lote BuscarProximoLote()
        {
            const string query = "SELECT MAX(Lote.`Id`) Id FROM Lote ";
            var id = _connection.ExecuteScalar<int>(query, commandTimeout: 0) + 1;
            return new Lote(id);
        }

        private void RegistrarLoteIntegracao(int id, IEnumerable<LoteIntegracao> loteIntegracao)
        {
            try
            {
                RemoverLoteItens(id);
                const string query = " REPLACE INTO LoteIntegracao " +
                    "(Lote_Id, Cadastro_Id, DataGeracao, DadosXml, Erros, Tipo, Status) VALUES " +
                    "(@Lote_Id, @Cadastro_Id, @DataGeracao, @DadosXml, @Erros, @Tipo, @Status)";
                _connection.Execute(query, loteIntegracao, commandTimeout: 0);
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao Salvar LoteIntegracao", e);
            }
        }

        private void RemoverLoteItens(int id)
        {
            try
            {
                const string query = "DELETE FROM LoteIntegracao WHERE Lote_Id = @Lote_Id";
                _connection.Execute(query, new { Lote_Id = id }, commandTimeout: 0);
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao Remover LoteIntegracao", e);
            }
        }

        #endregion

    }
}
