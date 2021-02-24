using DocumentsSP.Helper;
using Microsoft.SharePoint;
using System;

namespace DocumentsSP.Event.CarouselER
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class CarouselER : SPItemEventReceiver
    {
        /// <summary>
        /// 添加轮播图时触发本事件
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            try
            {
                if (properties.ListTitle == CommonHelper.carouselListName)
                {
                    string pName = properties.AfterUrl;
                    if (!CommonHelper.IsPicture(pName))
                    {
                        properties.ErrorMessage = "Please select an image upload !";
                        properties.Status = SPEventReceiverStatus.CancelWithError;
                        properties.Cancel = true;
                    }
                    else
                    {
                        using (SPWeb web = properties.OpenWeb())
                        {
                            var list = web.Lists.TryGetList(CommonHelper.carouselListName);
                            var items = list.GetItems();
                            if (items.Count >= 10)
                            {
                                properties.ErrorMessage = "You can only have 10 of them of most, Please delte others before adding it !";
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                                properties.Cancel = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("CarouselER__ItemAdding", ex.Message, properties.SiteId, properties.Web.CurrentUser);
            }

        }
    }
}