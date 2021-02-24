using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncCalculatedWS
{
    public class DocumentCalculated
    {

        public void Sync()
        {
            XMLHelper.SetLog("SyncCalculatedWS Start.", "Sync");
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.

                    var documentStatisticalDetails = web.Lists.GetByTitle(Constant.documentStatisticalDetails);
                    var documentCalculated = web.Lists.GetByTitle(Constant.documentCalculated);

                    CamlQuery query = CamlQuery.CreateAllItemsQuery();
                    var items = documentStatisticalDetails.GetItems(query);
                    context.Load(items);
                    context.ExecuteQuery();

                    if (items != null && items.Count > 0)
                    {
                        List<DocModel> list = new List<DocModel>();
                        // 遍历集合，构造Model
                        foreach (ListItem item in items)
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
                                    model.AveScore = GetAveScore(context, fId, web);
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

                        if (list.Count > 0)
                        {
                            try
                            {
                                foreach (var dc in list)
                                {
                                    CamlQuery fquery = new CamlQuery();
                                    fquery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + dc.FID + "</Value></Eq></Where></Query></View>";
                                    var citems = documentCalculated.GetItems(fquery);
                                    context.Load(citems);
                                    context.ExecuteQuery();

                                    if (citems != null && citems.Count > 0)
                                    {
                                        var citem = citems[0];
                                        citem["AveScore"] = Convert.ToDouble(dc.AveScore);
                                        citem["DownloadCount"] = dc.DownloadCount;
                                        citem["ClickCount"] = dc.ClickCount;
                                        citem.Update();
                                    }
                                    else
                                    {
                                        ListItemCreationInformation lcItem = new ListItemCreationInformation();
                                        var addItem = documentCalculated.AddItem(lcItem);
                                        addItem["FID"] = dc.FID;
                                        addItem["AveScore"] = Convert.ToDouble(dc.AveScore);
                                        addItem["DownloadCount"] = dc.DownloadCount;
                                        addItem["ClickCount"] = dc.ClickCount;
                                        addItem.Update();
                                    }
                                }
                                context.ExecuteQuery();
                            }
                            catch (Exception ex)
                            {
                                XMLHelper.SetLog(ex.Message, "Sync_documentCalculated");
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

        private string GetAveScore(ClientContext context, string fId, Web web)
        {
            string aveScore = "0";
            try
            {
                var scoreList = web.Lists.GetByTitle(Constant.scoreList);

                CamlQuery fquery = new CamlQuery();
                fquery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='FileID' /><Value Type='Text'>" + fId + "</Value></Eq></Where></Query></View>";
                var items = scoreList.GetItems(fquery);
                context.Load(items);
                context.ExecuteQuery();

                if (items != null && items.Count > 0)
                {
                    double totalScore = 0;
                    foreach (ListItem item in items)
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
