using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using AtendTeleMedicina.Domain.Contracts.Repositories.CadastroImobiliario;
using AtendTeleMedicina.Domain.Entities.CadastroImobiliario;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infrastructure.Data.Context;
using AtendTeleMedicina.Infrastructure.Data.Repositories.Base;

namespace AtendTeleMedicina.Infrastructure.Data.Repositories.CadastroImobiliario
{
    public class ImovelRepository : BaseRepository, IImovelRepository
    {
        private readonly StringBuilder _insertUpdateImovelProprietarios = new StringBuilder(
          @"MERGE ImovelProprietario AS IM
        USING (SELECT @Id, @ImovelId, @ContribuinteId, @ResponsavelCobranca) 
          AS Source_(Id, ImovelId, ContribuinteId, ResponsavelCobranca)
          ON (IM.Id = Source_.Id)
        WHEN MATCHED  
          THEN UPDATE SET 
            ImovelId = Source_.ImovelId, 
            ContribuinteId = Source_.ContribuinteId, 
            ResponsavelCobranca = Source_.ResponsavelCobranca
        WHEN NOT MATCHED 
            THEN INSERT (Id, ImovelId, ContribuinteId, ResponsavelCobranca) 
          VALUES (Source_.Id, Source_.ImovelId, Source_.ContribuinteId, Source_.ResponsavelCobranca);"
        );

