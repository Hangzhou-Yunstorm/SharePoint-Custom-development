using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SyncGroups
{
    public partial class SyncGroupsService : ServiceBase
    {
        public SyncGroupsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += new System.Timers.ElapsedEventHandler(SyncGroups);
            t.AutoReset = true;
            t.Enabled = true;
        }

        public void SyncGroups(object source, System.Timers.ElapsedEventArgs e)
        {
            if ((DateTime.Now - Constant.RunTime).TotalSeconds > Constant.TimeSpan)
            {
                Constant.RunTime = DateTime.Now;
                System.Timers.Timer tt = (System.Timers.Timer)source;
                tt.Enabled = false;

                try
                {
                    GroupOperate go = new GroupOperate();
                    go.Sync();
                }
                catch (Exception ex)
                {
                    XMLHelper.SetLog(ex.Message, "SyncGroups");
                }
                tt.Enabled = true;
            }
        }

        protected override void OnStop()
        {
        }
    }
}
