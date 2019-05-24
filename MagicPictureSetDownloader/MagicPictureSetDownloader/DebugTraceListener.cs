namespace MagicPictureSetDownloader
{
    using System.Diagnostics;

    public class DebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            
        }

        public override void WriteLine(string message)
        {
            Debugger.Break();
        }
    }
}
