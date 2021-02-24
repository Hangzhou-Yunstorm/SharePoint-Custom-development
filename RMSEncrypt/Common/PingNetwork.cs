using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PingNetwork
    {
        public static bool RMSServerConnect()
        {
            bool online = false; //是否在线  
            Ping ping = new Ping();
            PingReply pingReply = ping.Send("it-policy.dahuatech.com");
            if (pingReply.Status == IPStatus.Success)
            {
                online = true;
                
            }
            else
            {
                online = false;
            }

            return online;
        }
    }
}
