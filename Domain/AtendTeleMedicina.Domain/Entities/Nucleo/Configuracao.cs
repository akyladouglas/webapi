using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Configuracao : EntidadeBase
    {
        public int? Id { get; set; }
        public bool DemandaEspontanea { get; set; }
        public bool LoginComSenha { get; set; }
        public bool IntegraPec { get; set; }
        public int Modulo { get; set; }
        public string UrlAtend { get; set; }
        public string VersaoApp { get; set; }
        public string TipoConsulta { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string UsuarioAlterouId { get; set; }
        public User UsuarioAlterou { get; set; }
    }
}