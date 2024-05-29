using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
  public class EstabelecimentoProcedimento : EntidadeBase
  {
    public string EstabelecimentoId { get; set; }
    public Estabelecimento Estabelecimento { get; set; }
    public string ProcedimentoId { get; set; }
    public Procedimento Procedimento { get; set; }
    public int Cota { get; set; }
    public int CotaExecutor { get; set; }
    public DateTime DataAlteracao { get; set; }

    //public virtual string NomeFantasia { get; set; }
    //public virtual string Descricao { get; set; }
    //public virtual string Codigo { get; set; }
    //public virtual int CotaTotal { get; set; }
    //public virtual int CotaTotalExecutor { get; set; }
  }
}
