using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EztvMonitor.Core
{
    public static class Logger
    {
        public static void LogMessage(EventLogEntryType logType, string source, string description, int id)
        {
            LogMessage(0, logType, source, description, id);
        }

        public static void LogMessage(int entity, EventLogEntryType logType, string source, string description, int id)
        {
            try
            {
                // write this error to the Windows event log
                EventLog.WriteEntry(source, description, logType, id);
            }
            catch { }
        }
    }
}
