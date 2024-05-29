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
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class ProfissionalRepository : BaseRepository, IProfissionalRepository
    {
        //private readonly StringBuilder _queryAll = new StringBuilder(
        //@"SELECT
        //        DISTINCT CAST(Profissional.Id AS CHAR) AS Id,
        //        Profissional.Nome,
        //        Profissional.Cpf,
        //        Profissional.Cns,
        //        Profissional.CodigoAutenticacao,
        //        Profissional.DataNascimento,
        //        Profissional.Sexo,
        //        Profissional.Crm,
        //        Profissional.Username,
        //        Profissional.Email,
        //        Profissional.Senha,
        //        Profissional.Telefone,
        //        Profissional.Logradouro,
        //        Profissional.LogradouroNumero,
        //        Profissional.LogradouroComplemento,
        //        Profissional.LogradouroBairro,
        //        Profissional.LogradouroCep,
        //        Profissional.CidadeId,
        //        Profissional.UfAbreviado,
        //        Profissional.DataCadastro,
        //        Profissional.DataAlteracao,
        //        Profissional.Ativo,
        //        Profissional.Aceite,
        //        Profissional.Token,
        //        Profissional.Imagem,
        //        Profissional.OcupacaoId,
        //        Profissional.UltimoPerfilSelecionado, 
        //        Cidade.Ibge, 
        //        Cidade.Nome, 
        //        Cidade.UfAbreviado, 
        //        Cidade.UfExtenso,
        //        CAST(UsuarioClaim.Id AS CHAR) AS Id,
        //        UsuarioClaim.ProfissionalId,
        //        UsuarioClaim.ClaimType,
        //        UsuarioClaim.ClaimValue
        //        FROM
        //        Profissional 
        //        LEFT JOIN Cidade ON Cidade.Ibge = Profissional.CidadeId 
        //        LEFT JOIN UsuarioClaim ON UsuarioClaim.ProfissionalId = Profissional.Id
        //        LEFT JOIN EstabelecimentoProcedimentoHorario ON EstabelecimentoProcedimentoHorario.ProfissionalId = Profissional.Id ");
        
        private readonly StringBuilder _queryAll = new StringBuilder(
        @"SELECT
                DISTINCT CAST(Profissional.Id AS CHAR) AS Id,
                Profissional.Nome,
                Profissional.Cpf,
                Profissional.Cns,
                Profissional.CodigoAutenticacao,
                Profissional.DataNascimento,
                Profissional.Sexo,
                Profissional.Crm,                
                Profissional.CrmUF,
                Profissional.Conselho,
                Profissional.Username,
                Profissional.Email,
                Profissional.Senha,
                Profissional.Telefone,
                Profissional.Logradouro,
                Profissional.LogradouroNumero,
                Profissional.LogradouroComplemento,
                Profissional.LogradouroBairro,
                Profissional.LogradouroCep,
                Profissional.CidadeId,
                Profissional.UfAbreviado,
                Profissional.DataCadastro,
                Profissional.DataAlteracao,
                Profissional.Ativo,
                Profissional.Aceite,
                Profissional.Token,
                Profissional.Imagem,
                Profissional.OcupacaoId,
                Profissional.UltimoPerfilSelecionado, 
                Cidade.Ibge, 
                Cidade.Nome, 
                Cidade.UfAbreviado, 
                Cidade.UfExtenso,
                CAST(UsuarioClaim.Id AS CHAR) AS Id,
                UsuarioClaim.ProfissionalId,
                UsuarioClaim.ClaimType,
                UsuarioClaim.ClaimValue
                FROM
                Profissional 
                LEFT JOIN Cidade ON Cidade.Ibge = Profissional.CidadeId 
                LEFT JOIN UsuarioClaim ON UsuarioClaim.ProfissionalId = Profissional.Id
                LEFT JOIN EstabelecimentoProcedimentoHorario ON EstabelecimentoProcedimentoHorario.ProfissionalId = Profissional.Id ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"Select Count(DISTINCT(Profissional.Id)) From Profissional 
            LEFT JOIN EstabelecimentoProcedimentoHorario ON EstabelecimentoProcedimentoHorario.ProfissionalId = Profissional.Id ");
        private readonly MySqlConnection _connection;
        private readonly IUser _user;

        public ProfissionalRepository(IMySqlContext context, IUser user)
                : base(context)
        {
            _user = user;
            _connection = context.Connection;
        }

        public int Add(Profissional obj)
        {
            if (!string.IsNullOrEmpty(obj.Cpf) && CheckCpf(obj.Cpf)) throw new Exception("CPF já cadastrado.");
            if (!string.IsNullOrEmpty(obj.Crm) && CheckCrm(obj.Id, obj.Crm)) throw new Exception("CRM já cadastrado.");
            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?Nome", obj.Nome, DbType.String, ParameterDirection.Input);
            param.Add("?Username", obj.Username, DbType.String, ParameterDirection.Input);
            param.Add("?Senha", obj.Senha, DbType.String, ParameterDirection.Input);
            param.Add("?Cpf", obj.Cpf, DbType.String, ParameterDirection.Input);
            param.Add("?Cns", obj.Cns, DbType.String, ParameterDirection.Input);
            param.Add("?DataNascimento", obj.DataNascimento, DbType.DateTime, ParameterDirection.Input);
            param.Add("?Sexo", obj.Sexo, DbType.String, ParameterDirection.Input);
            param.Add("?Crm", obj.Crm, DbType.String, ParameterDirection.Input);
            param.Add("?CrmUF", obj.CrmUF, DbType.String, ParameterDirection.Input);
            param.Add("?Conselho", obj.Conselho, DbType.String, ParameterDirection.Input);
            param.Add("?Email", obj.Email, DbType.String, ParameterDirection.Input);
            param.Add("?Telefone", obj.Telefone, DbType.String, ParameterDirection.Input);
            param.Add("?Logradouro", obj.Logradouro, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroNumero", obj.LogradouroNumero, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroComplemento", obj.LogradouroComplemento, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroBairro", obj.LogradouroBairro, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroCep", obj.LogradouroCep, DbType.String, ParameterDirection.Input);
            param.Add("?CidadeId", obj.CidadeId, DbType.Int32, ParameterDirection.Input);
            param.Add("?UfAbreviado", obj.UfAbreviado, DbType.String, ParameterDirection.Input);
            param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
            param.Add("?DataCadastro", obj.DataCadastro, DbType.DateTime, ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);
            param.Add("?OcupacaoId", dbType: DbType.Int32, value: obj.OcupacaoId, direction: ParameterDirection.Input);


            const string insertQuery = @"
                  INSERT INTO Profissional(Id, Nome, Username, Senha, Cpf, Cns, DataNascimento, Sexo, Crm, CrmUF, Conselho, Email, Telefone, Logradouro, LogradouroNumero,
                                           LogradouroComplemento, LogradouroBairro, LogradouroCep, CidadeId, UfAbreviado, DataCadastro, DataAlteracao, Ativo, OcupacaoId)
                                   VALUES(?Id, ?Nome, ?Username, ?Senha, ?Cpf, ?Cns, ?DataNascimento, ?Sexo, ?Crm, ?CrmUF, ?Conselho, ?Email, ?Telefone, ?Logradouro, ?LogradouroNumero,
                                           ?LogradouroComplemento, ?LogradouroBairro, ?LogradouroCep, ?CidadeId, ?UfAbreviado, ?DataCadastro, ?DataAlteracao, ?Ativo, ?OcupacaoId)";

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", obj.Id.ToString(), "Inseriu Profissional", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                foreach (var item in obj.UserClaims)
                    item.ProfissionalId = obj.Id;

                var addClaims = "INSERT INTO UsuarioClaim (Id, ProfissionalId, ClaimType, ClaimValue) VALUES (@Id, @ProfissionalId, @ClaimType, @ClaimValue)";
                _connection.Execute(addClaims, obj.UserClaims, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "UsuarioClaim", obj.Id.ToString(), "Inseriu as Claims do Profissional.", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Profissional obj)
        {
            if (!string.IsNullOrEmpty(obj.Crm) && CheckCrm(obj.Id, obj.Crm)) throw new Exception("CRM já cadastrado.");
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var profissional = GetById(obj.Id);

            if (String.IsNullOrEmpty(obj.Imagem))
            {
                obj.Imagem = profissional.Imagem;
            }

            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?Nome", obj.Nome, DbType.String, ParameterDirection.Input);
            param.Add("?Cpf", obj.Cpf, DbType.String, ParameterDirection.Input);
            param.Add("?Cns", obj.Cns, DbType.String, ParameterDirection.Input);
            param.Add("?DataNascimento", obj.DataNascimento, DbType.DateTime, ParameterDirection.Input);
            param.Add("?Sexo", obj.Sexo, DbType.String, ParameterDirection.Input);
            param.Add("?Crm", obj.Crm, DbType.String, ParameterDirection.Input);
            param.Add("?CrmUF", obj.CrmUF, DbType.String, ParameterDirection.Input);
            param.Add("?Conselho", obj.Conselho, DbType.String, ParameterDirection.Input);
            param.Add("?Email", obj.Email, DbType.String, ParameterDirection.Input);
            param.Add("?Senha", obj.Senha, DbType.String, ParameterDirection.Input);
            param.Add("?Telefone", obj.Telefone, DbType.String, ParameterDirection.Input);
            param.Add("?Logradouro", obj.Logradouro, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroNumero", obj.LogradouroNumero, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroComplemento", obj.LogradouroComplemento, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroBairro", obj.LogradouroBairro, DbType.String, ParameterDirection.Input);
            param.Add("?LogradouroCep", obj.LogradouroCep, DbType.String, ParameterDirection.Input);
            param.Add("?CidadeId", obj.CidadeId, DbType.Int32, ParameterDirection.Input);
            param.Add("?UfAbreviado", obj.UfAbreviado, DbType.String, ParameterDirection.Input);
            param.Add("?DataAlteracao", obj.DataAlteracao, DbType.DateTime, ParameterDirection.Input);
            param.Add("?DataCadastro", obj.DataCadastro, DbType.DateTime, ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);
            param.Add("?CadastroEditado", dbType: DbType.Boolean, value: obj.CadastroEditado, direction: ParameterDirection.Input);
            param.Add("?DescricaoEditado", dbType: DbType.String, value: obj.DescricaoEditado, direction: ParameterDirection.Input);
            param.Add("?UsuarioEditou", dbType: DbType.String, value: obj.UsuarioEditou, direction: ParameterDirection.Input);
            param.Add("?TentativaLogin", dbType: DbType.Int32, value: 0, direction: ParameterDirection.Input);
            param.Add("?Imagem", dbType: DbType.String, value: obj.Imagem, direction: ParameterDirection.Input);
            param.Add("?OcupacaoId", dbType: DbType.Int32, value: obj.OcupacaoId, direction: ParameterDirection.Input);
            param.Add("?UltimoPerfilSelecionado", dbType: DbType.String, value: obj.UltimoPerfilSelecionado, direction: ParameterDirection.Input);

            const string updateQuery = @"
              UPDATE Profissional SET
              Nome = ?Nome,
              Cpf = ?Cpf,
              Cns = ?Cns,
              DataNascimento = ?DataNascimento,
              Sexo = ?Sexo,
              Crm = ?Crm,             
              CrmUF = ?CrmUF,
              Conselho = ?Conselho,
              Email = ?Email,
              Senha = ?Senha,
              Telefone = ?Telefone,
              Logradouro = ?Logradouro,
              LogradouroNumero = ?LogradouroNumero,
              LogradouroComplemento = ?LogradouroComplemento,
              LogradouroBairro = ?LogradouroBairro,
              LogradouroCep = ?LogradouroCep,
              CidadeId = ?CidadeId,
              UfAbreviado = ?UfAbreviado,
              DataCadastro = ?DataCadastro,
              DataAlteracao = ?DataAlteracao,
              Ativo = ?Ativo,
              CadastroEditado = ?CadastroEditado,
              DescricaoEditado = ?DescricaoEditado,
              UsuarioEditou = ?UsuarioEditou,
              TentativaLogin = ?TentativaLogin,
              Imagem = ?Imagem,
              OcupacaoId = ?OcupacaoId,
              UltimoPerfilSelecionado = ?UltimoPerfilSelecionado
              WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    if (obj.UserClaims.Count > 0)
                    {
                        var delClaims = "DELETE FROM UsuarioClaim WHERE ProfissionalId = ?Id";
                        _connection.Execute(delClaims, param, transaction);

                        foreach (var item in obj.UserClaims)
                        {
                            item.Id = Guid.NewGuid().ToString();
                            item.ProfissionalId = obj.Id;
                        }

                        var addClaims = "INSERT INTO UsuarioClaim (Id, ProfissionalId, ClaimType, ClaimValue) VALUES (@Id, @ProfissionalId, @ClaimType, @ClaimValue)";
                        _connection.Execute(addClaims, obj.UserClaims, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", obj.Id.ToString(), "Adicionou Claims ao Profissional", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    }
                    var ret = _connection.Execute(updateQuery, param, transaction);
                    if (!string.IsNullOrEmpty(_user.GetUserId()))
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", obj.Id.ToString(), "Atualizou Profissional", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateFoto(string id, Profissional obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", obj.Id, DbType.String, ParameterDirection.Input);
            param.Add("?Imagem", dbType: DbType.String, value: obj.Imagem, direction: ParameterDirection.Input);

            const string updateQuery = @"
              UPDATE Profissional SET
              Imagem = ?Imagem
              WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", obj.Id.ToString(), "Atualizou Foto do Profissional", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }




        public Profissional GetById(string id)
        {
            var query = new StringBuilder(@"SELECT
                CAST(Profissional.Id AS CHAR) AS Id,
                Profissional.Nome,
                Profissional.Cpf,
                Profissional.Cns,
                Profissional.DataNascimento,
                Profissional.Sexo,
                Profissional.Crm,                
                Profissional.CrmUF,
                Profissional.Conselho,
                Profissional.Email,
                Profissional.Username,
                Profissional.Senha,
                Profissional.Telefone,
                Profissional.Logradouro,
                Profissional.LogradouroNumero,
                Profissional.LogradouroComplemento,
                Profissional.LogradouroBairro,
                Profissional.LogradouroCep,
                Profissional.CidadeId,
                Profissional.UfAbreviado,
                Profissional.DataCadastro,
                Profissional.DataAlteracao,
                Profissional.Ativo,
                Profissional.CadastroEditado,
                Profissional.DescricaoEditado,
                Profissional.Imagem,
                Profissional.OcupacaoId,
                Profissional.UltimoPerfilSelecionado,
                CAST(Profissional.UsuarioEditou AS CHAR) AS UsuarioEditou,
                Profissional.Token,
                Cidade.Ibge, Cidade.Nome, Cidade.UfAbreviado, Cidade.UfExtenso,
                CAST(UsuarioClaim.Id AS CHAR) AS Id,
                UsuarioClaim.ProfissionalId,
                UsuarioClaim.ClaimType,
                UsuarioClaim.ClaimValue
                FROM Profissional
                LEFT JOIN Cidade ON Cidade.Ibge = Profissional.CidadeId
                LEFT JOIN UsuarioClaim ON UsuarioClaim.ProfissionalId = Profissional.Id ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND Profissional.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            try
            {
                var dictionary = new Dictionary<string, Profissional>();

                var profissional = _connection.Query<Profissional, Cidade, UserClaim, Profissional>(
                    query.ToString(),
                    (profissionalObj, cidade, claim) =>
                    {
                        if (!dictionary.TryGetValue(profissionalObj.Id, out var entity))
                        {
                            entity = profissionalObj;
                            entity.UserClaims = new List<UserClaim>();

                            dictionary.Add(profissionalObj.Id, entity);
                        }

                        if (cidade != null)
                            entity.Cidade = cidade;

                        if (claim != null)
                        {
                            entity.UserClaims.Add(claim);
                        }

                        return entity;
                    },
                    param,
                    splitOn: "Id, Ibge, Id",
                    commandTimeout: TimeOut,
                    commandType: CommandType.Text
                ).FirstOrDefault();

                return profissional;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<(IEnumerable<Profissional>, int)> GetByParam(Profissional filters, string sort, int? skip, int? take)
        {
            var query = new StringBuilder(
                @"SELECT
                CAST(Profissional.Id AS CHAR) AS Id,
                Profissional.Nome,
                Profissional.Cpf,
                Profissional.Cns,
                Profissional.CodigoAutenticacao,
                Profissional.DataNascimento,
                Profissional.Sexo,
                Profissional.Crm,
                Profissional.CrmUF,
                Profissional.Conselho,
                Profissional.Username,
                Profissional.Email,
                Profissional.Senha,
                Profissional.Telefone,
                Profissional.Logradouro,
                Profissional.LogradouroNumero,
                Profissional.LogradouroComplemento,
                Profissional.LogradouroBairro,
                Profissional.LogradouroCep,
                Profissional.CidadeId,
                Profissional.UfAbreviado,
                Profissional.DataCadastro,
                Profissional.DataAlteracao,
                Profissional.Ativo,
                Profissional.Aceite,
                Profissional.Token,
                Profissional.Imagem,
                Profissional.OcupacaoId,
                Profissional.UltimoPerfilSelecionado,
                Cidade.Ibge, 
                Cidade.Nome, 
                Cidade.UfAbreviado, 
                Cidade.UfExtenso
                FROM Profissional
                LEFT JOIN Cidade ON Cidade.Ibge = Profissional.CidadeId
                LEFT JOIN EstabelecimentoProcedimentoHorario ON EstabelecimentoProcedimentoHorario.ProfissionalId = Profissional.Id ");


            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var queryFilterProcedimento = new StringBuilder("");
            var param = new DynamicParameters();

            if (filters != null)
            {

                if (!string.IsNullOrEmpty(filters.Nome))
                {
                    queryFilter.Append(" AND Profissional.Nome LIKE ?Nome ");
                    param.Add("?Nome", dbType: DbType.String, value: $"%{filters.Nome}%", direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.Crm))
                {
                    queryFilter.Append(" AND Profissional.Crm LIKE ?Crm ");
                    param.Add("?Crm", dbType: DbType.String, value: $"%{filters.Crm}%", direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.Cpf))
                {
                    queryFilter.Append(" AND Profissional.Cpf LIKE ?Cpf ");
                    param.Add("?Cpf", dbType: DbType.String, value: $"%{filters.Cpf}%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Cns))
                {
                    queryFilter.Append(" AND Profissional.Cns LIKE ?Cns ");
                    param.Add("?Cns", dbType: DbType.String, value: $"%{filters.Cns}%", direction: ParameterDirection.Input);
                }
                if (filters.Ativo.HasValue)
                {
                    queryFilter.Append(" AND Profissional.Ativo = ?Ativo ");
                    param.Add("?Ativo", dbType: DbType.Boolean, value: filters.Ativo, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.ProcedimentoId))
                {
                    queryFilterProcedimento.Append(" AND EstabelecimentoProcedimentoHorario.ProcedimentoId = ?ProcedimentoId ");
                    param.Add("?ProcedimentoId", dbType: DbType.String, value: $"{filters.ProcedimentoId}", direction: ParameterDirection.Input);
                }

            }
            #endregion

            query.Append(queryFilter);
            query.Append($" GROUP BY Profissional.Id ");

            //_queryCountAll.Append(queryFilter);
            //_queryCountAll.Append($" LIMIT 0, 1");

            if (skip.HasValue && take.HasValue)
            {
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
                query.Append($" ORDER BY Profissional.{sort} LIMIT ?skip, ?take;");
            }

            var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            if (totalCount > 0)
            {
                var profissionais = await _connection.QueryAsync<Profissional, Cidade, Profissional>(query.ToString(), (p, c) => {
                    p.Cidade = c;
                    return p;
                }, param, splitOn: "Ibge", commandTimeout: TimeOut, commandType: CommandType.Text) ;
                    foreach (var profissional in profissionais)
                    {
                        param.Add("?profissionalId", dbType: DbType.String, value: profissional.Id, direction: ParameterDirection.Input);
                        var queryClaims = @"Select CAST(UsuarioClaim.Id AS CHAR) AS Id,
                                                        CAST(UsuarioClaim.ProfissionalId AS CHAR) AS ProfissionalId,
                                                        UsuarioClaim.ClaimType,
                                                        UsuarioClaim.ClaimValue
                                                        From UsuarioClaim WHERE ProfissionalId = ?profissionalId";

                        var claims = _connection.Query<UserClaim>(queryClaims.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).ToList();
                        if (claims != null) profissional.UserClaims = claims;
                    }

                return (profissionais, totalCount);
            }
            return (null, 0);


            #region Tipo1

            //var query = new StringBuilder(
            //    @"SELECT
            //    CAST(Profissional.Id AS CHAR) AS Id,
            //    Profissional.Nome,
            //    Profissional.Cpf,
            //    Profissional.Cns,
            //    Profissional.CodigoAutenticacao,
            //    Profissional.DataNascimento,
            //    Profissional.Sexo,
            //    Profissional.Crm,
            //    Profissional.Username,
            //    Profissional.Email,
            //    Profissional.Senha,
            //    Profissional.Telefone,
            //    Profissional.Logradouro,
            //    Profissional.LogradouroNumero,
            //    Profissional.LogradouroComplemento,
            //    Profissional.LogradouroBairro,
            //    Profissional.LogradouroCep,
            //    Profissional.CidadeId,
            //    Profissional.UfAbreviado,
            //    Profissional.DataCadastro,
            //    Profissional.DataAlteracao,
            //    Profissional.Ativo,
            //    Profissional.Aceite,
            //    Profissional.Token,
            //    Profissional.Imagem,
            //    Profissional.OcupacaoId,
            //    Profissional.UltimoPerfilSelecionado, 
            //    Cidade.Ibge, 
            //    Cidade.Nome, 
            //    Cidade.UfAbreviado, 
            //    Cidade.UfExtenso,
            //    CAST(UsuarioClaim.Id AS CHAR) AS Id,
            //    UsuarioClaim.ProfissionalId,
            //    UsuarioClaim.ClaimType,
            //    UsuarioClaim.ClaimValue
            //    FROM ( 
            //        SELECT
            //        CAST(p.Id AS CHAR) AS Id,
            //        p.Nome,
            //        p.Cpf,
            //        p.Cns,
            //        p.CodigoAutenticacao,
            //        p.DataNascimento,
            //        p.Sexo,
            //        p.Crm,
            //        p.Username,
            //        p.Email,
            //        p.Senha,
            //        p.Telefone,
            //        p.Logradouro,
            //        p.LogradouroNumero,
            //        p.LogradouroComplemento,
            //        p.LogradouroBairro,
            //        p.LogradouroCep,
            //        p.CidadeId,
            //        p.UfAbreviado,
            //        p.DataCadastro,
            //        p.DataAlteracao,
            //        p.Ativo,
            //        p.Aceite,
            //        p.Token,
            //        p.Imagem,
            //        p.OcupacaoId,
            //        p.UltimoPerfilSelecionado FROM Profissional AS p
            //        LEFT JOIN Cidade ON Cidade.Ibge = p.CidadeId
            //        LEFT JOIN UsuarioClaim ON UsuarioClaim.ProfissionalId = p.Id
            //    ");


            ////FROM
            ////    Profissional
            ////    LEFT JOIN Cidade ON Cidade.Ibge = Profissional.CidadeId
            ////    LEFT JOIN UsuarioClaim ON UsuarioClaim.ProfissionalId = Profissional.Id
            ////    LEFT JOIN EstabelecimentoProcedimentoHorario ON EstabelecimentoProcedimentoHorario.ProfissionalId = Profissional.Id

            //#region -- -- Filtros / Ordenação
            //var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            //var queryFilterProcedimento = new StringBuilder("");
            //var param = new DynamicParameters();

            //if (filters != null)
            //{

            //    if (!string.IsNullOrEmpty(filters.Nome))
            //    {
            //        queryFilter.Append(" AND Profissional.Nome LIKE ?Nome ");
            //        param.Add("?Nome", dbType: DbType.String, value: $"%{filters.Nome}%", direction: ParameterDirection.Input);
            //    }

            //    if (!string.IsNullOrEmpty(filters.Crm))
            //    {
            //        queryFilter.Append(" AND Profissional.Crm LIKE ?Crm ");
            //        param.Add("?Crm", dbType: DbType.String, value: $"%{filters.Crm}%", direction: ParameterDirection.Input);
            //    }

            //    if (!string.IsNullOrEmpty(filters.Cpf))
            //    {
            //        queryFilter.Append(" AND Profissional.Cpf LIKE ?Cpf ");
            //        param.Add("?Cpf", dbType: DbType.String, value: $"%{filters.Cpf}%", direction: ParameterDirection.Input);
            //    }
            //    if (!string.IsNullOrEmpty(filters.Cns))
            //    {
            //        queryFilter.Append(" AND Profissional.Cns LIKE ?Cns ");
            //        param.Add("?Cns", dbType: DbType.String, value: $"%{filters.Cns}%", direction: ParameterDirection.Input);
            //    }
            //    if (filters.Ativo.HasValue)
            //    {
            //        queryFilter.Append(" AND Profissional.Ativo = ?Ativo ");
            //        param.Add("?Ativo", dbType: DbType.Boolean, value: filters.Ativo, direction: ParameterDirection.Input);
            //    }

            //    if (!string.IsNullOrEmpty(filters.ProcedimentoId))
            //    {
            //        queryFilterProcedimento.Append(" AND EstabelecimentoProcedimentoHorario.ProcedimentoId = ?ProcedimentoId ");
            //        param.Add("?ProcedimentoId", dbType: DbType.String, value: $"{filters.ProcedimentoId}", direction: ParameterDirection.Input);
            //    }

            //}

            //query.Append(queryFilter);
            ////query.Append(queryFilterProcedimento);
            //_queryCountAll.Append(queryFilter);

            //query.Append($"ORDER BY {sort} ");



            //query.Append(@") AS Profissional
            //             LEFT JOIN Cidade ON Cidade.Ibge = Profissional.CidadeId
            //             LEFT JOIN UsuarioClaim ON UsuarioClaim.ProfissionalId = Profissional.Id

            //            ");

            //query.Append($" {sort.Replace("p.", "Profissional.")} ORDER BY Profissional.Nome");

            ////GROUP BY Profissional.

            //if (skip.HasValue && take.HasValue)
            //{
            //    query.Append($" LIMIT ?skip, ?take ");
            //    skip = (skip - 1) * take;
            //    param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
            //    param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);

            //    //query.Append($" ORDER BY Profissional.{sort} LIMIT ?skip, ?take;");
            //    //skip = (skip - 1) * take;
            //    //param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
            //    //param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            //}


            //#endregion

            //var totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

            //if (totalCount > 0)
            //{
            //    //var x = await _connection.QueryAsync<Profissional>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text);

            //    try
            //    {
            //        var list = new Dictionary<string, Profissional>();
            //        await _connection.QueryAsync<Profissional>(query.ToString(),
            //        new[]
            //        {
            //          typeof(Profissional), typeof(Cidade), typeof(UserClaim)
            //        },
            //        objects =>
            //        {
            //            var profissional = objects[0] as Profissional;
            //            var cidade = objects[1] as Cidade;
            //            var userClaim = objects[2] as UserClaim;

            //            if (!list.TryGetValue(profissional.Id, out var entity))
            //            {
            //                entity = profissional;
            //                if (entity.UserClaims == null)
            //                    entity.UserClaims = new List<UserClaim>();

            //                list.Add(profissional.Id, entity);
            //            }

            //            if (userClaim != null)
            //            {
            //                if (!entity.UserClaims.Any(x => x.Id == userClaim.Id))
            //                {
            //                    entity.UserClaims.Add(userClaim);
            //                }
            //            }

            //            if (cidade != null) entity.Cidade = cidade;

            //            return entity;
            //        }, param, splitOn: "Ibge, ProfissionalId", commandTimeout: TimeOut, commandType: CommandType.Text);

            //        return (list.Values, totalCount);


            //        //var profissionais = new Dictionary<string, Profissional>();
            //        //await _connection.QueryAsync<Profissional, Cidade, UserClaim, Profissional>(
            //        //    query.ToString(), (profissional, cidade, userClaim) =>
            //        //    {
            //        //        if (!profissionais.TryGetValue(profissional.Id, out var profEntity))
            //        //        {
            //        //            profissionais.Add(profissional.Id, profEntity = profissional);
            //        //        }

            //        //        if (profEntity.UserClaims == null)
            //        //        {
            //        //            profEntity.UserClaims = new List<UserClaim>();
            //        //        }
            //        //        if (userClaim == null) return profEntity;
            //        //        if (profEntity.UserClaims.All(p => p.Id != userClaim.Id))
            //        //        {
            //        //            profEntity.UserClaims.Add(userClaim);
            //        //        }

            //        //        profEntity.Cidade = cidade;

            //        //        return profEntity;
            //        //    }, param,
            //        //    splitOn: "Ibge, ProfissionalId",
            //        //    commandTimeout: 0,
            //        //    commandType: CommandType.Text);

            //        //return (profissionais.Values, totalCount);
            //    }
            //    catch (System.Exception e)
            //    {
            //        throw e;
            //    }


            //}
            //return (null, 0);

            #endregion
        }

        public int Delete(string id)
        {
            var query = @"UPDATE Profissional SET Ativo = 0 WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", id, "Desativou Profissional", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public Profissional GetEstabelecimentosById(string profissionalId)
        {
            var query = new StringBuilder(@"
                           SELECT
                            CAST(Profissional.Id AS CHAR) AS Id,
                            Profissional.Nome,
                            Profissional.Cpf,
                            Profissional.Cns,
                            Profissional.DataNascimento,
                            Profissional.Sexo,
                            Profissional.Crm,
                            Profissional.CrmUF,
                            Profissional.Conselho,
                            Profissional.Email,
                            Profissional.Telefone,
                            Profissional.Logradouro,
                            Profissional.LogradouroNumero,
                            Profissional.LogradouroComplemento,
                            Profissional.LogradouroBairro,
                            Profissional.LogradouroCep,
                            Profissional.CidadeId,
                            Profissional.UfAbreviado,
                            CAST(EstabelecimentoProfissional.ProfissionalId AS CHAR) AS ProfissionalId,
                            CAST(EstabelecimentoProfissional.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                            CAST(Estabelecimento.Id AS CHAR) As Id, 
                            Estabelecimento.NomeFantasia, 
                            Estabelecimento.Cnpj, 
                            Estabelecimento.Cnes,
                            Estabelecimento.Telefone1, 
                            Estabelecimento.Email, 
                            Estabelecimento.Deletado
                            FROM
                              (SELECT CAST(p.Id AS CHAR) AS Id,
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
                              p.UfAbreviado FROM Profissional p
                              WHERE p.Ativo = TRUE ");

            query.Append(@" ) AS Profissional
                        LEFT JOIN EstabelecimentoProfissional ON Profissional.Id = EstabelecimentoProfissional.ProfissionalId
                        LEFT JOIN Estabelecimento ON EstabelecimentoProfissional.EstabelecimentoId = Estabelecimento.Id ");

            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(profissionalId)) return null;

            query.Append(" AND Profissional.Id = ?profissionalId ");
            param.Add("?profissionalId", dbType: DbType.String, value: profissionalId, direction: ParameterDirection.Input);

            var result = new Dictionary<string, Profissional>();

            try
            {
                return _connection.Query<Profissional>(query.ToString(),
                    new[]
                    {
              typeof(Profissional), typeof(EstabelecimentoProfissional), typeof(Estabelecimento)
                    },
                    objects =>
                    {
                        var profissional = objects[0] as Profissional;
                        var estabelecimentoProfissional = objects[1] as EstabelecimentoProfissional;
                        var estabelecimento = objects[2] as Estabelecimento;

                        if (!result.TryGetValue(profissional.Id, out var entity))
                        {
                            entity = profissional;
                            if (entity.Estabelecimentos == null)
                            {
                                entity.Estabelecimentos = new List<Estabelecimento>();
                            }
                            result.Add(profissional.Id, entity);
                        }

                        if (estabelecimento != null)
                        {
                            if (!entity.Estabelecimentos.Any(x => x.Id == estabelecimento.Id))
                            {
                                entity.Estabelecimentos.Add(estabelecimento);
                            }
                        }

                        return entity;
                    }, param, splitOn: "ProfissionalId, EstabelecimentoId", commandTimeout: TimeOut, commandType: CommandType.Text)
                  .FirstOrDefault();

            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public Profissional GetByUserName(string userName)
        {

            var query = new StringBuilder(@"
                SELECT
                CAST(Profissional.Id AS CHAR) AS Id,
                Profissional.Nome,
                Profissional.Cpf,
                Profissional.Cns,
                Profissional.DataNascimento,
                Profissional.Sexo,
                Profissional.Crm,
                Profissional.CrmUF,
                Profissional.Conselho,
                Profissional.Email,
                Profissional.Username,
                Profissional.Senha,
                Profissional.Telefone,
                Profissional.Logradouro,
                Profissional.LogradouroNumero,
                Profissional.LogradouroComplemento,
                Profissional.LogradouroBairro,
                Profissional.LogradouroCep,
                Profissional.CidadeId,
                Profissional.UfAbreviado,
                Profissional.DataCadastro,
                Profissional.DataAlteracao,
                Profissional.Ativo,
                Profissional.CadastroEditado,
                Profissional.DescricaoEditado,
                CAST(Profissional.UsuarioEditou AS CHAR) AS UsuarioEditou,
                Profissional.Token,
                Profissional.TentativaLogin
                FROM Profissional");

            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            queryFilter.Append(" AND Username = ?Username");
            param.Add("?Username", dbType: DbType.String, value: userName, direction: ParameterDirection.Input);

            query.Append(queryFilter);

            return _connection.Query<Profissional>(query.ToString(), param, commandTimeout: 60, commandType: CommandType.Text).FirstOrDefault();

        }



        public Profissional Login(string username, string password)
        {
            var query = new StringBuilder(@"
                SELECT
                DISTINCT CAST(p.Id AS CHAR) AS Id,
                p.Nome,
                p.Cpf,
                p.Cns,
                p.DataNascimento,
                p.Sexo,
                p.Crm,
                p.CrmUF,
                p.Conselho,
                p.Email,
                p.Username,
                p.SenhaAlterada,
                p.Telefone,
                p.Logradouro,
                p.LogradouroNumero,
                p.LogradouroComplemento,
                p.LogradouroBairro,
                p.LogradouroCep,
                p.CidadeId,
                p.UfAbreviado,
                p.UfAbreviado AS Uf,
                p.DataCadastro,
                p.DataAlteracao,
                p.Ativo,
                p.TentativaLogin,
                p.Imagem,
                p.UltimoPerfilSelecionado,
                CAST(uc.Id AS CHAR) AS Id,
                CAST(uc.ProfissionalId AS CHAR) AS ProfissionalId,
                uc.ClaimType,
                uc.ClaimValue
                FROM Profissional p
                LEFT JOIN UsuarioClaim uc ON uc.ProfissionalId = p.Id ");

            query.Append("WHERE 1 = 1 ");

            var param = new DynamicParameters();

            query.Append(" AND p.Senha = ?Senha");
            param.Add("?Senha", dbType: DbType.String, value: password, direction: ParameterDirection.Input);

            if (username.Length == 11)
            {
                query.Append(" AND p.Username = ?Username ");
                param.Add("?Username", dbType: DbType.String, value: username, direction: ParameterDirection.Input);
            }
            else
            {
                query.Append(" AND p.Username = ?Username");
                param.Add("?Username", dbType: DbType.String, value: username, direction: ParameterDirection.Input);
            }


            var profissional = new Dictionary<string, Profissional>();
            _connection.Query<Profissional, UserClaim, Profissional>(
                query.ToString(), (p, uc) =>
                {
                    if (!profissional.TryGetValue(p.Id, out var profissionalEntity))
                    {
                        profissional.Add(p.Id, profissionalEntity = p);
                    }

                    if (profissionalEntity.UserClaims == null)
                    {
                        profissionalEntity.UserClaims = new List<UserClaim>();
                    }

                    if (uc == null) return profissionalEntity;
                    if (profissionalEntity.UserClaims.All(x => x.Id != uc.Id))
                    {
                        profissionalEntity.UserClaims.Add(uc);
                    }

                    return profissionalEntity;
                }, param,
                splitOn: "Id",
                commandTimeout: 60,
                commandType: CommandType.Text);

            if (profissional.Count == 0)
            {
                var obj = UpdateTentativaLogin(username);
                //var objRecuperado = GetByUserName(usuario);
            }

            var result = profissional.Values.FirstOrDefault();

            return result;
        }

        public int UpdateTentativaLogin(string usuario)
        {
            var updateQuery = @"
                        UPDATE Profissional SET
                        TentativaLogin = ?TentativaLogin 
                        WHERE Username = ?Username";

            var updateQueryDesativar = @"
                        UPDATE Profissional SET
                        Ativo = ?Ativo
                        WHERE Username = ?Username";

            try
            {
                var obj = GetByUserName(usuario);
                if (obj == null)
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", "N/A", $"Tentativa de login com Profissional inexistente: {usuario}", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                if (obj != null)
                {
                    //comeca a verificação das tentativas de login
                    obj.TentativaLogin = obj.TentativaLogin + 1;
                    if (obj.TentativaLogin <= 9)
                    {

                        var param = new DynamicParameters();
                        param.Add("?TentativaLogin", dbType: DbType.Int32, value: obj.TentativaLogin, direction: ParameterDirection.Input);
                        param.Add("?Username", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);
                        try
                        {
                            using (var transaction = _connection.BeginTransaction())
                            {
                                var ret = _connection.Execute(updateQuery, param, transaction);
                                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", usuario.ToString(), "Tentativa de login sem sucesso", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                                transaction.Commit();
                                return ret;
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    //se passar de 3 tentativas cai aqui
                    else
                    {

                        //verificar se esta ativo (para não ficar enchendo o banco de requisição pra setar ativo false)
                        if (obj.Ativo == false)
                        {
                            return 0;
                        }
                        // else para setar ativo false
                        else
                        {
                            var param = new DynamicParameters();
                            param.Add("?Ativo", dbType: DbType.Boolean, value: false, direction: ParameterDirection.Input);
                            param.Add("?Username", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);
                            try
                            {
                                using (var transaction = _connection.BeginTransaction())
                                {
                                    var ret = _connection.Execute(updateQueryDesativar, param, transaction);
                                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", usuario.ToString(), "Excedeu a quantidade de login e desativando o usuario por seguranca", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                                    transaction.Commit();
                                    return ret;
                                }
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                }
                else
                {
                    return 0;
                }


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int UpdateTentativas(string userId)
        {
            var updateQuery = @"
               UPDATE Profissional SET
               TentativaLogin = ?TentativaLogin
               WHERE Id = ?Id";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: userId, direction: ParameterDirection.Input);
            param.Add("?TentativaLogin", dbType: DbType.Int32, value: 0, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissinonal", userId.ToString(), "Realizou tentativa de login", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int UpdateSenha(string profissionalId, string password)
        {


            var query = @"UPDATE Profissional SET
                        Senha = ?Senha,
                        SenhaAlterada = ?SenhaAlterada,
                        DataAlteracao = ?DataAlteracao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: profissionalId, direction: ParameterDirection.Input);
            param.Add("?Senha", dbType: DbType.String, value: password, direction: ParameterDirection.Input);
            param.Add("?SenhaAlterada", dbType: DbType.Boolean, value: false, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {

                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", profissionalId, "Atualizou a Senha do Profissional", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IEnumerable<RelatorioProfissionalEscala>> GetProfissionalHorarios(AppParams filters)
        {

            var query =
                new StringBuilder(@"SELECT
                                    CAST(eph.Id AS CHAR) AS Id,
                                    CAST(eph.ProfissionalId AS CHAR) AS ProfissionalId,
                                    eph.Dia,                                    
                                    (SELECT MIN(Hora) FROM EstabelecimentoProcedimentoHorario WHERE ProfissionalId = p.Id AND Dia = eph.Dia) AS HoraInicial,
                                    (SELECT MAX(Hora) FROM EstabelecimentoProcedimentoHorario WHERE ProfissionalId = p.Id AND Dia = eph.Dia) AS HoraFinal,
                                    CAST(p.Id AS CHAR) AS Id,
                                    p.Nome,
                                    p.Cpf,
                                    p.Cns,
                                    p.Sexo,
                                    p.Crm
                                    FROM EstabelecimentoProcedimentoHorario eph
                                    INNER JOIN Profissional p ON eph.ProfissionalId = p.Id
                                    ");

            var param = new DynamicParameters();
            if (filters != null)
            {

                if (!string.IsNullOrEmpty(filters.ProfissionalId))
                {
                    query.Append(" AND p.Id = ?profissionalId ");
                    param.Add("?profissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
                }

                query.Append(" AND DATE(eph.DataHora) BETWEEN DATE(?dataInicial) AND  DATE(?dataFinal) ");
                param.Add("?dataInicial", dbType: DbType.DateTime, value: filters.DataInicial, direction: ParameterDirection.Input);
                param.Add("?dataFinal", dbType: DbType.DateTime, value: filters.DataFinal, direction: ParameterDirection.Input);

                query.Append(" GROUP BY p.Id, eph.Dia ORDER BY eph.Dia ASC ");
            }

            try
            {
                var list = await _connection.QueryAsync<RelatorioProfissionalEscala, Profissional, RelatorioProfissionalEscala>(
                    query.ToString(), (r, p) =>
                    {
                        r.Profissional = p;
                        return r;
                    }, param,
                    splitOn: "ProfissionalId, Id",
                    commandTimeout: 0,
                    commandType: CommandType.Text);

                return list;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<IEnumerable<Profissional>> GetProfissionalProcedimentos(AppParams filters, string sort, int? skip, int? take)
        {
            var query =
                new StringBuilder(@"SELECT
                                    CAST(p.Id AS CHAR) AS Id,
                                    p.Nome,
                                    p.Cpf,
                                    p.Cns,
                                    p.Sexo,
                                    p.Crm,
                                    CAST(UsuarioClaim.Id AS CHAR) AS Id,
                                    UsuarioClaim.ProfissionalId,
                                    UsuarioClaim.ClaimType,
                                    UsuarioClaim.ClaimValue
                                    FROM Profissional p
                                    LEFT JOIN EstabelecimentoProcedimentoHorario eph ON eph.ProfissionalId = p.Id
                                    LEFT JOIN UsuarioClaim ON UsuarioClaim.ProfissionalId = p.Id
                ");

            var param = new DynamicParameters();
            query.Append("WHERE 1 = 1 ");
            query.Append(" AND eph.Situacao = 0 ");
            if (filters != null)
            {

                if (!string.IsNullOrEmpty(filters.ProfissionalId))
                {
                    query.Append(" AND eph.ProfissionalId = ?profissionalId ");
                    param.Add("?profissionalId", dbType: DbType.String, value: filters.ProfissionalId, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.EstabelecimentoId))
                {
                    query.Append(" AND eph.EstabelecimentoId = ?estabelecimentoId ");
                    param.Add("?estabelecimentoId", dbType: DbType.String, value: filters.EstabelecimentoId, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.ProcedimentoId))
                {
                    query.Append(" AND eph.ProcedimentoId = ?procedimentoId ");
                    param.Add("?procedimentoId", dbType: DbType.String, value: filters.ProcedimentoId, direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.TipoDaConsulta))
                {
                    query.Append(" AND eph.TipoDaConsulta = ?TipoDaConsulta ");
                    param.Add("?TipoDaConsulta", dbType: DbType.String, value: filters.TipoDaConsulta, direction: ParameterDirection.Input);
                }

                if (filters.DataInicial.HasValue && filters.DataFinal.HasValue)
                {
                    query.Append(" AND DATE(eph.DataHora) BETWEEN DATE(?dataInicial) AND  DATE(?dataFinal) ");
                    param.Add("?dataInicial", dbType: DbType.DateTime, value: filters.DataInicial, direction: ParameterDirection.Input);
                    param.Add("?dataFinal", dbType: DbType.DateTime, value: filters.DataFinal, direction: ParameterDirection.Input);
                }

                query.Append(" GROUP BY p.Id ");

                if (skip.HasValue && take.HasValue)
                {
                    query.Append($" ORDER BY {sort} LIMIT ?skip, ?take ");
                    skip = (skip - 1) * take;
                    param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                    param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
                }
            }

            try
            {

                var profissionais = new Dictionary<string, Profissional>();
                await _connection.QueryAsync<Profissional, UserClaim, Profissional>(
                    query.ToString(), (profissional, userClaim) =>
                    {
                        if (!profissionais.TryGetValue(profissional.Id, out var profEntity))
                        {
                            profissionais.Add(profissional.Id, profEntity = profissional);
                        }

                        if (profEntity.UserClaims == null)
                        {
                            profEntity.UserClaims = new List<UserClaim>();
                        }
                        if (userClaim == null) return profEntity;
                        if (profEntity.UserClaims.All(p => p.Id != userClaim.Id))
                        {
                            profEntity.UserClaims.Add(userClaim);
                        }

                        return profEntity;
                    }, param,
                    splitOn: "Id, Id",
                    commandTimeout: 60,
                    commandType: CommandType.Text);

                return (profissionais.Values);
            }
            catch (System.Exception e)
            {
                throw e;
            }

            //try
            //{
            //    var list = await _connection.QueryAsync<Profissional>(
            //        query.ToString(),
            //        param,
            //        commandTimeout: 0,
            //        commandType: CommandType.Text);

            //    return list;
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}

        }


        public int AddToken(string id, string token)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (token == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", id, DbType.String, ParameterDirection.Input);
            param.Add("?Token", token, DbType.String, ParameterDirection.Input);


            const string updateQueryToken = @"
              UPDATE Profissional SET
              Token = ?Token
              WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {

                    var ret = _connection.Execute(updateQueryToken, param, transaction);

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateToken(string id, string token)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (token == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", id, DbType.String, ParameterDirection.Input);
            param.Add("?Token", token, DbType.String, ParameterDirection.Input);


            const string updateQueryToken = @"
              UPDATE Profissional SET
              Token = ?Token
              WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {

                    var ret = _connection.Execute(updateQueryToken, param, transaction);

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int AceiteTermosDoUso(string profissionalId, bool? aceite)
        {
            var query = @"UPDATE Profissional SET
                        Aceite = ?Aceite,
                        DataAlteracao = @DataAlteracao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Aceite", dbType: DbType.Boolean, value: aceite, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?Id", dbType: DbType.String, value: profissionalId, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", profissionalId.ToString(), $"Alterou Termos do Uso para {aceite}", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public Profissional GetByCpf(string cpf)
        {
            var query = new StringBuilder(@"
                    SELECT
                    CAST(p.Id AS CHAR) AS Id,
                    p.Senha,
                    p.Nome,
                    p.Cpf,
                    p.Cns,
                    p.CodigoAutenticacao,
                    p.DataNascimento,
                    p.Sexo,
                    p.Crm,
                    p.CrmUF,
                    p.Conselho,
                    p.Email,
                    p.Telefone,
                    p.Logradouro,
                    p.LogradouroNumero,
                    p.LogradouroComplemento,
                    p.LogradouroBairro,
                    p.LogradouroCep,
                    p.CidadeId,
                    p.UfAbreviado,
                    p.DataCadastro,
                    p.DataAlteracao,
                    p.Ativo,
                    p.Aceite,
                    p.Token,
                    p.OcupacaoId 
                    FROM
                    Profissional p ");


            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(cpf)) return null;

            queryFilter.Append(" AND p.Cpf = ?Cpf");
            param.Add("?Cpf", dbType: DbType.String, value: cpf, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            return _connection.Query<Profissional>(
                query.ToString(),
                param,
                commandTimeout: TimeOut,
                commandType: CommandType.Text)
              .SingleOrDefault();


        }

        public string UpdateCodigoAutenticacao(string profissionalId, string codigoAutenticacao)
        {
            var query = @"UPDATE Profissional SET
                        CodigoAutenticacao = ?CodigoAutenticacao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: profissionalId, direction: ParameterDirection.Input);
            param.Add("?CodigoAutenticacao", dbType: DbType.String, value: codigoAutenticacao, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", profissionalId, "Atualizou Código Autenticação", DateTime.Now, _user.GetUserOrigem(), profissionalId, _user.GetUserIp());

                    transaction.Commit();
                    return codigoAutenticacao;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckCpf(string cpf)
        {
            var query = new StringBuilder(@"SELECT COUNT(1) FROM Profissional WHERE ");

            var param = new DynamicParameters();
            query.Append(" Cpf = ?Cpf");
            param.Add("?Cpf", dbType: DbType.String, value: cpf, direction: ParameterDirection.Input);

            return _connection.Query<bool>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();
        }

        public bool CheckCrm(string id, string crm)
        {
            var query = new StringBuilder(@"SELECT COUNT(1) FROM Profissional WHERE ");

            var param = new DynamicParameters();
            query.Append(" Id <> ?Id AND");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(" Crm = ?Crm");
            param.Add("?Crm", dbType: DbType.String, value: crm, direction: ParameterDirection.Input);

            return _connection.Query<bool>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();
        }

        public int UpdateAtivarDesativar(Profissional obj)
        {
            var query = @"UPDATE Profissional SET
                        Ativo = ?Ativo,
                        CadastroEditado = ?CadastroEditado,
                        DescricaoEditado = ?DescricaoEditado,
                        UsuarioEditou = ?UsuarioEditou,
                        DataAlteracao = ?DataAlteracao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();

            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);
            param.Add("?CadastroEditado", dbType: DbType.Boolean, value: obj.CadastroEditado, direction: ParameterDirection.Input);
            param.Add("?DescricaoEditado", dbType: DbType.String, value: obj.DescricaoEditado, direction: ParameterDirection.Input);
            param.Add("?UsuarioEditou", dbType: DbType.String, value: obj.UsuarioEditou, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Profissional", obj.Id, "Profissional Ativado", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
