using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;

namespace DocumentsSP.Layouts
{
    public partial class DocumentsScoreReport : LayoutsPageBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取历史评分前20的文件（下载>100）
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetDatas()
        {
            HReportModel model = new HReportModel();

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            try
            {
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            GetDocumentCalculatedTimer timer = new GetDocumentCalculatedTimer();
                            var dcList = timer.GetDocumentCalculatedList(false);
                            dcList = dcList.OrderByDescending(T => T.AveScoreD).ToList();

                            if (dcList.Count > 0)
                            {
                                List<string> names = new List<string>();
                                List<double> values = new List<double>();
                                int n = 1;
                                var fileList = web.Lists.TryGetList(CommonHelper.docListName);

                                foreach (var mo in dcList)
                                {
                                    if (n > 20)
                                    {
                                        break;
                                    }

                                    var fquery = new SPQuery();
                                    fquery.ViewAttributes = "Scope=\"Recursive\"";
                                    fquery.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + mo.FID + "</Value></Eq></Where>";
                                    var items = fileList.GetItems(fquery);
                                    if (items != null && items.Count > 0)
                                    {
                                        var name = items[0].Name;
                                        names.Add(name);
                                        values.Add(mo.AveScoreD);
                                        n++;
                                    }
                                }
                                model.Names = names;
                                model.Values = values;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("DocumentsScoreReport.aspx__GetDatas", ex.Message);
            }
            return jsonSerializer.Serialize(model);
        }
        
    }
}
