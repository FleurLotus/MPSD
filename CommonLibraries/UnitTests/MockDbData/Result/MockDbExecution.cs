namespace MockDbData
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class MockDbExecution
    {
        internal MockDbExecution(IDbCommand cmd)
        {
            CommandType = cmd.CommandType;
            CommandText = cmd.CommandText;
            Parameters = cmd.Parameters.OfType<IDbDataParameter>().Select(p => new MockDbExecutionParameter(p)).ToList().AsReadOnly();
        }

        public CommandType CommandType { get; }
        public string CommandText { get; }
        public IReadOnlyList<MockDbExecutionParameter> Parameters { get; }
    }
}