using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SyncCalculatedWS
{
    public partial class SyncService : ServiceBase
    {
        public SyncService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += new System.Timers.ElapsedEventHandler(SyncDocument);
            t.AutoReset = true;
            t.Enabled = true;
        }

        public static bool isRunning = false;

        public void SyncDocument(object source, System.Timers.ElapsedEventArgs e)
        {
            if ((DateTime.Now - Constant.RunTime).TotalSeconds > Constant.TimeSpan && !isRunning)
            {
                Constant.RunTime = DateTime.Now;
                System.Timers.Timer tt = (System.Timers.Timer)source;
                tt.Enabled = false;
                isRunning = true;

                try
                {
                    SPDocumentCalculated go = new SPDocumentCalculated();
                    go.Sync();
                }
                catch (Exception ex)
                {
                    XMLHelper.SetLog(ex.Message, "SyncDocumentCalculated");
                }
                isRunning = false;
                tt.Enabled = true;
            }
        }

        protected override void OnStop()
        {
        }
    }
}
