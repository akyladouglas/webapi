using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
  public class EstabelecimentoProfissional : EntidadeBase
  {
    public string EstabelecimentoId { get; set; }
    public Estabelecimento Estabelecimento { get; set; }
    public string ProfissionalId { get; set; }
    public Profissional Profissional { get; set; }
  }
}
