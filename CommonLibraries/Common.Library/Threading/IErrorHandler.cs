namespace Common.Library.Threading
{
    using System;

    //From https://johnthiriet.com/mvvm-going-async-with-async-command/# and https://github.com/johnthiriet/AsyncVoid/blob/master/AsyncVoid
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

}
