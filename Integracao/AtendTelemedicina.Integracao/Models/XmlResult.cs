using System;

namespace AtendTelemedicina.Integracao.Models
{
    public class XmlResult
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; }
        public bool Status { get; set; }
        public string Content { get; set; }
    }
}
