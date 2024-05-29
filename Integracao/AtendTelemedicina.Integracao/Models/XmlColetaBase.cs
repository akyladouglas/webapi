using AtendTelemedicina.Integracao.Models.Helpers;
using AtendTelemedicina.Integracao.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace AtendTelemedicina.Integracao.Models
{

    public abstract class XmlColetaBase : IXmlColetaBase
    {
        #region Propriedades

        protected XElement Error = new XElement("Erros", null);

        #region dadoTransporteTransportXml

        private readonly XNamespace ns2 = "http://esus.ufsc.br/dadoinstalacao";

        private readonly XNamespace ns3 = "http://esus.ufsc.br/dadotransporte";

        /// <summary>
        ///     Tipo/classe do objeto serializado através do TBinaryProtocol.
        ///     Campo obrigatório
        /// </summary>
        public string uuidDadoSerializado { get; set; }

        /// <summary>
        ///     Código IBGE do dado serializado através do TBinaryProtocol.
        ///     Campo obrigatório
        /// </summary>
        public string codigoIbgeMunicipio { get; set; }

        /// <summary>
        ///     Código CNES da unidade de saúde.
        ///     Campo obrigatório
        /// </summary>
        public string cnesDadoSerializado { get; set; }

        /// <summary>
        ///     Código INE da equipe que gerou a ficha.
        /// </summary>
        public string ineDadoSerializado { get; set; }

        /// <summary>
        ///     Número do lote para controle interno dos arquivos enviados.
        /// </summary>
        public string numLote { get; set; }

        #endregion

        #region HeaderTransport

        /// <summary>
        ///     CNS do profissional.
        ///     Regras: CNS válido de acordo com o algoritmo.
        ///     Referência: Para ver o algoritmo utilizado, acesse: Cartão Net Datasus, em "Downloads" baixe o arquivo
        ///     e rotina de validação Java.
        ///     Observações: Esta entidade é utilizada para representar o profissional responsável pelas fichas.
        /// </summary>
        public string profissionalCNS { get; set; }

        public string cboCodigo_2002 { get; set; }

        /// <summary>
        ///     Código do CNES da unidade de saúde que o profissional está lotado.
        ///     Observações: Esta entidade é utilizada para representar o profissional responsável pelas fichas.
        /// </summary>
        public string cnes { get; set; }

        /// <summary>
        ///     Código INE da equipe do profissional.
        ///     Observações: Esta entidade é utilizada para representar o profissional responsável pelas fichas.
        /// </summary>
        public string ine { get; set; }

        public DateTime dataAtendimento { get; set; }


        #endregion

        #region dadosInstalacao

        public string ContraChave { get; set; }

        public string UuidInstalacao { get; set; }

        public string CpfOuCnpj { get; set; }

        public string NomeOuRazaoSocial { get; set; }

        public string Fone { get; set; }

        public string Email { get; set; }

        public string VersaoSistema { get; set; }

        public string NomeBancoDados { get; set; }

        #endregion

        #endregion

        public void Validator()
        {
            if (string.IsNullOrEmpty(uuidDadoSerializado.ToString()))
                throw new Exception("uuidDadoSerializado é obrigatório!");
            if (string.IsNullOrEmpty(codigoIbgeMunicipio))
                throw new Exception("codigoIbgeMunicipio é obrigatório!");
            if (string.IsNullOrEmpty(cnesDadoSerializado))
                throw new Exception("cnesDadoSerializado é obrigatório!");
        }

        public string Serialize(dynamic xmlCadastro)
        {
            Validator();

            if (Error.IsEmpty)
            {
                var root = new XElement(ns3 + "dadoTransporteTransportXml",
                    new XAttribute(XNamespace.Xmlns + "ns2", ns2.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "ns3", ns3.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "ns4", xmlCadastro.ns4.NamespaceName),
                    new XElement("uuidDadoSerializado", uuidDadoSerializado),
                    new XElement("tipoDadoSerializado", xmlCadastro.tipoDadoSerializado),
                    new XElement("codIbge", codigoIbgeMunicipio),
                    new XElement("cnesDadoSerializado", cnesDadoSerializado?.PadLeft(7, '0')),
                    new XElement("ineDadoSerializado", ineDadoSerializado?.PadLeft(10, '0')),
                    new XElement("numLote", numLote),

                    // Dados dos Cadastros na propriedade ColetaElement
                    xmlCadastro.GetColetaElement(),

                    GetDadosInstalacao(ns2, "remetente"),
                    GetDadosInstalacao(ns2, "originadora"),
                    GetVersao(4, 4, 1));
                return UtilitarioGenerico.ObjetoParaXml(root);
            }

            Error.Add(new XAttribute("Id", uuidDadoSerializado));
            return Error.ToString();
        }

        public XElement GetDadosInstalacao(XNamespace ns, string tagName)
        {
            var element = new XElement(ns + tagName,
                new XElement("contraChave", ContraChave),
                new XElement("uuidInstalacao", UuidInstalacao),
                new XElement("cpfOuCnpj", CpfOuCnpj),
                new XElement("nomeOuRazaoSocial", NomeOuRazaoSocial),
                !string.IsNullOrEmpty(Fone) ? new XElement("fone", Fone) : null,
                !string.IsNullOrEmpty(Email) ? new XElement("email", Email) : null,
                !string.IsNullOrEmpty(VersaoSistema) ? new XElement("versaoSistema", VersaoSistema) : null,
                !string.IsNullOrEmpty(NomeBancoDados) ? new XElement("nomeBancoDados", NomeBancoDados) : null
                );
            return element;
        }
        public abstract void PopularXml(Guid id, int lote, object entity, string cpfResponsavel, string nomeResponsavel);

        private static XElement GetVersao(int major, int minor, int revision)
        {
            return new XElement("versao",
                new XAttribute("major", major),
                new XAttribute("minor", minor),
                new XAttribute("revision", revision));
        }
    }
}
