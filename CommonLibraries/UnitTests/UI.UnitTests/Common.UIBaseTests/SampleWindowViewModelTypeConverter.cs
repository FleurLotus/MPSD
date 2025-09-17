namespace Common.UIBaseTests
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text.Json;

    // Use inside XamlTest to convert back the string sent to SampleWindowViewModel
    public class SampleWindowViewModelTypeConverter : TypeConverter
    {
        // Indicates if this converter can convert from a specified type.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        // Converts the given value object to the specified type.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return JsonSerializer.Deserialize<SampleWindowViewModel>(stringValue);
            }
            return base.ConvertFrom(context, culture, value);
        }

        // Indicates if this converter can convert the object to the specified type.
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        // Converts the given value object to the specified type.
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is SampleWindowViewModel vm)
            {
                return JsonSerializer.Serialize(vm);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
