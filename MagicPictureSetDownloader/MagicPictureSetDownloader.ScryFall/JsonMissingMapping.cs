namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json.Serialization;

    public static class JsonMissingMapping
    {
        private static readonly IDictionary<Type, PropertyInfo[]> Properties = new Dictionary<Type, PropertyInfo[]>();
        private static readonly IDictionary<PropertyInfo, JsonExtensionDataAttribute> JsonAttributes = new Dictionary<PropertyInfo, JsonExtensionDataAttribute>();
        private static readonly IDictionary<Type, JsonStringEnumMemberConverterOptionsAttribute> EnumDefault = new Dictionary<Type, JsonStringEnumMemberConverterOptionsAttribute>();

        private static PropertyInfo[] GetProperties(Type type)
        {
            if (!Properties.TryGetValue(type, out PropertyInfo[] props))
            {
                props = type.GetProperties();
                Properties.Add(type, props);
            }

            return props;
        }
        private static JsonExtensionDataAttribute GetJsonExtensionDataAttribute(PropertyInfo prop)
        {
            if (!JsonAttributes.TryGetValue(prop, out JsonExtensionDataAttribute attr))
            {
                attr = prop.GetCustomAttribute<JsonExtensionDataAttribute>();
                JsonAttributes.Add(prop, attr);
            }

            return attr;
        }
        private static JsonStringEnumMemberConverterOptionsAttribute GetJsonStringEnumMemberConverterOptionsAttribute(Type type)
        {
            if (!EnumDefault.TryGetValue(type, out JsonStringEnumMemberConverterOptionsAttribute opt))
            {
                opt = type.GetCustomAttribute<JsonStringEnumMemberConverterOptionsAttribute>();
                EnumDefault.Add(type, opt);
            }

            return opt;
        }

        public static IList<string> Check<T>(T value)
        {
            return Check(value, $"{value.GetType().Name}");
        }

        private static IList<string> Check(object o, string path)
        {
            if (o == null)
            {
                return Array.Empty<string>();
            }

            List<string> ret = new List<string>();

            Type t = o.GetType();
            if (t.IsEnum)
            {
                object unmappedValue = GetJsonStringEnumMemberConverterOptionsAttribute(t)?.Options?.DeserializationFailureFallbackValue;
                if (unmappedValue != null && o == unmappedValue)
                {
                    ret.Add($"{path} => unmappedValue for enum");
                    return ret;
                }
            }

            if (!t.IsClass || t == typeof(string) || t == typeof(Uri))
            {
                return Array.Empty<string>();
            }

            foreach (PropertyInfo prop in GetProperties(t))
            {
                object propvalue = prop.GetValue(o);
                Type propType = prop.PropertyType;

                JsonExtensionDataAttribute att = GetJsonExtensionDataAttribute(prop);
                if (att != null)
                {
                    if (propvalue is IDictionary<string, object> dic && dic.Count > 0)
                    {
                        ret.Add($"{path} => {string.Join(",", dic.Select(kv => $"{kv.Key}:{kv.Value}"))}");
                        continue;
                    }
                }

                if (typeof(IList).IsAssignableFrom(propType))
                {
                    if (propvalue is IList list && list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            IList<string> temp = Check(list[i], $"{path}.{prop.Name}[{i}]");
                            if (temp.Count > 0)
                            {
                                ret.AddRange(temp);
                            }
                        }
                    }
                }
                else if (propType.IsClass && propType != typeof(string) && propType != typeof(Uri))
                {
                    IList<string> temp = Check(propvalue, $"{path}.{prop.Name}");
                    if (temp.Count > 0)
                    {
                        ret.AddRange(temp);
                    }
                }
            }
            return ret;
        }
    }
}