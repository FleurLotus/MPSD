namespace Common.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public sealed class AssemblyResolver : IDisposable
    {
        private readonly TextWriter _logTextWriter;
        private readonly IEnumerable<string> _resolveDirectories;
        private readonly string _rootDir;
        private volatile bool _disposed;
        private readonly HashSet<string> _loaded;
        private readonly HashSet<string> _notLoaded;

        public AssemblyResolver(IEnumerable<string> resolveDirectories, TextWriter logTextWriter = null)
        {
            _notLoaded = new HashSet<string>();
            _loaded = new HashSet<string>();
            _logTextWriter = logTextWriter;

            Assembly assembly  = Assembly.GetEntryAssembly();
            //Could be null in unit test
            _rootDir = assembly != null ? Path.GetDirectoryName(assembly.Location) : Environment.CurrentDirectory;
            _resolveDirectories = resolveDirectories ?? new List<string>();

            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad += OnCurrentDomainAssemblyLoad;
        }

        public HashSet<string> Loaded
        {
            get { return new HashSet<string>(_loaded); }
        }
        public HashSet<string> NotLoaded
        {
            get { return new HashSet<string>(_notLoaded); }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_disposed)
            {
                return;
            }

            _disposed = true;

            AppDomain.CurrentDomain.AssemblyResolve -= OnCurrentDomainAssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad -= OnCurrentDomainAssemblyLoad;
        }

        private void OnCurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (_logTextWriter != null)
            {
                Console.Error.WriteLine("AssemblyResolver: Loaded assembly '{0}' ({1})", new AssemblyName(args.LoadedAssembly.FullName).Name, args.LoadedAssembly.Location);
            }
        }
        private Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);

            if (_notLoaded.Contains(assemblyName.Name))
            {
                return null;
            }

            foreach (string d2 in _resolveDirectories)
            {
                string d = d2;
                if (!Path.IsPathRooted(d))
                {
                    d = Path.Combine(_rootDir, d2);
                }

                if (!Directory.Exists(d))
                {
                    continue;
                }

                string assemblyPath = Path.Combine(d, assemblyName.Name + ".dll");
                if (!File.Exists(assemblyPath))
                {
                    continue;
                }

                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                _loaded.Add(assemblyName.Name);
                return assembly;
            }

            _notLoaded.Add(assemblyName.Name);

            _logTextWriter?.WriteLine("AssemblyResolver: Could not find assembly '{0}'", assemblyName.Name);

            return null;
        }
    }
}