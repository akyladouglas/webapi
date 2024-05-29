using System;
using System.Collections.Generic;
using System.Text;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
  public class Procedimento : EntidadeBase
  {
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
  }
}
