using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class DataHelper
    {
        public static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }

        public static int CalcularAnos(DateTime DataNascimento, DateTime DataAtual)
        {
            return (DataAtual.Year - DataNascimento.Year - 1) +
                   (((DataAtual.Month > DataNascimento.Month) ||
                     ((DataAtual.Month == DataNascimento.Month) && (DataAtual.Day >= DataNascimento.Day))) ? 1 : 0);
        }

    }
}
