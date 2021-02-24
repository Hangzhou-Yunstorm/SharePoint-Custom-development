using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using System.Collections.Generic;
using System.DirectoryServices;
using DocumentsSP.Helper;
using System.Linq;
using DocumentsSP.Model;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace DocumentsSP.Layouts
{
    public partial class EditUsers : LayoutsPageBase
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public static string groupName = "";
        /// <summary>
        /// 组ID
        /// </summary>
        public static string groupId = "";

        public string selContent = "";

        public static List<string> oldUsers = new List<string>();

        //public string currentUsers = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            groupId = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(groupId))
            {
                return;
            }
            selContent = GetSelContent();
        }

        /// <summary>
        /// 获取勾选的用户
        /// </summary>
        /// <returns>勾选的用户</returns>
        private string GetSelContent()
        {
            string selContentTrs = "";
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var group = web.SiteGroups.GetByID(Convert.ToInt32(groupId));
                        groupName = group.Name;
                        var users = group.Users;

                        foreach (SPUser user in users)
                        {

                            var nameList = user.Name.Split('\\');
                            var name = nameList[nameList.Length - 1];

                            var accountList = user.LoginName.Split('\\');
                            var account = accountList[accountList.Length - 1];

                            oldUsers.Add(account);

                            var delAccount = Microsoft.JScript.GlobalObject.escape(account);
                            var del = "<img src=\"images/del.png\" onclick=\"deleteObj('" + delAccount + "')\" title=\"Delete " + name + "\"  />";
                            var selContentTr = "<tr id=\"td_" + account + "\" title=\"" + name + "\"><td id=\"tdd_" + account + "\">" + name + "</td><td>" + del + "</td></tr>";

                            selContentTrs += selContentTr;
                        }
                    }
                }
            });
            return selContentTrs;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetCurrentUsers(string groupId)
        {
            string json = "[]";
            try
            {
                List<EditMembersModel> mList = new List<EditMembersModel>();
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            var group = web.SiteGroups.GetByID(Convert.ToInt32(groupId));
                            groupName = group.Name;
                            var users = group.Users;

                            foreach (SPUser user in users)
                            {

                                var nameList = user.Name.Split('\\');
                                var name = nameList[nameList.Length - 1];

                                var accountList = user.LoginName.Split('\\');
                                var account = accountList[accountList.Length - 1];

                                //currentUsers += "{name:'" + name + "',account:'" + account + "'},";
                                mList.Add(new EditMembersModel()
                                {
                                    account = account,
                                    name = name
                                });
                            }
                        }
                    }
                });
                DataContractJsonSerializer dcs = new DataContractJsonSerializer(mList.GetType());
                //序列化
                using (MemoryStream stream = new MemoryStream())
                {
                    dcs.WriteObject(stream, mList);
                    json = Encoding.UTF8.GetString(stream.ToArray());
                }
                return json;
            }
            catch (Exception ex)
            {
                return "[]";
            }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="userNames">用户名集合</param>
        /// <returns>返回结果</returns>
        [WebMethod]
        public static string AddUsers(string userNames)
        {
            userNames = Microsoft.JScript.GlobalObject.unescape(userNames);
            string msg = string.Empty;
            var siteId = SPContext.Current.Site.ID;
            var cUser = SPContext.Current.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        try
                        {
                            //var userArray = oldNames.Split(',');
                            var userList = oldUsers;

                            var groups = web.SiteGroups;
                            var group = groups.GetByName(groupName);

                            var logList = web.Lists.TryGetList(CommonHelper.logListName);

                            List<string> keepUses = new List<string>();
                            List<string> addUses = new List<string>();
                            if (!string.IsNullOrEmpty(userNames))
                            {
                                var listNames = userNames.Split(',');

                                foreach (var name in listNames)
                                {
                                    if (!userList.Contains(name))
                                    {
                                        addUses.Add(name);
                                    }
                                    else
                                    {
                                        keepUses.Add(name);
                                    }
                                }
                                List<string> sucUser = new List<string>();
                                foreach (var addName in addUses)
                                {
                                    try
                                    {
                                        var account = CommonHelper.Domain + "\\" + addName;
                                        account = "i:0#.w|" + account;
                                        try
                                        {
                                            SPUser spuser = web.EnsureUser(account);

                                            if (spuser != null)
                                            {
                                                group.AddUser(spuser);
                                            }
                                            else
                                            {
                                                group.AddUser(account, "", "", "");
                                            }
                                        }
                                        catch
                                        {
                                            group.AddUser(account, "", "", "");
                                        }

                                        sucUser.Add(addName);
                                    }
                                    catch (Exception e)
                                    {
                                        CommonHelper.SetErrorLog("EditUsers.aspx__AddUsers_AddUser", e.Message);
                                    }
                                }

                                string aNames = string.Join(",", sucUser.ToArray());
                                var listItem = logList.AddItem();
                                listItem["Title"] = "Add group <" + groupName + "> users";
                                listItem["Operate"] = "Add";
                                listItem["Operater"] = cUser;
                                listItem["Operator"] = cUser.Name;
                                listItem["OperatorId"] = cUser.LoginName;
                                listItem["ServerIP"] = CommonHelper.GetServerHostName();
                                listItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                                listItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                                listItem["ObjectName"] = aNames;
                                listItem["ObjectType"] = "User";
                                listItem.Update();
                            }
                            foreach (var kName in keepUses)
                            {
                                userList.Remove(kName);
                            }
                            if (userList.Count > 0)
                            {
                                foreach (var rName in userList)
                                {
                                    try
                                    {
                                        var account = CommonHelper.Domain + "\\" + rName;
                                        account = "i:0#.w|" + account;
                                        SPUser user = web.EnsureUser(account);
                                        group.RemoveUser(user);
                                    }
                                    catch (Exception e)
                                    {
                                        CommonHelper.SetErrorLog("EditUsers.aspx__AddUsers_RemoveUser", e.Message);
                                    }
                                }

                                string dNames = string.Join(",", userList.ToArray());
                                var dItem = logList.AddItem();
                                dItem["Title"] = "Delete group <" + groupName + "> users";
                                dItem["Operate"] = "Delete";
                                dItem["Operater"] = cUser;
                                dItem["Operator"] = cUser.Name;
                                dItem["OperatorId"] = cUser.LoginName;
                                dItem["ServerIP"] = CommonHelper.GetServerHostName();
                                dItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                                dItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                                dItem["ObjectName"] = dNames;
                                dItem["ObjectType"] = "User";
                                dItem.Update();
                            }
                        }
                        catch (Exception e)
                        {
                            CommonHelper.SetErrorLog("EditUsers.aspx__AddUsers", e.Message);
                        }
                        web.AllowUnsafeUpdates = false;

                    }
                }
            });
            return msg;
        }

    }
}
