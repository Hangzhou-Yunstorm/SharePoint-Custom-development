using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace DocumentsSP.Layouts
{
    public partial class MoFileView : LayoutsPageBase
    {
        public string ViewStr = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string fileUrl = Request.QueryString["Url"];
            var extension = fileUrl.Substring(fileUrl.LastIndexOf('.')).ToLower();
            if (extension == ".png" || extension == ".jpg" || extension == ".gif" || extension == ".bmp" || extension == ".jpeg" || extension == ".tiff" || extension == ".img")
            {
                ViewStr = "<img src = \"" + fileUrl + "\" />";
            }
            //else if (extension == ".mp4" || extension == ".ogg" || extension == ".mov" || extension == ".webm")
            //{
            //    ViewStr = "<video autoplay=\"\" controls=\"\"><source src = \"" + fileUrl + "\"></source></video>";
            //}
            else
            {
                ViewStr = "The file format is not supported.";
            }
        }
    }
}
