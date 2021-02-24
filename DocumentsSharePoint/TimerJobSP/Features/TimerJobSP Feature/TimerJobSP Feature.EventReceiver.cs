using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace TimerJobSP.Features.TimerJobSP_Feature
{
    /// <summary>
    /// 此类用于处理在激活、停用、安装、卸载和升级功能的过程中引发的事件。
    /// </summary>
    /// <remarks>
    /// 附加到此类的 GUID 可能会在打包期间使用，不应进行修改。
    /// </remarks>

    [Guid("6691ce15-6938-4266-acb2-9407ccdc593b")]
    public class TimerJobSP_FeatureEventReceiver : SPFeatureReceiver
    {
        const string TimerJobName = "PushEmailsTimerJobS";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;
            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Title == TimerJobName)
                {
                    job.Delete();
                }
            }
            PushEmailsTimerJob UpdateTitle = new PushEmailsTimerJob(TimerJobName, site.WebApplication);

            //// 每10分钟执行一次
            //SPMinuteSchedule minuteSchedule = new SPMinuteSchedule();
            //minuteSchedule.BeginSecond = 0;
            //minuteSchedule.EndSecond = 59;
            //minuteSchedule.Interval = 10;
            //UpdateTitle.Schedule = minuteSchedule;

            // 每天1-6点执行一次
            SPDailySchedule dailySchedule = new SPDailySchedule();
            dailySchedule.BeginHour = 1;
            dailySchedule.EndHour = 6;
            UpdateTitle.Schedule = dailySchedule;

            UpdateTitle.Update();

        }


        // 取消对以下方法的注释，以便处理在停用某个功能前引发的事件。

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;
            //遍历此站点集下的所有计时器
            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Name == TimerJobName)
                {
                    job.Delete();
                }
            }
        }


        // 取消对以下方法的注释，以便处理在安装某个功能后引发的事件。

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // 取消对以下方法的注释，以便处理在卸载某个功能前引发的事件。

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // 取消对以下方法的注释，以便处理在升级某个功能时引发的事件。

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
