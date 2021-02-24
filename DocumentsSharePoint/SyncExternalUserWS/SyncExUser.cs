using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace SyncExternalUserWS
{
    public class SyncExUser
    {
        /// <summary>
        /// 批量同步外部人员信息
        /// </summary>
        /// <param name="users">人员列表</param>
        /// <returns>同步结果</returns>
        public void SyncUsers(List<UserModel> users)
        {
            XMLHelper.SetLog("SyncUsers start.", "SyncUsers");
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    // The SharePoint web at the URL.
                    Web web = context.Web;
                    // 外部人员列表
                    var externalList = web.Lists.GetByTitle(Constant.externalList);

                    foreach (var user in users)
                    {
                        try
                        {
                            string account = user.LOGONID;
                            string name = user.NAME;
                            string psw = user.PASSWORD;
                            string region = user.REGION;
                            string country = user.COUNTRY;
                            bool state = true;

                            CamlQuery query = new CamlQuery();
                            query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Account' /><Value Type='Text'>" + account + "</Value></Eq></Where></View></Query>";

                            var items = externalList.GetItems(query);
                            context.Load(items);
                            context.ExecuteQuery();

                            if (items != null && items.Count > 0)
                            {
                                ListItem updateItem = items[0];
                                updateItem["Account"] = account;
                                updateItem["ObjectName"] = name;
                                updateItem["PassWord"] = psw;
                                updateItem["Region"] = region;
                                updateItem["EnabledState"] = state;
                                updateItem["Country"] = country;
                                updateItem["Tag"] = "Sync Create";
                                updateItem.Update();
                                context.ExecuteQuery();
                            }
                            else
                            {
                                ListItemCreationInformation item = new ListItemCreationInformation();
                                ListItem addItem = externalList.AddItem(item);
                                addItem["Account"] = account;
                                addItem["ObjectName"] = name;
                                addItem["PassWord"] = psw;
                                addItem["Region"] = region;
                                addItem["EnabledState"] = state;
                                addItem["Country"] = country;
                                addItem["Tag"] = "Sync Create";
                                addItem.Update();
                                context.ExecuteQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            XMLHelper.SetLog(ex.Message + " user :" + user.LOGONID, "SyncUsers");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "SyncUsers");
            }
            XMLHelper.SetLog("SyncUsers end.", "SyncUsers");
        }


        /// <summary>
        /// 根据账户删除用户
        /// </summary>
        /// <param name="account">账户</param>
        public void Delete(string account)
        {
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.

                    var externalList = web.Lists.GetByTitle(Constant.externalList);

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Account' /><Value Type='Text'>" + account + "</Value></Eq></Where></View></Query>";

                    var items = externalList.GetItems(query);
                    context.Load(items);
                    context.ExecuteQuery();

                    if (items != null && items.Count > 0)
                    {
                        while (items.Count > 0)
                        {
                            items[0].DeleteObject();
                            context.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message + " account : " + account, "Delete");
            }
        }

    }
}
