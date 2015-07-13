namespace Common.ViewModel.Version
{
    using System.Reflection;

    using Common.Library.Extension;

    public class VersionViewModel : NotifyPropertyChangedBase
    {
        public VersionViewModel()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            AssemblyCopyrightAttribute[] copyrightAttrib = entryAssembly.GetCustomAttributes<AssemblyCopyrightAttribute>(false);
            if (copyrightAttrib != null && copyrightAttrib.Length >= 1)
            {
                Copyright = copyrightAttrib[0].Copyright;
            }
            AssemblyDescriptionAttribute[] descriptionAttrib = entryAssembly.GetCustomAttributes<AssemblyDescriptionAttribute>(false);
            if (descriptionAttrib != null && descriptionAttrib.Length >= 1)
            {
                Description = descriptionAttrib[0].Description;
            }
            
            AssemblyName assemblyName = entryAssembly.GetName();
            Name = assemblyName.Name;
            Version = assemblyName.Version.ToString();
        }

        public string Version { get; private set; }
        public string Name { get; private set; }
        public string Copyright { get; private set; }
        public string Description { get; private set; }
    }
}
