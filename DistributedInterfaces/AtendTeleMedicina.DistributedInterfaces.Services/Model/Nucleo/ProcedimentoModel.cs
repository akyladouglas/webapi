using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class ProcedimentoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Sexo { get; set; }
        public int Grupo { get; set; }
        public int IdadeMin { get; set; }
        public int IdadeMax { get; set; }
        public int CotaTotal { get; set; }
        public int CotaTotalExecutor { get; set; }
        public int CotaEstabelecimento { get; set; }
        public int CotaProfissional { get; set; }
        public double Valor { get; set; }
        #endregion

        public void ValidarGet()
        {
            ValidarString();
        }

        public void ValidarString()
        {
            Descricao = ValidateText(Descricao, 120);
            Codigo = ValidateText(Codigo, 120);
        }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Descricao, "Procedimento.Descricao", "Descrição Inválida")
                .IsNotNullOrEmpty(Descricao, "Procedimento.Codigo", "Código Inválida"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Descricao, "Procedimento.Descricao", "Descrição Inválida")
                .IsNotNullOrEmpty(Descricao, "Procedimento.Codigo", "Código Inválida"));
        }
    }
}
