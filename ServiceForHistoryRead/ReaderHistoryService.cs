using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceForHistoryRead
{
    public partial class ReaderHistoryService : ServiceBase
    {
        public ReaderHistoryService()
        {
            InitializeComponent();

            logger = new EventLog();
            this.AutoLog = false;
            if (!EventLog.SourceExists("Reader Service"))
            {
                EventLog.CreateEventSource(
                    "Reader Service", "HService");
            }
            ////////////////////
            logger.Source = "Reader Service";
            logger.Log = "HService";
            logger.WriteEntry("Started logger.", EventLogEntryType.Information);

            tm = new TimerCallback(readerThread);
            this.AutoLog = false;
        }
        Timer timer;
        TimerCallback tm;
        EventLog logger;
        protected override void OnStart(string[] args)
        {
            logger.WriteEntry("Started service.", EventLogEntryType.Information);
            //Reader.DoReading();
            int num = 0;
            logger.WriteEntry("Started timer.", EventLogEntryType.Information);
            timer = new Timer(tm, num, 0, 7200000);
        }

        protected override void OnStop()
        {
            timer.Dispose();
            logger.WriteEntry("Stoped service.", EventLogEntryType.Information);
        }

        public static void readerThread(object obj)
        {
            Reader.DoReading();
        }
    }
}
