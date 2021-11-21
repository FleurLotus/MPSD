namespace Common.Library.Threading
{
    using System;
    using System.Threading.Tasks;

    //From https://johnthiriet.com/mvvm-going-async-with-async-command/# and https://github.com/johnthiriet/AsyncVoid/blob/master/AsyncVoid
    /// <summary>
    /// Extension methods for System.Threading.Tasks.Task
    /// </summary> 
    public static class SafeFireAndForgetExtensions
    {
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }
}
    
