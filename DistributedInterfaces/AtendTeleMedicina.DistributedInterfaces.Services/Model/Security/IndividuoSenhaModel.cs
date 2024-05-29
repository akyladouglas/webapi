using System;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Security
{
    public class IndividuoSenhaModel : BaseModel
    {
        public string Cpf { get; set; }
        public string CodigoAutenticacao { get; set; }
        public string Senha { get; set; }
        public string Confirmacao { get; set; }

        public void ValidarEditar()
        {
            ValidarString();
        }

        public void ValidarString()
        {
            Cpf = ValidateText(Cpf, 11);
            Senha = ValidateText(Senha, 255);
        }

        public void ValidarGet()
        {
            ValidarString();
        }
    }
}
