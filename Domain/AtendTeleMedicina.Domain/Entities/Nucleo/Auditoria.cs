using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
  public class Auditoria : EntidadeBase
  {
    #region Propriedades
    public string Tabela { get; set; }
    public string TabelaId { get; set; }
    public string Acao { get; set; }
    public DateTime DataHora { get; set; }
    public string Origem { get; set; }
    public string AtorId { get; set; }
    public string Ip { get; set; }
    public string Justificativa { get; set; }
    public virtual string Usuario { get; set; }
    public virtual string Profissional { get; set; }
    public virtual string Individuo { get; set; }
    #endregion

    #region Construtores
    public Auditoria()
    {

    }
    #endregion
  }
}
