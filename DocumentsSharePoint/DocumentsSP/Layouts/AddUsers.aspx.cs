using DocumentsSP.Helper;
using DocumentsSP.Model;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Services;

namespace DocumentsSP.Layouts
{
    public partial class AddUsers : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userNames"></param>
        /// <returns>添加结果</returns>
        [WebMethod]
        public static string AddGroup(string groupName, string userNames)
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
                            try
                            {
                                var alreadyGroup = groups.GetByName(groupName);
                                if (alreadyGroup != null)
                                    msg = "The user group name already exists, please rename it !";
                            }
                            catch { }

                            if (string.IsNullOrEmpty(msg))
                            {
                                SPMember oMember = cUser;
                                groups.Add(groupName, oMember, null, string.Empty);

                                var group = groups.GetByName(groupName);

                                var logList = web.Lists.TryGetList(CommonHelper.logListName);
                                var listItem = logList.AddItem();
                                listItem["Title"] = "Add group";
                                listItem["Operate"] = "Add";
                                listItem["Operater"] = cUser;
                                listItem["Operator"] = cUser.Name;
                                listItem["OperatorId"] = cUser.LoginName;
                                listItem["ServerIP"] = CommonHelper.GetServerHostName();
                                listItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                                listItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                                listItem["ObjectName"] = groupName;
                                listItem["ObjectType"] = "User";
                                listItem.Update();

                                if (!string.IsNullOrEmpty(userNames))
                                {
                                    List<string> sucUser = new List<string>();
                                    var listNames = userNames.Split(',');
                                    foreach (var name in listNames)
                                    {
                                        try
                                        {
                                            var account = CommonHelper.Domain + "\\" + name;
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
                                            sucUser.Add(name);
                                        }
                                        catch (Exception e)
                                        {
                                            CommonHelper.SetErrorLog("AddUsers.aspx__AddGroup_AddUser", e.Message);
                                        }
                                    }
                                    string aNames = string.Join(",", sucUser.ToArray());
                                    var uItem = logList.AddItem();
                                    uItem["Title"] = "Add group <" + groupName + "> users";
                                    uItem["Operate"] = "Add";
                                    uItem["Operater"] = cUser;
                                    uItem["Operator"] = cUser.Name;
                                    listItem["OperatorId"] = cUser.LoginName;
                                    listItem["ServerIP"] = CommonHelper.GetServerHostName();
                                    uItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                                    uItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                                    uItem["ObjectName"] = aNames;
                                    uItem["ObjectType"] = "User";
                                    uItem.Update();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            CommonHelper.SetErrorLog("AddUsers.aspx__AddGroup", e.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
            return msg;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns>所有用户</returns>
        [WebMethod]
        public static string GetUsers()
        {
            string groupUser = "[]";
            try
            {
                BllHelper bll = new BllHelper();
                var users = bll.GetAllUsers();

                //序列化
                DataContractJsonSerializer json = new DataContractJsonSerializer(users.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, users);
                    groupUser = Encoding.UTF8.GetString(stream.ToArray());
                }

                return groupUser;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("AddUsers.aspx__GetUsers", ex.Message);
                return groupUser;
            }
        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <returns>搜索用户集合</returns>
        [WebMethod]
        public static string GetUsersBySearch(string search)
        {
            string groupUser = "[]";
            try
            {
                BllHelper bll = new BllHelper();
                var users = bll.GetAllUsers();
                if (!string.IsNullOrEmpty(search))
                {
                    users = (from u in users where u.Name.ToLower().Contains(search.ToLower()) || u.Account == search select u).ToList();
                }

                //序列化
                DataContractJsonSerializer json = new DataContractJsonSerializer(users.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, users);
                    groupUser = Encoding.UTF8.GetString(stream.ToArray());
                }

                return groupUser;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("AddUsers.aspx__GetUsersBySearch", ex.Message);
                return groupUser;
            }
        }

        #region AD 读取
        /// <summary>
        /// 获取部门
        /// </summary>
        /// <returns>部门</returns>
        [WebMethod]
        public static string GetDepartment()
        {
            string obj = "[]";
            try
            {
                DirectoryEntry de = new DirectoryEntry(CommonHelper.LDAPUrl, CommonHelper.AdminId, CommonHelper.AdminPassword);
                DirectorySearcher deSearch = new DirectorySearcher(de);
                deSearch.Filter = "((objectClass=organizationalUnit))";
                deSearch.SearchScope = SearchScope.Subtree;
                SearchResultCollection result = deSearch.FindAll();
                if (result.Count > 0)
                {
                    List<DepartModel> ouList = new List<DepartModel>();
                    for (int i = 0; i < result.Count; i++)
                    {
                        DirectoryEntry deDept = result[i].GetDirectoryEntry();
                        ouList.Add(new DepartModel() { DepartName = deDept.Name.Replace("OU=", ""), DepartId = i + 1, DepartParentName = deDept.Parent.Name.Replace("OU=", ""), DepartPath = deDept.Path, DepartParentId = 0 });
                    }

                    // set DepartParentId
                    foreach (var depart in ouList)
                    {
                        var childList = (from u in ouList where u.DepartParentName == depart.DepartName select u).ToList();
                        foreach (var cd in childList)
                        {
                            cd.DepartParentId = depart.DepartId;
                        }
                    }

                    //序列化
                    DataContractJsonSerializer json = new DataContractJsonSerializer(ouList.GetType());
                    using (MemoryStream stream = new MemoryStream())
                    {
                        json.WriteObject(stream, ouList);
                        obj = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("AddUsers.aspx__GetDepartment", ex.Message);
                return obj;
            }
        }

        /// <summary>
        /// 根据部门ID获取人员
        /// </summary>
        /// <param name="deptID">部门ID</param>
        /// <returns>人员</returns>
        [WebMethod]
        public static string GetEmployee(string ldapOUUrl)
        {
            string groupUser = "[]";
            try
            {
                DirectoryEntry de = new DirectoryEntry(ldapOUUrl, CommonHelper.AdminId, CommonHelper.AdminPassword);
                DirectorySearcher deSearch = new DirectorySearcher(de);
                deSearch.Filter = "(&(objectCategory=person)(objectClass=user))";
                deSearch.SearchScope = SearchScope.Subtree;
                SearchResultCollection result = deSearch.FindAll();
                if (result.Count > 0)
                {
                    List<UserModel> users = new List<UserModel>();
                    var siteId = SPContext.Current.Site.ID;
                    var cUser = SPContext.Current.Web.CurrentUser;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    DirectoryEntry deuser = result[i].GetDirectoryEntry();
                                    if (deuser.SchemaClassName == "user")
                                    {
                                        var account = CommonHelper.GetPropertyValueString(deuser, "sAMAccountName");
                                        var email = CommonHelper.GetPropertyValueString(deuser, "mail");

                                        var userAccount = "i:0#.w|" + CommonHelper.Domain + "\\" + account;
                                        try
                                        {
                                            SPUser spuser = web.EnsureUser(userAccount);
                                            if (string.IsNullOrEmpty(spuser.Email))
                                            {
                                                spuser.Email = email;
                                                spuser.Update();
                                            }
                                        }
                                        catch { }

                                        UserModel user = new UserModel() { UserName = deuser.Name.Replace("CN=", ""), DepartmentName = deuser.Parent.Name.Replace("OU=", ""), UserId = deuser.Path, UserAccount = account };
                                        users.Add(user);
                                    }
                                }
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    });

                    //序列化
                    DataContractJsonSerializer json = new DataContractJsonSerializer(users.GetType());
                    using (MemoryStream stream = new MemoryStream())
                    {
                        json.WriteObject(stream, users);
                        groupUser = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
                return groupUser;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("AddUsers.aspx__GetEmployee", ex.Message);
                return groupUser;
            }
        }

        /// <summary>
        /// 根据输入框获取人员
        /// </summary>
        /// <param name="search">输入字符串</param>
        /// <returns>人员</returns>
        [WebMethod]
        public static string GetEmployeeByInput(string search)
        {
            string searchUser = "[]";
            try
            {
                DirectoryEntry de = new DirectoryEntry(CommonHelper.LDAPUrl, CommonHelper.AdminId, CommonHelper.AdminPassword);
                DirectorySearcher deSearch = new DirectorySearcher(de);
                deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(CN=*" + search + "*))";
                deSearch.SearchScope = SearchScope.Subtree;
                SearchResultCollection result = deSearch.FindAll();
                if (result.Count > 0)
                {
                    List<UserModel> users = new List<UserModel>();
                    var siteId = SPContext.Current.Site.ID;
                    var cUser = SPContext.Current.Web.CurrentUser;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;
                                for (int i = 0; i < result.Count; i++)
                                {
                                    DirectoryEntry deuser = result[i].GetDirectoryEntry();
                                    var account = CommonHelper.GetPropertyValueString(deuser, "sAMAccountName");
                                    var email = CommonHelper.GetPropertyValueString(deuser, "mail");

                                    var userAccount = "i:0#.w|" + CommonHelper.Domain + "\\" + account;
                                    try
                                    {
                                        SPUser spuser = web.EnsureUser(userAccount);
                                        if (string.IsNullOrEmpty(spuser.Email))
                                        {
                                            spuser.Email = email;
                                            spuser.Update();
                                        }
                                    }
                                    catch { }

                                    UserModel user = new UserModel() { UserName = deuser.Name.Replace("CN=", ""), DepartmentName = deuser.Parent.Name.Replace("OU=", ""), UserId = deuser.Path, UserAccount = account };
                                    users.Add(user);
                                }
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    });

                    //序列化
                    DataContractJsonSerializer json = new DataContractJsonSerializer(users.GetType());
                    using (MemoryStream stream = new MemoryStream())
                    {
                        json.WriteObject(stream, users);
                        searchUser = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
                return searchUser;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("AddUsers.aspx__GetEmployeeByInput", ex.Message);
                return searchUser;
            }
        }
        #endregion
    }
}
