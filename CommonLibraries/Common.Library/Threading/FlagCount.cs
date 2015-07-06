namespace Common.Library.Threading
{
    using System;
    using System.Collections.Generic;

    internal class FlagCount
    {
        private readonly IDictionary<object, int> _count = new Dictionary<object, int>();

        public FlagCount(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }

        public bool IsFlagSet(object source)
        {
            lock (_count)
            {
                int value;

                return _count.TryGetValue(source, out value) && value > 0;
            }
        }
        public void Increment(object source)
        {
            lock (_count)
            {
                int value;
                if (!_count.TryGetValue(source, out value))
                    value = 0;

                value++;
                _count[source] = value;
            }
        }
        public void Decrement(object source)
        {
            lock (_count)
            {
                int value;
                if (!_count.TryGetValue(source, out value) || value <= 0)
                    throw new Exception("Can't decrement");

                value--;

                if (value > 0)
                    _count[source] = value;
                else
                    _count.Remove(source);
            }
        }
    }
}
