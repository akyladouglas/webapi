using System;
using System.Data.SqlClient;

namespace AtendTeleMedicina.Infrastructure.Data.Context
{
    public interface ISqlServerContext : IDisposable
    {
        SqlConnection Connection { get; }
    }
}
