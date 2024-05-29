using MySql.Data.MySqlClient;
using System;

namespace AtendTeleMedicina.Infra.Data.Context
{
    public interface IMySqlContext : IDisposable
    {
        MySqlConnection Connection { get; }
    }
}
