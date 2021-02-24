using System;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;
using DocumentsSP.Helper;
using System.Collections.Generic;

namespace DocumentsSP.Event.WhiteListER
{
    /// <summary>
    /// 列表项事件
    /// </summary>
    public class WhiteListER : SPItemEventReceiver
    {
        /// <summary>
        /// 添加白名单时触发本事件
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            try
            {
                if (properties.ListTitle == CommonHelper.whiteList)
                {
                    var addItem = properties.AfterProperties;
                    var per = addItem["Permissions"];
                    var userNamesStr = addItem["ObjectName"].ToString();
                    if (string.IsNullOrEmpty(userNamesStr))
                    {
                        properties.ErrorMessage = "The Users are not allowed to be empty, please add users to add it !";
                        properties.Status = SPEventReceiverStatus.CancelWithError;
                        properties.Cancel = true;
                    }
                    else
                    {
                        string[] userNames = Regex.Split(userNamesStr, ";", RegexOptions.IgnoreCase);
                        List<string> names = new List<string>();
                        string claim = "i:0#.w|";
                        foreach (var name in userNames)
                        {
                            if (name.Contains(claim))
                            {
                                string uName = name;
                                if (uName.StartsWith("#"))
                                {
                                    uName = uName.Substring(1);
                                }
                                names.Add(uName);
                            }
                        }

                        string aName = "";
                        foreach (var userName in names)
                        {
                            var name = userName;
                            using (SPWeb web = properties.OpenWeb())
                            {
                                SPUser user = web.EnsureUser(name);
                                var list = web.Lists.TryGetList(CommonHelper.whiteList);
                                var query = new SPQuery();
                                query.Query = "<Where><And>" +
                                              "<Eq><FieldRef Name='Permissions' /><Value Type='MultiChoice'>" + per + "</Value></Eq>" +
                                              "<Contains><FieldRef Name='ObjectName' LookupId='True' /><Value Type='UserMulti'>" + user.ID + "</Value></Contains>" +
                                              "</And></Where>";
                                var items = list.GetItems(query);

                                if (items.Count > 0)
                                {
                                    aName += " '" + name + "' ";
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(aName))
                        {
                            properties.ErrorMessage = "The User" + aName + "already have permissions " + per + ", please remove them and add it later !";
                            properties.Status = SPEventReceiverStatus.CancelWithError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WhiteListER__ItemAdding", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }
        }

        /// <summary>
        /// 正在更新项
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            try
            {
                if (properties.ListTitle == CommonHelper.whiteList)
                {
                    var addItem = properties.AfterProperties;
                    var per = addItem["Permissions"];
                    var userNamesStr = addItem["ObjectName"].ToString();
                    if (string.IsNullOrEmpty(userNamesStr))
                    {
                        properties.ErrorMessage = "The Users are not allowed to be empty, please add users to update it !";
                        properties.Status = SPEventReceiverStatus.CancelWithError;
                        properties.Cancel = true;
                    }
                    else
                    {
                        string[] userNames = Regex.Split(userNamesStr, ";", RegexOptions.IgnoreCase);
                        List<string> names = new List<string>();
                        string claim = "i:0#.w|";
                        foreach (var name in userNames)
                        {
                            if (name.Contains(claim))
                            {
                                string uName = name;
                                if (uName.StartsWith("#"))
                                {
                                    uName = uName.Substring(1);
                                }
                                names.Add(uName);
                            }
                        }

                        string aName = "";
                        foreach (var userName in names)
                        {
                            var name = userName;
                            using (SPWeb web = properties.OpenWeb())
                            {
                                SPUser user = web.EnsureUser(name);
                                var list = web.Lists.TryGetList(CommonHelper.whiteList);
                                var query = new SPQuery();
                                query.Query = "<Where><And>" +
                                              "<Eq><FieldRef Name='Permissions' /><Value Type='MultiChoice'>" + per + "</Value></Eq>" +
                                              "<Contains><FieldRef Name='ObjectName' LookupId='True' /><Value Type='UserMulti'>" + user.ID + "</Value></Contains>" +
                                              "</And></Where>";
                                var items = list.GetItems(query);

                                if (items.Count > 0)
                                {
                                    bool isSelf = true;
                                    foreach (SPListItem item in items)
                                    {
                                        if (item.ID != properties.ListItemId)
                                        {
                                            isSelf = false;
                                        }
                                    }
                                    if (!isSelf)
                                    {
                                        aName += " '" + name + "' ";
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(aName))
                        {
                            properties.ErrorMessage = "The User" + aName + "already have permissions " + per + ", please remove them and update it later !";
                            properties.Status = SPEventReceiverStatus.CancelWithError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WhiteListER__ItemUpdating", ex.Message, properties.Site.ID, properties.Web.CurrentUser);
            }
        }
    }
}