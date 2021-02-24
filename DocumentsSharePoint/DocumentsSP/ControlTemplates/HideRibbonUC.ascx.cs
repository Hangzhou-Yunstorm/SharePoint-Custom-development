using Microsoft.SharePoint.WebControls;
using System;
using System.Web.UI;

namespace DocumentsSP.ControlTemplates
{
    /// <summary>
    /// Suggestion.New
    /// </summary>
    public partial class HideRibbonUC : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SPRibbon ribbon = SPRibbon.GetCurrent(this.Page);
                if (ribbon != null)
                {
                    ribbon.TrimById("Ribbon.ListForm.Edit.Clipboard");
                    ribbon.TrimById("Ribbon.ListForm.Edit.Actions");
                    ribbon.TrimById("Ribbon.ListForm.Edit.SpellCheck");
                    ribbon.TrimById("Ribbon.EditingTools.CPEditTab");
                    ribbon.TrimById("Ribbon.EditingTools.CPInsert.Tables");
                    ribbon.TrimById("Ribbon.EditingTools.CPInsert.Media.Media");
                    ribbon.TrimById("Ribbon.EditingTools.CPInsert.Links");
                    ribbon.TrimById("Ribbon.EditingTools.CPInsert.Embed");
                    ribbon.TrimById("Ribbon.Image.Image.Properties");
                    ribbon.TrimById("Ribbon.Image.Image.Styles");
                    ribbon.TrimById("Ribbon.Image.Image.Arrange");
                    ribbon.TrimById("Ribbon.EditingTools.CPInsert.Media.Image.Menu.Image.FromSharePoint");
                }
            }
            catch
            { }
        }
    }
}
