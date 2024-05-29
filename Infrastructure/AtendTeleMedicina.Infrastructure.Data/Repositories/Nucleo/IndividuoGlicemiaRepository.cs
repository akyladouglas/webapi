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
using System.Transactions;
using System.Data.Common;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class IndividuoGlicemiaRepository : BaseRepository, IIndividuoGlicemiaRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
            @"SELECT 
                CAST(ig.Id AS CHAR) AS Id,
                CAST(ig.IndividuoId AS CHAR) AS IndividuoId,
                ig.DataCadastro,
                ig.RespondeuCafe,
                ig.CafeAntes,
                ig.CafeDepois,
                ig.RespondeuAlmoco,
                ig.AlmocoAntes,
                ig.AlmocoDepois,
                ig.RespondeuJanta,
                ig.JantaAntes,
                ig.JantaDepois,
                ig.RespondeuDormirMadrugada,
                ig.AntesDormirMadrugada,
                ig.Observacoes,
                CAST(i.Id AS CHAR) AS Id,
                i.NomeCompleto,
                i.Cpf,
                i.DataNascimento,
                TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
                i.Cns,
                i.Email,
                i.TelefoneCelular,
                i.Sexo
                FROM IndividuoGlicemia ig 
                LEFT JOIN Individuo i ON i.Id = ig.IndividuoId
                ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
            @"SELECT COUNT(ig.Id) FROM IndividuoGlicemia ig");

        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public IndividuoGlicemiaRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public string Add(IndividuoGlicemia obj)
        {
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?RespondeuCafe", dbType: DbType.Boolean, value: obj.RespondeuCafe, direction: ParameterDirection.Input);
            param.Add("?CafeAntes", dbType: DbType.Int32, value: obj.CafeAntes, direction: ParameterDirection.Input);
            param.Add("?CafeDepois", dbType: DbType.Int32, value: obj.CafeDepois, direction: ParameterDirection.Input);
            param.Add("?RespondeuAlmoco", dbType: DbType.Boolean, value: obj.RespondeuAlmoco, direction: ParameterDirection.Input);
            param.Add("?AlmocoAntes", dbType: DbType.Int32, value: obj.AlmocoAntes, direction: ParameterDirection.Input);
            param.Add("?AlmocoDepois", dbType: DbType.Int32, value: obj.AlmocoDepois, direction: ParameterDirection.Input);
            param.Add("?RespondeuJanta", dbType: DbType.Boolean, value: obj.RespondeuJanta, direction: ParameterDirection.Input);
            param.Add("?JantaAntes", dbType: DbType.Int32, value: obj.JantaAntes, direction: ParameterDirection.Input);
            param.Add("?JantaDepois", dbType: DbType.Int32, value: obj.JantaDepois, direction: ParameterDirection.Input);
            param.Add("?RespondeuDormirMadrugada", dbType: DbType.Boolean, value: obj.RespondeuDormirMadrugada, direction: ParameterDirection.Input);
            param.Add("?AntesDormirMadrugada", dbType: DbType.Int32, value: obj.AntesDormirMadrugada, direction: ParameterDirection.Input);
            param.Add("?Observacoes", dbType: DbType.String, value: obj.Observacoes, direction: ParameterDirection.Input);
          
           
            const string insertQuery = @"INSERT INTO IndividuoGlicemia (Id, IndividuoId, DataCadastro, RespondeuCafe, CafeAntes, CafeDepois, RespondeuAlmoco, AlmocoAntes, AlmocoDepois, RespondeuJanta,JantaAntes, JantaDepois, RespondeuDormirMadrugada, AntesDormirMadrugada, Observacoes)
                                                     VALUES (?Id, ?IndividuoId, ?DataCadastro, ?RespondeuCafe, ?CafeAntes, ?CafeDepois, ?RespondeuAlmoco, ?AlmocoAntes, ?AlmocoDepois, ?RespondeuJanta, ?JantaAntes, ?JantaDepois, ?RespondeuDormirMadrugada, ?AntesDormirMadrugada, ?Observacoes)";

            using (var transaction = _connection.BeginTransaction())
            {
                _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoGlicemia", obj.Id.ToString(), "Inseriu IndividuoGlicemia", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return obj.Id;
            }
        }

        public string Update(string id, IndividuoGlicemia obj)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (obj == null) return null;
            obj.Id = id;
            
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?IndividuoId", dbType: DbType.String, value: obj.IndividuoId, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?RespondeuCafe", dbType: DbType.Boolean, value: obj.RespondeuCafe, direction: ParameterDirection.Input);
            param.Add("?CafeAntes", dbType: DbType.Int32, value: obj.CafeAntes, direction: ParameterDirection.Input);
            param.Add("?CafeDepois", dbType: DbType.Int32, value: obj.CafeDepois, direction: ParameterDirection.Input);
            param.Add("?RespondeuAlmoco", dbType: DbType.Boolean, value: obj.RespondeuAlmoco, direction: ParameterDirection.Input);
            param.Add("?AlmocoAntes", dbType: DbType.Int32, value: obj.AlmocoAntes, direction: ParameterDirection.Input);
            param.Add("?AlmocoDepois", dbType: DbType.Int32, value: obj.AlmocoDepois, direction: ParameterDirection.Input);
            param.Add("?RespondeuJanta", dbType: DbType.Boolean, value: obj.RespondeuJanta, direction: ParameterDirection.Input);
            param.Add("?JantaAntes", dbType: DbType.Int32, value: obj.JantaAntes, direction: ParameterDirection.Input);
            param.Add("?JantaDepois", dbType: DbType.Int32, value: obj.JantaDepois, direction: ParameterDirection.Input);
            param.Add("?RespondeuDormirMadrugada", dbType: DbType.Boolean, value: obj.RespondeuDormirMadrugada, direction: ParameterDirection.Input);
            param.Add("?AntesDormirMadrugada", dbType: DbType.Int32, value: obj.AntesDormirMadrugada, direction: ParameterDirection.Input);
            param.Add("?Observacoes", dbType: DbType.String, value: obj.Observacoes, direction: ParameterDirection.Input);



            const string updateQuery = @"UPDATE IndividuoGlicemia SET
                                IndividuoId = ?IndividuoId, 
                                DataCadastro = ?DataCadastro,
                                RespondeuCafe = ?RespondeuCafe,
                                CafeAntes = ?CafeAntes, 
                                CafeDepois = ?CafeDepois, 
                                RespondeuAlmoco = ?RespondeuAlmoco, 
                                AlmocoAntes = ?AlmocoAntes, 
                                AlmocoDepois = ?AlmocoDepois, 
                                RespondeuJanta = ?RespondeuJanta,
                                JantaAntes = ?JantaAntes, 
                                JantaDepois = ?JantaDepois, 
                                RespondeuDormirMadrugada = ?RespondeuDormirMadrugada,
                                AntesDormirMadrugada = ?AntesDormirMadrugada,
                                Observacoes = ?Observacoes
                                WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var user = _user.GetUserId();
                    var origem = _user.GetUserOrigem();
                    var ip = _user.GetUserIp();

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "IndividuoGlicemia", obj.Id.ToString(), "Editou IndividuoGlicemia", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return obj.Id;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IndividuoGlicemia GetById(string id)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND ig.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var individuoGlicemia = new IndividuoGlicemia();
            _connection.QueryAsync<IndividuoGlicemia, Individuo, IndividuoGlicemia>(
                _queryAll.ToString(), (ig, i) =>
                {
                    individuoGlicemia = ig;
                    individuoGlicemia.Individuo = i;

                    return individuoGlicemia;
                },
                param,
                splitOn: "Id, Id",
                commandTimeout: TimeOut,
                commandType: CommandType.Text);

            return (individuoGlicemia);
        }

        public async Task<(IEnumerable<IndividuoGlicemia>, int)> GetByParam(IndividuoGlicemia filters, string sort, int? skip, int? take)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();
            if (filters != null)
            {
               
                    if (!string.IsNullOrEmpty(filters?.Id))
                    {
                        queryFilter.Append(" AND ig.Id = ?Id");
                        param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(filters?.IndividuoId))
                    {
                        queryFilter.Append(" And ig.IndividuoId = ?IndividuoId");
                        param.Add("?IndividuoId", dbType: DbType.String, value: filters.IndividuoId, direction: ParameterDirection.Input);
                    }
                    if (filters.DataCadastro != DateTime.MinValue)
                    {
                        queryFilter.Append(" And DATE(ig.DataCadastro) = DATE(?DataCadastro)");
                        param.Add("?DataCadastro", dbType: DbType.DateTime, value: filters.DataCadastro.Date, direction: ParameterDirection.Input);
                    }
                    if (filters.DataInicial != null && filters.DataFinal != null)
                    {
                        filters.DataFinal = filters.DataFinal ?? DateTime.Now;

                        queryFilter.Append(" AND DATE(ig.DataCadastro) BETWEEN DATE(?DataInicial) AND DATE(?DataFinal)");
                        param.Add("?DataInicial", dbType: DbType.Date, value: filters.DataInicial, direction: ParameterDirection.Input);
                        param.Add("?DataFinal", dbType: DbType.Date, value: filters.DataFinal, direction: ParameterDirection.Input);
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
                var individuosGlicemia = new Dictionary<string, IndividuoGlicemia>();
                await _connection.QueryAsync<IndividuoGlicemia, Individuo, IndividuoGlicemia>(
                    _queryAll.ToString(), (ig, i) => {
                        if (!individuosGlicemia.TryGetValue(ig.Id, out var individuoGlicemiaEntity))
                        {
                            individuosGlicemia.Add(ig.Id, individuoGlicemiaEntity = ig);
                        }
                        individuoGlicemiaEntity.Individuo = i;

                        return individuoGlicemiaEntity;
                    },
                    param,
                    splitOn: "Id, Id",
                    commandTimeout: TimeOut, 
                    commandType: CommandType.Text);
                return (individuosGlicemia.Values, totalCount);
            }
            return (null, 0);
        }

    }
}
