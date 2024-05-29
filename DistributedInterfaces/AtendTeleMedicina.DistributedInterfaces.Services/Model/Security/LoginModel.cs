using System;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Security
{
    public class LoginModel : BaseModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Audience { get; set; }
        public string Scope { get; set; }
        public string FaceToken { get; set; }

    }
}
