﻿namespace Common.ViewModel.Exception
{
    using System;
    using System.Text;

    using Common.ViewModel.Dialog;

    public class ExceptionViewModel : DialogViewModelBase
    {
        public ExceptionViewModel(Exception exception)
        {
            Exception ex = exception;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                for (int j = 0; j < i; j++)
                {
                    sb.Append('\t');
                }
                sb.AppendLine(ex.Message);

                i++;
                ex = ex.InnerException;
            }
            ExceptionText = sb.ToString();
        }

        public string ExceptionText { get; }
    }
}
