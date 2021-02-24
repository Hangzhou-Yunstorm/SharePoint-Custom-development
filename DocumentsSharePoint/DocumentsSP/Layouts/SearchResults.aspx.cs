using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;

namespace DocumentsSP.Layouts
{
    public partial class SearchResults : LayoutsPageBase
    {
        public string webUrl = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            webUrl = SPContext.Current.Web.Url;
        }
    }
}
