namespace MockDbData
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using System.Text.RegularExpressions;

    [Flags]
    internal enum MatchingLevel
    {
        CommandType = 1,
        CommandParameterValue = 1 << 1,
        CommandParameterCount = 1 << 2,
        CommandText = 1 << 3,
    }
    public class MockDbMatchingRule
    {
        private readonly Dictionary<object, object> _parametersValues;
        internal MockDbMatchingRule(CommandType commandType)
        {
            _parametersValues = new Dictionary<object, object>();
            CommandType = commandType;
            MatchingLevel = MatchingLevel.CommandType;
        }
        private MockDbMatchingRule(MockDbMatchingRule other)
        {
            CommandType = other.CommandType;
            MatchingLevel = other.MatchingLevel;
            CommandTextReg = other.CommandTextReg;
            ParameterCount = other.ParameterCount;
            _parametersValues = new Dictionary<object, object>(other._parametersValues);
        }
        public CommandType CommandType { get; }
        public string CommandTextReg { get; private set; }
        public int? ParameterCount { get; private set; }
        public IReadOnlyDictionary<object, object> ParameterValues { get { return new ReadOnlyDictionary<object, object>(_parametersValues); } }
        internal MatchingLevel MatchingLevel { get; private set; }
        public static MockDbMatchingRule CreateRule(CommandType commandType)
        {
            return new MockDbMatchingRule(commandType);
        }
        public MockDbMatchingRule SetParameterValue<T>(int parameterPosition, T value) where T : struct
        {
            return SetParameterValueInternal(parameterPosition, value);
        }
        public MockDbMatchingRule SetParameterValue(int parameterPosition, string value)
        {
            return SetParameterValueInternal(parameterPosition, value);
        }
        private MockDbMatchingRule SetParameterValueInternal(int parameterPosition, object value)
        {
            if (parameterPosition < 0)
            {
                throw new ArgumentException($"{nameof(parameterPosition)} must be greater of equals to 0.", nameof(parameterPosition));
            }
            MockDbMatchingRule copy = new MockDbMatchingRule(this);
            copy.MatchingLevel |= MatchingLevel.CommandParameterValue;
            copy._parametersValues[parameterPosition] = value;
            return copy;
        }
        public MockDbMatchingRule SetParameterValue<T>(string parameterName, T value) where T : struct
        {
            return SetParameterValueInternal(parameterName, value);
        }
        public MockDbMatchingRule SetParameterValue(string parameterName, string value)
        {
            return SetParameterValueInternal(parameterName, value);
        }
        private MockDbMatchingRule SetParameterValueInternal(string parameterName, object value)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }
            MockDbMatchingRule copy = new MockDbMatchingRule(this);
            copy.MatchingLevel |= MatchingLevel.CommandParameterValue;
            copy._parametersValues[parameterName] = value;
            return copy;
        }
        public MockDbMatchingRule SetCommandTextReg(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            MockDbMatchingRule copy = new MockDbMatchingRule(this);
            copy.MatchingLevel |= MatchingLevel.CommandText;
            copy.CommandTextReg = text;
            return copy;
        }
        public MockDbMatchingRule SetParameterCount(int parameterCount)
        {
            if (parameterCount < 0)
            {
                throw new ArgumentException($"{nameof(parameterCount)} must be greater of equals to 0.", nameof(parameterCount));
            }
            MockDbMatchingRule copy = new MockDbMatchingRule(this);
            copy.ParameterCount = parameterCount;
            copy.MatchingLevel |= MatchingLevel.CommandParameterCount;
            return copy;
        }
        internal bool Match(DbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.CommandType != CommandType)
            {
                return false;
            }

            if (ParameterCount.HasValue)
            {
                if (ParameterCount.Value != command.Parameters.Count)
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(CommandTextReg))
            {
                if (string.IsNullOrWhiteSpace(command.CommandText))
                {
                    return false;
                }

                Regex reg = new Regex(CommandTextReg);

                if (!reg.IsMatch(command.CommandText))
                {
                    return false;
                }
            }

            if (ParameterValues.Count > 0)
            {
                foreach (var kv in ParameterValues)
                {
                    if (kv.Key is string key)
                    {
                        if (command.Parameters.IndexOf(key) < 0 || !Equals(command.Parameters[key].Value, kv.Value))
                        {
                            return false;
                        }
                    }
                    else if (kv.Key is int i)
                    {
                        if (command.Parameters.Count <= i || !Equals(command.Parameters[i].Value, kv.Value))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"CommandType={CommandType}");
            if (!string.IsNullOrEmpty(CommandTextReg))
            {
                sb.Append($" && CommandTextReg={CommandTextReg}");
            }
            if (ParameterCount.HasValue)
            {
                sb.Append($" && ParameterCount={ParameterCount.Value}");
            }
            if (ParameterValues.Count > 0)
            {
                foreach (var kv in ParameterValues)
                {
                    if (kv.Key is string key)
                    {
                        sb.Append($" && \"{key}\"={kv.Value}");
                    }
                    else if (kv.Key is int i)
                    {
                        sb.Append($" && pos {i}={kv.Value}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}