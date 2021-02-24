using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyncCalculatedWS
{
    public class SPDocumentCalculated
    {

        public void Sync()
        {
            XMLHelper.SetLog("SyncCalculatedWS Start.", "Sync");
            try
            {
                using (SPSite site = new SPSite(Constant.webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        XMLHelper.SetLog("SyncCalculatedWS WebUrl: " + web.Url, "Sync");

                        var documentStatisticalDetails = web.Lists.TryGetList(Constant.documentStatisticalDetails);
                        var documentCalculated = web.Lists.TryGetList(Constant.documentCalculated);

                        var items = documentStatisticalDetails.GetItems();

                        if (items != null && items.Count > 0)
                        {
                            List<DocModel> list = new List<DocModel>();
                            // 遍历集合，构造Model
                            foreach (SPListItem item in items)
                            {
                                try
                                {
                                    var fId = item["FID"].ToString();
                                    var mos = list.Where(T => T.FID == fId).ToList();
                                    if (mos != null && mos.Count > 0)
                                    {
                                        var mo = mos.FirstOrDefault();
                                        mo.DownloadCount += Convert.ToInt32(item["DownloadCount"]);
                                        mo.ClickCount += Convert.ToInt32(item["ClickCount"]);
                                    }
                                    else
                                    {
                                        DocModel model = new DocModel();
                                        model.FID = fId;
                                        model.AveScore = GetAveScore(fId, web);
                                        model.DownloadCount = Convert.ToInt32(item["DownloadCount"]);
                                        model.ClickCount = Convert.ToInt32(item["ClickCount"]);
                                        list.Add(model);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    XMLHelper.SetLog(ex.Message, "Sync_documentStatisticalDetails");
                                }
                            }
                            XMLHelper.SetLog("SyncCalculatedWS Total Count ：" + list.Count, "Sync");
                            if (list.Count > 0)
                            {
                                try
                                {
                                    int addCount = 0;
                                    int updateCount = 0;
                                    int sameCount = 0;
                                    foreach (var dc in list)
                                    {
                                        SPQuery fquery = new SPQuery();
                                        fquery.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + dc.FID + "</Value></Eq></Where>";
                                        var citems = documentCalculated.GetItems(fquery);

                                        if (citems != null && citems.Count > 0)
                                        {
                                            var citem = citems[0];
                                            if (IsUpdate(dc, citem))
                                            {
                                                citem["AveScore"] = Convert.ToDouble(dc.AveScore);
                                                citem["DownloadCount"] = dc.DownloadCount;
                                                citem["ClickCount"] = dc.ClickCount;
                                                citem.Update();

                                                updateCount++;
                                            }
                                            else
                                            {
                                                sameCount++;
                                            }
                                        }
                                        else
                                        {
                                            var addItem = documentCalculated.AddItem();
                                            addItem["FID"] = dc.FID;
                                            addItem["AveScore"] = Convert.ToDouble(dc.AveScore);
                                            addItem["DownloadCount"] = dc.DownloadCount;
                                            addItem["ClickCount"] = dc.ClickCount;
                                            addItem.Update();

                                            addCount++;
                                        }
                                    }

                                    XMLHelper.SetLog("Add count :" + addCount, "Sync_documentCalculated");
                                    XMLHelper.SetLog("Update count :" + updateCount, "Sync_documentCalculated");
                                    XMLHelper.SetLog("Same count :" + sameCount, "Sync_documentCalculated");
                                }
                                catch (Exception ex)
                                {
                                    XMLHelper.SetLog(ex.Message, "Sync_documentCalculated");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "Sync");
            }
            XMLHelper.SetLog("SyncCalculatedWS End.", "Sync");
        }

        // 是否更新
        public bool IsUpdate(DocModel dc, SPListItem citem)
        {
            bool isUpdate = false;
            try
            {
                if (Convert.ToDouble(citem["AveScore"]) != Convert.ToDouble(dc.AveScore)
                    || Convert.ToInt32(citem["DownloadCount"]) != dc.DownloadCount
                    || Convert.ToInt32(citem["ClickCount"]) != dc.ClickCount)
                {
                    isUpdate = true;
                }
            }
            catch (Exception ex)
            {
                isUpdate = true;
                XMLHelper.SetLog("SyncCalculatedWS IsUpdate Exception :" + ex.Message, "Sync IsUpdate");
            }
            return isUpdate;
        }

        private string GetAveScore(string fId, SPWeb web)
        {
            string aveScore = "0";
            try
            {
                var scoreList = web.Lists.TryGetList(Constant.scoreList);

                SPQuery fquery = new SPQuery();
                fquery.Query = "<Where><Eq><FieldRef Name='FileID' /><Value Type='Text'>" + fId + "</Value></Eq></Where>";
                var items = scoreList.GetItems(fquery);

                if (items != null && items.Count > 0)
                {
                    double totalScore = 0;
                    foreach (SPListItem item in items)
                    {
                        var scoreD = Convert.ToDouble(item["Score"]);
                        totalScore += scoreD;
                    }
                    aveScore = (totalScore / items.Count).ToString("f1");
                }

            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "GetAveScore");
            }
            return aveScore;
        }

    }
}
