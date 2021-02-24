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
    public partial class UserGroups : LayoutsPageBase
    {
        /// <summary>
        /// 组集合
        /// </summary>
        //public string Groups = "";

        /// <summary>
        /// 操作
        /// </summary>
        public string Operate = "";

        /// <summary>
        /// 初始化，获取组集合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var siteId = SPContext.Current.Site.ID;
                var cUser = SPContext.Current.Web.CurrentUser;

                Operate = "<img onclick=\"addGroup()\" src=\"images/add.png\" alt=\"add\" class=\"addBtn\" />";
                if (cUser.IsSiteAdmin)
                {
                    Operate += "<img onclick=\"deleteGroup()\" src=\"images/del.png\" alt=\"delete\" class=\"delBtn\" />";
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("UserGroups.aspx__Page_Load", ex.Message);
            }

        }

        [WebMethod]
        public static string GetGroups()
        {
            string json = "[]";
            try
            {
                var siteId = SPContext.Current.Site.ID;
                var cUser = SPContext.Current.Web.CurrentUser;
                List<GroupModel> gList = new List<Model.GroupModel>();

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            var groups = web.SiteGroups;
                            foreach (SPGroup gp in groups)
                            {
                                var gName = gp.Name;
                                var hideName = gp.Name;
                                var gId = gp.ID;
                                var gUsers = "";
                                var users = gp.Users;
                                foreach (SPUser user in users)
                                {
                                    if (gUsers.Length > 30)
                                    {
                                        gUsers += "....";
                                        break;
                                    }
                                    gUsers += user.Name + ",";
                                }
                                if (!string.IsNullOrEmpty(gUsers))
                                {
                                    gUsers = gUsers.Substring(0, gUsers.Length - 1);
                                }

                                var oName = gp.Name.Replace(" ", ",");
                                var icon = "<img src=\"/_layouts/15/Images/group.png\" class=\"groupImg\" />";

                                string operation = "";
                                if (!cUser.IsSiteAdmin)
                                {
                                    if (!gp.Description.StartsWith("Overseas_") && gName != CommonHelper.adminGroup1 && gName != CommonHelper.adminGroup2)
                                    {
                                        operation = "<img src=\"/_layouts/15/Images/edit.png\" onclick=changeName(\"" + oName + "\") class=\"groupEditImg\" />";
                                    }
                                }
                                else
                                {
                                    operation = "<img src=\"/_layouts/15/Images/edit.png\" onclick=changeName(\"" + oName + "\") class=\"groupEditImg\" />";
                                }
                                gName = "<a href=\"/_layouts/15/GroupMembers.aspx?ID=" + gId + "\" >" + gName + "</a>";
                                gUsers = "<a href=\"/_layouts/15/GroupMembers.aspx?ID=" + gId + "\" >" + gUsers + "</a>";
                                gList.Add(new Model.GroupModel()
                                {
                                    id = gId,
                                    name = gName,
                                    members = gUsers,
                                    icon = icon,
                                    hideName = hideName,
                                    operation = operation
                                });
                                //Groups += "{id:'" + gId + "',name:'" + gName + "',members: '" + gUsers + "',icon: '" + icon + "',hideName: '" + hideName + "',operation: '" + operation + "'},";
                            }
                        }
                    }
                });

                DataContractJsonSerializer dcs = new DataContractJsonSerializer(gList.GetType());
                //序列化
                using (MemoryStream stream = new MemoryStream())
                {
                    dcs.WriteObject(stream, gList);
                    json = Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("UserGroups.aspx__GetGroups", ex.Message);
                json = "[]";
            }
            return json;
        }

        /// <summary>
        /// 修改组名
        /// </summary>
        /// <param name="oldName">原组名</param>
        /// <param name="newName">新组名</param>
        /// <returns></returns>
        [WebMethod]
        public static string RenameGroup(string oldName, string newName)
        {
            string msg = string.Empty;

            var siteId = SPContext.Current.Site.ID;
            var cUser = SPContext.Current.Web.CurrentUser;
            if (!cUser.IsSiteAdmin)
            {
                if (oldName == CommonHelper.adminGroup1 || oldName == CommonHelper.adminGroup2)
                {
                    msg = "No";
                    return msg;
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(oldName))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;
                                var groups = web.SiteGroups;
                                var group = groups.GetByName(oldName);
                                group.Name = newName;
                                group.Update();

                                var logList = web.Lists.TryGetList(CommonHelper.logListName);
                                var listItem = logList.AddItem();
                                listItem["Title"] = "Update group";
                                listItem["Operate"] = "Update";
                                listItem["Operater"] = cUser;
                                listItem["Operator"] = cUser.Name;
                                listItem["OperatorId"] = cUser.LoginName;
                                listItem["ServerIP"] = CommonHelper.GetServerHostName();
                                listItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                                listItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                                listItem["ObjectName"] = oldName + "-->" + newName;
                                listItem["ObjectType"] = "User";
                                listItem.Update();

                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("UserGroups.aspx__RenameGroup", ex.Message);
            }
            return msg;
        }

        /// <summary>
        /// 删除用户组
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        [WebMethod]
        public static string DeleteGroup(string ids)
        {
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
                            var groups = web.SiteGroups;
                            var idList = ids.Split(',');
                            for (int m = 0; m < idList.Length; m++)
                            {
                                var gName = groups.GetByID(Convert.ToInt32(idList[m])).Name;
                                groups.Remove(gName);

                                var logList = web.Lists.TryGetList(CommonHelper.logListName);
                                var listItem = logList.AddItem();
                                listItem["Title"] = "Delete group : " + gName;
                                listItem["Operate"] = "Delete";
                                listItem["Operater"] = cUser;
                                listItem["Operator"] = cUser.Name;
                                listItem["OperatorId"] = cUser.LoginName;
                                listItem["ServerIP"] = CommonHelper.GetServerHostName();
                                listItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                                listItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                                listItem["ObjectName"] = gName;
                                listItem["ObjectType"] = "User";
                                listItem.Update();
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("UserGroups.aspx__DeleteGroup", ex.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
            return msg;
        }

    }
}
