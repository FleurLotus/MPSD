namespace Common.WPF.Behavior.UI.UnitTests
{
    using System;

    using XamlTest;

    public class TreeViewModelSerializer : ISerializer
    {
        public bool CanSerialize(Type type, ISerializer rootSerializer)
        {
            return type == typeof(TreeViewModel);
        }

        public object Deserialize(Type type, string value, ISerializer rootSerializer)
        {
            throw new NotImplementedException();
        }

        public string Serialize(Type type, object value, ISerializer rootSerializer)
        {
            return ((TreeViewModel) value).DisplayValue;
        }
    }
}