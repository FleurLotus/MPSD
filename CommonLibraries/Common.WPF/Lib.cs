namespace Common.WPF
{
    using System.ComponentModel;
    using System.Windows;

    public static class Lib
    {
        public static bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
    }
}
