using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace SyncExternalUserWS
{
    public partial class SyncExternalUser : ServiceBase
    {
        public SyncExternalUser()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += new System.Timers.ElapsedEventHandler(SyncUser);
            t.AutoReset = true;
            t.Enabled = true;
        }

        public void SyncUser(object source, System.Timers.ElapsedEventArgs e)
        {
            if ((DateTime.Now - Constant.RunTime).TotalSeconds > Constant.TimeSpan)
            {
                Constant.RunTime = DateTime.Now;
                System.Timers.Timer tt = (System.Timers.Timer)source;

                tt.Enabled = false;
                try
                {
                    List<UserModel> models = CommonHelper.GetUsers();
                    if (models.Count > 0)
                    {
                        SyncExUser seu = new SyncExUser();
                        seu.SyncUsers(models);
                    }
                }
                catch (Exception ex)
                {
                    XMLHelper.SetLog(ex.Message, "SyncUser");
                }
                tt.Enabled = true;
            }
        }

        protected override void OnStop()
        {
        }
    }
}
