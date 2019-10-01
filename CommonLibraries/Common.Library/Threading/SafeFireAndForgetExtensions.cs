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

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
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
    
