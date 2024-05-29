using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AtendTelemedicina.Integracao.Models.Helpers
{
   public static class UtilitarioGenerico
    {
        /// <summary>
        ///     Gera um hexadecimal de uma cor aleatória.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GerarCorAleatoria()
        {
            var aleatorio = new Random();
            var hexCor = $"{aleatorio.Next(0, 0xFFFFFF):X}";
            while (hexCor.Length < 6)
                hexCor = "0" + hexCor;
            return "#" + hexCor;
        }

        /// <summary>
        /// Converte uma string no formato XML em um objeto.
        /// </summary>
        /// <typeparam name="T">A classe</typeparam>
        /// <param name="xml">O Xml.</param>
        /// <returns>A classe T.</returns>
        public static T XmlParaObjeto<T>(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }

        /// <summary>
        /// Converte um objeto em uma string no formato XML.
        /// </summary>
        /// <typeparam name="T">A classe</typeparam>
        /// <param name="obj">O objeto.</param>
        /// <returns>System.String.</returns>
        public static string ObjetoParaXml<T>(T obj)
        {
            using (StringWriter writer = new Utf8StringWriter())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(writer, obj);
                return writer.ToString();
            }

            //using (var stringWriter = new StringWriterUTF8())
            //{
            //    var xmlSerializer = new XmlSerializer(typeof(T));
            //    xmlSerializer.Serialize(stringWriter, obj);
            //    return stringWriter.ToString();
            //}
        }

        public class Utf8StringWriter : StringWriter
        {
            // Use UTF8 encoding but write no BOM to the wire
            public override Encoding Encoding
            {
                get { return new UTF8Encoding(false); } 
            }
        }

        /// <summary>
        /// Salva uma string XML em uma arquivo, especificando o nome do arquivo.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="arquivo">O nome do arquivo.</param>
        public static void SalvarXml(string xml, string arquivo)
        {
            var defaultPath = @"C:\AS_Arquivos\"; //ConfigurationManager.AppSettings["ASFilesPath"];
            GarantirAcesso(defaultPath);
            var xdoc = new XmlDocument();
            xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
            xdoc.LoadXml(xml);
            xdoc.Save(defaultPath + arquivo);
        }

        /// <summary>
        /// Salva qualquer conteúdo no formato de string em um arquivo, especificando o nome do arquivo
        /// ou o caminho completo concatenado ao nome do arquivo.
        /// </summary>
        /// <param name="conteudo">O conteúdo.</param>
        /// <param name="arquivo">O nome do arquivo.</param>
        public static void TextoParaArquivo(string conteudo, string arquivo)
        {
            var defaultPath = @"C:\AS_Arquivos\"; //ConfigurationManager.AppSettings["ASFilesPath"];
            GarantirAcesso(defaultPath);
            File.WriteAllText(defaultPath + arquivo, conteudo);
        }

        //TODO: Analisar o método abaixo, verificar se ele está gravando uma lista de arquivos xml no zip, se não estiver, fazer com que funcione desta forma
        /// <summary>
        /// Comprime no formato zip qualquer tipo de arquivo.
        /// </summary>
        /// <param name="nomeArquivo">O nome arquivo.</param>
        /// <param name="conteudoArquivo">O conteúdo do arquivo.</param>
        /// <param name="nomeDoArquivoZip">O nome do arquivo zip.</param>
        public static Stream GerarArquivoZip(IDictionary<string, string> arquivos, string nomeDoArquivoZip)
        {
            //var defaultPath = ConfigurationManager.AppSettings["ASFilesPath"]; // substituo no core?
            var defaultPath = "";
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true, UTF8Encoding.UTF8))
                {
                    foreach (var arquivo in arquivos)
                    {
                        var file = archive.CreateEntry(arquivo.Key + ".esus.xml");

                        using (var entryStream = file.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.Write(arquivo.Value);
                        }
                    }
                }

                GarantirAcesso(defaultPath);

                using (var fileStream = new FileStream(defaultPath + nomeDoArquivoZip, FileMode.Create))
                {
                    memoryStream.Position = 0;
                    memoryStream.WriteTo(fileStream);
                }

                return memoryStream;
            }
        }

        /// <summary>
        /// Cria uma pasta se não existir e garante todas as permissões de usuário na mesma.
        /// </summary>
        /// <param name="caminho">O caminho.</param>
        public static void GarantirAcesso(string caminho)
        {
            var exists = Directory.Exists(caminho);
            // Caso não exista, a pasta será criada.
            if (!exists) { var di = Directory.CreateDirectory(caminho); }

            var dInfo = new DirectoryInfo(caminho);
            var dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
    }
}
