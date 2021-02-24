
using Microsoft.SharePoint;
using System.Collections.Generic;

namespace MobikeSPO
{
    public static class CommonHelper
    {
        /// <summary>
        /// Object转Bool
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>Bool</returns>
        public static bool ObjToBool(object o)
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

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="PId">父Id</param>
        /// <param name="listName">表名</param>
        /// <returns>添加ID</returns>
        public static int AddComment(string text, int pId, string listName, int pageId)
        {
            int addId = 0;
            using (SPSite site = new SPSite(CustomData.SPUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    //获取List
                    SPList list = web.Lists.TryGetList(listName);
                    SPListItem addItem = list.AddItem();
                    addItem["CommentText"] = text;
                    addItem["Title"] = "Text";
                    addItem["PId"] = pId;
                    addItem["PageId"] = pageId;
                    addItem.Update();
                    web.AllowUnsafeUpdates = false;
                    addId = addItem.ID;
                }
            }
            return addId;
        }

        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="pId">父Id</param>
        /// <param name="listName">表名</param>
        /// <param name="pageId">页面Id</param>
        /// <returns></returns>
        public static List<CommentModel> GetCommentsByPId(int pId, string listName, int pageId)
        {
            List<CommentModel> mos = new List<CommentModel>();
            using (SPSite site = new SPSite(CustomData.SPUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //获取List
                    SPList list = web.Lists.TryGetList(listName);

                    var query = new SPQuery();
                    query.Query = "<Where><And>" +
                                          "<Eq><FieldRef Name=\"PId\"/><Value Type='Number'>" + pId + "</Value></Eq>" +
                                          "<Eq><FieldRef Name=\"PageId\"/><Value Type='Number'>" + pageId + "</Value></Eq></And></Where>" +
                                          "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>";

                    var items = list.GetItems(query);
                    foreach (SPListItem item in items)
                    {
                        CommentModel mo = new CommentModel();
                        mo.ID = item.ID;
                        mo.PageId = pageId;
                        mo.PId = pId;
                        mo.CommentText = item["CommentText"].ToString();
                        mo.Name = item[SPBuiltInFieldId.Editor].ToString().Split('#')[1];
                        mo.Date = item[SPBuiltInFieldId.Modified].ToString();
                        mos.Add(mo);
                    }
                }
            }
            return mos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listName">表名</param>
        /// <param name="pageId">页面Id</param>
        /// <returns></returns>
        public static int MaxContent(string listName, int pageId)
        {
            int items = 0;
            using (SPSite site = new SPSite(CustomData.SPUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //获取List
                    SPList list = web.Lists.TryGetList(listName);

                    var query = new SPQuery();
                    query.Query = "<Where>" +
                                          "<Eq><FieldRef Name=\"PageId\"/><Value Type='Number'>" + pageId + "</Value></Eq></Where>";
                    if (list.GetItems(query)!=null)
                    {
                        items = list.GetItems(query).Count;
                    }
                }
            }
            return items;
        }


    }
}
