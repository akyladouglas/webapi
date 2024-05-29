using Dapper;
using AtendTeleMedicina.Domain.Contracts.Repositories.Security;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;

namespace AtendTeleMedicina.Infra.Data.Repositories.Security
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly StringBuilder _queryAll = new StringBuilder(
          @"SELECT
                    CAST(Id AS CHAR) AS Id,
                    Nome,
                    Cpf,
                    Email,
                    Username,
                    Senha,
                    CidadeId,
                    Uf,
                    DataCadastro,
                    DataAlteracao,
                    Ativo,
                    Aceite,
                    TentativaLogin,
                    Telefone,
                    Cns,
                    CodigoAutenticacao,
                    DataNascimento,
                    Sexo,
                    Logradouro,
                    LogradouroNumero,
                    LogradouroComplemento,
                    LogradouroBairro,
                    LogradouroCep,
                    EstabelecimentoId,
                    OcupacaoId
                    FROM
                    Usuario u");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
          @"SELECT Count(Id) 
                    FROM Usuario ");
        private readonly MySqlConnection _connection;
        private readonly IUser _user;
        public UserRepository(IMySqlContext context, IUser user)
          : base(context)
        {
            _connection = context.Connection;
            _user = user;
        }

        public int Add(User cadastro, string userId)
        {
            const string insertQuery = @"
             INSERT INTO Usuario (Id, Nome, Cpf, Cns, Email, Telefone, Username, Senha, CidadeId, Uf, DataCadastro, DataAlteracao, Ativo, EstabelecimentoId, OcupacaoId) 
             VALUES              (?Id, ?Nome, ?Cpf, ?Cns, ?Email, ?Telefone, ?Username, ?Senha, ?CidadeId, ?Uf, ?DataCadastro, ?DataAlteracao, ?Ativo, ?EstabelecimentoId, ?OcupacaoId)";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: cadastro.Id, direction: ParameterDirection.Input);
            param.Add("?Nome", dbType: DbType.String, value: cadastro.Nome, direction: ParameterDirection.Input);
            param.Add("?Cpf", dbType: DbType.String, value: cadastro.Cpf, direction: ParameterDirection.Input);
            param.Add("?Cns", dbType: DbType.String, value: cadastro.Cns, direction: ParameterDirection.Input);
            param.Add("?Email", dbType: DbType.String, value: cadastro.Email, direction: ParameterDirection.Input);
            param.Add("?Telefone", dbType: DbType.String, value: cadastro.Telefone, direction: ParameterDirection.Input);
            param.Add("?Username", dbType: DbType.String, value: cadastro.Username, direction: ParameterDirection.Input);
            param.Add("?Senha", dbType: DbType.String, value: cadastro.Senha, direction: ParameterDirection.Input);
            param.Add("?CidadeId", dbType: DbType.String, value: cadastro.CidadeId, direction: ParameterDirection.Input);
            param.Add("?Uf", dbType: DbType.String, value: cadastro.Uf, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: cadastro.Ativo, direction: ParameterDirection.Input);
            param.Add("?EstabelecimentoId", dbType: DbType.String, value: cadastro.EstabelecimentoId, direction: ParameterDirection.Input);
            param.Add("?OcupacaoId", dbType: DbType.Int32, value: cadastro.OcupacaoId, direction: ParameterDirection.Input);

            var checkCpf = _connection.Query<User>("SELECT Cpf FROM Usuario WHERE Cpf = ?Cpf", param).FirstOrDefault();
            if (checkCpf != null) throw new Exception("Cpf já cadastrado");
            if (GetByUserName(cadastro.Username) != null) throw new Exception("Nome de Usuário já cadastrado");

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {

                    var ret = _connection.Execute(insertQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", cadastro.Id.ToString(), "Inseriu Usuário", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    var newUser = GetByUserName(cadastro.Username);

                    if (cadastro.UserClaims.Any())
                    {
                        foreach (var item in cadastro.UserClaims) {
                            item.Id = Guid.NewGuid().ToString();
                            item.UsuarioId = cadastro.Id;
                        }   

                        var addClaims = "INSERT INTO UsuarioClaim (Id, UsuarioId, ClaimType, ClaimValue) VALUES (@Id, @UsuarioId, @ClaimType, @ClaimValue)";
                        _connection.Execute(addClaims, cadastro.UserClaims, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "UsuarioClaim", cadastro.Id.ToString(), "Inseriu as Claims do Usuário", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    }

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public int Update(User cadastro)
        {
            var updateQuery = "";

            if (string.IsNullOrEmpty(cadastro.Senha))
            {
                updateQuery = @"
               UPDATE Usuario SET
               Nome = ?Nome, 
               Cpf = ?Cpf,
               Cns = ?Cns,
               Username = ?Username,
               Email = ?Email,
               Telefone = ?Telefone,
               CidadeId = ?CidadeId,
               Uf = ?Uf,
               Ativo = ?Ativo,
               CadastroEditado = ?CadastroEditado,
               UltimoPerfilSelecionado = ?UltimoPerfilSelecionado,
               DescricaoEditado = ?DescricaoEditado,
               UsuarioEditou = ?UsuarioEditou,
               OcupacaoId = ?OcupacaoId
               WHERE Id = ?Id";
            }
            else
            {
                updateQuery = @"
               UPDATE Usuario SET
               Nome = ?Nome, 
               Cpf = ?Cpf,
               Cns = ?Cns,
               Username = ?Username,
               Senha = ?Senha,
               SenhaAlterada = ?SenhaAlterada,
               UltimoPerfilSelecionado = ?UltimoPerfilSelecionado,
               EstabelecimentoId = ?EstabelecimentoId,
               Email = ?Email,
               Telefone = ?Telefone,
               Senha = ?Senha,
               CidadeId = ?CidadeId,
               Uf = ?Uf,
               Ativo = ?Ativo,
               CadastroEditado = ?CadastroEditado,
               DescricaoEditado = ?DescricaoEditado,
               UsuarioEditou = ?UsuarioEditou,
               OcupacaoId = ?OcupacaoId
               WHERE Id = ?Id";
            }

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: cadastro.Id, direction: ParameterDirection.Input);
            param.Add("?Nome", dbType: DbType.String, value: cadastro.Nome, direction: ParameterDirection.Input);
            param.Add("?Cpf", dbType: DbType.String, value: cadastro.Cpf, direction: ParameterDirection.Input);
            param.Add("?Cns", dbType: DbType.String, value: cadastro.Cns, direction: ParameterDirection.Input);
            param.Add("?Username", dbType: DbType.String, value: cadastro.Username, direction: ParameterDirection.Input);
            param.Add("?UltimoPerfilSelecionado", dbType: DbType.String, value: cadastro.UltimoPerfilSelecionado, direction: ParameterDirection.Input);
            if (!string.IsNullOrEmpty(cadastro.Senha))
            {
                param.Add("?Senha", dbType: DbType.String, value: cadastro.Senha, direction: ParameterDirection.Input);
            }
            param.Add("?Email", dbType: DbType.String, value: cadastro.Email, direction: ParameterDirection.Input);
            param.Add("?Telefone", dbType: DbType.String, value: cadastro.Telefone, direction: ParameterDirection.Input);
            param.Add("?CidadeId", dbType: DbType.String, value: cadastro.CidadeId, direction: ParameterDirection.Input);
            param.Add("?Uf", dbType: DbType.String, value: cadastro.Uf, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: cadastro.Ativo, direction: ParameterDirection.Input);
            param.Add("?CadastroEditado", dbType: DbType.Boolean, value: cadastro.CadastroEditado, direction: ParameterDirection.Input);
            param.Add("?DescricaoEditado", dbType: DbType.String, value: cadastro.DescricaoEditado, direction: ParameterDirection.Input);
            param.Add("?UsuarioEditou", dbType: DbType.String, value: cadastro.UsuarioEditou, direction: ParameterDirection.Input);
            param.Add("?SenhaAlterada", dbType: DbType.Boolean, value: cadastro.SenhaAlterada, direction: ParameterDirection.Input);
            param.Add("?EstabelecimentoId", dbType: DbType.String, value: cadastro.EstabelecimentoId, direction: ParameterDirection.Input);
            if(cadastro.Nome != "ADMINISTRADOR") param.Add("?OcupacaoId", dbType: DbType.Int32, value: cadastro.OcupacaoId, direction: ParameterDirection.Input);
            else param.Add("?OcupacaoId", dbType: DbType.Int32, value: null, direction: ParameterDirection.Input);
            param.RemoveUnused = true;

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    if (cadastro.UserClaims.Count > 0)
                    {
                        var delClaims = "DELETE FROM UsuarioClaim WHERE UsuarioId = ?Id";
                        _connection.Execute(delClaims, param, transaction);

                        foreach (var item in cadastro.UserClaims)
                        {
                            item.Id = Guid.NewGuid().ToString();
                            item.UsuarioId = cadastro.Id;
                        }

                        var addClaims = "INSERT INTO UsuarioClaim (Id, UsuarioId, ClaimType, ClaimValue) VALUES (@Id, @UsuarioId, @ClaimType, @ClaimValue)";
                        _connection.Execute(addClaims, cadastro.UserClaims, transaction);
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "UsuarioClaim", cadastro.Id.ToString(), "Atualizou Claims do Usuário", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    }

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    if (!string.IsNullOrEmpty(_user.GetUserId()))
                        RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", cadastro.Id.ToString(), "Atualizou Usuário", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

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
            var query = @"UPDATE Usuario SET Ativo = FALSE, DescricaoEditado = 'Usuario Desativado' WHERE Id = ?Id";
            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", id.ToString(), "Desativou Usuário", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public User GetById(string id)
        {
            var query = new StringBuilder(@"
                    SELECT
                    CAST(Usuario.Id AS CHAR) AS Id,
                    CAST(Usuario.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                    Usuario.Nome,
                    Usuario.Cpf,                    
                    Usuario.Cns,
                    Usuario.Email,
                    Usuario.Telefone,
                    Usuario.UltimoPerfilSelecionado,
                    Usuario.Username,
                    Usuario.Senha,
                    Usuario.CidadeId,
                    Usuario.Uf,
                    Usuario.DataCadastro,
                    Usuario.DataAlteracao,
                    Usuario.Ativo,
                    Usuario.Aceite,
                    Usuario.TentativaLogin,
                    Usuario.OcupacaoId,
                    Cidade.Ibge, 
                    Cidade.Nome, 
                    Cidade.UfAbreviado, 
                    Cidade.UfExtenso
                    FROM
                    Usuario
                    LEFT JOIN Cidade ON Cidade.Ibge = Usuario.CidadeId ");

            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" And Id = ?Id ");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

            query.Append(queryFilter);
            var users = new Dictionary<string, User>();
            _connection.QueryAsync<User, Cidade, User>(
                    query.ToString(), (u, c) =>
                    {
                        if (!users.TryGetValue(u.Id, out var userEntity))
                        {
                            users.Add(u.Id, userEntity = u);
                        }

                        userEntity.Cidade = c;

                        return userEntity;
                    }, param,
                    splitOn: "Id, Ibge",
                    commandTimeout: 60,
                    commandType: CommandType.Text);

            var user = users.Values.FirstOrDefault();

            if (user != null)
            {
                param.Add("?UsuarioId", dbType: DbType.String, value: user.Id, direction: ParameterDirection.Input);
                user.UserClaims = (IList<UserClaim>)
                    _connection.Query<UserClaim>("SELECT CAST(Id AS CHAR) AS Id, CAST(UsuarioId AS CHAR) AS UsuarioId, ClaimType, ClaimValue FROM UsuarioClaim WHERE UsuarioId = ?UsuarioId",
                    param, commandTimeout: 60, commandType: CommandType.Text);
            }
            return user;
        }

        public User GetByUserName(string userName)
        {

            var query = new StringBuilder(@"
                    SELECT
                    CAST(Usuario.Id AS CHAR) AS Id,
                    Usuario.Nome,
                    Usuario.Cpf,
                    Usuario.Email,
                    Usuario.Username,
                    Usuario.Senha,
                    Usuario.CidadeId,
                    Usuario.Uf,
                    Usuario.DataCadastro,
                    Usuario.DataAlteracao,
                    Usuario.Ativo,
                    Usuario.TentativaLogin,
                    CAST(Usuario.EstabelecimentoId AS CHAR) AS EstabelecimentoId
                    FROM
                    Usuario ");

            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            queryFilter.Append(" And Username = @Username");
            param.Add("@Username", dbType: DbType.String, value: userName, direction: ParameterDirection.Input);

            query.Append(queryFilter);

            return _connection.Query<User>(query.ToString(), param, commandTimeout: 60, commandType: CommandType.Text).FirstOrDefault();

        }

        public User Login(string usuario, string senha)
        {
            try
            {
                var query = new StringBuilder(@"
                    SELECT
                    CAST(Usuario.Id AS CHAR) AS Id,
                    CAST(Usuario.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                    Usuario.Nome,
                    Usuario.Cpf,                    
                    Usuario.Cns,
                    Usuario.Email,
                    Usuario.Username,
                    Usuario.Senha,
                    Usuario.SenhaAlterada,
                    Usuario.CidadeId,
                    Usuario.UltimoPerfilSelecionado,
                    Usuario.Uf,
                    Usuario.DataCadastro,
                    Usuario.DataAlteracao,
                    Usuario.Ativo,
                    Usuario.TentativaLogin,
                    Usuario.OcupacaoId,
                    CAST(UsuarioClaim.Id AS CHAR) AS Id,
                    CAST(UsuarioClaim.UsuarioId AS CHAR) AS UsuarioId,
                    UsuarioClaim.ClaimType,
                    UsuarioClaim.ClaimValue
                    FROM
                    Usuario
                    LEFT JOIN UsuarioClaim ON UsuarioClaim.UsuarioId = Usuario.Id");
                var queryFilter = new StringBuilder(" Where 1 = 1 ");
                var param = new DynamicParameters();

                if (string.IsNullOrEmpty(usuario)) return null;

            
                queryFilter.Append(" And Username = ?Usuario");
                param.Add("?Usuario", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);
                queryFilter.Append(" And Senha = ?Senha");
                param.Add("?Senha", dbType: DbType.String, value: senha, direction: ParameterDirection.Input);

                query.Append(queryFilter);
            

                var users = new Dictionary<string, User>();
                _connection.Query<User, UserClaim, User>(
                    query.ToString(), (user, userClaim) =>
                    {
                        if (!users.TryGetValue(user.Id, out var userEntity))
                        {
                            users.Add(user.Id, userEntity = user);
                        }

                        if (userEntity.UserClaims == null)
                        {
                            userEntity.UserClaims = new List<UserClaim>();
                        }

                        if (userClaim == null) return userEntity;
                        if (userEntity.UserClaims.All(x => x.Id != userClaim.Id))
                        {
                            userEntity.UserClaims.Add(userClaim);
                        }

                        return userEntity;
                    }, param,
                    splitOn: "Id",
                    commandTimeout: 60,
                    commandType: CommandType.Text);

                if (users.Count == 0) {
                    var obj = UpdateTentativaLogin(usuario);
                    //var objRecuperado = GetByUserName(usuario);
                }

                return users.Values.FirstOrDefault();

            }
            catch(Exception e)
            {
                throw e;
            }

        }






        public int UpdateTentativaLogin(string usuario)
        {
            var updateQuery = @"
                            UPDATE Usuario SET
                            TentativaLogin = ?TentativaLogin 
                            WHERE Username = ?Username";

            var updateQueryDesativar = @"
                            UPDATE Usuario SET
                            Ativo = ?Ativo
                            WHERE Username = ?Username";
            try
            {

                var obj = GetByUserName(usuario);
                if (obj != null) {

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
                                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", usuario.ToString(), "Tentou fazer login e deu errado", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                                transaction.Commit();
                                return ret;
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    //se passar das tentativas cai aqui
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
                                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", usuario.ToString(), "Excedeu a quantidade de login e desativando o usuario por seguranca", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
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
            catch (Exception e){
                throw e;
            }
            
        }

        public int UpdateTentativas(string userId)
        {
            var updateQuery = @"
               UPDATE Usuario SET
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
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", userId.ToString(), "Atualizou Usuário", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<(IEnumerable<User>, int)> GetByParam(User filters, string sort, int? skip, int? take)
        {
            var query = new StringBuilder(@"
                    SELECT
                    CAST(Usuario.Id AS CHAR) AS Id,
                    CAST(Usuario.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                    Usuario.Nome,
                    Usuario.Cpf,
                    Usuario.Cns,
                    Usuario.Email,
                    Usuario.Telefone,
                    Usuario.Username,
                    Usuario.Senha,
                    Usuario.CidadeId,
                    Usuario.Uf,
                    Usuario.DataCadastro,
                    Usuario.DataAlteracao,
                    Usuario.Ativo,
                    Usuario.OcupacaoId,
                    CAST(UsuarioClaim.Id AS CHAR) AS Id,
                    CAST(UsuarioClaim.UsuarioId AS CHAR) AS UsuarioId,
                    UsuarioClaim.ClaimType,
                    UsuarioClaim.ClaimValue,
                    Cidade.Ibge, Cidade.Nome, Cidade.UfAbreviado, Cidade.UfExtenso
                    FROM
                    Usuario
                    LEFT JOIN UsuarioClaim ON UsuarioClaim.UsuarioId = Usuario.Id
                    LEFT JOIN Cidade ON Cidade.Ibge = Usuario.CidadeId ");

            var queryCount = new StringBuilder();
            queryCount.Append(_queryCountAll);

            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Id))
                {
                    queryFilter.Append(" AND Usuario.Id = ?Id");
                    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Cpf))
                {
                    queryFilter.Append(" And Usuario.Cpf LIKE ?Cpf ");
                    param.Add("@Cpf", dbType: DbType.String, value: $"%{filters.Cpf}%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Nome))
                {
                    queryFilter.Append(" And Usuario.Nome LIKE ?Nome ");
                    param.Add("@Nome", dbType: DbType.String, value: $"%{filters.Nome}%", direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Uf))
                {
                    queryFilter.Append(" And Usuario.Uf = ?Uf");
                    param.Add("@Uf", dbType: DbType.String, value: filters.Uf, direction: ParameterDirection.Input);
                }
                if (filters.CidadeId != 0)
                {
                    queryFilter.Append(" And Usuario.CidadeId = ?CidadeId");
                    param.Add("@CidadeId", dbType: DbType.Int32, value: filters.CidadeId, direction: ParameterDirection.Input);
                }
                if (filters.Ativo.HasValue)
                {
                    queryFilter.Append(" AND Usuario.Ativo = ?Ativo ");
                    param.Add("?Ativo", dbType: DbType.Boolean, value: filters.Ativo, direction: ParameterDirection.Input);
                }

            }
            query.Append(queryFilter);
            queryCount.Append(queryFilter);

            if (skip.HasValue && take.HasValue)
            {
                query.Append($" ORDER BY Usuario.{sort} LIMIT @skip, @take;");
                skip = (skip - 1) * take;
                param.Add("@skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("@take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            try
            {
                var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: 60, commandType: CommandType.Text).SingleOrDefault();

                var users = new Dictionary<string, User>();
                await _connection.QueryAsync<User, UserClaim, Cidade, User>(
                    query.ToString(), (user, userClaim, cidade) =>
                    {
                        if (!users.TryGetValue(user.Id, out var userEntity))
                        {
                            users.Add(user.Id, userEntity = user);
                        }

                        if (userEntity.UserClaims == null)
                        {
                            userEntity.UserClaims = new List<UserClaim>();
                        }

                        if (userClaim == null) return userEntity;

                        if (userEntity.UserClaims.All(x => x.Id != userClaim.Id))
                        {
                            userEntity.UserClaims.Add(userClaim);
                        }
                        userEntity.Cidade = cidade;

                        return userEntity;
                    }, param,
                    splitOn: "Id, Ibge",
                    commandTimeout: 60,
                    commandType: CommandType.Text);

                return (users.Values, totalCount);
            }
            catch (System.Exception e)
            {
                throw e;
            }


        }

        public int AceiteTermosDoUso(string usuarioId, bool? aceite)
        {
            var query = @"UPDATE Usuario SET
                        Aceite = ?Aceite,
                        DataAlteracao = @DataAlteracao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Aceite", dbType: DbType.Boolean, value: aceite, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);
            param.Add("?Id", dbType: DbType.String, value: usuarioId, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", usuarioId.ToString(), $"Alterou Termos do Uso para {aceite}", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public bool ValidateCredentials(string userName, string password)
        {
            //User user;
            #region -- Filtro

            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(userName)) return false;
            if (string.IsNullOrEmpty(password)) return false;

            queryFilter.Append(" And [User].UserName = @UserName");
            param.Add("@UserName", dbType: DbType.String, value: userName, direction: ParameterDirection.Input);
            queryFilter.Append(" And [User].PasswordHash = @PasswordHash");
            param.Add("@PasswordHash", dbType: DbType.String, value: password, direction: ParameterDirection.Input);
            var query = _queryAll + queryFilter.ToString();
            #endregion

            var users = new Dictionary<string, User>();
            //_connection.Query<User, UserClaim, User>(
            //query,
            //(user, userClaim) =>
            //{
            //    if (!users.TryGetValue(user.Id, out var userEntity))
            //    {
            //        users.Add(user.Id, userEntity = user);
            //    }

            //    if (userEntity.UserClaims == null)
            //    {
            //        userEntity.UserClaims = new List<UserClaim>();
            //    }

            //    if (userClaim == null) return userEntity;
            //    if (userEntity.UserClaims.All(x => x.Id != userClaim.Id))
            //    {
            //        userEntity.UserClaims.Add(userClaim);
            //    }

            //    return userEntity;
            //}, param,
            //splitOn: "Id",
            //commandTimeout: 60,
            //commandType: CommandType.Text);

            return users.Values.FirstOrDefault() != null;
        }

        public int UpdateSenha(string usuarioId, string password)
        {
            var query = @"UPDATE Usuario SET
                        Senha = ?Senha,
                        SenhaAlterada = ?SenhaAlterada,
                        DataAlteracao = ?DataAlteracao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: usuarioId, direction: ParameterDirection.Input);
            param.Add("?Senha", dbType: DbType.String, value: password, direction: ParameterDirection.Input);
            param.Add("?SenhaAlterada", dbType: DbType.Boolean, value: false, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", usuarioId, "Atualizou a Senha do Usuario", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public User GetByCpf(string cpf)
        {
            var query = new StringBuilder(@"
                SELECT
                CAST(u.Id AS CHAR) AS Id,
                u.Senha,
                u.Nome,
                u.Cpf,
                u.Email,
                u.Cns,
                u.CodigoAutenticacao,
                u.Telefone,
                u.Logradouro,
                u.LogradouroNumero,
                u.LogradouroComplemento,
                u.LogradouroBairro,
                u.LogradouroCep,
                u.CidadeId,
                u.Uf,
                u.DataCadastro,
                u.DataAlteracao,
                u.Ativo,
                u.Aceite,
                u.SenhaAlterada,
                u.OcupacaoId,
                CAST(u.EstabelecimentoId AS CHAR)
                FROM
                Usuario u ");


            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(cpf)) return null;

            queryFilter.Append(" AND u.Cpf = ?Cpf");
            param.Add("?Cpf", dbType: DbType.String, value: cpf, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            return _connection.Query<User>(
                query.ToString(),
                param,
                commandTimeout: TimeOut,
                commandType: CommandType.Text)
                .SingleOrDefault();


        }
        public string UpdateCodigoAutenticacao(string usuarioId, string codigoAutenticacao)
        {
            var query = @"UPDATE Usuario SET
                    CodigoAutenticacao = ?CodigoAutenticacao
                    WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: usuarioId, direction: ParameterDirection.Input);
            param.Add("?CodigoAutenticacao", dbType: DbType.String, value: codigoAutenticacao, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", usuarioId, "Atualizou Código Autenticação", DateTime.Now, _user.GetUserOrigem(), usuarioId, _user.GetUserIp());

                    transaction.Commit();
                    return codigoAutenticacao;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateAtivo(User obj)
        {
            var query = @"UPDATE Usuario SET
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
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Usuario", obj.Id, "Usuario Ativado", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

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