        private readonly StringBuilder _queryAll = new StringBuilder(
           @"SELECT  Imovel.Id, Imovel.Inscricao, Imovel.EnderecoNumero, Imovel.EnderecoComplemento, Imovel.EnderecoDistrito,
              Imovel.EnderecoSetor, Imovel.EnderecoQuadra, Imovel.EnderecoLote, Imovel.EnderecoUnidade, Imovel.EnderecoSecao,
              Imovel.EnderecoLado, Imovel.UsuarioCadastro, Imovel.UsuarioCadastroData, Imovel.UsuarioAlteracao, Imovel.UsuarioAlteracaoData,
              Endereco.Id, Endereco.Logradouro, Endereco.Bairro, Endereco.Cep,
              Cidade.Ibge, Cidade.Nome, Cidade.UfAbreviado, Cidade.UfExtenso,
              ImovelProprietario.Id, ImovelProprietario.RazaoSocialNome, ImovelProprietario.CnpjCpf, ImovelProprietario.Telefone,
              ImovelProprietario.Email, ImovelProprietario.ResponsavelCobranca,
              ImovelProprietario.EnderecoNumero, ImovelProprietario.EnderecoComplemento,
              IPE.Id, IPE.Logradouro, IPE.Bairro, IPE.Cep,
              IPC.Ibge, IPC.Nome, IPC.UfAbreviado, IPC.UfExtenso
            FROM Imovel 
              INNER JOIN Endereco ON Imovel.EnderecoId = Endereco.Id
              INNER JOIN Cidade ON Endereco.CidadeIbge = Cidade.Ibge
              LEFT JOIN ImovelProprietario ON Imovel.Inscricao = ImovelProprietario.Inscricao
              INNER JOIN Endereco IPE ON ImovelProprietario.EnderecoId = IPE.Id
              INNER JOIN Cidade IPC ON IPE.CidadeIbge = IPC.Ibge
      ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(@"
      SELECT Count(distinct Imovel.Id) FROM Imovel 
        INNER JOIN Endereco ON Imovel.EnderecoId = Endereco.Id
        INNER JOIN Cidade ON Endereco.CidadeIbge = Cidade.Ibge
        LEFT JOIN ImovelProprietario ON Imovel.Inscricao = ImovelProprietario.Inscricao
        INNER JOIN Endereco IPE ON ImovelProprietario.EnderecoId = IPE.Id
        INNER JOIN Cidade IPC ON IPE.CidadeIbge = IPC.Ibge");

        private readonly SqlConnection _connection;

        public ImovelRepository(ISqlServerContext context)
          : base(context)
        {
            _connection = context.Connection;
        }

        public int Add(Imovel cadastro, string userId)
        {
            var insertImovel = new StringBuilder(
              @"MERGE Imovel as i
          USING (SELECT @Inscricao, @Status, @EnderecoId, @EnderecoNumero, @EnderecoComplemento, @EnderecoDistrito,
                   @EnderecoSetor, @EnderecoQuadra, @EnderecoLote, @EnderecoUnidade, @EnderecoSecao, @EnderecoLado, @UsuarioCadastro, @UsuarioCadastroData, @UsuarioAlteracao, @UsuarioAlteracaoData)
                 AS Source_(Inscricao, [Status], EnderecoId, EnderecoNumero, EnderecoComplemento, EnderecoDistrito,
                   EnderecoSetor, EnderecoQuadra, EnderecoLote, EnderecoUnidade, EnderecoSecao, EnderecoLado, UsuarioCadastro, UsuarioCadastroData, UsuarioAlteracao, UsuarioAlteracaoData)
          ON (I.Inscricao = Source_.Inscricao)
            WHEN NOT MATCHED
              THEN INSERT (Inscricao, [Status], EnderecoId, EnderecoNumero, EnderecoComplemento, EnderecoDistrito,
                      EnderecoSetor, EnderecoQuadra, EnderecoLote, EnderecoUnidade, EnderecoSecao, EnderecoLado, UsuarioCadastro, UsuarioCadastroData, UsuarioAlteracao, UsuarioAlteracaoData)
                   VALUES(Source_.Inscricao, Source_.[Status], Source_.EnderecoId, Source_.EnderecoNumero, Source_.EnderecoComplemento, Source_.EnderecoDistrito,
                      Source_.EnderecoSetor, Source_.EnderecoQuadra, Source_.EnderecoLote, Source_.EnderecoUnidade, Source_.EnderecoSecao, Source_.EnderecoLado, Source_.UsuarioCadastro, Source_.UsuarioCadastroData, Source_.UsuarioAlteracao, Source_.UsuarioAlteracaoData);"
            );
            var param = new DynamicParameters();
            param.Add("@Inscricao", dbType: DbType.String, value: cadastro.Inscricao, direction: ParameterDirection.Input);
            param.Add("@EnderecoId", dbType: DbType.String, value: cadastro.Endereco.Id, direction: ParameterDirection.Input);
            param.Add("@EnderecoNumero", dbType: DbType.String, value: cadastro.EnderecoNumero, direction: ParameterDirection.Input);
            param.Add("@EnderecoComplemento", dbType: DbType.String, value: cadastro.EnderecoComplemento, direction: ParameterDirection.Input);
            param.Add("@EnderecoDistrito", dbType: DbType.String, value: cadastro.EnderecoDistrito, direction: ParameterDirection.Input);
            param.Add("@EnderecoSetor", dbType: DbType.String, value: cadastro.EnderecoSetor, direction: ParameterDirection.Input);
            param.Add("@EnderecoQuadra", dbType: DbType.String, value: cadastro.EnderecoQuadra, direction: ParameterDirection.Input);
            param.Add("@EnderecoLote", dbType: DbType.String, value: cadastro.EnderecoLote, direction: ParameterDirection.Input);
            param.Add("@EnderecoUnidade", dbType: DbType.String, value: cadastro.EnderecoUnidade, direction: ParameterDirection.Input);
            param.Add("@EnderecoSecao", dbType: DbType.String, value: cadastro.EnderecoSecao, direction: ParameterDirection.Input);
            param.Add("@EnderecoLado", dbType: DbType.String, value: cadastro.EnderecoLado, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {

                var retImovel = _connection.Execute(insertImovel.ToString(), param, transaction);
                foreach (var proprietario in cadastro.ImovelProprietarios)
                {
                    var paramProprietario = new DynamicParameters();
                    paramProprietario.Add("@Id", dbType: DbType.String, value: proprietario.Id, direction: ParameterDirection.Input);
                    paramProprietario.Add("@Inscricao", dbType: DbType.String, value: cadastro.Inscricao, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@RazaoSocialNome", dbType: DbType.String, value: proprietario.RazaoSocialNome, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@CnpjCpf", dbType: DbType.String, value: proprietario.CnpjCpf, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@Telefone", dbType: DbType.String, value: proprietario.Telefone, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@Email", dbType: DbType.String, value: proprietario.Email, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@ResponsavelCobranca", dbType: DbType.Int32, value: proprietario.ResponsavelCobranca, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@EnderecoId", dbType: DbType.String, value: proprietario.Endereco.Id, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@EnderecoNumero", dbType: DbType.String, value: proprietario.EnderecoNumero, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@EnderecoComplemento", dbType: DbType.String, value: proprietario.EnderecoComplemento, direction: ParameterDirection.Input);

                    _connection.Execute(_insertUpdateImovelProprietarios.ToString(), paramProprietario, transaction);
                }
                transaction.Commit();
                return retImovel;
            }
        }

        public int Update(Imovel cadastro, string userId)
        {
            var updateImovel = new StringBuilder(
              @"MERGE Imovel as i
          USING (SELECT @Inscricao, @Status, @EnderecoId, @EnderecoNumero, @EnderecoComplemento, @EnderecoDistrito,
                   @EnderecoSetor, @EnderecoQuadra, @EnderecoLote, @EnderecoUnidade, @EnderecoSecao, @EnderecoLado, @UsuarioCadastro, @UsuarioCadastroData, @UsuarioAlteracao, @UsuarioAlteracaoData)
                 AS Source_(Inscricao, [Status], EnderecoId, EnderecoNumero, EnderecoComplemento, EnderecoDistrito,
                   EnderecoSetor, EnderecoQuadra, EnderecoLote, EnderecoUnidade, EnderecoSecao, EnderecoLado, UsuarioCadastro, UsuarioCadastroData, UsuarioAlteracao, UsuarioAlteracaoData)
          ON (I.Inscricao = Source_.Inscricao)
          WHEN MATCHED  
            THEN UPDATE SET
              [Status] = Source_.[Status],
              EnderecoId = Source_.EnderecoId,
              EnderecoNumero = Source_.EnderecoNumero,
              EnderecoComplemento = Source_.EnderecoComplemento,
              EnderecoDistrito = Source_.EnderecoDistrito,
              EnderecoSetor = Source_.EnderecoSetor,
              EnderecoQuadra = Source_.EnderecoQuadra,
              EnderecoLote = Source_.EnderecoLote,
              EnderecoUnidade = Source_.EnderecoUnidade,
              EnderecoSecao = Source_.EnderecoSecao,
              EnderecoLado = Source_.EnderecoLado,
              UsuarioCadastro = Source_.UsuarioCadastro,
              UsuarioCadastroData = Source_.UsuarioCadastroData,
              UsuarioAlteracao = Source_.UsuarioAlteracao,
              UsuarioAlteracaoData = Source_.UsuarioAlteracaoData;"
            );
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();
                param.Add("@Inscricao", dbType: DbType.String, value: cadastro.Inscricao, direction: ParameterDirection.Input);
                //  param.Add("@Status", dbType: DbType.String, value: cadastro.Status, direction: ParameterDirection.Input);
                param.Add("@EnderecoId", dbType: DbType.String, value: cadastro.Endereco.Id, direction: ParameterDirection.Input);
                param.Add("@EnderecoNumero", dbType: DbType.String, value: cadastro.EnderecoNumero, direction: ParameterDirection.Input);
                param.Add("@EnderecoComplemento", dbType: DbType.String, value: cadastro.EnderecoComplemento, direction: ParameterDirection.Input);
                param.Add("@EnderecoDistrito", dbType: DbType.String, value: cadastro.EnderecoDistrito, direction: ParameterDirection.Input);
                param.Add("@EnderecoSetor", dbType: DbType.String, value: cadastro.EnderecoSetor, direction: ParameterDirection.Input);
                param.Add("@EnderecoQuadra", dbType: DbType.String, value: cadastro.EnderecoQuadra, direction: ParameterDirection.Input);
                param.Add("@EnderecoLote", dbType: DbType.String, value: cadastro.EnderecoLote, direction: ParameterDirection.Input);
                param.Add("@EnderecoUnidade", dbType: DbType.String, value: cadastro.EnderecoUnidade, direction: ParameterDirection.Input);
                param.Add("@EnderecoSecao", dbType: DbType.String, value: cadastro.EnderecoSecao, direction: ParameterDirection.Input);
                param.Add("@EnderecoLado", dbType: DbType.String, value: cadastro.EnderecoLado, direction: ParameterDirection.Input);
                //    param.Add("@UsuarioCadastro", dbType: DbType.String, value: cadastro.UsuarioCadastro, direction: ParameterDirection.Input);
                //    param.Add("@UsuarioCadastroData", dbType: DbType.DateTime, value: cadastro.UsuarioCadastroData, direction: ParameterDirection.Input);
                //    param.Add("@UsuarioAlteracao", dbType: DbType.String, value: cadastro.UsuarioAlteracao, direction: ParameterDirection.Input);
                //   param.Add("@UsuarioAlteracaoData", dbType: DbType.DateTime, value: cadastro.@UsuarioAlteracaoData, direction: ParameterDirection.Input);

                var retImovel = _connection.Execute(updateImovel.ToString(), param, transaction);
                foreach (var proprietario in cadastro.ImovelProprietarios)
                {
                    var paramProprietario = new DynamicParameters();
                    paramProprietario.Add("@Id", dbType: DbType.String, value: proprietario.Id, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@Inscricao", dbType: DbType.String, value: cadastro.Inscricao, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@RazaoSocialNome", dbType: DbType.String, value: proprietario.RazaoSocialNome, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@CnpjCpf", dbType: DbType.String, value: proprietario.CnpjCpf, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@Telefone", dbType: DbType.String, value: proprietario.Telefone, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@Email", dbType: DbType.String, value: proprietario.Email, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@ResponsavelCobranca", dbType: DbType.Int32, value: proprietario.ResponsavelCobranca, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@EnderecoId", dbType: DbType.String, value: proprietario.Endereco.Id, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@EnderecoNumero", dbType: DbType.String, value: proprietario.EnderecoNumero, direction: ParameterDirection.Input);
                    //paramProprietario.Add("@EnderecoComplemento", dbType: DbType.String, value: proprietario.EnderecoComplemento, direction: ParameterDirection.Input);

                    _connection.Execute(_insertUpdateImovelProprietarios.ToString(), paramProprietario, transaction);
                }
                transaction.Commit();
                return retImovel;
            }
        }

        public Imovel GetById(string id)
        {
            #region -- Filtro
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (!string.IsNullOrEmpty(id))
            {
                queryFilter.Append(" And Imovel.Inscricao = @Inscricao");
                param.Add("@Inscricao", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            }

            _queryAll.Append(queryFilter);
            #endregion

            var imoveis = new Dictionary<string, Imovel>();
            _connection.Query<Imovel, Endereco, Cidade, ImovelProprietario, Endereco, Cidade, Imovel>(
              _queryAll.ToString(),
              (i, endereco, cidade, imovelProprietario, e, c) =>
              {
                  if (!imoveis.TryGetValue(i.Inscricao, out var imovelEntity))
                  {
                      imoveis.Add(i.Inscricao, imovelEntity = i);
                  }

                  if (imovelEntity.Endereco == null)
                  {
                      if (endereco != null)
                      {
                          imovelEntity.Endereco = endereco;
                          if (cidade != null)
                          {
                              // imovelEntity.Endereco.Cidade = cidade;
                          }
                      }
                  }

                  if (imovelEntity.ImovelProprietarios == null)
                  {
                      imovelEntity.ImovelProprietarios = new List<ImovelProprietario>();
                  }

                  if (imovelProprietario == null) return imovelEntity;
                  //if (e != null)
                  //{
                  //  imovelProprietario.Endereco = e;
                  //  if (c != null)
                  //  {
                  //    imovelProprietario.Endereco.Cidade = c;
                  //  }
                  //}

                  if (!imovelEntity.ImovelProprietarios.Any(x => x.Id == imovelProprietario.Id))
                  {
                      imovelEntity.ImovelProprietarios.Add(imovelProprietario);
                  }

                  return imovelEntity;
              }, param,
              splitOn: "Id, Ibge, Id, Id, Ibge",
              commandTimeout: 60, commandType: CommandType.Text);

            return imoveis.Values.FirstOrDefault();

        }

        public IEnumerable<Imovel> GetByParam(Imovel filters, string sort, int? currentPage, int? pageSize, out int totalCount)
        {
            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" Where 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Inscricao))
                {
                    queryFilter.Append(" And Imovel.Inscricao = @Inscricao");
                    param.Add("@Inscricao", dbType: DbType.String, value: filters.Inscricao, direction: ParameterDirection.Input);
                }

                //if (filters.ImovelProprietarios != null)
                //{
                //  if (!string.IsNullOrEmpty(filters.ImovelProprietarios.First().RazaoSocialNome))
                //  {
                //    queryFilter.Append(" And Contains(ImovelProprietario.RazaoSocialNome,  @RazaoSocialNome)");
                //    param.Add("@RazaoSocialNome", dbType: DbType.String, value: filters.ImovelProprietarios.First().RazaoSocialNome,
                //      direction: ParameterDirection.Input);
                //  }

                //  if (!string.IsNullOrEmpty(filters.ImovelProprietarios.First().CnpjCpf))
                //  {
                //    queryFilter.Append(" And ImovelProprietario.CnpjCpf = @CnpjCpf");
                //    param.Add("@CnpjCpf", dbType: DbType.String, value: filters.ImovelProprietarios.First().CnpjCpf,
                //      direction: ParameterDirection.Input);
                //  }
                //}
            }
            _queryAll.Append(queryFilter);
            _queryCountAll.Append(queryFilter);

            if (currentPage.HasValue && pageSize.HasValue)
            {
                _queryAll.Append(" ORDER BY Imovel.Inscricao OFFSET ((@currentPage - 1) * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY;");
                param.Add("@currentPage", dbType: DbType.Int32, value: currentPage, direction: ParameterDirection.Input);
                param.Add("@pageSize", dbType: DbType.Int32, value: pageSize, direction: ParameterDirection.Input);
            }
            #endregion

            var imoveis = new Dictionary<string, Imovel>();
            totalCount = _connection.Query<int>(_queryCountAll.ToString(), param, commandTimeout: 60, commandType: CommandType.Text).SingleOrDefault();

            _connection.Query<Imovel, Endereco, Cidade, ImovelProprietario, Endereco, Cidade, Imovel>(
             _queryAll.ToString(),
             (imovel, endereco, cidade, imovelProprietario, e, c) =>
             {
                 if (!imoveis.TryGetValue(imovel.Inscricao, out var imovelEntity))
                 {
                     imoveis.Add(imovel.Inscricao, imovelEntity = imovel);
                 }

                 if (imovelEntity.Endereco == null)
                 {
                     if (endereco != null)
                     {
                         imovelEntity.Endereco = endereco;
                         if (cidade != null)
                         {
                             // imovelEntity.Endereco.Cidade = cidade;
                         }
                     }
                 }

                 if (imovelEntity.ImovelProprietarios == null)
                 {
                     imovelEntity.ImovelProprietarios = new List<ImovelProprietario>();
                 }

                 //if (imovelProprietario == null) return imovelEntity;
                 //if (e != null)
                 //{
                 //  imovelProprietario.Endereco = e;
                 //  if (c != null)
                 //  {
                 //    imovelProprietario.Endereco.Cidade = c;
                 //  }
                 //}

                 if (imovelEntity.ImovelProprietarios.All(x => x.Id != imovelProprietario.Id))
                 {
                     imovelEntity.ImovelProprietarios.Add(imovelProprietario);
                 }

                 return imovelEntity;
             }, param,
             splitOn: "Id, Ibge, Id, Id, Ibge",
              commandTimeout: 60, commandType: CommandType.Text);
            return totalCount == 0 ? null : imoveis.Values;
        }
    }
}
