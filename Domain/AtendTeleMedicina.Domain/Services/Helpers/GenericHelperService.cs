using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Services.Helpers
{
  public class GenericHelperService
  {
    public static string GerarCodigoAutenticacao()
    {
      return new Random().Next(0, 10000000).ToString();
    }

  }
}
