using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using DocumentsSP.Helper;
using DocumentsSP.Model;
using System.Collections.Generic;

namespace DocumentsSP.Layouts
{
    public partial class SelectExternalUser : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取外部人员列表
        /// </summary>
        /// <returns>外部人员列表</returns>
        [WebMethod]
        public static string AddShareCatalog(string expiration, string userAccounts, string ids, string readOrWrite)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(ids) || string.IsNullOrEmpty(userAccounts))
            {
                return msg;
            }
            bool isWrite = false;
            if (readOrWrite == "1")
            {
                isWrite = true;
            }
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
                            string eTime;
                            if (string.IsNullOrEmpty(expiration))
                            {
                                eTime = null;
                            }
                            else
                            {
                                eTime = expiration;
                            }
                            string[] idArr = ids.Split(',');
                            string[] userAccountArr = userAccounts.Split(',');

                            var docList = web.Lists.TryGetList(CommonHelper.docListName);
                            var shareCatalogList = web.Lists.TryGetList(CommonHelper.shareCatalogList);

                            for (int m = 0; m < idArr.Length; m++)
                            {
                                var docItem = docList.GetItemById(Convert.ToInt32(idArr[m]));
                                if (docItem.FileSystemObjectType == SPFileSystemObjectType.Folder)
                                {
                                    if (docItem["FID"] == null)
                                    {
                                        var fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                                        docItem["FID"] = fId;
                                        //docItem.SystemUpdate(false);
                                        docItem.Update();

                                        docItem.ModerationInformation.Comment = "Automatic Approval of items";
                                        docItem.ModerationInformation.Status = SPModerationStatusType.Approved;//自动审批文件夹
                                        docItem.Update();
                                    }
                                    var folder = docItem.Folder;
                                    for (int n = 0; n < userAccountArr.Length; n++)
                                    {
                                        var userAccount = userAccountArr[n];

                                        var query = new SPQuery();
                                        query.Query = "<Where><And>" +
                                                      "<Eq><FieldRef Name='FolderId' /><Value Type='Text'>" + docItem["FID"].ToString() + "</Value></Eq>" +
                                                      "<Eq><FieldRef Name='UserAccount' /><Value Type='Text'>" + userAccount + "</Value></Eq>" +
                                                      "</And></Where>";
                                        var sList = shareCatalogList.GetItems(query);
                                        if (sList != null && sList.Count > 0)
                                        {
                                            var sItem = sList[0];
                                            sItem["Expiration"] = eTime;
                                            sItem["Sharer"] = cUser;
                                            sItem["CanWrite"] = isWrite;
                                            sItem["Time"] = DateTime.Now.ToLocalTime();
                                            sItem["FolderPath"] = folder.ServerRelativeUrl;
                                            sItem.SystemUpdate(false);
                                        }
                                        else
                                        {
                                            var aItem = shareCatalogList.AddItem();
                                            aItem["UserAccount"] = userAccount;
                                            aItem["Folder"] = CommonHelper.GetFolderUrl(docItem["FID"].ToString());
                                            aItem["FolderId"] = docItem["FID"].ToString();
                                            aItem["FolderPath"] = folder.ServerRelativeUrl;
                                            aItem["Expiration"] = eTime;
                                            aItem["Sharer"] = cUser;
                                            aItem["CanWrite"] = isWrite;
                                            aItem["Time"] = DateTime.Now.ToLocalTime();
                                            aItem.Update();
                                        }
                                    }
                                }
                                else
                                {
                                    if (docItem["FID"] == null)
                                    {
                                        var fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                                        docItem["FID"] = fId;
                                        docItem["ParentFolder"] = CommonHelper.GetParentFolder(fId);
                                        docItem.SystemUpdate(false);
                                    }
                                    var file = docItem.File;
                                    for (int n = 0; n < userAccountArr.Length; n++)
                                    {
                                        var userAccount = userAccountArr[n];

                                        var query = new SPQuery();
                                        query.Query = "<Where><And>" +
                                                      "<Eq><FieldRef Name='FolderId' /><Value Type='Text'>" + file.UniqueId + "</Value></Eq>" +
                                                      "<Eq><FieldRef Name='UserAccount' /><Value Type='Text'>" + userAccount + "</Value></Eq>" +
                                                      "</And></Where>";
                                        var sList = shareCatalogList.GetItems(query);
                                        if (sList != null && sList.Count > 0)
                                        {
                                            var sItem = sList[0];
                                            sItem["Expiration"] = eTime;
                                            sItem["Sharer"] = cUser;
                                            sItem["CanWrite"] = isWrite;
                                            sItem["FolderPath"] = file.ServerRelativeUrl;
                                            sItem["Time"] = DateTime.Now.ToLocalTime();
                                            sItem.SystemUpdate(false);
                                        }
                                        else
                                        {
                                            var aItem = shareCatalogList.AddItem();
                                            aItem["UserAccount"] = userAccount;
                                            aItem["Folder"] = CommonHelper.GetFileUrl("File", docItem["FID"].ToString());
                                            aItem["FolderId"] = docItem["FID"].ToString();
                                            aItem["FolderPath"] = file.ServerRelativeUrl;
                                            aItem["Expiration"] = eTime;
                                            aItem["Sharer"] = cUser;
                                            aItem["CanWrite"] = isWrite;
                                            aItem["Time"] = DateTime.Now.ToLocalTime();
                                            aItem.Update();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            CommonHelper.SetErrorLog("SelectExternalUser.aspx__SetShareCatalog", e.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
            return msg;
        }

        /// <summary>
        /// 获取外部人员列表
        /// </summary>
        /// <returns>外部人员列表</returns>
        [WebMethod]
        public static string GetEmployee(string region)
        {
            region = Microsoft.JScript.GlobalObject.unescape(region);
            string externalUsers = "[]";
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
                            try
                            {
                                var externalUserList = web.Lists.TryGetList(CommonHelper.externalUserList);
                                SPQuery query = new SPQuery();
                                query.Query = "<Where><Eq><FieldRef Name='Region' /><Value Type='Text'>" + region + "</Value></Eq></Where>";

                                // 获取区域用户
                                var items = externalUserList.GetItems(query);
                                if (items != null && items.Count > 0)
                                {

                                    List<ExternalUserModel> users = new List<ExternalUserModel>();
                                    foreach (SPItem item in items)
                                    {
                                        var name = item["ObjectName"];
                                        var account = item["Account"];
                                        if (name != null && account != null)
                                        {
                                            ExternalUserModel user = new ExternalUserModel() { Name = name.ToString(), Account = account.ToString() };
                                            users.Add(user);
                                        }
                                    }

                                    //序列化
                                    DataContractJsonSerializer json = new DataContractJsonSerializer(users.GetType());
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        json.WriteObject(stream, users);
                                        externalUsers = Encoding.UTF8.GetString(stream.ToArray());
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                CommonHelper.SetErrorLog("SelectExternalUser.aspx__GetExternalUserList", e.Message);
                            }
                            web.AllowUnsafeUpdates = false;

                        }
                    }
                });
                return externalUsers;
            }

            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("SelectExternalUser.aspx__GetEmployee", ex.Message);
                return externalUsers;
            }
        }

        /// <summary>
        /// 根据输入框获取外部人员列表
        /// </summary>
        /// <param name="search">输入字符串</param>
        /// <returns>外部人员列表</returns>
        [WebMethod]
        public static string GetEmployeeByInput(string search, string region)
        {
            string searchUser = "[]";
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
                            try
                            {
                                var externalUserList = web.Lists.TryGetList(CommonHelper.externalUserList);
                                var query = new SPQuery();
                                query.Query = "<Where><And>" +
                                              "<Contains><FieldRef Name='ObjectName' /><Value Type='Text'>" + search + "</Value></Contains>" +
                                              "<Eq><FieldRef Name='Region' /><Value Type='Text'>" + region + "</Value></Eq>" +
                                              "</And></Where>";

                                var items = externalUserList.GetItems(query);

                                if (items != null && items.Count > 0)
                                {

                                    List<ExternalUserModel> users = new List<ExternalUserModel>();
                                    foreach (SPItem item in items)
                                    {
                                        var name = item["ObjectName"];
                                        var account = item["Account"];
                                        if (name != null && account != null)
                                        {
                                            ExternalUserModel user = new ExternalUserModel() { Name = name.ToString(), Account = account.ToString() };
                                            users.Add(user);
                                        }
                                    }

                                    //序列化
                                    DataContractJsonSerializer json = new DataContractJsonSerializer(users.GetType());
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        json.WriteObject(stream, users);
                                        searchUser = Encoding.UTF8.GetString(stream.ToArray());
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                CommonHelper.SetErrorLog("SelectExternalUser.aspx__GetExternalUserList", e.Message);
                            }
                            web.AllowUnsafeUpdates = false;

                        }
                    }
                });
                return searchUser;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("SelectExternalUser.aspx__GetEmployeeByInput", ex.Message);
                return searchUser;
            }
        }
    }
}
