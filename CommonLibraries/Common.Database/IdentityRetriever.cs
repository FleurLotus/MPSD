namespace Common.Database
{
    using System;
    using System.Data;

    public static class IdentityRetriever
    {
        private const string SQLDefaultQuery = "SELECT @@IDENTITY";

        /// <summary>
        /// Gets or sets the database-specific identity retrieval query.
        /// Must be configured once at application startup before any database operations.
        /// </summary>
        /// <remarks>
        /// Different database engines use different SQL for identity retrieval:
        /// <list type="bullet">
        /// <item><description>SQL Server: SELECT @@IDENTITY</description></item>
        /// <item><description>SQLite: SELECT last_insert_rowid()</description></item>
        /// <item><description>MySQL: SELECT LAST_INSERT_ID()</description></item>
        /// <item><description>PostgreSQL: SELECT lastval()</description></item>
        /// </list>
        /// 
        /// This property allows Common.Database to remain database-agnostic without
        /// taking dependencies on specific database drivers. The application layer
        /// is responsible for configuring the appropriate query for its database.
        /// 
        /// <para><strong>Thread Safety:</strong> This property should only be set once during
        /// application initialization. It is not designed for runtime modification.</para>
        /// 
        /// <para><strong>Security:</strong> This property expects a static SQL query string
        /// set at compile-time or application startup. It is not intended to accept 
        /// user input or runtime-generated SQL.</para>
        /// </remarks>
        public static string IdentityQuery { get; set; }

        /// <summary>
        /// Retrieves the last auto-generated identity value from the database.
        /// </summary>
        /// <param name="cmd">the database command (must be in the same transaction as the INSERT)</param>
        /// <returns>The last inserted identity value</returns>
        /// <remarks>
        /// Must be executed within the same transaction as the INSERT for correct behavior,
        /// particularly with SQL Server Compact Edition. See
        /// http://connect.microsoft.com/SQLServer/feedback/details/653675/sql-ce-4-0-select-identity-returns-null
        /// </remarks>
        public static int GetId(IDbCommand cmd)
        {
            cmd.CommandText = IdentityQuery ?? SQLDefaultQuery;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            object id = cmd.ExecuteScalar();
            return Convert.ToInt32(id);
        }
    }
}