using AtendTelemedicina.Integracao.Models;
using AtendTelemedicina.Integracao.Models.Helpers;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AtendTelemedicina.Integracao
{
    public class Integrador
    {
        private readonly string _pathXSD;

        public Integrador(string pathXSD)
        {
            _pathXSD = pathXSD;
        }

        public XmlResult GerarXMLIndividuoSimplificado(int lote, Individuo cadastro, string cpfResponsavel, string nomeResponsavel)
        {
            Guid _id;
            // Tratamento para evitar duplicidade de registros no PEC
            if (string.IsNullOrEmpty(cadastro.DadoSerializado.ToString()))
            {
                _id = Guid.NewGuid();
                cadastro.DadoSerializado = _id;
            }
            else
            {
                _id = (Guid)cadastro.DadoSerializado;
            }

            try
            {
                var xml = XmlIndividuoSimplificadoToString(_id, lote, cadastro, cpfResponsavel, nomeResponsavel);

                var xdoc = new XmlDocument();
                xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
                xdoc.LoadXml(xml);

                XMLHelper.ValidaXML("cadastroindividual", xml, _pathXSD + "/cadastroindividual.xsd");
                return new XmlResult { Id = _id, Status = true, Content = xml, Tipo = "IndividuoSimplificado" };
            }
            catch (Exception e)
            {
                return new XmlResult { Id = _id, Status = false, Content = e.Message, Tipo = "IndividuoSimplificado" };
            }
        }

        public string XmlIndividuoSimplificadoToString(Guid id, int lote, Individuo individuo, string cpfResponsavel, string nomeResponsavel)
        {
            var xmlIndividuo = new XmlIndividuoSimplificado();
            xmlIndividuo.PopularXml(id, lote, individuo, cpfResponsavel, nomeResponsavel);
            var xmlOut = xmlIndividuo.Serialize(xmlIndividuo);
            return xmlOut;
        }

        public XmlResult GerarXMLFichaDeProcedimento(int lote, Atendimento cadastro, string cpfResponsavel, string nomeResponsavel)
        {
            Guid _id;
            // Tratamento para evitar duplicidade de registros no PEC
            if (string.IsNullOrEmpty(cadastro.DadoSerializado.ToString()))
            {
                _id = Guid.NewGuid();
                cadastro.DadoSerializado = _id;
            }
            else
            {
                _id = (Guid)cadastro.DadoSerializado;
            }

            try
            {
                var xml = XmlFichaProcedimentoToString(_id, lote, cadastro, cpfResponsavel, nomeResponsavel);

                var xdoc = new XmlDocument();
                xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
                xdoc.LoadXml(xml);

                XMLHelper.ValidaXML("fichaprocedimentomaster", xml, _pathXSD + "/fichaprocedimentomaster.xsd");
                return new XmlResult { Id = _id, Status = true, Content = xml, Tipo = "Procedimento" };
            }
            catch (Exception e)
            {
                return new XmlResult { Id = _id, Status = false, Content = e.Message, Tipo = "Procedimento" };
            }
        }

        public string XmlFichaProcedimentoToString(Guid id, int lote, Atendimento atendimento, string cpfResponsavel, string nomeResponsavel)
        {
            var xmlProcedimento = new XmlProcedimento();
            xmlProcedimento.PopularXml(id, lote, atendimento, cpfResponsavel, nomeResponsavel);
            var xmlOut = xmlProcedimento.Serialize(xmlProcedimento);
            return xmlOut;
        }

        public XmlResult GerarXMLFichaDeAtendimento(int lote, Atendimento cadastro, string cpfResponsavel, string nomeResponsavel)
        {
            Guid _id;
            // Tratamento para evitar duplicidade de registros no PEC
            if (string.IsNullOrEmpty(cadastro.DadoSerializado.ToString()))
            {
                _id = Guid.NewGuid();
                cadastro.DadoSerializado = _id;
            }
            else
            {
                _id = (Guid)cadastro.DadoSerializado;
            }

            try
            {
                var xml = XmlFichaAtendimentoToString(_id, lote, cadastro, cpfResponsavel, nomeResponsavel);

                var xdoc = new XmlDocument();
                xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
                xdoc.LoadXml(xml);

                XMLHelper.ValidaXML("fichaatendimentoindividualmaster", xml, _pathXSD + "/fichaatendimentoindividualmaster.xsd");
                return new XmlResult { Id = _id, Status = true, Content = xml, Tipo = "Atendimento" };
            }
            catch (Exception e)
            {
                return new XmlResult { Id = _id, Status = false, Content = e.Message, Tipo = "Atendimento" };
            }
        }

        public XmlResult GerarXMLFichaDeAtendimentoDomiciliar(int lote, Atendimento cadastro, string cpfResponsavel, string nomeResponsavel)
        {
            Guid _id;
            // Tratamento para evitar duplicidade de registros no PEC
            if (string.IsNullOrEmpty(cadastro.DadoSerializado.ToString()))
            {
                _id = Guid.NewGuid();
                cadastro.DadoSerializado = _id;
            }
            else
            {
                _id = (Guid)cadastro.DadoSerializado;
            }

            try
            {
                var xml = XmlFichaAtendimentoDomiciliarToString(_id, lote, cadastro, cpfResponsavel, nomeResponsavel);

                var xdoc = new XmlDocument();
                xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
                xdoc.LoadXml(xml);

                XMLHelper.ValidaXML("fichaatendimentodomiciliarmaster", xml, _pathXSD + "/fichaatendimentodomiciliarmaster.xsd");
                return new XmlResult { Id = _id, Status = true, Content = xml, Tipo = "AtendimentoDomiciliar" };
            }
            catch (Exception e)
            {
                return new XmlResult { Id = _id, Status = false, Content = e.Message, Tipo = "AtendimentoDomiciliar" };
            }
        }

        public string XmlFichaAtendimentoToString(Guid id, int lote, Atendimento atendimento, string cpfResponsavel, string nomeResponsavel)
        {
            var xmlAtendimento = new XmlFichaAtendimento();
            xmlAtendimento.PopularXml(id, lote, atendimento, cpfResponsavel, nomeResponsavel);
            var xmlOut = xmlAtendimento.Serialize(xmlAtendimento);
            return xmlOut;
        }

        public string XmlFichaAtendimentoDomiciliarToString(Guid id, int lote, Atendimento atendimento, string cpfResponsavel, string nomeResponsavel)
        {
            var xmlAtendimento = new XmlFichaAtendimentoDomiciliar();
            xmlAtendimento.PopularXml(id, lote, atendimento, cpfResponsavel, nomeResponsavel);
            var xmlOut = xmlAtendimento.Serialize(xmlAtendimento);
            return xmlOut;
        }


    }
}
