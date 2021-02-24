using DocumentsSP.Helper;
using DocumentsSP.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentsSP.Event.EventReceiverRMSHandle
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class EventReceiverRMSHandle : SPItemEventReceiver
    {
        /// <summary>
        /// 已添加项目时触发本事件
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                if (properties.ListTitle == CommonHelper.docListName)
                {
                    SPListItem item = properties.ListItem;
                    if (item != null)
                    {
                        var fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                        item["FID"] = fId;
                        if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                        {
                            item["ParentFolder"] = CommonHelper.GetParentFolder(fId);
                        }
                        item["IsFullControl"] = false;
                        item["IsRead"] = false;
                        item["IsPrint"] = false;
                        item["IsSave"] = false;
                        item["IsEdit"] = false;
                        item["Users"] = null;
                        item.SystemUpdate(false);

                        if (item.FileSystemObjectType == SPFileSystemObjectType.File)//文件
                        {
                            int currentState = int.Parse(item["_ModerationStatus"].ToString());//0已批准；1未通过；2待定；3草稿
                            if (currentState == 2)
                            {
                                if ("SHAREPOINT\\system".Equals(item.File.Author.LoginName))
                                {
                                    item.File.Approve("Automatic approval by system");
                                }
                                else
                                {
                                    SendMailApprove(properties);
                                }
                            }

                            #region 上传日志
                            var siteId = properties.Site.ID;
                            var cUser = properties.Web.CurrentUser;
                            LogModel aModel = new LogModel();
                            aModel.Title = "Upload file : " + item.Name;
                            aModel.Operater = cUser;
                            aModel.Operator = cUser.Name;
                            aModel.OperatorId = cUser.LoginName;
                            aModel.ServerIP = CommonHelper.GetServerHostName();
                            aModel.Department = CommonHelper.GetSubGroupName(cUser);
                            aModel.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                            aModel.Operate = "Upload";
                            aModel.ObjectName = item.Url;
                            aModel.ObjectType = "File";
                            CommonHelper.SetLog(aModel, siteId);
                            #endregion
                        }
                        else if (item.FileSystemObjectType == SPFileSystemObjectType.Folder)//文件夹
                        {
                            NewFolder(properties);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__ItemAdded", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }
        }

        /// <summary>
        /// 已更新项目时触发本事件
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            try
            {
                if (properties.ListTitle == CommonHelper.docListName)
                {
                    SPListItem item = properties.ListItem;
                    if (item != null)
                    {
                        string aFID = properties.AfterProperties["FID"] == null ? "" : properties.AfterProperties["FID"].ToString();
                        string bFID = properties.BeforeProperties["FID"] == null ? "" : properties.BeforeProperties["FID"].ToString();
                        if (aFID == bFID || aFID == "")
                        {
                            //if (item["FID"] == null && bFID == "")
                            //{
                            //    var fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                            //    item["FID"] = fId;
                            //    if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                            //    {
                            //        item["ParentFolder"] = CommonHelper.GetParentFolder(fId);
                            //    }
                            //    item.SystemUpdate(false);
                            //}

                            if (item.FileSystemObjectType == SPFileSystemObjectType.File)//文件
                            {
                                var siteId = properties.Site.ID;
                                var cUser = properties.Web.CurrentUser;

                                bool isAdd = properties.AfterUrl == properties.BeforeUrl;
                                bool isRms = NewRMSTask(properties, isAdd, false);

                                if (isAdd)
                                {
                                    int currentState = int.Parse(item["_ModerationStatus"].ToString());//0已批准；1未通过；2待定；3草稿
                                    if (currentState == 1)
                                    {
                                        SendMailRejected(properties);//审批不通过，邮件通知作者
                                    }
                                    else
                                    {
                                        #region 新版本
                                        try
                                        {
                                            SPListItemVersion curVison = null;
                                            foreach (SPListItemVersion v in item.Versions)
                                            {
                                                if (v.IsCurrentVersion)
                                                {
                                                    curVison = v;
                                                    break;
                                                }
                                            }
                                            string itemVison = "V" + curVison.VersionLabel;
                                            string beforeVision = properties.BeforeProperties["vti_sourcecontrolversion"].ToString();
                                            if (currentState == 2 && itemVison != beforeVision)
                                            {
                                                if ("SHAREPOINT\\system".Equals(item.File.ModifiedBy.LoginName))
                                                {
                                                    item.File.Approve("Automatic approval by system");
                                                }
                                                else
                                                {
                                                    SendMailApprove(properties);//新版本，邮件通知作者
                                                }
                                                if (!isRms)
                                                {
                                                    NewRMSTask(properties, isAdd, true);
                                                }

                                                #region 新版本日志
                                                LogModel aModel = new LogModel();
                                                aModel.Title = "Upload new version file : " + item.Name;
                                                aModel.Operater = cUser;
                                                aModel.Operator = cUser.Name;
                                                aModel.OperatorId = cUser.LoginName;
                                                aModel.ServerIP = CommonHelper.GetServerHostName();
                                                aModel.Department = CommonHelper.GetSubGroupName(cUser);
                                                aModel.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                                                aModel.Operate = "Upload";
                                                aModel.ObjectName = item.Url;
                                                aModel.ObjectType = "File";
                                                CommonHelper.SetLog(aModel, siteId);
                                                #endregion

                                            }
                                        }
                                        catch { }
                                        #endregion
                                    }
                                    #region 审批
                                    var beforeState = properties.BeforeProperties["vti_doclibmodstat"];
                                    if (beforeState != null && !string.IsNullOrEmpty(beforeState.ToString()))
                                    {
                                        var bState = Convert.ToInt32(beforeState);
                                        if (bState == 2 && (currentState == 1 || currentState == 0))
                                        {
                                            // 审批通过
                                            if (currentState == 0)
                                            {
                                                // 订阅文件
                                                AddSubscribe(properties);
                                            }
                                            var stateStr = currentState == 0 ? "approved" : "rejected";
                                            #region 审批日志
                                            LogModel model = new LogModel();
                                            model.Operate = "Approval";
                                            model.Operater = cUser;
                                            model.Operator = cUser.Name;
                                            model.OperatorId = cUser.LoginName;
                                            model.ServerIP = CommonHelper.GetServerHostName();
                                            model.Department = CommonHelper.GetSubGroupName(cUser);
                                            model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                                            model.ObjectName = item.Url;
                                            model.ObjectType = "File";
                                            model.Title = "The file was " + stateStr + " , File name is '" + item.Name + " '.";
                                            CommonHelper.SetLog(model, siteId);
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                                else //重命名文件
                                {
                                    #region 重命名日志
                                    LogModel model = new LogModel();
                                    var bname = properties.BeforeUrl.Substring(properties.BeforeUrl.LastIndexOf("/") + 1);
                                    var aname = properties.AfterUrl.Substring(properties.AfterUrl.LastIndexOf("/") + 1);
                                    model.Title = "Update file：'" + bname + "' to '" + aname + "'";
                                    model.Operate = "Update";
                                    model.Operater = cUser;
                                    model.Operator = cUser.Name;
                                    model.OperatorId = cUser.LoginName;
                                    model.ServerIP = CommonHelper.GetServerHostName();
                                    model.Department = CommonHelper.GetSubGroupName(cUser);
                                    model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                                    model.ObjectName = properties.AfterUrl;
                                    model.ObjectType = item.FileSystemObjectType.ToString();
                                    CommonHelper.SetLog(model, siteId);
                                    #endregion
                                }
                            }
                            else if (item.FileSystemObjectType == SPFileSystemObjectType.Folder)//文件夹
                            {
                                ReNameFolder(properties);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(aFID) && !string.IsNullOrEmpty(bFID))
                            {
                                // 大文件修改FID，使用最新的FID
                                string newFID = Convert.ToDecimal(aFID) > Convert.ToDecimal(bFID) ? aFID : bFID;

                                item["FID"] = newFID;
                                if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                                {
                                    item["ParentFolder"] = CommonHelper.GetParentFolder(newFID);
                                }
                                item.SystemUpdate(false);

                                NewRMSTask(properties, true, false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__ItemUpdated", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }
        }

        /// <summary>
        /// 已删除项目时触发本事件
        /// </summary>
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            try
            {
                if (properties.ListTitle == CommonHelper.docListName)
                {
                    var siteId = properties.Site.ID;
                    var cUser = properties.Web.CurrentUser;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = properties.Web)
                            {
                                web.AllowUnsafeUpdates = true;

                                string FileSystemObjectType = "";
                                if (properties.BeforeUrl.Contains("."))
                                {
                                    FileSystemObjectType = "File";
                                }
                                else
                                {
                                    FileSystemObjectType = "Folder";
                                }

                                #region 日志
                                LogModel model = new LogModel();
                                model.Operate = "Delete";
                                model.Operater = cUser;
                                model.Operator = cUser.Name;
                                model.OperatorId = cUser.LoginName;
                                model.ServerIP = CommonHelper.GetServerHostName();
                                model.Department = CommonHelper.GetSubGroupName(cUser);
                                model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                                model.ObjectName = properties.BeforeUrl;
                                model.ObjectType = FileSystemObjectType;
                                var fname = properties.BeforeUrl.Substring(properties.BeforeUrl.LastIndexOf("/") + 1);
                                model.Title = "Delete " + FileSystemObjectType + "：" + fname;
                                CommonHelper.SetLog(model, siteId);
                                #endregion

                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__ItemDeleted", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }
        }

        /// <summary>
        /// 添加项目时触发本事件
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            try
            {
                if (properties.ListTitle == CommonHelper.docListName)
                {
                    string url = properties.AfterUrl;
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (url.Contains("."))
                        {
                            int dIndex = url.IndexOf(CommonHelper.docListName);
                            string tmp = url.Substring(dIndex + CommonHelper.docListName.Length + 1);
                            if (tmp.IndexOf('/') == -1 || tmp == properties.ListTitle)
                            {
                                properties.ErrorMessage = "Upload in the root directory is not allowed";
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                                properties.Cancel = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__ItemAdding", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                properties.ErrorMessage = ex.ToString();
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.Cancel = true;
            }
        }

        /// <summary>
        /// 更新项目时触发本事件
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            if (properties.ListTitle == CommonHelper.docListName)
            {
                try
                {
                    string url = properties.BeforeUrl;
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (!url.Contains("."))
                        {
                            if (url == CommonHelper.CommonPath && properties.AfterUrl != properties.BeforeUrl)
                            {
                                properties.ErrorMessage = "Common folder do not support the change !";
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                                properties.Cancel = true;
                            }

                            if (url == CommonHelper.RegionPath && properties.AfterUrl != properties.BeforeUrl)
                            {
                                properties.ErrorMessage = "Region folder do not support the change !";
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                                properties.Cancel = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("EventReceiverRMSHandle__ItemUpdating", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                }
            }
        }

        /// <summary>
        /// 添加订阅消息
        /// </summary>
        private void AddSubscribe(SPItemEventProperties properties)
        {
            var siteId = properties.Site.ID;
            var cUser = properties.Web.CurrentUser;
            try
            {
                SPListItem item = properties.ListItem;
                SPFolder pFolder = item.File.ParentFolder;

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            try
                            {
                                SetSubscribe(item, pFolder, web);
                            }
                            catch (Exception ex)
                            {
                                CommonHelper.SetErrorLog("EventReceiverRMSHandle__SetSubscribe", ex.Message, siteId, cUser);
                            }
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__AddSubscribe", ex.Message, siteId, cUser);
            }
        }

        /// <summary>
        /// 遍历添加订阅消息
        /// </summary>
        /// <param name="item">文件Item</param>
        /// <param name="folder">文件夹</param>
        /// <param name="web">Web</param>
        private void SetSubscribe(SPListItem item, SPFolder folder, SPWeb web)
        {
            SPList spList = web.Lists.TryGetList(CommonHelper.sDirectoryList);
            var query = new SPQuery();
            query.Query = "<Where><Eq><FieldRef Name='FolderId' /><Value Type='Text'>" + folder.Item["FID"].ToString() + "</Value></Eq></Where>";
            var spItems = spList.GetItems(query);

            if (spItems != null && spItems.Count > 0)
            {
                var subscribeList = web.Lists.TryGetList(CommonHelper.subscribeList);
                var mailUserList = web.Lists.TryGetList(CommonHelper.userEmaiPushList);

                foreach (SPListItem spItem in spItems)
                {
                    string userName = spItem["Subscriber"].ToString();
                    SPFieldUser _user = (SPFieldUser)spItem.Fields["Subscriber"];
                    SPFieldUserValue userValue = (SPFieldUserValue)_user.GetFieldValue(userName);
                    var user = userValue.User;

                    var userquery = new SPQuery();
                    userquery.Query = "<Where><Eq><FieldRef Name='EmailUser' LookupId='True' /><Value Type='User'>" + user.ID + "</Value></Eq></Where>";
                    var userItems = mailUserList.GetItems(userquery);
                    if (userItems == null || userItems.Count == 0)
                    {
                        var listItem = mailUserList.AddItem();
                        listItem["EmailUser"] = user;
                        listItem["IsPush"] = true;
                        listItem.Update();
                    }

                    var uquery = new SPQuery();
                    uquery.Query = "<Where><And>" +
                                   "<Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"].ToString() + "</Value></Eq>" +
                                   "<Eq><FieldRef Name='UName' LookupId='True' /><Value Type='User'>" + user.ID + "</Value></Eq>" +
                                   "</And></Where>";
                    var uItems = subscribeList.GetItems(uquery);

                    // 预防订阅目录含有包含关系
                    if (uItems == null || uItems.Count == 0)
                    {
                        var listItem = subscribeList.AddItem();
                        listItem["FileName"] = CommonHelper.GetFileUrl(item.Name, item["FID"].ToString());
                        listItem["Time"] = DateTime.Now.ToLocalTime();
                        listItem["UName"] = user;
                        listItem["Creator"] = item.File.Author;
                        listItem["CreateTime"] = item["Created"];
                        listItem["Folder"] = CommonHelper.GetParentFolder(item["FID"].ToString());
                        listItem["FolderPath"] = item.File.ParentFolder.ServerRelativeUrl;
                        listItem["FID"] = item["FID"].ToString();
                        listItem["IconUrl"] = CommonHelper.GetImage(item.File.IconUrl);
                        listItem.Update();
                    }
                }
            }
            if (folder.ParentFolder.ServerRelativeUrl != "/" + CommonHelper.docListName)
            {
                SetSubscribe(item, folder.ParentFolder, web);
            }
        }

        /// <summary>
        /// 新增加密任务
        /// </summary>
        /// <param name="properties">SPItemEventProperties</param>
        private bool NewRMSTask(SPItemEventProperties properties, bool isAdd, bool isNewVersion)
        {
            bool isRms = false;
            try
            {
                SPListItem item = properties.ListItem;
                string fileName = item.Name;

                var siteId = properties.Site.ID;
                var cUser = properties.Web.CurrentUser;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;//允许修改

                            if (CommonHelper.IsRMSFile(fileName))//判断格式
                            {
                                SPItemEventDataCollection sedc = properties.AfterProperties;
                                SPItemEventDataCollection sedcBefore = properties.BeforeProperties;
                                if (IsRMSHandle(properties, isNewVersion))
                                {
                                    SPList list = web.Lists.TryGetList(CommonHelper.rmsTask);
                                    if (list != null)
                                    {
                                        try
                                        {
                                            SPListItem itemNew = list.AddItem();//新增一条加密任务
                                            itemNew["Title"] = fileName;
                                            itemNew["SourcePath"] = web.Url + "/" + item.Url;
                                            itemNew["State"] = 0;
                                            itemNew["Content"] = "";

                                            if (isNewVersion)
                                            {
                                                itemNew["IsFullControl"] = sedcBefore["IsFullControl"];
                                                itemNew["IsRead"] = sedcBefore["IsRead"];
                                                itemNew["IsPrint"] = sedcBefore["IsPrint"];
                                                itemNew["IsSave"] = sedcBefore["IsSave"];
                                                itemNew["IsEdit"] = sedcBefore["IsEdit"];
                                                itemNew["Users"] = sedcBefore["Users"];
                                                itemNew["FID"] = item["FID"];
                                            }
                                            else
                                            {
                                                itemNew["IsFullControl"] = sedc["IsFullControl"];
                                                itemNew["IsRead"] = sedc["IsRead"];
                                                itemNew["IsPrint"] = sedc["IsPrint"];
                                                itemNew["IsSave"] = sedc["IsSave"];
                                                itemNew["IsEdit"] = sedc["IsEdit"];
                                                itemNew["Users"] = sedc["Users"];
                                                itemNew["FID"] = item["FID"];
                                            }
                                            itemNew.SystemUpdate(false);//加密库里不记录版本

                                            isRms = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            CommonHelper.SetErrorLog("EventReceiverRMSHandle__NewRMSTask__AddRMSItem", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                                        }

                                        #region RMS日志
                                        LogModel model = new LogModel();
                                        model.Title = "Insert RMS task: " + fileName;
                                        model.Operate = "Insert RMS task";
                                        model.Operater = cUser;
                                        model.Operator = cUser.Name;
                                        model.OperatorId = cUser.LoginName;
                                        model.ServerIP = CommonHelper.GetServerHostName();
                                        model.Department = CommonHelper.GetSubGroupName(cUser);
                                        model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                                        model.ObjectName = item.Url;
                                        model.ObjectType = item.FileSystemObjectType.ToString();

                                        CommonHelper.SetLog(model, siteId);
                                        #endregion
                                    }
                                }
                                else if (!isAdd)
                                {
                                    ReNameRmsFile(web, properties.AfterUrl, properties.BeforeUrl, fileName, item["FID"].ToString());
                                }
                            }
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__NewRMSTask", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }
            return isRms;
        }

        /// <summary>
        /// 重命名加密文件
        /// </summary>
        /// <param name="web">网站web</param>
        /// <param name="afterUrl">修改后的url</param>
        /// <param name="beforeUrl">修改前的url</param>
        public static void ReNameRmsFile(SPWeb web, string afterUrl, string beforeUrl, string afterName, string fId)
        {
            try
            {
                if (!string.IsNullOrEmpty(afterUrl) && !string.IsNullOrEmpty(beforeUrl) && afterUrl != beforeUrl)//重命名文件
                {
                    var rmsfileList = web.Lists.TryGetList(CommonHelper.docRMSListName);

                    var fquery = new SPQuery();
                    fquery.ViewAttributes = "Scope=\"Recursive\"";
                    fquery.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where>";

                    var fitems = rmsfileList.GetItems(fquery);
                    if (fitems != null && fitems.Count > 0)
                    {
                        var rmsItem = fitems[0];
                        rmsItem["FileLeafRef"] = afterName;
                        rmsItem.SystemUpdate(false);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__ReNameRmsFile", ex.Message, web.Site.ID, web.CurrentUser);
            }
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="properties">SPItemEventProperties</param>
        private void NewFolder(SPItemEventProperties properties)
        {
            string aUrl = properties.AfterUrl;
            if (!string.IsNullOrEmpty(aUrl))//新建文件夹
            {
                try
                {
                    var siteId = properties.Site.ID;
                    var cUser = properties.Web.CurrentUser;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;//允许修改

                                SPListItem item = web.Lists.TryGetList(CommonHelper.docListName).GetItemById(properties.ListItemId);

                                try
                                {
                                    //审批掉原文件夹
                                    item.ModerationInformation.Comment = "Automatic Approval of items";
                                    item.ModerationInformation.Status = SPModerationStatusType.Approved;//自动审批文件夹
                                    item.Update();
                                }
                                catch (Exception ex)
                                {
                                    CommonHelper.SetErrorLog("EventReceiverRMSHandle__Approval", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                                }

                                #region 日志
                                LogModel model = new LogModel();
                                model.Title = "Add folder：" + aUrl.Substring(aUrl.LastIndexOf("/") + 1);
                                model.Operate = "Add";
                                model.Operater = cUser;
                                model.Operator = cUser.Name;
                                model.OperatorId = cUser.LoginName;
                                model.ServerIP = CommonHelper.GetServerHostName();
                                model.Department = CommonHelper.GetSubGroupName(cUser);
                                model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                                model.ObjectName = aUrl;
                                model.ObjectType = item.FileSystemObjectType.ToString();
                                CommonHelper.SetLog(model, siteId);
                                #endregion

                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("EventReceiverRMSHandle__NewFolder", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                }
            }
        }

        /// <summary>
        /// 重命名文件夹
        /// </summary>
        /// <param name="properties">SPItemEventProperties</param>
        private void ReNameFolder(SPItemEventProperties properties)
        {
            string bUrl = properties.BeforeUrl;
            string aUrl = properties.AfterUrl;

            var siteId = properties.Site.ID;
            var cUser = properties.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;//允许修改
                        try
                        {
                            SPListItem item = web.Lists.TryGetList(CommonHelper.docListName).GetItemById(properties.ListItemId);
                            if (item == null)
                            {
                                item = properties.List.GetItemById(properties.ListItemId);
                            }
                            int currentState = int.Parse(item["_ModerationStatus"].ToString());//0已批准；1未通过；2待定；3草稿
                            if (currentState != 0)
                            {
                                item.ModerationInformation.Comment = "Automatic Approval of items";
                                item.ModerationInformation.Status = SPModerationStatusType.Approved;//自动审批文件夹
                                item.Update();
                            }

                            if (!string.IsNullOrEmpty(bUrl) && !string.IsNullOrEmpty(aUrl) && bUrl != aUrl)//重命名文件夹
                            {
                                #region 日志
                                LogModel model = new LogModel();
                                model.Title = "Update folder：'" + bUrl.Substring(bUrl.LastIndexOf("/") + 1) + "' to '" + aUrl.Substring(aUrl.LastIndexOf("/") + 1) + "'";
                                model.Operate = "Update";
                                model.Operater = cUser;
                                model.Operator = cUser.Name;
                                model.OperatorId = cUser.LoginName;
                                model.ServerIP = CommonHelper.GetServerHostName();
                                model.Department = CommonHelper.GetSubGroupName(cUser);
                                model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                                model.ObjectName = aUrl;
                                model.ObjectType = item.FileSystemObjectType.ToString();
                                CommonHelper.SetLog(model, siteId);
                                #endregion
                            }
                        }

                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("EventReceiverRMSHandle__ReNameFolder", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        /// <summary>
        /// 根据选择的策略判断是否需要进行加密
        /// </summary>
        /// <param name="sedc">SPItemEventDataCollection</param>
        /// <returns>是否需要进行加密</returns>
        private bool IsRMSHandle(SPItemEventProperties properties, bool isNewVersion)
        {
            SPItemEventDataCollection sedc = properties.AfterProperties;
            SPItemEventDataCollection sedcBefore = properties.BeforeProperties;
            try
            {
                // 修改后
                bool FIsFullControl = ObjToBool(sedc["IsFullControl"]);
                bool FIsRead = ObjToBool(sedc["IsRead"]);
                bool FIsPrint = ObjToBool(sedc["IsPrint"]);
                bool FIsSave = ObjToBool(sedc["IsSave"]);
                bool FIsEdit = ObjToBool(sedc["IsEdit"]);

                // 修改前
                bool bIsFullControl = ObjToBool(sedcBefore["IsFullControl"]);
                bool bIsRead = ObjToBool(sedcBefore["IsRead"]);
                bool bIsPrint = ObjToBool(sedcBefore["IsPrint"]);
                bool bIsSave = ObjToBool(sedcBefore["IsSave"]);
                bool bIsEdit = ObjToBool(sedcBefore["IsEdit"]);

                // 修改加密属性
                bool isEditPor = (FIsFullControl || FIsRead || FIsPrint || FIsSave || FIsEdit) &&
                   (FIsFullControl != bIsFullControl || FIsRead != bIsRead || FIsPrint != bIsPrint || FIsSave != bIsSave || FIsEdit != bIsEdit);

                string afUsers = sedc["Users"] == null ? "" : sedc["Users"].ToString();
                string beUsers = sedcBefore["Users"] == null ? "" : sedcBefore["Users"].ToString();
                // 修改加密人员
                bool isEditUser = (FIsFullControl || FIsRead || FIsPrint || FIsSave || FIsEdit) && afUsers != beUsers;
                // 新版本
                bool isNewVersionRms = (isNewVersion && (bIsFullControl || bIsRead || bIsPrint || bIsSave || bIsEdit));

                //如果选择任意策略，并且之前没有选择则加密，否则不加密
                if (isEditPor || isNewVersionRms || isEditUser)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__IsRMSHandle", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                return false;
            }
        }

        /// <summary>
        /// Object转Bool
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>Bool</returns>
        private static bool ObjToBool(object o)
        {
            if (o == null)
            {
                return false;
            }
            else
            {
                if (o.ToString().ToLower() == "false")
                {
                    return false;
                }
                else if (o.ToString().ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        #region 邮件相关
        /// <summary>
        /// 邮件接收人
        /// </summary>
        static List<PriorityUser> list = new List<PriorityUser>();

        /// <summary>
        /// 发送拒绝邮件
        /// </summary>
        /// <param name="properties">SPItemEventProperties</param>
        private void SendMailRejected(SPItemEventProperties properties)
        {
            try
            {
                List<string> tmpList = new List<string>();
                SPListItem item = properties.ListItem;
                SPUser author = item.File.Author;
                string tmpEmail = author.Email;
                //tmpEmail = "zhang.chuang@yunstorm.com"; // TODO
                if (!string.IsNullOrEmpty(tmpEmail) && tmpEmail.IndexOf("@") > -1)
                {
                    tmpList.Add(tmpEmail);
                }
                SPUser approver = properties.Web.CurrentUser;

                if (approver != null && approver.Email != tmpEmail)//当前用户（审批人）不为空，并且文件上传者不是自己
                {
                    string url = GetFolderUrl(properties.AfterUrl);
                    string docTitle = item.Name;
                    string approverName = approver.Name;
                    string docTime = item["Modified"].ToString();
                    string docPath = properties.WebUrl + "/" + url;
                    string docDescription = "";
                    if (item["_ModerationComments"] != null)
                    {
                        docDescription = item["_ModerationComments"].ToString();
                    }
                    string docUrl = properties.WebUrl + "/Documents/Forms/Rejected%20Request.aspx";

                    EmailHelper email = new EmailHelper(CommonHelper.mailUser, CommonHelper.mailPwd, CommonHelper.mailSmtp);
                    email.mailSubject = "Reject request";
                    string mailContent = EmailHelper.GeneratorRejectMailContent(docTitle, approverName, docTime, docPath, docDescription, docUrl);
                    email.mailBody = EmailHelper.GeneratorMailHeader("Reject request") + mailContent + EmailHelper.GeneratorMailFooter("Reject request");
                    email.isbodyHtml = true;    //是否是HTML
                    email.mailToArray = tmpList;
                    email.Send(properties.Site.ID, approver);

                    #region 记录日志
                    LogModel model = new LogModel();
                    model.Title = "Send rejected mail to :" + author.Email;
                    model.Operate = "Mail";
                    model.Operater = approver;
                    model.Operator = approver.Name;
                    model.OperatorId = approver.LoginName;
                    model.ServerIP = CommonHelper.GetServerHostName();
                    model.Department = CommonHelper.GetSubGroupName(approver);
                    model.DepartmentId = CommonHelper.GetSubGroupId(approver);
                    model.ObjectName = item.Url;
                    model.ObjectType = "File";
                    CommonHelper.SetLog(model, properties.SiteId);
                    #endregion

                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__SendMailPass", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }

        }

        /// <summary>
        /// 发送审批邮件
        /// </summary>
        /// <param name="properties">SPItemEventProperties</param>
        private void SendMailApprove(SPItemEventProperties properties)
        {
            try
            {
                // 文件夹路径
                string url = GetFolderUrl(properties.AfterUrl);

                list = GetUserByFolder(properties.SiteId, "ApproveItems", url, 0, "ManagePermissions", properties.Web.CurrentUser);
                //list.Add(new PriorityUser() { level = 1, user = "zhang.chuang@yunstorm.com" }); // TODO

                if (list.Count == 0)//没有审批人，将逐个往上遍历
                {
                    GetListByParentFolder(ref list, properties.SiteId, "ApproveItems", url, 0, "ManagePermissions", properties.Web.CurrentUser);
                }
                #region 用户人数大于0
                if (list.Count > 0)
                {
                    List<string> tmpList = new List<string>();
                    GetUserByLevel(list, tmpList, 0, properties.SiteId, properties.Web.CurrentUser);// 获取权限级别高的用户

                    SPListItem item = properties.ListItem;
                    string docTitle = item.Name;
                    string docAuthor = properties.UserDisplayName;
                    string docTime = item["Modified"].ToString();
                    //string docPath = properties.WebUrl + "/" + url;
                    string docDescription = "";
                    if (item["_CheckinComment"] != null)
                    {
                        docDescription = item["_CheckinComment"].ToString();
                    }
                    //string docUrl = properties.WebUrl + "/Documents/Forms/Pending%20With%20Me.aspx";
                    string hostName = CommonHelper.GetCountryName();
                    string docUrl = CommonHelper.MiddleDomain + "/MiddlePage.aspx?HostName=" + hostName;

                    EmailHelper email = new EmailHelper(CommonHelper.mailUser, CommonHelper.mailPwd, CommonHelper.mailSmtp);
                    email.mailSubject = "Pending with me";
                    string mailContent = EmailHelper.GeneratorApproveMailContent(docTitle, docAuthor, docTime, url, docDescription, docUrl, hostName);
                    email.mailBody = EmailHelper.GeneratorMailHeader("Pending with me") + mailContent + EmailHelper.GeneratorMailFooter("Pending with me ( " + hostName + " )");
                    email.isbodyHtml = true;    //是否是HTML
                    email.mailToArray = tmpList;
                    email.Send(properties.Site.ID, properties.Web.CurrentUser);

                    #region 记录日志
                    //日志
                    var cUser = properties.Web.CurrentUser;
                    LogModel model = new LogModel();
                    model.Title = "Send approve mail: " + item.Name;
                    model.Operate = "Mail";
                    model.Operater = cUser;
                    model.Operator = cUser.Name;
                    model.OperatorId = cUser.LoginName;
                    model.ServerIP = CommonHelper.GetServerHostName();
                    model.Department = CommonHelper.GetSubGroupName(cUser);
                    model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                    model.ObjectName = string.Join(",", tmpList.ToArray());
                    model.ObjectType = "File";
                    CommonHelper.SetLog(model, properties.SiteId);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__SendMailApprove", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }
        }

        public bool isMailToCount2Big(int count, SPUser cUser, Guid siteId, string folderUrl)
        {
            try
            {
                if (count > CommonHelper.mailToMaxCount)
                {
                    LogModel model = new LogModel();
                    model.Title = "There is too much approval for the directory in which the file is located !";
                    model.Operate = "Mail";
                    model.Operater = cUser;
                    model.Operator = cUser.Name;
                    model.OperatorId = cUser.LoginName;
                    model.ServerIP = CommonHelper.GetServerHostName();
                    model.Department = CommonHelper.GetSubGroupName(cUser);
                    model.DepartmentId = CommonHelper.GetSubGroupId(cUser);
                    model.ObjectName = folderUrl;
                    model.ObjectType = "Folder";
                    CommonHelper.SetLog(model, siteId);
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 逐级往上遍历文件夹并获取该文件夹的审批人
        /// </summary>
        /// <param name="list">PriorityUsers</param>
        /// <param name="siteId">siteId</param>
        /// <param name="permissions">权限</param>
        /// <param name="url">当前文件夹的路径</param>
        /// <param name="level">遍历次数（级别）</param>
        /// <param name="lowPermissions">降低级别的权限</param>
        private void GetListByParentFolder(ref List<PriorityUser> list, Guid siteId, string permissions, string url, int level, string lowPermissions, SPUser cUser)
        {
            try
            {
                int index = url.LastIndexOf('/');
                if (index > 0)
                {
                    url = url.Substring(0, index);
                    if (url != CommonHelper.docListName)
                    {
                        list = GetUserByFolder(siteId, permissions, url, level, lowPermissions, cUser);
                    }
                }
                if ((list == null || list.Count == 0) && level <= 10)
                {
                    level++;
                    GetListByParentFolder(ref list, siteId, permissions, url, level, lowPermissions, cUser);
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__GetListByParentFolder", ex.Message, siteId, cUser);
            }
        }

        /// <summary>
        /// 获取权限级别高的用户
        /// </summary>
        /// <param name="pus">所有用户</param>
        /// <param name="users">要返回的用户</param>
        /// <param name="level">权限级别</param>
        private void GetUserByLevel(List<PriorityUser> pus, List<string> users, int level, Guid siteId, SPUser cUser)
        {
            try
            {
                foreach (var pu in pus)
                {
                    if (pu.level == level)
                    {
                        if (!users.Contains(pu.user))
                        {
                            users.Add(pu.user);
                        }
                    }
                }
                if (users == null || users.Count == 0)
                {
                    level++;
                    GetUserByLevel(pus, users, level, siteId, cUser);
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__GetUserByLevel", ex.Message, siteId, cUser);
                throw;
            }
        }

        /// <summary>
        /// 根据文件夹路径获取该文件夹拥有指定权限的用户
        /// </summary>
        /// <param name="siteId">siteId</param>
        /// <param name="permissions">权限</param>
        /// <param name="url">文件夹路径</param>
        /// <param name="level">优先级</param>
        /// <param name="lowPermissions">降低优先级的权限（如果有permissions权限的用户同时具有lowPermissions权限，那么优先级将降低一层）</param>
        /// <returns>拥有指定权限的用户</returns>
        private List<PriorityUser> GetUserByFolder(Guid siteId, string permissions, string url, int level, string lowPermissions, SPUser cUser)
        {
            List<PriorityUser> list = new List<PriorityUser>();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPFolder folder = web.GetFolder(url);
                            if (folder != null && folder.Exists && folder.Item != null)
                            {
                                SPRoleAssignmentCollection roles = folder.Item.RoleAssignments;//获取当前文件夹的所有角色分配
                                if (roles != null)
                                {
                                    foreach (SPRoleAssignment role in roles)//遍历当前文件夹的所有角色分配
                                    {
                                        SPPrincipal member = role.Member;//角色
                                        SPRoleDefinitionBindingCollection per = role.RoleDefinitionBindings;//角色权限
                                        foreach (SPRoleDefinition sprd in per)//遍历权限
                                        {
                                            string spr = sprd.BasePermissions.ToString();
                                            if (spr.IndexOf(permissions) > -1 || "FullMask".Equals(spr))//拥有指定权限（审批：ApproveItems）
                                            {
                                                if (member.GetType() == typeof(SPUser))//用户
                                                {
                                                    int tmpLevel = level;
                                                    if (spr.IndexOf(lowPermissions) > -1)//如果同时具有该权限，那么优先级降低一个级别
                                                    {
                                                        tmpLevel = level + 1;
                                                    }
                                                    SPUser user = web.AllUsers.GetByID(member.ID);
                                                    PriorityUser pu = new PriorityUser() { user = user.Email, level = tmpLevel };
                                                    if (!string.IsNullOrEmpty(user.Email) && user.Email.IndexOf('@') > 0 && !list.Contains(pu))
                                                    {
                                                        list.Add(pu);
                                                    }
                                                }
                                                else if (member.GetType() == typeof(SPGroup))//用户组
                                                {
                                                    SPGroup group = web.Groups.GetByID(member.ID);
                                                    SPUserCollection users = group.Users;

                                                    foreach (SPUser user in users)//遍历用户组里的用户
                                                    {
                                                        int tmpLevel = level;
                                                        if (spr.IndexOf(lowPermissions) > -1)//如果同时具有该权限，那么优先级降低一个级别
                                                        {
                                                            tmpLevel = level + 1;
                                                        }
                                                        PriorityUser pu = new PriorityUser() { user = user.Email, level = tmpLevel };
                                                        if (!string.IsNullOrEmpty(user.Email) && user.Email.IndexOf('@') > 0 && !list.Contains(pu))
                                                        {
                                                            list.Add(pu);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
                return list;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("EventReceiverRMSHandle__GetUserByFolder", ex.Message, siteId, cUser);
                throw;
            }
        }

        /// <summary>
        /// 根据文件路径获取文件夹路径
        /// </summary>
        /// <param name="url">文件路径</param>
        /// <returns>文件夹路径</returns>
        private static string GetFolderUrl(string url)
        {
            int index = url.LastIndexOf('/');
            string nUrl = url.Substring(0, index);
            return nUrl;
        }

        #endregion

        /// <summary>
        /// 用户优先级属性
        /// </summary>
        public class PriorityUser
        {
            /// <summary>
            /// 用户
            /// </summary>
            public string user { get; set; }
            //优先级
            public int level { get; set; }
        }

        /// <summary>
        /// 正在删除项
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            if (properties.ListTitle == CommonHelper.docListName)
            {
                try
                {
                    string url = properties.BeforeUrl;
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (!url.Contains("."))
                        {
                            if (url == CommonHelper.CommonPath)
                            {
                                properties.ErrorMessage = "Common folder do not support to delete !";
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                                properties.Cancel = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("EventReceiverRMSHandle__ItemUpdating", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
                }
            }
        }
    }
}