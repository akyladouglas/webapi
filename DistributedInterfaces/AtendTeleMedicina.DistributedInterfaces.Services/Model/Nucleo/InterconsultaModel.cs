using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class InterconsultaModel : BaseModel
    {
        #region Propriedades
        public ClassificacaoDeRisco ClassificacaoDeRisco { get; set; }
        public string DiscussaoDoCaso { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }

        //Adicionando listas de Cid e Ciap
        public List<InterconsultaCidModel> Cid { get; set; }
        public List<InterconsultaCiapModel> Ciap { get; set; }
        #endregion

        #region Métodos de Validações

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract().Requires()
                .IsNotNull(ClassificacaoDeRisco, "ClassificacaoDeRisco", "A Classificação de Risco é obrigatória")
                .IsNotNullOrEmpty(DiscussaoDoCaso, "DiscussaoDoCaso", "A Discussão do Caso é obrigatória"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNull(ClassificacaoDeRisco, "ClassificacaoDeRisco", "A Classificação de Risco é obrigatória")
                .IsNotNullOrEmpty(DiscussaoDoCaso, "DiscussaoDoCaso", "A Discussão do Caso é obrigatória"));
        }

        public void ValidarString()
        {
            DiscussaoDoCaso = ValidarDiscussaoDoCaso(DiscussaoDoCaso, 500);
        }

        public void ValidarGet()
        {
            ValidarString();
        }

        private string ValidarDiscussaoDoCaso(string text, int maxLength)
        {
            return string.IsNullOrEmpty(text) || text.Length > maxLength ? null : text;
        }
        #endregion

    }
}
