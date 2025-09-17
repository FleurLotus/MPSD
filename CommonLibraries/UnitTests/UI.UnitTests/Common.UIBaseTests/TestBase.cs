namespace Common.UIBaseTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using NUnit.Framework;

    using XamlTest;

    public abstract class TestBase
    {
        private const string Namespace = @"
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
";
        private const string DefaultWindowsProperties = @"
    mc:Ignorable=""d""
    Height=""200"" Width=""500""
    TextElement.FontWeight=""Regular""
    TextElement.FontSize=""13""
    TextOptions.TextFormattingMode=""Ideal""
    TextOptions.TextRenderingMode=""Auto""
    Title=""Test Window""
    WindowStartupLocation=""CenterScreen""
";

        protected bool AttachedDebuggerToRemoteProcess { get; set; } = true;

        protected IApp App { get; private set; }

        protected async Task<IVisualElement<T>> LoadXaml<T>(string xaml, params (string namespacePrefix, Type type)[] additionalNamespaceDeclarations)
        {
            return await CreateWindowWith<T>(xaml, additionalNamespaceDeclarations);
        }

        protected async Task<IVisualElement> LoadXaml(string xaml)
        {
            return await CreateWindowWith(xaml);
        }

        protected async Task<IVisualElement> LoadUserControl<TControl>()
            where TControl : UserControl
        {
            return await LoadUserControl(typeof(TControl));
        }

        protected async Task<IVisualElement> LoadUserControl(Type userControlType)
        {
            return await CreateWindowWithUserControl(userControlType);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected async Task InitializeWithDesign(string applicationResourcesXaml = null, params string[] assemblies)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            StackFrame userFrame = new StackTrace(1, true).GetFrames().FirstOrDefault(frame =>
            {
                Type declaringType = frame.GetMethod()?.DeclaringType;
                Assembly assembly = declaringType?.Assembly;
                return assembly != null && assembly != currentAssembly &&
                       !assembly.FullName.StartsWith("System.") &&
                       !declaringType.Name.Contains("AsyncStateMachine");
            });

            Assembly assembly = userFrame?.GetMethod()?.DeclaringType?.Assembly;

            HashSet<string> allAssemblies = new HashSet<string>(assemblies ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase)
            {
                currentAssembly.Location,
                assembly?.Location?? currentAssembly.Location,
            };

            applicationResourcesXaml ??= @"
<ResourceDictionary 
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <ResourceDictionary.MergedDictionaries>
    </ResourceDictionary.MergedDictionaries>
</ResourceDictionary>";

            await App.Initialize(applicationResourcesXaml, allAssemblies.ToArray());
        }

        protected async Task RegisterSerializer<T>() where T : ISerializer, new()
        {
            await App.RegisterSerializer<T>();
        }

        [SetUp]
        public async ValueTask InitializeAsync()
        {
            App = await XamlTest.App.StartRemote(new AppOptions
            {
#if !DEBUG
                MinimizeOtherWindows = !System.Diagnostics.Debugger.IsAttached,
#endif
                AllowVisualStudioDebuggerAttach = AttachedDebuggerToRemoteProcess,
            });
        }

        [TearDown]
        public async ValueTask DisposeAsync()
        {
            await App.DisposeAsync();
        }

        private async Task<IVisualElement<T>> CreateWindowWith<T>(string xaml, params (string namespacePrefix, Type type)[] additionalNamespaceDeclarations)
        {
            StringBuilder extraNamespaceDeclarations = new StringBuilder("");
            foreach ((string namespacePrefix, Type type) in additionalNamespaceDeclarations)
            {
                extraNamespaceDeclarations.AppendLine($@"xmlns:{namespacePrefix}=""clr-namespace:{type.Namespace};assembly={type.Assembly.GetName().Name}""");
            }

            string windowXaml =
@$"<Window
    {Namespace}
    {extraNamespaceDeclarations}
    {DefaultWindowsProperties}
    Topmost=""True""
    >
    {xaml}
</Window>";
            IWindow window = await App.CreateWindow(windowXaml);
            return await window.GetElement<T>(".Content");
        }
        private async Task<IVisualElement> CreateWindowWith(string xaml)
        {
            string windowXaml =
@$"<Window
    {Namespace}
    {DefaultWindowsProperties}
    Topmost=""True""
    >
    {xaml}
</Window>";
            IWindow window = await App.CreateWindow(windowXaml);
            return await window.GetElement(".Content");
        }
        private async Task<IVisualElement> CreateWindowWithUserControl(Type userControlType)
        {
            string windowXaml =
@$"<Window
    {Namespace}
    xmlns:local=""clr-namespace:{userControlType.Namespace};assembly={userControlType.Assembly.GetName().Name}""
    {DefaultWindowsProperties}
    Topmost=""False""
    >
    <Grid>
        <local:{userControlType.Name} x:Name=""Container"" VerticalAlignment=""Center"" />
    </Grid>
</Window>";
            IWindow window = await App.CreateWindow(windowXaml);
            return await window.GetElement("Container.Content");
        }

        protected async Task LeftClick(IVisualElement element)
        {
            await element.LeftClick();
            await Task.Delay(100);
        }
    }
}