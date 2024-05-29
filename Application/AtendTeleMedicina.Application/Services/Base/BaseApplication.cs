using AtendTeleMedicina.Application.Contracts.Base;
using System;

namespace AtendTeleMedicina.Application.Services.Base
{
  public class BaseApplication: IBaseApplication
  {
    public void Dispose()
    {
      GC.SuppressFinalize(this);
    }
  }
}
