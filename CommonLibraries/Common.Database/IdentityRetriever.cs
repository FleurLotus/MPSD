

namespace Common.Database
{
    using System;
    using System.Data;
    using System.Data.Common;

    public static class IdentityRetriever
    {
        private const string SQLDefaultQuery = "SELECT @@IDENTITY";

        //ALERT: Warning, sql injection!
        public static string IdentityQuery { get; set; }
        
        public static int GetId(DbCommand cmd)
        {
            //must be done in the transaction because of the way, SQLCE works
            //http://connect.microsoft.com/SQLServer/feedback/details/653675/sql-ce-4-0-select-identity-returns-null
            //It also works for others SGBDR
            cmd.CommandText = IdentityQuery ?? SQLDefaultQuery;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            object id = cmd.ExecuteScalar();
            return Convert.ToInt32(id);
        }

    }
}
