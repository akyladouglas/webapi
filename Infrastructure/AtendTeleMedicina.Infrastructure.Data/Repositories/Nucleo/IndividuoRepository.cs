using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Base;
using Dapper;
using MySql.Data.MySqlClient;

namespace AtendTeleMedicina.Infra.Data.Repositories.Nucleo
{
    public class IndividuoRepository : BaseRepository, IIndividuoRepository
    {
        private readonly IUser _user;

        private readonly StringBuilder _queryAll = new StringBuilder(
       @"SELECT
            CAST(i.Id AS CHAR) AS Id,
            CAST(i.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
            i.Cpf,
            i.Cpf AS UserName,
            i.Cns,
            i.Email,
            i.CodigoAutenticacao,
            i.TelefoneCelular,
            i.NomeCompleto,
            i.NomeDaMae,
            i.NomeSocial,
            i.DataNascimento,
            TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
            i.Sexo,
            i.RacaOuCor,
            i.DataCadastro,
            i.DataAlteracao,
            i.Latitude,
            i.Longitude,
            i.Logradouro,
            i.LogradouroNumero,
            i.LogradouroComplemento,
            i.LogradouroCep,
            i.LogradouroBairro,
            i.UfAbreviado,
            i.CidadeId,
            i.Ativo,
            i.RespondeuComorbidade,
            i.Comorbidades,
            i.Hipertenso,
            i.Diabetes,
            i.Fumante,
            i.Asma,
            i.DoencaCoracao,
            i.DoencaPulmao,
            i.DoencaRins,
            i.DoencaFigado,
            i.DoencaCancer,
            i.Transplantado,
            i.DoencaComprometeImunidade,
            i.LugarComCasosCorona,
            i.DataInicioSintomas,
            i.Obesidade,
            i.Gestante,
            i.DoencaNeurologica,
            i.AnemiaFalciforme,
            i.NotificacaoToken,
            i.Face,
            i.DocumentFront,
            i.DocumentBack,
            i.Imagem,
            i.NomeSocial,
            i.TemMaeDesconhecida,
            i.TemPaiDesconhecido,   
            CAST(i.FaceToken AS CHAR) AS FaceToken,
            i.FaceTokenValidade,
            c.Ibge, c.Nome, c.UfAbreviado, c.UfExtenso,
            CAST(uc.Id AS CHAR) AS Id,
            CAST(uc.IndividuoId AS CHAR) AS IndividuoId,
            uc.ClaimType,
            uc.ClaimValue
            FROM Individuo i
            LEFT JOIN Cidade c ON c.Ibge = i.CidadeId
            LEFT JOIN UsuarioClaim uc ON uc.IndividuoId = i.Id ");

        private readonly StringBuilder _queryCountAll = new StringBuilder(
      @"SELECT COUNT(i.Id) FROM Individuo i");
        private readonly MySqlConnection _connection;

        public IndividuoRepository(IMySqlContext context, IUser user)
            : base(context)
        {
            _user = user;
            _connection = context.Connection;
        }

        public int Add(Individuo obj)
        {

            if (!string.IsNullOrEmpty(obj.Cpf) && CheckCPF(obj.Cpf)) throw new Exception("CPF já cadastrado.");
            if (!string.IsNullOrEmpty(obj.Cns) && CheckCNS(obj.Cns)) throw new Exception("CNS já cadastrado.");
            if (string.IsNullOrEmpty(obj.UfAbreviado)) { obj.UfAbreviado = "NI"; };
            if (obj.CidadeId == "" || obj.CidadeId == null) { obj.CidadeId = "1100014"; }
            //if (!string.IsNullOrEmpty(obj.Email) && CheckEmail(obj.Email)) throw new Exception("Email já cadastrado");
            if (!string.IsNullOrEmpty(obj.TelefoneCelular) && CheckTelefone(obj.TelefoneCelular)) throw new Exception("Telefone já cadastrado.");

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
            param.Add("?EstabelecimentoId", dbType: DbType.String, value: obj.EstabelecimentoId, direction: ParameterDirection.Input);
            param.Add("?Cpf", dbType: DbType.String, value: obj.Cpf, direction: ParameterDirection.Input);
            param.Add("?Cns", dbType: DbType.String, value: obj.Cns, direction: ParameterDirection.Input);
            param.Add("?Email", dbType: DbType.String, value: obj.Email, direction: ParameterDirection.Input);
            param.Add("?CodigoAutenticacao", dbType: DbType.String, value: obj.CodigoAutenticacao, direction: ParameterDirection.Input);
            param.Add("?TelefoneCelular", dbType: DbType.String, value: obj.TelefoneCelular, direction: ParameterDirection.Input);
            param.Add("?NomeCompleto", dbType: DbType.String, value: obj.NomeCompleto, direction: ParameterDirection.Input);
            param.Add("?NomeDaMae", dbType: DbType.String, value: obj.NomeDaMae, direction: ParameterDirection.Input);
            param.Add("?NomeSocial", dbType: DbType.String, value: obj.NomeSocial, direction: ParameterDirection.Input);
            param.Add("?DataNascimento", dbType: DbType.DateTime, value: obj.DataNascimento, direction: ParameterDirection.Input);
            param.Add("?Sexo", dbType: DbType.Boolean, value: obj.Sexo, direction: ParameterDirection.Input);
            param.Add("?RacaOuCor", dbType: DbType.Int32, value: obj.RacaOuCor, direction: ParameterDirection.Input);
            param.Add("?DataCadastro", dbType: DbType.DateTime, value: obj.DataCadastro, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
            param.Add("?DataInicioSintomas", dbType: DbType.DateTime, value: obj.DataInicioSintomas, direction: ParameterDirection.Input);
            param.Add("?Latitude", dbType: DbType.Double, value: obj.Latitude, direction: ParameterDirection.Input);
            param.Add("?Longitude", dbType: DbType.Double, value: obj.Longitude, direction: ParameterDirection.Input);
            param.Add("?Logradouro", dbType: DbType.String, value: obj.Logradouro, direction: ParameterDirection.Input);
            param.Add("?LogradouroNumero", dbType: DbType.String, value: obj.LogradouroNumero, direction: ParameterDirection.Input);
            param.Add("?LogradouroComplemento", dbType: DbType.String, value: obj.LogradouroComplemento, direction: ParameterDirection.Input);
            param.Add("?LogradouroCep", dbType: DbType.String, value: obj.LogradouroCep, direction: ParameterDirection.Input);
            param.Add("?LogradouroBairro", dbType: DbType.String, value: obj.LogradouroBairro, direction: ParameterDirection.Input);
            param.Add("?UfAbreviado", dbType: DbType.String, value: obj.UfAbreviado, direction: ParameterDirection.Input);
            param.Add("?CidadeId", dbType: DbType.String, value: obj.CidadeId, direction: ParameterDirection.Input);
            param.Add("?Nacionalidade", dbType: DbType.Int32, value: obj.Nacionalidade, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);
            param.Add("?RespondeuComorbidade", dbType: DbType.Boolean, value: obj.RespondeuComorbidade, direction: ParameterDirection.Input);
            param.Add("?Comorbidades", dbType: DbType.String, value: obj.Comorbidades, direction: ParameterDirection.Input);
            param.Add("?Hipertenso", dbType: DbType.Boolean, value: obj.Hipertenso, direction: ParameterDirection.Input);
            param.Add("?Diabetes", dbType: DbType.Boolean, value: obj.Diabetes, direction: ParameterDirection.Input);
            param.Add("?Fumante", dbType: DbType.Boolean, value: obj.Fumante, direction: ParameterDirection.Input);
            param.Add("?Asma", dbType: DbType.Boolean, value: obj.Asma, direction: ParameterDirection.Input);
            param.Add("?DoencaCoracao", dbType: DbType.Boolean, value: obj.DoencaCoracao, direction: ParameterDirection.Input);
            param.Add("?DoencaPulmao", dbType: DbType.Boolean, value: obj.DoencaPulmao, direction: ParameterDirection.Input);
            param.Add("?DoencaRins", dbType: DbType.Boolean, value: obj.DoencaRins, direction: ParameterDirection.Input);
            param.Add("?DoencaFigado", dbType: DbType.Boolean, value: obj.DoencaFigado, direction: ParameterDirection.Input);
            param.Add("?DoencaCancer", dbType: DbType.Boolean, value: obj.DoencaCancer, direction: ParameterDirection.Input);
            param.Add("?Transplantado", dbType: DbType.Boolean, value: obj.Transplantado, direction: ParameterDirection.Input);
            param.Add("?DoencaComprometeImunidade", dbType: DbType.Boolean, value: obj.DoencaComprometeImunidade, direction: ParameterDirection.Input);
            param.Add("?LugarComCasosCorona", dbType: DbType.Boolean, value: obj.LugarComCasosCorona, direction: ParameterDirection.Input);
            param.Add("?Obesidade", dbType: DbType.Boolean, value: obj.Obesidade, direction: ParameterDirection.Input);
            param.Add("?Gestante", dbType: DbType.Boolean, value: obj.Gestante, direction: ParameterDirection.Input);
            param.Add("?DoencaNeurologica", dbType: DbType.Boolean, value: obj.DoencaNeurologica, direction: ParameterDirection.Input);
            param.Add("?AnemiaFalciforme", dbType: DbType.Boolean, value: obj.AnemiaFalciforme, direction: ParameterDirection.Input);
            param.Add("?Imagem", dbType: DbType.String, value: obj.Imagem, direction: ParameterDirection.Input);
            param.Add("?Senha", dbType: DbType.String, value: obj.Senha, direction: ParameterDirection.Input);
            param.Add("?NomeDaMae", dbType: DbType.String, value: obj.NomeDaMae, direction: ParameterDirection.Input);
            param.Add("?TemMaeDesconhecida", dbType: DbType.Boolean, value: obj.TemMaeDesconhecida, direction: ParameterDirection.Input);
            param.Add("?NomeDoPai", dbType: DbType.String, value: obj.NomeDoPai, direction: ParameterDirection.Input);
            param.Add("?TemPaiDesconhecido", dbType: DbType.Boolean, value: obj.TemPaiDesconhecido, direction: ParameterDirection.Input);
            param.Add("?Nacionalidade", dbType: DbType.Int32, value: obj.Nacionalidade, direction: ParameterDirection.Input);
            param.Add("?UfDeNascimentoAbreviado", dbType: DbType.String, value: obj.UfDeNascimentoAbreviado, direction: ParameterDirection.Input);
            param.Add("?CidadeDeNascimentoIbge", dbType: DbType.Int32, value: obj.CidadeDeNascimentoIbge, direction: ParameterDirection.Input);
            param.Add("?NaturalizacaoData", dbType: DbType.DateTime, value: obj.NaturalizacaoData, direction: ParameterDirection.Input);
            param.Add("?NaturalizacaoPortaria", dbType: DbType.String, value: obj.NaturalizacaoPortaria, direction: ParameterDirection.Input);
            param.Add("?PaisDeNascimento", dbType: DbType.Int32, value: obj.PaisDeNascimento, direction: ParameterDirection.Input);
            param.Add("?DataEntradaNoPais", dbType: DbType.DateTime, value: obj.DataEntradaNoPais, direction: ParameterDirection.Input);
            param.Add("?FrequentaEscola", dbType: DbType.Boolean, value: obj.FrequentaEscola, direction: ParameterDirection.Input);
            param.Add("?GrauDeInstrucao", dbType: DbType.Int32, value: obj.GrauDeInstrucao, direction: ParameterDirection.Input);
            param.Add("?PisPasep", dbType: DbType.String, value: obj.PisPasep, direction: ParameterDirection.Input);

            const string insertQuery = @"
              INSERT INTO Individuo (
                                    Id,
                                    EstabelecimentoId,
                                    Cpf,
                                    Cns,
                                    Email,
                                    CodigoAutenticacao,
                                    TelefoneCelular,
                                    NomeCompleto,
                                    NomeSocial,
                                    DataNascimento,
                                    Sexo,
                                    RacaOuCor,
                                    DataCadastro,
                                    DataAlteracao,
                                    DataInicioSintomas,
                                    Latitude,
                                    Longitude,
                                    Logradouro,
                                    LogradouroNumero,
                                    LogradouroComplemento,
                                    LogradouroCep,
                                    LogradouroBairro,
                                    UfAbreviado,
                                    CidadeId,
                                    Ativo,
                                    RespondeuComorbidade,
                                    Comorbidades,
                                    Hipertenso,
                                    Diabetes,
                                    Fumante,
                                    Asma,
                                    DoencaCoracao,
                                    DoencaPulmao,
                                    DoencaRins,
                                    DoencaFigado,
                                    DoencaCancer,
                                    Transplantado,
                                    DoencaComprometeImunidade,
                                    LugarComCasosCorona,
                                    Obesidade,
                                    Gestante,
                                    DoencaNeurologica,
                                    AnemiaFalciforme,
                                    Imagem,
                                    Senha,
                                    NomeDaMae,
                                    TemMaeDesconhecida,
                                    NomeDoPai,
                                    TemPaiDesconhecido,
                                    Nacionalidade,
                                    UfDeNascimentoAbreviado,
                                    CidadeDeNascimentoIbge,
                                    NaturalizacaoData,
                                    NaturalizacaoPortaria,
                                    PaisDeNascimento,
                                    DataEntradaNoPais,
                                    FrequentaEscola,
                                    GrauDeInstrucao,
                                    PisPasep
                                    ) 
                             VALUES (
                                    ?Id,
                                    ?EstabelecimentoId,
                                    ?Cpf,
                                    ?Cns,
                                    ?Email,
                                    ?CodigoAutenticacao,
                                    ?TelefoneCelular,
                                    ?NomeCompleto,
                                    ?NomeSocial,
                                    ?DataNascimento,
                                    ?Sexo,
                                    ?RacaOuCor,
                                    ?DataCadastro,
                                    ?DataAlteracao,
                                    ?DataInicioSintomas,
                                    ?Latitude,
                                    ?Longitude,
                                    ?Logradouro,
                                    ?LogradouroNumero,
                                    ?LogradouroComplemento,
                                    ?LogradouroCep,
                                    ?LogradouroBairro,
                                    ?UfAbreviado,
                                    ?CidadeId,
                                    ?Ativo,
                                    ?RespondeuComorbidade,
                                    ?Comorbidades,
                                    ?Hipertenso,
                                    ?Diabetes,
                                    ?Fumante,
                                    ?Asma,
                                    ?DoencaCoracao,
                                    ?DoencaPulmao,
                                    ?DoencaRins,
                                    ?DoencaFigado,
                                    ?DoencaCancer,
                                    ?Transplantado,
                                    ?DoencaComprometeImunidade,
                                    ?LugarComCasosCorona,
                                    ?Obesidade,
                                    ?Gestante,
                                    ?DoencaNeurologica,
                                    ?AnemiaFalciforme,
                                    ?Imagem,
                                    ?Senha,
                                    ?NomeDaMae,
                                    ?TemMaeDesconhecida,
                                    ?NomeDoPai,
                                    ?TemPaiDesconhecido,
                                    ?Nacionalidade,
                                    ?UfDeNascimentoAbreviado,
                                    ?CidadeDeNascimentoIbge,
                                    ?NaturalizacaoData,
                                    ?NaturalizacaoPortaria,
                                    ?PaisDeNascimento,
                                    ?DataEntradaNoPais,
                                    ?FrequentaEscola,
                                    ?GrauDeInstrucao,
                                    ?PisPasep
                                    )";

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(insertQuery, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", obj.Id.ToString(), "Inseriu Indivíduo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                foreach (var item in obj.UserClaims)
                    item.IndividuoId = obj.Id;

                var addClaims = "INSERT INTO UsuarioClaim (Id, IndividuoId, ClaimType, ClaimValue) VALUES (@Id, @IndividuoId, @ClaimType, @ClaimValue)";
                _connection.Execute(addClaims, obj.UserClaims, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "UsuarioClaim", obj.Id.ToString(), "Inseriu as Claims do Indivíduo.", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                transaction.Commit();
                return ret;
            }
        }

        public int Update(string id, Individuo obj)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (obj == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?Cpf", dbType: DbType.String, value: obj.Cpf, direction: ParameterDirection.Input);
            param.Add("?Cns", dbType: DbType.String, value: obj.Cns, direction: ParameterDirection.Input);
            param.Add("?Email", dbType: DbType.String, value: obj.Email, direction: ParameterDirection.Input);
            param.Add("?TelefoneCelular", dbType: DbType.String, value: obj.TelefoneCelular, direction: ParameterDirection.Input);
            param.Add("?NomeCompleto", dbType: DbType.String, value: obj.NomeCompleto, direction: ParameterDirection.Input);
            param.Add("?NomeDaMae", dbType: DbType.String, value: obj.NomeDaMae, direction: ParameterDirection.Input);
            param.Add("?NomeSocial", dbType: DbType.String, value: obj.NomeSocial, direction: ParameterDirection.Input);
            param.Add("?DataNascimento", dbType: DbType.DateTime, value: obj.DataNascimento, direction: ParameterDirection.Input);
            param.Add("?Sexo", dbType: DbType.Boolean, value: obj.Sexo, direction: ParameterDirection.Input);
            param.Add("?RacaOuCor", dbType: DbType.Int32, value: obj.RacaOuCor, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao, direction: ParameterDirection.Input);
            param.Add("?Latitude", dbType: DbType.Double, value: obj.Latitude, direction: ParameterDirection.Input);
            param.Add("?Longitude", dbType: DbType.Double, value: obj.Longitude, direction: ParameterDirection.Input);
            param.Add("?Logradouro", dbType: DbType.String, value: obj.Logradouro, direction: ParameterDirection.Input);
            param.Add("?LogradouroNumero", dbType: DbType.String, value: obj.LogradouroNumero, direction: ParameterDirection.Input);
            param.Add("?LogradouroComplemento", dbType: DbType.String, value: obj.LogradouroComplemento, direction: ParameterDirection.Input);
            param.Add("?LogradouroCep", dbType: DbType.String, value: obj.LogradouroCep, direction: ParameterDirection.Input);
            param.Add("?LogradouroBairro", dbType: DbType.String, value: obj.LogradouroBairro, direction: ParameterDirection.Input);
            param.Add("?UfAbreviado", dbType: DbType.String, value: obj.UfAbreviado, direction: ParameterDirection.Input);
            param.Add("?CidadeId", dbType: DbType.Int32, value: obj.CidadeId, direction: ParameterDirection.Input);
            param.Add("?CidadeDeNascimentoIbge", dbType: DbType.Int32, value: obj.CidadeDeNascimentoIbge, direction: ParameterDirection.Input);
            param.Add("?Nacionalidade", dbType: DbType.Int32, value: obj.Nacionalidade, direction: ParameterDirection.Input);
            param.Add("?Ativo", dbType: DbType.Boolean, value: obj.Ativo, direction: ParameterDirection.Input);
            param.Add("?RespondeuComorbidade", dbType: DbType.Boolean, value: obj.RespondeuComorbidade, direction: ParameterDirection.Input);
            param.Add("?Comorbidades", dbType: DbType.String, value: obj.Comorbidades, direction: ParameterDirection.Input);
            param.Add("?Hipertenso", dbType: DbType.Boolean, value: obj.Hipertenso, direction: ParameterDirection.Input);
            param.Add("?Diabetes", dbType: DbType.Boolean, value: obj.Diabetes, direction: ParameterDirection.Input);
            param.Add("?Fumante", dbType: DbType.Boolean, value: obj.Fumante, direction: ParameterDirection.Input);
            param.Add("?Asma", dbType: DbType.Boolean, value: obj.Asma, direction: ParameterDirection.Input);
            param.Add("?DoencaCoracao", dbType: DbType.Boolean, value: obj.DoencaCoracao, direction: ParameterDirection.Input);
            param.Add("?DoencaPulmao", dbType: DbType.Boolean, value: obj.DoencaPulmao, direction: ParameterDirection.Input);
            param.Add("?DoencaRins", dbType: DbType.Boolean, value: obj.DoencaRins, direction: ParameterDirection.Input);
            param.Add("?DoencaFigado", dbType: DbType.Boolean, value: obj.DoencaFigado, direction: ParameterDirection.Input);
            param.Add("?DoencaCancer", dbType: DbType.Boolean, value: obj.DoencaCancer, direction: ParameterDirection.Input);
            param.Add("?Transplantado", dbType: DbType.Boolean, value: obj.Transplantado, direction: ParameterDirection.Input);
            param.Add("?DoencaComprometeImunidade", dbType: DbType.Boolean, value: obj.DoencaComprometeImunidade, direction: ParameterDirection.Input);
            param.Add("?LugarComCasosCorona", dbType: DbType.Boolean, value: obj.LugarComCasosCorona, direction: ParameterDirection.Input);
            param.Add("?DataInicioSintomas", dbType: DbType.DateTime, value: obj.DataInicioSintomas, direction: ParameterDirection.Input);
            param.Add("?Obesidade", dbType: DbType.Boolean, value: obj.Obesidade, direction: ParameterDirection.Input);
            param.Add("?Gestante", dbType: DbType.Boolean, value: obj.Gestante, direction: ParameterDirection.Input);
            param.Add("?DoencaNeurologica", dbType: DbType.Boolean, value: obj.DoencaNeurologica, direction: ParameterDirection.Input);
            param.Add("?AnemiaFalciforme", dbType: DbType.Boolean, value: obj.AnemiaFalciforme, direction: ParameterDirection.Input);
            param.Add("?Imagem", dbType: DbType.String, value: obj.Imagem, direction: ParameterDirection.Input);
            param.Add("?TemMaeDesconhecida", dbType: DbType.Boolean, value: obj.TemMaeDesconhecida, direction: ParameterDirection.Input);
            param.Add("?NomeDoPai", dbType: DbType.String, value: obj.NomeDoPai, direction: ParameterDirection.Input);
            param.Add("?TemPaiDesconhecido", dbType: DbType.Boolean, value: obj.TemPaiDesconhecido, direction: ParameterDirection.Input);
            param.Add("?Nacionalidade", dbType: DbType.Int32, value: obj.Nacionalidade, direction: ParameterDirection.Input);
            param.Add("?UfDeNascimentoAbreviado", dbType: DbType.String, value: obj.UfDeNascimentoAbreviado, direction: ParameterDirection.Input);
            param.Add("?NaturalizacaoData", dbType: DbType.DateTime, value: obj.NaturalizacaoData, direction: ParameterDirection.Input);
            param.Add("?NaturalizacaoPortaria", dbType: DbType.String, value: obj.NaturalizacaoPortaria, direction: ParameterDirection.Input);
            param.Add("?PaisDeNascimento", dbType: DbType.Int32, value: obj.PaisDeNascimento, direction: ParameterDirection.Input);
            param.Add("?DataEntradaNoPais", dbType: DbType.DateTime, value: obj.DataEntradaNoPais, direction: ParameterDirection.Input);
            param.Add("?FrequentaEscola", dbType: DbType.Boolean, value: obj.FrequentaEscola, direction: ParameterDirection.Input);
            param.Add("?GrauDeInstrucao", dbType: DbType.Int32, value: obj.GrauDeInstrucao, direction: ParameterDirection.Input);
            param.Add("?PisPasep", dbType: DbType.String, value: obj.PisPasep, direction: ParameterDirection.Input);
            param.Add("?DadoSerializado", dbType: DbType.String, value: obj.DadoSerializado, direction: ParameterDirection.Input);
            param.Add("?LoteIntegracaoId", dbType: DbType.Int32, value: obj.LoteIntegracaoId, direction: ParameterDirection.Input);


            const string updateQuery = @"
              UPDATE Individuo SET
                    Cpf = ?Cpf,
                    Cns = ?Cns,
                    Email = ?Email,
                    TelefoneCelular = ?TelefoneCelular,
                    NomeCompleto = ?NomeCompleto,
                    NomeDaMae = ?NomeDaMae,
                    NomeSocial = ?NomeSocial,
                    DataNascimento = ?DataNascimento,
                    Sexo = ?Sexo,
                    RacaOuCor = ?RacaOuCor,
                    DataAlteracao = ?DataAlteracao,
                    Latitude = ?Latitude,
                    Longitude = ?Longitude,
                    Logradouro = ?Logradouro,
                    LogradouroNumero = ?LogradouroNumero,
                    LogradouroComplemento = ?LogradouroComplemento,
                    LogradouroCep = ?LogradouroCep,
                    LogradouroBairro = ?LogradouroBairro,
                    UfAbreviado = ?UfAbreviado,
                    CidadeId = ?CidadeId,
                    Ativo = ?Ativo,
                    RespondeuComorbidade = ?RespondeuComorbidade,
                    Comorbidades = ?Comorbidades,
                    Hipertenso = ?Hipertenso,
                    Diabetes = ?Diabetes,
                    Fumante = ?Fumante,
                    Asma = ?Asma,
                    DoencaCoracao = ?DoencaCoracao,
                    DoencaPulmao = ?DoencaPulmao,
                    DoencaRins = ?DoencaRins,
                    DoencaFigado = ?DoencaFigado,
                    DoencaCancer = ?DoencaCancer,
                    Transplantado = ?Transplantado,
                    DoencaComprometeImunidade = ?DoencaComprometeImunidade,
                    LugarComCasosCorona = ?LugarComCasosCorona,
                    DataInicioSintomas = ?DataInicioSintomas,
                    Obesidade = ?Obesidade,
                    Gestante = ?Gestante,
                    DoencaNeurologica = ?DoencaNeurologica,
                    AnemiaFalciforme = ?AnemiaFalciforme,
                    Imagem = ?Imagem,
                    NomeDaMae = ?NomeDaMae,
                    TemMaeDesconhecida = ?TemMaeDesconhecida,
                    NomeDoPai = ?NomeDoPai,
                    TemPaiDesconhecido = ?TemPaiDesconhecido,
                    Nacionalidade = ?Nacionalidade,
                    UfDeNascimentoAbreviado = ?UfDeNascimentoAbreviado,
                    CidadeDeNascimentoIbge = ?CidadeDeNascimentoIbge,
                    NaturalizacaoData = ?NaturalizacaoData,
                    NaturalizacaoPortaria = ?NaturalizacaoPortaria,
                    PaisDeNascimento = ?PaisDeNascimento,
                    DataEntradaNoPais = ?DataEntradaNoPais,
                    FrequentaEscola = ?FrequentaEscola,
                    GrauDeInstrucao = ?GrauDeInstrucao,
                    PisPasep = ?PisPasep,
                    LoteIntegracaoId = NULL,
                    DadoSerializado = NULL
                    WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {

                    //if (obj.UserClaims.Count() != 0)
                    //{
                    //    var delClaims = "DELETE FROM UsuarioClaim WHERE IndividuoId = ?Id";
                    //    _connection.Execute(delClaims, param, transaction);
                    //}
                    //if (obj.UserClaims.Any())
                    //{
                    //    foreach (var item in obj.UserClaims)
                    //        item.IndividuoId = obj.Id;

                    //    var addClaims = "INSERT INTO UsuarioClaim (Id, IndividuoId, ClaimType, ClaimValue) VALUES (@Id, @IndividuoId, @ClaimType, @ClaimValue)";
                    //    _connection.Execute(addClaims, obj.UserClaims, transaction);
                    //    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "UsuarioClaim", obj.Id.ToString(), "Atualizou Claims do Indivíduo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    //}

                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", id, "Editou Individuo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp(), obj.Justificativa);

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public int UpdateNotificacaoToken(string id, string token)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (token == null) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?NotificacaoToken", dbType: DbType.String, value: token, direction: ParameterDirection.Input);


            const string updateQuery = @"
              UPDATE Individuo SET
                    NotificacaoToken = ?NotificacaoToken
                    WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", id, "Editou NotificacaoToken Individuo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public int Update(string id, string password, bool changePass)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (string.IsNullOrEmpty(password)) return 0;

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            param.Add("?Senha", dbType: DbType.String, value: password, direction: ParameterDirection.Input);


            const string updateQuery = @"
              UPDATE Individuo SET
                    Senha = ?Senha
                    WHERE Id = ?Id";

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {


                    var ret = _connection.Execute(updateQuery, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", id, "Atualizou a Senha do Indivíduo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Individuo GetById(string id)
        {
            var query = new StringBuilder(@"SELECT
                                    CAST(i.Id AS CHAR) AS Id,
                                    i.Cpf,
                                    i.Cpf AS UserName,
                                    i.Cns,
                                    i.Email,
                                    i.CodigoAutenticacao,
                                    i.TelefoneCelular,
                                    i.NomeCompleto,
                                    i.NomeDaMae,
                                    i.NomeDoPai,
                                    i.NomeSocial,
                                    i.DataNascimento,
                                    TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
                                    i.Sexo,
                                    i.RacaOuCor,
                                    i.DataCadastro,
                                    i.DataAlteracao,
                                    i.Latitude,
                                    i.Longitude,
                                    i.Logradouro,
                                    i.LogradouroNumero,
                                    i.LogradouroComplemento,
                                    i.LogradouroCep,
                                    i.LogradouroBairro,
                                    i.UfAbreviado,
                                    i.CidadeId,
                                    i.Ativo,
                                    i.RespondeuComorbidade,
                                    i.Comorbidades,
                                    i.Hipertenso,
                                    i.Diabetes,
                                    i.Fumante,
                                    i.Asma,
                                    i.DoencaCoracao,
                                    i.DoencaPulmao,
                                    i.DoencaRins,
                                    i.DoencaFigado,
                                    i.DoencaCancer,
                                    i.Transplantado,
                                    i.DoencaComprometeImunidade,
                                    i.LugarComCasosCorona,
                                    i.DataInicioSintomas,
                                    i.Obesidade,
                                    i.Gestante,
                                    i.DoencaNeurologica,
                                    i.AnemiaFalciforme,
                                    i.NotificacaoToken,
                                    i.Face,
                                    i.DocumentFront,
                                    i.DocumentBack,
                                    i.Imagem,
                                    i.NomeSocial,
                                    i.TemMaeDesconhecida,
                                    i.TemPaiDesconhecido,   
                                    i.FrequentaEscola,
                                    i.Nacionalidade,   
                                    i.GrauDeInstrucao,
                                    i.PisPasep,
                                    i.UfDeNascimentoAbreviado,
                                    i.CidadeDeNascimentoIbge,
                                    i.NaturalizacaoData,
                                    i.NaturalizacaoPortaria,
                                    i.PaisDeNascimento,
                                    i.DataEntradaNoPais,
                                    i.DadoSerializado,
                                    i.LoteIntegracaoId,
                                    CAST(i.FaceToken AS CHAR) AS FaceToken,
                                    i.FaceTokenValidade,
                                    CAST(a.Id AS CHAR) AS Id,
                                    CAST(a.IndividuoId AS CHAR) AS IndividuoId,
                                    CAST(a.ProcedimentoId AS CHAR) AS ProcedimentoId,
                                    a.Autorizado,
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
                                    FROM Individuo i
                                    LEFT JOIN IndividuoProcedimentoAutorizacao a ON a.IndividuoId = i.Id
                                    LEFT JOIN Procedimento ON Procedimento.Id = a.ProcedimentoId ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(id)) return null;

            queryFilter.Append(" AND i.Id = ?Id");
            param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var dictionary = new Dictionary<string, Individuo>();

            try
            {
                return _connection.Query<Individuo>(query.ToString(),
                     new[]
                     {
              typeof(Individuo), typeof(IndividuoProcedimentoAutorizacao), typeof(Procedimento)
                     },
                     objects =>
                     {
                         var individuo = objects[0] as Individuo;
                         var autorizacao = objects[1] as IndividuoProcedimentoAutorizacao;
                         var procedimento = objects[2] as Procedimento;

                         if (!dictionary.TryGetValue(individuo.Id, out var entity))
                         {
                             entity = individuo;
                             if (entity.Autorizacoes == null)
                                 entity.Autorizacoes = new List<IndividuoProcedimentoAutorizacao>();

                             dictionary.Add(individuo.Id, entity);
                         }

                         if (autorizacao != null)
                         {
                             if (!entity.Autorizacoes.Any(x => x.Id == autorizacao.Id))
                             {
                                 if (procedimento != null)
                                     autorizacao.Procedimento = procedimento;
                                 entity.Autorizacoes.Add(autorizacao);
                             }
                         }
                         return entity;
                     }, param, splitOn: "Id, Id, Id", commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Individuo GetByCpf(string cpf)
        {
            var query = new StringBuilder(
               @"SELECT
                    CAST(i.Id AS CHAR) AS Id,
                    CAST(i.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
                    i.Cpf,
                    i.Cpf AS UserName,
                    i.Cns,
                    i.Email,
                    i.CodigoAutenticacao,
                    i.TelefoneCelular,
                    i.NomeCompleto,
                    i.NomeDaMae,
                    i.NomeSocial,
                    i.DataNascimento,
                    TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
                    i.Sexo,
                    i.RacaOuCor,
                    i.DataCadastro,
                    i.DataAlteracao,
                    i.Latitude,
                    i.Longitude,
                    i.Logradouro,
                    i.LogradouroNumero,
                    i.LogradouroComplemento,
                    i.LogradouroCep,
                    i.LogradouroBairro,
                    i.UfAbreviado,
                    i.CidadeId,
                    i.Ativo,
                    i.RespondeuComorbidade,
                    i.Comorbidades,
                    i.Hipertenso,
                    i.Diabetes,
                    i.Fumante,
                    i.Asma,
                    i.DoencaCoracao,
                    i.DoencaPulmao,
                    i.DoencaRins,
                    i.DoencaFigado,
                    i.DoencaCancer,
                    i.Transplantado,
                    i.DoencaComprometeImunidade,
                    i.LugarComCasosCorona,
                    i.DataInicioSintomas,
                    i.Obesidade,
                    i.Gestante,
                    i.DoencaNeurologica,
                    i.AnemiaFalciforme,
                    i.NotificacaoToken,
                    i.Face,
                    i.DocumentFront,
                    i.DocumentBack,
                    i.Imagem,
                    i.NomeSocial,
                    CAST(i.FaceToken AS CHAR) AS FaceToken,
                    i.FaceTokenValidade,
                    c.Ibge, c.Nome, c.UfAbreviado, c.UfExtenso,
                    CAST(uc.Id AS CHAR) AS Id,
                    CAST(uc.IndividuoId AS CHAR) AS IndividuoId,
                    uc.ClaimType,
                    uc.ClaimValue
                    FROM Individuo i
                    LEFT JOIN Cidade c ON c.Ibge = i.CidadeId
                    LEFT JOIN UsuarioClaim uc ON uc.IndividuoId = i.Id ");

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1=1");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(cpf)) return null;

            queryFilter.Append(" AND i.Cpf = ?Cpf");
            param.Add("?Cpf", dbType: DbType.String, value: cpf, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var individuo = new Dictionary<string, Individuo>();
            var result = _connection.Query<Individuo, UserClaim, Individuo>(
                query.ToString(), (i, uc) =>
                {
                    if (!individuo.TryGetValue(i.Id, out var individuoEntity))
                    {
                        individuo.Add(i.Id, individuoEntity = i);
                    }

                    if (individuoEntity.UserClaims == null)
                    {
                        individuoEntity.UserClaims = new List<UserClaim>();
                    }

                    if (uc == null) return individuoEntity;
                    if (individuoEntity.UserClaims.All(x => x.Id != uc.Id))
                    {
                        individuoEntity.UserClaims.Add(uc);
                    }

                    return individuoEntity;
                }, param,
                splitOn: "Id, Ibge, IndividuoId",
                commandTimeout: 60,
                commandType: CommandType.Text).FirstOrDefault();

            return result;
        }

        public Individuo GetByCns(string cns)
        {
            var query = _queryAll;

            #region -- Filtro
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (string.IsNullOrEmpty(cns)) return null;

            queryFilter.Append(" AND i.Cns = ?cns");
            param.Add("?cns", dbType: DbType.String, value: cns, direction: ParameterDirection.Input);
            query.Append(queryFilter);
            #endregion

            var individuo = new Dictionary<string, Individuo>();
            var result = _connection.Query<Individuo, UserClaim, Individuo>(
                query.ToString(), (i, uc) =>
                {
                    if (!individuo.TryGetValue(i.Id, out var individuoEntity))
                    {
                        individuo.Add(i.Id, individuoEntity = i);
                    }

                    if (individuoEntity.UserClaims == null)
                    {
                        individuoEntity.UserClaims = new List<UserClaim>();
                    }

                    if (uc == null) return individuoEntity;
                    if (individuoEntity.UserClaims.All(x => x.Id != uc.Id))
                    {
                        individuoEntity.UserClaims.Add(uc);
                    }

                    return individuoEntity;
                }, param,
                splitOn: "Id, Ibge, IndividuoId",
                commandTimeout: 60,
                commandType: CommandType.Text).FirstOrDefault();

            return result;
        }

        public async Task<(IEnumerable<Individuo>, int)> GetByParam(Individuo filters, string sort, int? skip, int? take)
        {

            var query = _queryAll;

            var queryCount = new StringBuilder();
            queryCount.Append(_queryCountAll);


            #region -- -- Filtros / Ordenação
            var queryFilter = new StringBuilder(" WHERE 1 = 1 ");
            var param = new DynamicParameters();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Id))
                {
                    queryFilter.Append(" AND i.Id = ?Id ");
                    param.Add("?Id", dbType: DbType.String, value: filters.Id, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.CidadeId))
                {
                    queryFilter.Append(" AND i.CidadeId = ?CidadeId ");
                    param.Add("?Cpf", dbType: DbType.String, value: filters.CidadeId, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.NomeCompleto))
                {
                    queryFilter.Append(" AND i.NomeCompleto LIKE ?Nome ");
                    param.Add("?Nome", dbType: DbType.String, value: $"%{filters.NomeCompleto}%", direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.Email))
                {
                    queryFilter.Append(" AND i.Email LIKE ?Email ");
                    param.Add("?Email", dbType: DbType.String, value: $"%{filters.Email}%", direction: ParameterDirection.Input);
                }

                if (!string.IsNullOrEmpty(filters.Cpf))
                {
                    queryFilter.Append(" AND i.Cpf = ?Cpf ");
                    param.Add("?Cpf", dbType: DbType.String, value: filters.Cpf, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.Cns))
                {
                    queryFilter.Append(" AND i.Cns = ?Cns ");
                    param.Add("?Cns", dbType: DbType.String, value: filters.Cns, direction: ParameterDirection.Input);
                }
                if (!string.IsNullOrEmpty(filters.TelefoneCelular))
                {
                    queryFilter.Append(" AND i.TelefoneCelular = ?TelefoneCelular ");
                    param.Add("?TelefoneCelular", dbType: DbType.String, value: filters.TelefoneCelular, direction: ParameterDirection.Input);
                }
                if (filters.Ativo.HasValue)
                {
                    queryFilter.Append(" AND i.Ativo = ?Ativo ");
                    param.Add("?Ativo", dbType: DbType.Boolean, value: filters.Ativo, direction: ParameterDirection.Input);
                }

            }
            query.Append(queryFilter);
            queryCount.Append(queryFilter);


            if (skip.HasValue && take.HasValue)
            {
                query.Append($" ORDER BY {sort} LIMIT ?skip, ?take;");
                skip = (skip - 1) * take;
                param.Add("?skip", dbType: DbType.Int32, value: skip, direction: ParameterDirection.Input);
                param.Add("?take", dbType: DbType.Int32, value: take, direction: ParameterDirection.Input);
            }
            #endregion

            try
            {
                var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();

                var individuos = new Dictionary<string, Individuo>();
                await _connection.QueryAsync<Individuo, UserClaim, Individuo>(
                    query.ToString(), (i, uc) =>
                    {
                        if (!individuos.TryGetValue(i.Id, out var individuoEntity))
                        {
                            individuos.Add(i.Id, individuoEntity = i);
                        }

                        if (individuoEntity.UserClaims == null)
                        {
                            individuoEntity.UserClaims = new List<UserClaim>();
                        }

                        if (uc == null) return individuoEntity;
                        if (individuoEntity.UserClaims.All(x => x.Id != uc.Id))
                        {
                            individuoEntity.UserClaims.Add(uc);
                        }

                        return individuoEntity;
                    }, param,
                    splitOn: "Id, Ibge",
                    commandTimeout: 60,
                    commandType: CommandType.Text);

                queryFilter = new StringBuilder("");

                return (individuos.Values, totalCount);
            }
            catch (System.Exception e)
            {
                throw e;
            }

        }

        public Task Delete(string id)
        {
            try
            {
                var query = @"UPDATE Individuo 
                            SET Ativo = FALSE,
                            Cpf = '',
                            Cns = ''
                            WHERE Id = ?Id";
                var param = new DynamicParameters();
                param.Add("?Id", dbType: DbType.String, value: id, direction: ParameterDirection.Input);

                using (var transaction = _connection.BeginTransaction())
                {
                    _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", id.ToString(), "Desativou o Indivíduo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                    transaction.Commit();
                    return Task.CompletedTask;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Individuo Login(string usuario, string senha)
        {
            var query = new StringBuilder(@"
            SELECT
            CAST(i.Id AS CHAR) AS Id,
            i.Cpf,
            i.Email,
            i.TelefoneCelular,
            i.NomeCompleto,
            i.NomeDaMae,
            i.DataNascimento,
            i.Sexo,
            i.RacaOuCor,
            i.DataCadastro,
            i.DataAlteracao,
            i.Latitude,
            i.Longitude,
            i.Logradouro,
            i.LogradouroNumero,
            i.LogradouroComplemento,
            i.LogradouroCep,
            i.LogradouroBairro,
            i.UfAbreviado,
            i.CidadeId,
            i.Ativo,
            i.Imagem,
            CAST(uc.Id AS CHAR) AS Id,
            CAST(uc.IndividuoId AS CHAR) AS IndividuoId,
            uc.ClaimType,
            uc.ClaimValue
            FROM Individuo i
            LEFT JOIN UsuarioClaim uc ON uc.IndividuoId = i.Id ");

            query.Append("WHERE 1 = 1 ");

            var param = new DynamicParameters();
            param.Add("?Senha", dbType: DbType.String, value: senha, direction: ParameterDirection.Input);

            if (usuario.Length == 11)
            {
                query.Append(" AND i.Cpf = ?Cpf ");
                param.Add("?Cpf", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);

            }
            else
            {
                query.Append(" AND i.TelefoneCelular = ?Telefone");
                param.Add("?Telefone", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);
            }

            query.Append(" AND i.Senha = ?Senha");

            var agente = new Dictionary<string, Individuo>();
            _connection.Query<Individuo, UserClaim, Individuo>(
                query.ToString(), (i, uc) =>
                {
                    if (!agente.TryGetValue(i.Id, out var individuoEntity))
                    {
                        agente.Add(i.Id, individuoEntity = i);
                    }

                    if (individuoEntity.UserClaims == null)
                    {
                        individuoEntity.UserClaims = new List<UserClaim>();
                    }

                    if (uc == null) return individuoEntity;
                    if (individuoEntity.UserClaims.All(x => x.Id != uc.Id))
                    {
                        individuoEntity.UserClaims.Add(uc);
                    }

                    return individuoEntity;
                }, param,
                splitOn: "Id",
                commandTimeout: 60,
                commandType: CommandType.Text);

            return agente.Values.FirstOrDefault();
        }

        public Individuo LoginPortal(string username)
        {
            var query = new StringBuilder(@"
            SELECT
            CAST(i.Id AS CHAR) AS Id,
            i.Cpf,
            i.Cns,
            i.Email,
            i.TelefoneCelular,
            i.NomeCompleto,
            i.NomeDaMae,
            i.DataNascimento,
            i.Sexo,
            i.RacaOuCor,
            i.DataCadastro,
            i.DataAlteracao,
            i.Latitude,
            i.Longitude,
            i.Logradouro,
            i.LogradouroNumero,
            i.LogradouroComplemento,
            i.LogradouroCep,
            i.LogradouroBairro,
            i.UfAbreviado,
            i.CidadeId,
            i.Ativo,
            i.Imagem,
            CAST(uc.Id AS CHAR) AS Id,
            CAST(uc.IndividuoId AS CHAR) AS IndividuoId,
            uc.ClaimType,
            uc.ClaimValue
            FROM Individuo i
            LEFT JOIN UsuarioClaim uc ON uc.IndividuoId = i.Id ");

            query.Append("WHERE 1 = 1 ");

            var param = new DynamicParameters();

            if (username.Length == 15)
            {
                query.Append(" AND i.Cns = ?Cns ");
                param.Add("?Cns", dbType: DbType.String, value: username, direction: ParameterDirection.Input);

            }
            else if (username.Length == 11)
            {
                query.Append(" AND i.Cpf = ?Cpf ");
                param.Add("?Cpf", dbType: DbType.String, value: username, direction: ParameterDirection.Input);

            }
            else
            {
                query.Append(" AND i.TelefoneCelular = ?Telefone");
                param.Add("?Telefone", dbType: DbType.String, value: username, direction: ParameterDirection.Input);
            }
            try
            {
                var agente = new Dictionary<string, Individuo>();


                _connection.Query<Individuo, UserClaim, Individuo>(
                    query.ToString(), (i, uc) =>
                    {
                        if (!agente.TryGetValue(i.Id, out var individuoEntity))
                        {
                            agente.Add(i.Id, individuoEntity = i);
                        }

                        if (individuoEntity.UserClaims == null)
                        {
                            individuoEntity.UserClaims = new List<UserClaim>();
                        }

                        if (uc == null) return individuoEntity;
                        if (individuoEntity.UserClaims.All(x => x.Id != uc.Id))
                        {
                            individuoEntity.UserClaims.Add(uc);
                        }

                        return individuoEntity;
                    }, param,
                    splitOn: "Id",
                    commandTimeout: 60,
                    commandType: CommandType.Text);

                return agente.Values.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int UpdateSenha(string individuoId, string senha, string acao)
        {
            var query = @"UPDATE Individuo SET
                        Senha = ?Senha,
                        DataAlteracao = ?DataAlteracao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: individuoId, direction: ParameterDirection.Input);
            param.Add("?Senha", dbType: DbType.String, value: senha, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: DateTime.Now, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", individuoId, $"Efetuou {acao} da Senha do Individuo", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string UpdateCodigoAutenticacao(string individuoId, string codigoAutenticacao)
        {
            var query = @"UPDATE Individuo SET
                        CodigoAutenticacao = ?CodigoAutenticacao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();
            param.Add("?Id", dbType: DbType.String, value: individuoId, direction: ParameterDirection.Input);
            param.Add("?CodigoAutenticacao", dbType: DbType.String, value: codigoAutenticacao, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", individuoId, "Atualizou Código Autenticação", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return codigoAutenticacao;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int AtualizarComorbidade(string individuoId, Individuo obj)
        {
            var query = @"UPDATE Individuo SET
                        DoencaRins = ?DoencaRins,
                        DoencaFigado = ?DoencaFigado,
                        DoencaCancer = ?DoencaCancer,
					    Hipertenso = ?Hipertenso,
                        Diabetes = ?Diabetes,
                        Fumante = ?Fumante,
                        Asma = ?Asma,
                        DoencaCoracao = ?DoencaCoracao,
                        DoencaPulmao = ?DoencaPulmao,
                        Transplantado = ?Transplantado,
                        DoencaComprometeImunidade = ?DoencaComprometeImunidade,
                        DataInicioSintomas = ?DataInicioSintomas,
                        RespondeuComorbidade = ?RespondeuComorbidade,
                        Latitude = ?Latitude,
                        Longitude = ?Longitude,
                        DataAlteracao = ?DataAlteracao
                        WHERE Id = ?Id ";

            var param = new DynamicParameters();

            param.Add("?Id", dbType: DbType.String, value: individuoId, direction: ParameterDirection.Input);
            param.Add("?Transplantado", dbType: DbType.Boolean, value: obj.Transplantado, direction: ParameterDirection.Input);
            param.Add("?DoencaComprometeImunidade", dbType: DbType.Boolean, value: obj.DoencaComprometeImunidade, direction: ParameterDirection.Input);
            param.Add("?Fumante", dbType: DbType.Boolean, value: obj.Fumante, direction: ParameterDirection.Input);
            param.Add("?Hipertenso", dbType: DbType.Boolean, value: obj.Hipertenso, direction: ParameterDirection.Input);
            param.Add("?Asma", dbType: DbType.Boolean, value: obj.Asma, direction: ParameterDirection.Input);
            param.Add("?Diabetes", dbType: DbType.Boolean, value: obj.Diabetes, direction: ParameterDirection.Input);
            param.Add("?DoencaPulmao", dbType: DbType.Boolean, value: obj.DoencaPulmao, direction: ParameterDirection.Input);
            param.Add("?DoencaCoracao", dbType: DbType.Boolean, value: obj.DoencaCoracao, direction: ParameterDirection.Input);
            param.Add("?DoencaRins", dbType: DbType.Boolean, value: obj.DoencaRins, direction: ParameterDirection.Input);
            param.Add("?DoencaFigado", dbType: DbType.Boolean, value: obj.DoencaFigado, direction: ParameterDirection.Input);
            param.Add("?DoencaCancer", dbType: DbType.Boolean, value: obj.DoencaCancer, direction: ParameterDirection.Input);
            param.Add("?RespondeuComorbidade", dbType: DbType.Boolean, value: obj.RespondeuComorbidade, direction: ParameterDirection.Input);
            param.Add("?Latitude", dbType: DbType.Double, value: obj.Latitude, direction: ParameterDirection.Input);
            param.Add("?Longitude", dbType: DbType.Double, value: obj.Longitude, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao = obj.DataAlteracao.AddHours(-3), direction: ParameterDirection.Input);
            param.Add("?DataInicioSintomas", dbType: DbType.DateTime, value: obj.DataInicioSintomas, direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", individuoId.ToString(), "Atualizou Comorbidades", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public int AtualizarComorbidadeTriagem(string individuoId, Individuo obj)
        {
            var query = @"UPDATE Individuo SET
                    DoencaRins = ?DoencaRins,
                    DoencaFigado = ?DoencaFigado,
                    DoencaCancer = ?DoencaCancer,
					Hipertenso = ?Hipertenso,
                    Diabetes = ?Diabetes,
                    Fumante = ?Fumante,
                    Asma = ?Asma,
                    DoencaCoracao = ?DoencaCoracao,
                    DoencaPulmao = ?DoencaPulmao,
                    Transplantado = ?Transplantado,
                    DoencaComprometeImunidade = ?DoencaComprometeImunidade,
                    DataInicioSintomas = ?DataInicioSintomas,
                    RespondeuComorbidade = ?RespondeuComorbidade,
                    Latitude = ?Latitude,
                    Longitude = ?Longitude,
                    DataAlteracao = ?DataAlteracao
                    WHERE Id = ?Id ";

            var param = new DynamicParameters();

            param.Add("?Id", dbType: DbType.String, value: individuoId, direction: ParameterDirection.Input);
            param.Add("?Transplantado", dbType: DbType.Boolean, value: obj.Transplantado, direction: ParameterDirection.Input);
            param.Add("?DoencaComprometeImunidade", dbType: DbType.Boolean, value: obj.DoencaComprometeImunidade, direction: ParameterDirection.Input);
            param.Add("?Fumante", dbType: DbType.Boolean, value: obj.Fumante, direction: ParameterDirection.Input);
            param.Add("?Hipertenso", dbType: DbType.Boolean, value: obj.Hipertenso, direction: ParameterDirection.Input);
            param.Add("?Asma", dbType: DbType.Boolean, value: obj.Asma, direction: ParameterDirection.Input);
            param.Add("?Diabetes", dbType: DbType.Boolean, value: obj.Diabetes, direction: ParameterDirection.Input);
            param.Add("?DoencaPulmao", dbType: DbType.Boolean, value: obj.DoencaPulmao, direction: ParameterDirection.Input);
            param.Add("?DoencaCoracao", dbType: DbType.Boolean, value: obj.DoencaCoracao, direction: ParameterDirection.Input);
            param.Add("?DoencaRins", dbType: DbType.Boolean, value: obj.DoencaRins, direction: ParameterDirection.Input);
            param.Add("?DoencaFigado", dbType: DbType.Boolean, value: obj.DoencaFigado, direction: ParameterDirection.Input);
            param.Add("?DoencaCancer", dbType: DbType.Boolean, value: obj.DoencaCancer, direction: ParameterDirection.Input);
            param.Add("?RespondeuComorbidade", dbType: DbType.Boolean, value: obj.RespondeuComorbidade, direction: ParameterDirection.Input);
            param.Add("?Latitude", dbType: DbType.Double, value: obj.Latitude, direction: ParameterDirection.Input);
            param.Add("?Longitude", dbType: DbType.Double, value: obj.Longitude, direction: ParameterDirection.Input);
            param.Add("?DataInicioSintomas", dbType: DbType.DateTime, value: obj.DataInicioSintomas, direction: ParameterDirection.Input);
            param.Add("?DataAlteracao", dbType: DbType.DateTime, value: obj.DataAlteracao = obj.DataAlteracao.AddHours(-3), direction: ParameterDirection.Input);

            using (var transaction = _connection.BeginTransaction())
            {
                var ret = _connection.Execute(query, param, transaction);
                RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", individuoId.ToString(), "Atualizou Comorbidades", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());
                transaction.Commit();
                return ret;
            }
        }

        public bool CheckCPF(string cpf)
        {
            var query = new StringBuilder(@"SELECT COUNT(1) FROM Individuo WHERE ");

            var param = new DynamicParameters();
            query.Append(" Cpf = ?Cpf");
            param.Add("?Cpf", dbType: DbType.String, value: cpf, direction: ParameterDirection.Input);

            return _connection.Query<bool>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();
        }

        public bool CheckCNS(string cns)
        {
            var query = new StringBuilder(@"SELECT COUNT(1) FROM Individuo WHERE ");

            var param = new DynamicParameters();
            query.Append(" Cns = ?Cns");
            param.Add("?Cns", dbType: DbType.String, value: cns, direction: ParameterDirection.Input);

            return _connection.Query<bool>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();
        }

        public bool CheckEmail(string email)
        {
            var query = new StringBuilder(@"SELECT COUNT(1) FROM Individuo WHERE ");

            var param = new DynamicParameters();
            query.Append(" Email = ?Email");
            param.Add("?Email", dbType: DbType.String, value: email, direction: ParameterDirection.Input);

            return _connection.Query<bool>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();
        }

        public bool CheckTelefone(string telefone)
        {
            var query = new StringBuilder(@"SELECT COUNT(1) FROM Individuo WHERE ");

            var param = new DynamicParameters();
            query.Append(" TelefoneCelular = ?TelefoneCelular");
            param.Add("?TelefoneCelular", dbType: DbType.String, value: telefone, direction: ParameterDirection.Input);

            return _connection.Query<bool>(query.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).FirstOrDefault();
        }


        public object GenerateFaceToken(string usuario)
        {
            if (!CheckCPF(usuario)) throw new Exception("Paciente não encontrado.");

            var query = @"UPDATE Individuo 
                SET 
                FaceToken = ?FaceToken,
                FaceTokenValidade = ?FaceTokenValidity
                WHERE Cpf = ?Cpf";

            var faceToken = Guid.NewGuid().ToString();
            var faceTokenValidity = DateTime.Now.AddSeconds(10);
            var param = new DynamicParameters();
            param.Add("?Cpf", dbType: DbType.String, value: usuario, direction: ParameterDirection.Input);
            param.Add("?FaceToken", dbType: DbType.String, value: faceToken, direction: ParameterDirection.Input);
            param.Add("?FaceTokenValidity", dbType: DbType.DateTime, value: faceTokenValidity, direction: ParameterDirection.Input);

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    var ret = _connection.Execute(query, param, transaction);
                    RegistrarAcao(_connection, Guid.NewGuid().ToString(), "Individuo", usuario, "Gerou Token Facial", DateTime.Now, _user.GetUserOrigem(), _user.GetUserId(), _user.GetUserIp());

                    transaction.Commit();
                    return new
                    {
                        token = faceToken,
                        validUntil = faceTokenValidity
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<(IEnumerable<Individuo>, int)> PendentesIntegracao(int mes, int ano, string estabelecimentoId)
        {
            var query = new StringBuilder($@"
            SELECT
            CAST(i.Id AS CHAR) AS Id,
            CAST(i.EstabelecimentoId AS CHAR) AS EstabelecimentoId,
            i.Cpf,
            i.Cns,
            i.Email,
            i.TelefoneCelular,
            i.NomeCompleto,
            i.NomeDaMae,
            i.NomeSocial,
			i.DadoSerializado,
			i.LoteIntegracaoId,
            i.DataNascimento,
            TIMESTAMPDIFF(YEAR, i.DataNascimento , NOW()) AS Idade,
            i.Sexo,
            i.RacaOuCor,
            i.DataCadastro,
            i.DataAlteracao,
            i.Latitude,
            i.Longitude,
            i.Logradouro,
            i.LogradouroNumero,
            i.LogradouroComplemento,
            i.LogradouroCep,
            i.LogradouroBairro,
            i.UfAbreviado,
            i.CidadeId,
            i.Ativo,
            i.RespondeuComorbidade,
            i.Comorbidades,
            i.Hipertenso,
            i.Diabetes,
            i.Fumante,
            i.Asma,
            i.DoencaCoracao,
            i.DoencaPulmao,
            i.DoencaRins,
            i.DoencaFigado,
            i.DoencaCancer,
            i.Transplantado,
            i.DoencaComprometeImunidade,
            i.LugarComCasosCorona,
            i.DataInicioSintomas,
            i.Obesidade,
            i.Gestante,
            i.DoencaNeurologica,
            i.AnemiaFalciforme,
            i.CidadeDeNascimentoIbge,
            i.TemMaeDesconhecida,
            i.Nacionalidade,
            i.NomeDoPai,
            i.PisPasep,
            i.PaisDeNascimento,
            i.TemPaiDesconhecido,
            i.GrauDeInstrucao,
            i.FrequentaEscola,
            c.Ibge, c.Nome, c.UfAbreviado, c.UfExtenso,
            CAST(a.Id AS CHAR) AS Id, CAST(a.IndividuoId AS CHAR) AS IndividuoId,
            CAST(e.Id AS CHAR) AS Id, e.NomeFantasia, e.Cnes, e.CodIne, e.CodIbgeMun,
            CAST(p.Id AS CHAR) Id, p.Nome, p.Cpf, p.Cns, p.Crm, p.CidadeId, p.UfAbreviado, p.Ativo
            FROM Individuo i
            INNER JOIN Cidade c ON c.Ibge = i.CidadeId
            LEFT JOIN Agendamento a ON a.IndividuoId = i.Id
            LEFT JOIN Estabelecimento e on e.Id = a.EstabelecimentoId
            LEFT JOIN Profissional p ON p.Id = a.ProfissionalId
            WHERE i.Ativo = 1
            AND EXTRACT(MONTH FROM i.DataAlteracao) = ?mes
            AND EXTRACT(YEAR FROM i.DataAlteracao) = ?ano ");

            var queryCount = new StringBuilder();
            queryCount.Append(@"SELECT COUNT(1) FROM Individuo i
            INNER JOIN Cidade c ON c.Ibge = i.CidadeId
            LEFT JOIN Agendamento a ON a.IndividuoId = i.Id
            LEFT JOIN Estabelecimento e on e.Id = a.EstabelecimentoId
            LEFT JOIN Profissional p ON p.Id = a.ProfissionalId
            WHERE i.Ativo = 1
            AND EXTRACT(MONTH FROM i.DataAlteracao) = ?mes
            AND EXTRACT(YEAR FROM i.DataAlteracao) = ?ano ");

            #region -- -- Filtros / Ordenação
            var param = new DynamicParameters();

            param.Add("?estabelecimentoId", dbType: DbType.String, value: estabelecimentoId, direction: ParameterDirection.Input);
            param.Add("?mes", dbType: DbType.Int32, value: mes, direction: ParameterDirection.Input);
            param.Add("?ano", dbType: DbType.Int32, value: ano, direction: ParameterDirection.Input);

            if (!string.IsNullOrEmpty(estabelecimentoId))
            {
                query.Append(" AND e.Id = ?estabelecimentoId ");
                queryCount.Append(" AND e.Id = ?estabelecimentoId ");
            }

            #endregion

            try
            {
                var totalCount = _connection.Query<int>(queryCount.ToString(), param, commandTimeout: TimeOut, commandType: CommandType.Text).SingleOrDefault();
                var list = new Dictionary<string, Individuo>();

                if (totalCount > 0)
                {
                    await _connection.QueryAsync<Individuo>(query.ToString(),
                        new[]
                        {
                        typeof(Individuo), typeof(Cidade), typeof(Agendamento), typeof(Estabelecimento), typeof(Profissional)
                        },
                        objects =>
                        {
                            var individuo = objects[0] as Individuo;
                            var cidade = objects[1] as Cidade;
                            var agendamento = objects[2] as Agendamento;
                            var estabelecimento = objects[3] as Estabelecimento;
                            var profissional = objects[4] as Profissional;

                            if (!list.TryGetValue(individuo.Id, out var entity))
                            {
                                entity = individuo;
                                list.Add(individuo.Id, entity);
                            }

                            if (entity.Cidade == null)
                                if (cidade != null)
                                    entity.Cidade = cidade;
                            if (entity.Estabelecimento == null)
                                if (estabelecimento != null)
                                    entity.Estabelecimento = estabelecimento;
                            if (entity.Profissional == null)
                                if (profissional != null)
                                    entity.Profissional = profissional;

                            return entity;
                        }, param, splitOn: "Id, Ibge, Id, Id, Id", commandTimeout: 0, commandType: CommandType.Text);
                    return (list.Values, totalCount);
                }
                return (list.Values, 0);
            }
            catch (System.Exception e)
            {
                throw e;
            }

        }

        public void ConfirmarIntegracao(int lote, Individuo obj)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var param = new DynamicParameters();
                param.Add("?LoteIntegracaoId", dbType: DbType.Int32, value: lote, direction: ParameterDirection.Input);
                param.Add("?IndividuoId", dbType: DbType.String, value: obj.Id, direction: ParameterDirection.Input);
                param.Add("?DadoSerializado", dbType: DbType.String, value: obj.DadoSerializado, direction: ParameterDirection.Input);

                var query = @"UPDATE Individuo SET
                        LoteIntegracaoId = ?LoteIntegracaoId,
                        DadoSerializado = ?DadoSerializado
                        WHERE Id = ?IndividuoId";

                _connection.Execute(query, param, commandTimeout: 0);
                transaction.Commit();
            }
        }

    }
}
