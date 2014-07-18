using System.Reflection;
using Common.Libray;

namespace Common.ViewModel
{
    public class VersionViewModel : NotifyPropertyChangedBase
    {
        public VersionViewModel()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            AssemblyCopyrightAttribute[] attrib = entryAssembly.GetCustomAttributes<AssemblyCopyrightAttribute>(false);
            if (attrib != null && attrib.Length >= 1)
            {
                Copyright = attrib[0].Copyright;
            }
            
            AssemblyName assemblyName = entryAssembly.GetName();
            Name = assemblyName.Name;
            Version = assemblyName.Version.ToString();
        }

        public string Version { get; private set; }
        public string Name { get; private set; }
        public string Copyright { get; private set; }
    }
}
