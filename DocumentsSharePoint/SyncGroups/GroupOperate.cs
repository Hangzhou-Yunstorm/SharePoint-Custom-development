using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SyncGroups
{
    public class GroupOperate
    {
        /// <summary>
        /// 用户组同步
        /// </summary>
        /// <returns>同步信息</returns>
        public void Sync()
        {
            XMLHelper.SetLog("GroupOperate start.", "Sync");
            try
            {
                BllHelper bll = new BllHelper();
                List<GroupUser> allUsers = bll.GetAllUsers();

                if (allUsers != null && allUsers.Count > 0)
                {
                    List<string> updateGroups = new List<string>();
                    List<string> deleteGroups = new List<string>();

                    using (ClientContext context = CommonHelper.GetClientContext())
                    {
                        // The SharePoint web at the URL.
                        Web web = context.Web;
                        var groups = web.SiteGroups;

                        #region  全员组
                        try
                        {

                            Depart rootDepart = new Depart();
                            rootDepart.DepartId = Constant.rootDepartId;
                            rootDepart.DepartName_En = Constant.AllUserGroup;
                            rootDepart.DepartName = Constant.AllUserGroup;
                            SyncGroupAndUsers(groups, web, context, allUsers, rootDepart, updateGroups);
                        }
                        catch (Exception ex)
                        {
                            XMLHelper.SetLog(ex.Message, "Sync_AllGroup");
                        }
                        #endregion

                        #region 二级部门组
                        List<string> gIds = new List<string>();
                        try
                        {
                            // 所有二级部门
                            var subDeparts = bll.GetAllSubDeparts(Constant.rootDepartId);
                            foreach (var subDepart in subDeparts)
                            {
                                // 二级部门用户
                                var subUsers = bll.GetUsersByPId(subDepart.DepartId);

                                var isAdd = SyncGroupAndUsers(groups, web, context, subUsers, subDepart, updateGroups);
                                if (isAdd)
                                {
                                    gIds.Add(subDepart.DepartId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            XMLHelper.SetLog(ex.Message, "Sync_SubGroups");
                        }
                        #endregion

                        #region  产品部下的三级部门
                        try
                        {
                            //产品部下的子部门
                            var productSubDeparts = bll.GetDepartsByIds(Constant.productSubIds);
                            foreach (var productSubDepart in productSubDeparts)
                            {

                                var productSubUsers = bll.GetUsersByPId(productSubDepart.DepartId);
                                var isAdd = SyncGroupAndUsers(groups, web, context, productSubUsers, productSubDepart, updateGroups);
                                if (isAdd)
                                {
                                    gIds.Add(productSubDepart.DepartId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            XMLHelper.SetLog(ex.Message, "Sync_ProductSubGroups");
                        }

                        #endregion

                        #region 组禁用
                        try
                        {
                            // 部门被禁用，删除组
                            context.Load(groups);
                            context.ExecuteQuery();
                            for (var m = 0; m < groups.Count; m++)
                            {
                                var group = groups[m];

                                if (group.Title != Constant.AllUserGroup)
                                {
                                    if (group.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                                    {
                                        var gId = group.Description.Replace("Overseas_", "");
                                        if (!gIds.Contains(gId))
                                        {
                                            try
                                            {
                                                deleteGroups.Add(group.Title);
                                            }
                                            catch (Exception ex)
                                            {
                                                XMLHelper.SetLog(ex.Message, "Sync_RemoveGroups:" + group.Title);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            XMLHelper.SetLog(ex.Message, "Sync_DeleteGroups");
                        }

                        #endregion

                    }

                    #region Email
                    try
                    {
                        if (deleteGroups.Count > 0)
                        {
                            CommonHelper.SendDeleteGroupMail(deleteGroups);
                        }
                        if (updateGroups.Count > 0)
                        {
                            CommonHelper.SendUpdateGroupMail(updateGroups);
                        }
                    }
                    catch (Exception ex)
                    {
                        XMLHelper.SetLog(ex.Message, "Sync_SendMail");
                    }

                    #endregion
                }
                else
                {
                    Constant.RunTime = DateTime.Now.AddHours(-1);
                    XMLHelper.SetLog("GroupOperate AllUser is empty.", "No Run Sync");
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "Sync");
            }
            XMLHelper.SetLog("GroupOperate end.", "Sync");
        }

        /// <summary>
        /// 同步组及用户
        /// </summary>
        private bool SyncGroupAndUsers(GroupCollection groups, Web web, ClientContext context, List<GroupUser> allUsers, Depart depart, List<string> updateGroups)
        {
            var groupName = depart.DepartName_En;
            if (string.IsNullOrEmpty(groupName))
            {
                return false;
            }
            try
            {
                // 日志
                XMLHelper.SetLog("User's count : " + allUsers.Count, "Group：" + groupName);

                Group group = null;
                // 判断组是否存在，是否改名
                try
                {
                    context.Load(groups);
                    context.ExecuteQuery();

                    if (groupName == Constant.AllUserGroup)
                    {
                        #region GKS-All
                        try
                        {
                            group = groups.GetByName(groupName);
                        }
                        catch (Exception ex)
                        {
                            XMLHelper.SetLog(ex.Message, "SyncGroupAndUsers_AllGroup：" + groupName);
                        }
                        #endregion
                    }
                    else
                    {
                        #region GKS-Other
                        for (var m = 0; m < groups.Count; m++)
                        {
                            var gp = groups[m];
                            if (gp.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                            {
                                var gId = gp.Description.Replace("Overseas_", "");
                                if (gId == depart.DepartId)
                                {
                                    if (gp.Title != groupName)
                                    {
                                        // 记录邮件 
                                        string updateGroup = gp.Title + "   -->   " + groupName;
                                        updateGroups.Add(updateGroup);

                                        try
                                        {
                                            // 修改名称
                                            gp.Title = groupName;
                                            gp.Update();
                                            context.ExecuteQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            XMLHelper.SetLog(ex.Message, "SyncGroupAndUsers_UpdateGroupName");
                                        }
                                    }
                                    group = gp;
                                    break;
                                }
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    XMLHelper.SetLog(ex.Message, "SyncGroupAndUsers_GetGroup：" + groupName);
                }

                #region  组不存在，创建
                try
                {
                    if (group == null)
                    {
                        GroupCreationInformation gItem = new GroupCreationInformation();
                        gItem.Title = groupName;
                        gItem.Description = "Overseas_" + depart.DepartId;
                        group = groups.Add(gItem);
                        group.Owner = web.CurrentUser;
                        context.Load(group);
                        context.ExecuteQuery();

                    }
                }
                catch (Exception ex)
                {
                    XMLHelper.SetLog(ex.Message, "SyncGroupAndUsers_AddGroup:" + groupName);
                }
                #endregion

                #region 删除用户
                // 组用户
                var gUsers = group.Users;
                context.Load(gUsers);
                context.ExecuteQuery();
                if (gUsers.Count > 0)
                {
                    for (int m = 0; m < gUsers.Count; m++)
                    {
                        User user = gUsers[m];

                        try
                        {
                            //var account = user.LoginName.Replace(Constant.enSure, "");
                            var loginNameList = user.LoginName.Split('\\');
                            var account = loginNameList[loginNameList.Length - 1];

                            var users = (from u in allUsers where u.Account == account select u).ToList();
                            // 如果list不存在，删除组用户
                            if (users == null || users.Count == 0)
                            {
                                gUsers.Remove(user);
                                context.ExecuteQuery();
                                m--;

                                XMLHelper.SetLog("Delete user  " + user.LoginName, "Sync Delete User");
                            }
                        }
                        catch (Exception ex)
                        {
                            XMLHelper.SetLog(ex.Message, "SyncGroupAndUsers_RemoveUser:" + user.LoginName);
                        }
                    }
                }
                #endregion

                #region 添加用户
                foreach (var aUser in allUsers)
                {
                    var loginName = Constant.enSure + aUser.Account;
                    try
                    {
                        User user = null;
                        if (gUsers.Count > 0)
                        {
                            try
                            {
                                // 是否存在
                                user = gUsers.GetByLoginName(loginName);
                                context.Load(user);
                                context.ExecuteQuery();
                            }
                            catch
                            {
                                user = web.EnsureUser(loginName);
                                gUsers.AddUser(user);

                                XMLHelper.SetLog("Add user  " + loginName, "Sync Add User 1");
                            }
                        }
                        else
                        {
                            user = web.EnsureUser(loginName);
                            gUsers.AddUser(user);

                            XMLHelper.SetLog("Add user  " + user.LoginName, "Sync Add User 2");
                        }
                        context.Load(user);
                        context.ExecuteQuery();

                        #region 更新邮箱
                        if (string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(aUser.Email))
                        {
                            user.Email = aUser.Email;
                            user.Update();
                            context.ExecuteQuery();

                            XMLHelper.SetLog("Update user email  " + aUser.Email, "Update user email");
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        XMLHelper.SetLog(ex.Message, "SyncGroupAndUsers_AddUser:" + loginName);
                    }
                }
                context.ExecuteQuery();
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "SyncGroupAndUsers,User's count:" + allUsers.Count);
            }
            return true;
        }

    }
}
