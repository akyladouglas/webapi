using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Security
{
    public class UserClaimModel
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string IndividuoId { get; set; }
        public string ProfissionalId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
