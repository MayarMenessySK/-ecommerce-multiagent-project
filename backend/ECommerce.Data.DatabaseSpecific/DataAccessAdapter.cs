using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.DQE.PostgreSql;

namespace ECommerce.Data.DatabaseSpecific;

/// <summary>
/// DataAccessAdapter for PostgreSQL using LLBLGen Pro
/// This is the main entry point for database operations
/// </summary>
public partial class DataAccessAdapter : SD.LLBLGen.Pro.ORMSupportClasses.DataAccessAdapter
{
    /// <summary>
    /// CTor
    /// </summary>
    public DataAccessAdapter() : base(new DynamicQueryEngine(), new PostgreSqlDQEConfiguration())
    {
    }

    /// <summary>
    /// CTor with connection string
    /// </summary>
    public DataAccessAdapter(string connectionString) : base(connectionString, new DynamicQueryEngine(), new PostgreSqlDQEConfiguration())
    {
    }

    /// <summary>
    /// CTor with connection string and keep connection open flag
    /// </summary>
    public DataAccessAdapter(string connectionString, bool keepConnectionOpen) : base(connectionString, keepConnectionOpen, new DynamicQueryEngine(), new PostgreSqlDQEConfiguration())
    {
    }
}
