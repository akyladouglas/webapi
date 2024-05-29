using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace AtendTelemedicina.Integracao.Models.Helpers
{
    public class XMLHelper
    {
        public static string ValidaXML(XmlDocument DocXML, string xsd)
        {
            try
            {
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add("", xsd);
                DocXML.Schemas.Add(schemas);
                DocXML.Validate(RetornoXML);
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static bool ValidaXML(string targetNamespace, string xml, string xsd)
        {
            var DocXML = new XmlDocument();
            DocXML.LoadXml(xml);

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.XmlResolver = new XmlUrlResolver();
            schemas.Add("http://esus.ufsc.br/" + targetNamespace, xsd);

            DocXML.Schemas.Add(schemas);
            try
            {
                DocXML.Validate(ValidationEventHandler);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        static void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
                throw new Exception("\t Aviso: esquema não foi encontrado. Nenhuma validação ocorreu." + e.Message);
            else
                throw new Exception("\t Erro de Validação: " + e.Message);
        }

        private static void RetornoXML(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
                throw new Exception("\t Aviso: esquema não foi encontrado. Nenhuma validação ocorreu." + e.Message);
            else
                throw new Exception("\t Erro de Validação: " + e.Message);
        }
    }
}
