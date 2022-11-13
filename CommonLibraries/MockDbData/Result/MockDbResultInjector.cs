namespace MockDbData
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    public class MockDbResultInjector
    {
        private MockDbResult _globalResult;
        private readonly Dictionary<MockDbMatchingRule, MockDbResult> _rules = new Dictionary<MockDbMatchingRule, MockDbResult>();
        public MockDbResultInjector()
        {
        }
        public void AddGlobalResult(MockDbResult mockDbResult)
        {
            if (mockDbResult == null)
            {
                throw new ArgumentNullException(nameof(mockDbResult));
            }
            _globalResult = mockDbResult;
        }
        public void AddResult(MockDbMatchingRule matchingRule, MockDbResult mockDbResult)
        {
            if (matchingRule == null)
            {
                throw new ArgumentNullException(nameof(matchingRule));
            }
            if (mockDbResult == null)
            {
                throw new ArgumentNullException(nameof(mockDbResult));
            }
            _rules[matchingRule] = mockDbResult;
        }
        public MockDbResult GetMockDbResult(DbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            int matchingLevel = -1;
            IList<KeyValuePair<MockDbMatchingRule, MockDbResult>> matching = new List<KeyValuePair<MockDbMatchingRule, MockDbResult>>();

            foreach (KeyValuePair<MockDbMatchingRule, MockDbResult> kv in _rules)
            {
                MockDbMatchingRule rule = kv.Key;

                if (rule.Match(command))
                {
                    if ((int)rule.MatchingLevel > matchingLevel)
                    {
                        matching.Clear();
                        matchingLevel = (int)rule.MatchingLevel;
                        matching.Add(kv);

                    }
                    else if ((int)rule.MatchingLevel == matchingLevel)
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
    }
}
    