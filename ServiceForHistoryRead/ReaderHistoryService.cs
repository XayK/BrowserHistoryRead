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
            tm = new TimerCallback(readerThread);
        }
        Timer timer;
        TimerCallback tm;
        protected override void OnStart(string[] args)
        {
            int num = 0;
            timer = new Timer(tm, num, 0, 7200000);
        }

        protected override void OnStop()
        {
            timer.Dispose();
        }

        public static void readerThread(object obj)
        {
            Reader.DoReading();
        }
    }
}
