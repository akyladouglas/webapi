using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
  public static class SheetHelper
  {
    public static ICell GetCell(
          this ISheet sheet, int linha, int coluna)
    {
      IRow row;
      int indiceLinha = linha - 1;
      row = sheet.GetRow(indiceLinha);
      if (row == null)
        row = sheet.CreateRow(indiceLinha);

      ICell cell;
      int indiceColuna = coluna - 1;
      cell = row.GetCell(indiceColuna);
      if (cell == null)
        cell = row.CreateCell(indiceColuna);

      return cell;
    }
  }
}
