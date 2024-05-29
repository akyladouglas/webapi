using System;
using System.Collections.Generic;
using System.Linq;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class ConfiguracaoModel : BaseModel
    {
        public int Id { get; set; }
        public bool DemandaEspontanea { get; set; }
        public bool LoginComSenha { get; set; }
        public bool IntegraPec { get; set; }
        public string VersaoApp { get; set; }
        public string UrlAtend { get; set; }
        public string TipoConsulta { get; set; }
        public string Modulo { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string UsuarioAlterouId { get; set; }
        public UserModel UsuarioAlterou { get; set; }

    }
}
