namespace Common.WPF
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Interop;

    public static class Lib
    {
        public static bool SoftwareRenderMode { get; set; }

        public static bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
        //https://blogs.msdn.microsoft.com/jgoldb/2007/10/10/performance-improvements-in-wpf-in-net-3-5-3-0-sp1/
        public static void ForceSoftwareRenderModeIfNeeded(Visual visual)
        {
            if (SoftwareRenderMode)
            {
                HwndSource hwndSource = PresentationSource.FromVisual(visual) as HwndSource;
                HwndTarget hwndTarget = hwndSource.CompositionTarget;
                // this is the new WPF API to force render mode.
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;
            }
        }
    }
}
