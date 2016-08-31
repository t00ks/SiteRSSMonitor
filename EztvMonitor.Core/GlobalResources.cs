using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EztvMonitor.Core
{
    public static class GlobalResources
    {
        public const string EVENTSOURCE = "EztvMonitorSvc";
    }

    public static class ExceptionExtention
    {
        public static string GetMessage(this Exception ex, string method = "")
        {
            var message = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(method))
            {
                message.AppendLine("Method: " + method);
            }

            message.AppendLine("message: " + ex.Message);
            message.AppendLine("stack trace: " + ex.StackTrace);
            message.AppendLine("source: " + ex.Source);
            var innerException = ex.InnerException;
            while (innerException != null)
            {
                message.AppendLine("******Inner Exeption********");
                message.AppendLine("message: " + innerException.Message);
                message.AppendLine("stack trace: " + innerException.StackTrace);
                message.AppendLine("source: " + innerException.Source);
                message.AppendLine("****************************");

                innerException = innerException.InnerException;
            }
            return message.ToString();
        }
    }
}
