using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using System.Web.Services;
using System.Linq;

namespace DocumentsSP.Layouts
{
    public partial class ChangeDepartment : LayoutsPageBase
    {
        public string groupsJson = "";
        public string groupListJson = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var groups = web.SiteGroups;
                        var thdepartIds = CommonHelper.TDGroupIds;
                        foreach (SPGroup group in groups)
                        {
                            if (group.Name != CommonHelper.adminGroup2 && group.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                            {
                                var departId = group.Description.Replace("Overseas_", "");
                                if (!thdepartIds.Contains(departId))
                                {
                                    var groupName = group.Name;
                                    int endIndex;
                                    bool b = CommonHelper.GetStrIndex(groupName, 30, out endIndex);
                                    if (!b)
                                    {
                                        groupName = groupName.Substring(0, endIndex - 1) + "...";
                                    }
                                    groupsJson += "<option value=\"" + departId + "\">" + groupName + "</option>";
                                    groupListJson += "<option value=\"" + groupName + "\" />";
                                }
                            }
                        }
                    }
                }
            });
        }

        [WebMethod]
        public static string ChangeDepart(string oldName, string departName, string departId)
        {
            string msg = "";
            try
            {
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            var logList = web.Lists.TryGetList(CommonHelper.logListName);

                            SPQuery dquery = new SPQuery();
                            dquery.Query = "<Where>" +
                                                 "<Contains><FieldRef Name='Department' /><Value Type='Text'>" + oldName + "</Value></Contains>" +
                                                 "</Where>";
                            var logins = logList.GetItems(dquery);
                            bool isExist = false;

                            #region 遍历操作
                            foreach (SPListItem log in logins)
                            {
                                var departmentName = log["Department"] == null ? "" : log["Department"].ToString();
                                var departmentId = log["DepartmentId"] == null ? "" : log["DepartmentId"].ToString();


                                if (!string.IsNullOrEmpty(departmentName))
                                {
                                    var names = departmentName.Split(',').ToList();
                                    var ids = departmentId.Split(',').ToList();

                                    int nu = names.FindIndex(T => T == oldName);

                                    // 存在
                                    if (nu > -1)
                                    {
                                        isExist = true;

                                        names[nu] = departName;
                                        if (ids.Count > nu)
                                        {
                                            ids[nu] = departId;
                                        }
                                        else
                                        {
                                            ids.Add(departId);
                                        }

                                        string newName = string.Join(",", names);
                                        string newId = string.Join(",", ids);

                                        log["Department"] = newName;
                                        log["DepartmentId"] = newId;
                                        log.SystemUpdate(false);
                                    }
                                }
                            }
                            #endregion

                            web.AllowUnsafeUpdates = false;

                            if (isExist)
                            {
                                msg = "Update successful !";
                            }
                            else
                            {
                                msg = "The modified group cannot be found !";
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                CommonHelper.SetErrorLog("UserUploadsReport.aspx__GetDatas", ex.Message);
            }
            return msg;
        }

    }
}
