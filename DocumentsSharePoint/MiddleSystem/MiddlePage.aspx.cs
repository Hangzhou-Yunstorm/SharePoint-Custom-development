using System;

namespace MiddleSystem
{
    public partial class MiddlePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string hostName = Request.QueryString["HostName"];
            string localHostName = CommonHelper.GetCountryName();

            string returnUrl = string.Empty;

            if (hostName.ToLower() == localHostName.ToLower())
            {
                returnUrl = CommonHelper.webUrl + "/Documents/Forms/Pending%20With%20Me.aspx";
            }
            else
            {
                string domain = CommonHelper.GetDomainName(hostName);
                returnUrl = domain + "/Documents/Forms/FileApprove.aspx?HostName=" + hostName;
            }
            Response.Redirect(returnUrl);
        }
    }
}