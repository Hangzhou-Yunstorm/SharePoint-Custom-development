using Microsoft.SharePoint.Client;
using System;
using System.Net;

namespace SyncCalculatedWS
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取ClientContext
        /// </summary>
        /// <returns>ClientContext</returns>
        public static ClientContext GetClientContext()
        {
            ClientContext context = new ClientContext(Constant.webUrl);
            try
            {
                context.Credentials = new NetworkCredential(Constant.loginName, Constant.psw, Constant.domain);
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "GetClientContext");
            }
            return context;
        }

    }
}