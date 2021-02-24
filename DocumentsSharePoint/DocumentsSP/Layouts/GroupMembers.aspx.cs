using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using DocumentsSP.Helper;
using System.Collections.Generic;
using DocumentsSP.Model;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace DocumentsSP.Layouts
{
    public partial class GroupMembers : LayoutsPageBase
    {
        /// <summary>
        /// 成员集合
        /// </summary>
        public string Members = "";
        /// <summary>
        /// 组id
        /// </summary>
        public static string groupId = "";
        /// <summary>
        /// 组名称
        /// </summary>
        public static string groupName = "";
        /// <summary>
        /// 操作
        /// </summary>
        public string Operate = "";

        /// <summary>
        /// 初始化，获取组成员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            groupId = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(groupId))
            {
                return;
            }

            try
            {
                var siteId = SPContext.Current.Site.ID;
                var cUser = SPContext.Current.Web.CurrentUser;

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;

                            var group = web.SiteGroups.GetByID(Convert.ToInt32(groupId));
                            groupName = group.Name;

                            if (!cUser.IsSiteAdmin)
                            {
                                if (!group.Description.StartsWith("Overseas_") && groupName != CommonHelper.adminGroup1 && groupName != CommonHelper.adminGroup2)
                                {
                                    Operate = "<img onclick=\"addUsers()\" src=\"images/add.png\" alt=\"add\" class=\"addBtn\" />" +
                                                "<img onclick=\"deleteUsers()\" src=\"images/del.png\" alt=\"delete\" class=\"delBtn\" />";
                                }
                            }
                            else
                            {
                                Operate = "<img onclick=\"addUsers()\" src=\"images/add.png\" alt=\"add\" class=\"addBtn\" />" +
                                                   "<img onclick=\"deleteUsers()\" src=\"images/del.png\" alt=\"delete\" class=\"delBtn\" />";
                            }
                            web.AllowUnsafeUpdates = true;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GroupMembers.aspx__Page_Load", ex.Message);
            }
        }

        [WebMethod]
        public static string GetUsers(string groupId)
        {
            string json = "[]";
            try
            {
                var siteId = SPContext.Current.Site.ID;
                var cUser = SPContext.Current.Web.CurrentUser;
                List<MembersModel> mList = new List<MembersModel>();
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;

                            var group = web.SiteGroups.GetByID(Convert.ToInt32(groupId));
                            var users = group.Users;

                            foreach (SPUser user in users)
                            {
                                var loginNameList = user.LoginName.Split('\\');
                                var account = loginNameList[loginNameList.Length - 1];

                                var nameList = user.Name.Split('\\');
                                var name = nameList[nameList.Length - 1];

                                mList.Add(new Model.MembersModel()
                                {
                                    id = account,
                                    name = name,
                                    email = user.Email
                                });
                                //Members += "{id:'" + account + "',name:'" + name + "',email: '" + user.Email + "'},";
                            }
                            web.AllowUnsafeUpdates = true;
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
            }
            catch (Exception ex)
            {
                json = "[]";
                CommonHelper.SetErrorLog("GroupMembers.aspx__GetUsers", ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="names">用户名集合</param>
        /// <returns>返回删除结果</returns>
        [WebMethod]
        public static string DeleteUsers(string names)
        {
            try
            {
                var siteId = SPContext.Current.Site.ID;
                var cUser = SPContext.Current.Web.CurrentUser;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            var group = web.SiteGroups.GetByID(Convert.ToInt32(groupId));
                            var list = names.Split(',');
                            foreach (var name in list)
                            {
                                var account = CommonHelper.Domain + "\\" + name;
                                account = "i:0#.w|" + account;
                                SPUser user = web.EnsureUser(account);
                                group.RemoveUser(user);
                            }

                            var logList = web.Lists.TryGetList(CommonHelper.logListName);
                            var dItem = logList.AddItem();
                            dItem["Title"] = "Delete group <" + group.Name + "> users";
                            dItem["Operate"] = "Delete";
                            dItem["Operater"] = cUser;
                            dItem["Operator"] = cUser.Name;
                            dItem["OperatorId"] = cUser.LoginName;
                            dItem["ServerIP"] = CommonHelper.GetServerHostName();
                            dItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                            dItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                            dItem["ObjectName"] = names;
                            dItem["ObjectType"] = "User";
                            dItem.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GroupMembers.aspx__DeleteUsers", ex.Message);
            }
            return string.Empty;
        }
    }
}
