using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using EztvMonitor.Core;

namespace EztvMonitor.Service
{
    public partial class MonitorService : ServiceBase
    {
        private System.Timers.Timer _startupTimer;
        
        private Processor processor;

        public MonitorService()
        {
            InitializeComponent();
            _startupTimer = new System.Timers.Timer();
            _startupTimer.Interval = 15000;
            _startupTimer.Elapsed += new System.Timers.ElapsedEventHandler(_startupTimer_Elapsed);
        }

        protected override void OnStart(string[] args)
        {
            // log starting event - NB if the server is starting up, this may not work, so ensure we can retry
            lock (this)
            {
                if (LogStartingEvent())
                    StartProcessing();
                else
                    _startupTimer.Start();
            }
        }

        protected override void OnStop()
        {
            Logger.LogMessage(EventLogEntryType.Information, GlobalResources.EVENTSOURCE, "xaps service stopping...", 0);

            try
            {
                if (processor != null)
                {
                    processor.Stop();
                }
                processor = null;
                Logger.LogMessage(EventLogEntryType.Information, GlobalResources.EVENTSOURCE, "xaps service stopped", 0);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(EventLogEntryType.Error, GlobalResources.EVENTSOURCE, ex.Message, 0);
            }
        }

        private void _startupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _startupTimer.Stop();

            if (LogStartingEvent())
            {
                StartProcessing();
            }
            else
            {
                _startupTimer.Start();
            }
        }

        private bool LogStartingEvent()
        {
            try
            {
                Logger.LogMessage(EventLogEntryType.Information, GlobalResources.EVENTSOURCE, "eztv service starting...", 0);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void StartProcessing()
        {
            try
            {
                if (processor != null)
                {
                    throw new ApplicationException("processor is not null");
                }
                processor = new Processor();
                processor.Start();

                Logger.LogMessage(EventLogEntryType.Information, GlobalResources.EVENTSOURCE, "xaps service started", 0);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(EventLogEntryType.Error, GlobalResources.EVENTSOURCE, ex.Message, 0);
            }
        }
    }
}
