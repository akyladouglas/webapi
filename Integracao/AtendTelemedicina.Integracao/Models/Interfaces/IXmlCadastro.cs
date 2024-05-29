using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace AtendTelemedicina.Integracao.Models.Interfaces
{
    public interface IXmlCadastro
    {
        XNamespace ns4 { get; set; }
        int tipoDadoSerializado { get; set; }

        XElement GetColetaElement();
        XElement GetHeaderTransport();

        void SetDadosInstalacao(string contraChave, string uuidInstalacao, string cpfOuCnpj, string nomeOuRazaoSocial,
           string versaoSistema = null, string nomeBancoDados = null, string fone = null, string email = null);
    }
}
