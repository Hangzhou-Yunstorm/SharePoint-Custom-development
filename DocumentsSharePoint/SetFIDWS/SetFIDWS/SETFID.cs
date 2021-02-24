using Microsoft.SharePoint;
using System;

namespace SetFIDWS
{
    public class SETFID
    {
        public void Sync()
        {
            XMLHelper.SetLog("SyncFID Start.", "Sync");
            try
            {
                using (SPSite site = new SPSite(Constant.webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        XMLHelper.SetLog("Sync  WebUrl: " + web.Url, "Sync Start");
                        var documents = web.Lists.TryGetList("Documents");

                        #region 空FID设置
                        var query = new SPQuery();
                        query.ViewAttributes = "Scope=\"RecursiveAll\"";
                        query.Query = "<Where><IsNull><FieldRef Name='FID' /></IsNull></Where>";

                        var items = documents.GetItems(query);

                        XMLHelper.SetLog("Sync  SETFID Count : " + items.Count, "Set Null Start");

                        foreach (SPListItem item in items)
                        {
                            var fid = item["FID"];
                            if (fid == null)
                            {
                                var fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                                item["FID"] = fId;
                                if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                                {
                                    item["ParentFolder"] = GetParentFolder(fId);
                                }
                                item.Update();

                                try
                                {
                                    //审批
                                    item.ModerationInformation.Comment = "Automatic Approval of items";
                                    item.ModerationInformation.Status = SPModerationStatusType.Approved;//自动审批
                                    item.Update();
                                }
                                catch
                                {
                                    XMLHelper.SetLog("Automatic Approval Error" + item.Url, "Automatic Approval Error");
                                }

                                XMLHelper.SetLog("Sync  SET FID, Url : " + item.Url, "Set FID");
                            }
                        }
                        XMLHelper.SetLog("Sync  SETFID Count : " + items.Count, "Set Null End");
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "Sync");
            }
            XMLHelper.SetLog("Sync FID.", "Sync End");
        }

        private static SPFieldUrlValue GetParentFolder(string fId)
        {
            SPFieldUrlValue value = new SPFieldUrlValue();
            value.Description = "Folder";
            value.Url = "/_layouts/15/ReturnFolder.aspx?FID=" + fId;
            return value;
        }

    }
}
