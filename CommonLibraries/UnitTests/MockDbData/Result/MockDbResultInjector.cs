namespace MockDbData
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    public class MockDbResultInjector
    {
        private MockDbResult _globalResult;
        private int? _nonQueryResult;
        private readonly Dictionary<MockDbMatchingRule, MockDbResult> _rules = new Dictionary<MockDbMatchingRule, MockDbResult>();
        private readonly List<MockDbExecution> _executions = new List<MockDbExecution>();

        public MockDbResultInjector()
        {
        }

        public IReadOnlyList<MockDbExecution> Executions { get { return _executions.AsReadOnly(); } }

        public void AddGlobalExecuteNonQueryResult(int nonQueryResult)
        {
            _nonQueryResult = nonQueryResult;
        }
        public void AddGlobalResult(MockDbResult mockDbResult)
        {
            ArgumentNullException.ThrowIfNull(mockDbResult);

            _globalResult = mockDbResult;
        }
        public void AddResult(MockDbMatchingRule matchingRule, MockDbResult mockDbResult)
        {
            ArgumentNullException.ThrowIfNull(matchingRule);
            ArgumentNullException.ThrowIfNull(mockDbResult);

            _rules[matchingRule] = mockDbResult;
        }
        internal MockDbResult GetMockDbResult(DbCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            int matchingLevel = -1;
            IList<KeyValuePair<MockDbMatchingRule, MockDbResult>> matching = new List<KeyValuePair<MockDbMatchingRule, MockDbResult>>();

            foreach (KeyValuePair<MockDbMatchingRule, MockDbResult> kv in _rules)
            {
                MockDbMatchingRule rule = kv.Key;

                if (rule.Match(command))
                {
                    if ((int) rule.MatchingLevel > matchingLevel)
                    {
                        matching.Clear();
                        matchingLevel = (int) rule.MatchingLevel;
                        matching.Add(kv);

                    }
                    else if ((int) rule.MatchingLevel == matchingLevel)
                    {
                        if (rule.ParameterValues.Count > matching[0].Key.ParameterValues.Count)
                        {
                            matching.Clear();
                            matching.Add(kv);
                        }
                        else if (rule.ParameterValues.Count == matching[0].Key.ParameterValues.Count)
                        {
                            matching.Add(kv);
                        }
                    }
                }
            }

            if (matching.Count > 0)
            {
                return matching[0].Value;
            }

            return _globalResult;
        }
        internal int? GetExecuteNonQueryResult(DbCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            _executions.Add(new MockDbExecution(command));

            return _nonQueryResult;
        }
    }
}