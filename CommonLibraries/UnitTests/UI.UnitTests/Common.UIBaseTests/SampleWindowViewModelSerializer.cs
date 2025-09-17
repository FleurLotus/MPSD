namespace Common.UIBaseTests
{
    using System;
    using System.Text.Json;

    using XamlTest;

    //Used for return of SampleWindowViewModel from DataContext property setting in tests
    public class SampleWindowViewModelSerializer : ISerializer
    {
        public bool CanSerialize(Type type, ISerializer rootSerializer)
        {
            return type == typeof(SampleWindowViewModel);
        }

        public object Deserialize(Type type, string value, ISerializer rootSerializer)
        {
            return JsonSerializer.Deserialize<SampleWindowViewModel>(value);
        }

        public string Serialize(Type type, object value, ISerializer rootSerializer)
        {
            return ((SampleWindowViewModel) value).ToString();
        }
    }
}
