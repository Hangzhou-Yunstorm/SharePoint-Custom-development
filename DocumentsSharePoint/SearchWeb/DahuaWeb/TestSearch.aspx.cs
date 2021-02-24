using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DahuaWeb
{
    public partial class TestSearch : System.Web.UI.Page
    {
        public string token = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            SymmCrypt symmCrypt = new SymmCrypt();
            token = symmCrypt.DESEnCode(Constant.key + CommonHelper.GetTimeSpanByDate(), Constant.key, Constant.key);
        }
    }
}