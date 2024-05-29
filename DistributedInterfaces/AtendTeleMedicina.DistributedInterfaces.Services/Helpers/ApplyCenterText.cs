using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class ApplyCenterText
    {
        public void CenterText(ISheet sheet, int numeroProximaLinha, int numeroColuna)
        {
            // Obtém a célula desejada
            ICell cell = sheet.GetRow(numeroProximaLinha).GetCell(numeroColuna);

            // Verifica se a célula existe antes de aplicar o estilo
            if (cell != null)
            {
                // Obtém o estilo da célula
                ICellStyle cellStyle = cell.CellStyle;

                // Configura o alinhamento horizontal para centralizar
                cellStyle.Alignment = HorizontalAlignment.Center;

                // Configura o alinhamento vertical para centralizar
                cellStyle.VerticalAlignment = VerticalAlignment.Center;

                // Configura o tamanho da fonte
                cellStyle.SetFont(new XSSFFont() { FontHeightInPoints = 14, FontName = "Arial", IsBold = false });

                // Confgura as bordas em todos os lados
                cellStyle.BorderTop = BorderStyle.Thin;
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;

                // Aplica o estilo de volta à célula
                cell.CellStyle = cellStyle;
            }
        }
    }
}
