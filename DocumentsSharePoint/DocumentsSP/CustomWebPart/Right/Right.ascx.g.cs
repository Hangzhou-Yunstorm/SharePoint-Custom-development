﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DocumentsSP.CustomWebPart.Right {
    using System.Web.UI.WebControls.Expressions;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Text;
    using System.Web.UI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.SessionState;
    using System.Configuration;
    using Microsoft.SharePoint;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.ComponentModel.DataAnnotations;
    using System.Web.UI.WebControls;
    using System.Web.Security;
    using System;
    using Microsoft.SharePoint.Utilities;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint.WebControls;
    using System.CodeDom.Compiler;
    
    
    public partial class Right {
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebPartCodeGenerator", "14.0.0.0")]
        public static implicit operator global::System.Web.UI.TemplateControl(Right target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private void @__BuildControlTree(global::DocumentsSP.CustomWebPart.Right.Right @__ctrl) {
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n<div style=\"width: 460px; height: 580px;\">\r\n    <div class=\"TopTab\">\r\n       " +
                        " <div class=\"Ttab_bt Ttab-active noBorderRight\">History</div>\r\n        <div id=\"" +
                        "Subscription_Title\" class=\"Ttab_bt noBorderRight\">Subscription</div>\r\n    </div>" +
                        "\r\n    <div class=\"TopContent\">\r\n        <div class=\"Tcon_detail show\">\r\n        " +
                        "    <div id=\"HistoryFileJson\" class=\"detail\"></div>\r\n            <div class=\"mor" +
                        "e\">\r\n                <a href=\"/Lists/Browsing History/AllItems.aspx\" title=\"More" +
                        "\">\r\n                    <img src=\"/_layouts/15/images/more.png\" />\r\n            " +
                        "    </a>\r\n            </div>\r\n        </div>\r\n        <div class=\"Tcon_detail\">\r" +
                        "\n            <div id=\"SubscriptionFileJson\" class=\"detail\"></div>\r\n            <" +
                        "div class=\"more\">\r\n                <a href=\"/Lists/SubscribeList/AllItems.aspx\" " +
                        "title=\"More\">\r\n                    <img src=\"/_layouts/15/images/more.png\" />\r\n " +
                        "               </a>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<sc" +
                        "ript>\r\n    $(document).ready(function () {\r\n        var tabT = $(\".Ttab_bt\");\r\n " +
                        "       for (var i = 0; i < tabT.length; i++) {\r\n            tabT[i].index = i;\r\n" +
                        "            tabT[i].onclick = function () {\r\n                $(this).addClass(\'T" +
                        "tab-active\').siblings().removeClass(\'Ttab-active\');\r\n                $(\".Tcon_de" +
                        "tail:eq(\" + this.index + \")\").addClass(\'show\').siblings().removeClass(\'show\');\r\n" +
                        "            };\r\n        }\r\n\r\n        $.ajax({\r\n            type: \"post\",\r\n      " +
                        "      url: \"/_layouts/15/WebPartContent.aspx/GetHistoryList\",\r\n            data:" +
                        " \"\",\r\n            contentType: \"application/json;charset=utf-8\",\r\n            da" +
                        "taType: \"json\",\r\n            success: function (response, textStatus) {\r\n       " +
                        "         $(\"#HistoryFileJson\").html(response.d);\r\n            },\r\n            er" +
                        "ror: function (XMLHttpRequest, textStatus, errorThrown) {\r\n                conso" +
                        "le.log(textStatus);\r\n            }\r\n        });\r\n\r\n        $.ajax({\r\n           " +
                        " type: \"post\",\r\n            url: \"/_layouts/15/WebPartContent.aspx/GetSubscribeL" +
                        "ist\",\r\n            data: \"\",\r\n            contentType: \"application/json;charset" +
                        "=utf-8\",\r\n            dataType: \"json\",\r\n            success: function (response" +
                        ", textStatus) {\r\n                $(\"#SubscriptionFileJson\").html(response.d);\r\n " +
                        "           },\r\n            error: function (XMLHttpRequest, textStatus, errorThr" +
                        "own) {\r\n                console.log(textStatus);\r\n            }\r\n        });\r\n\r\n" +
                        "        $.ajax({\r\n            type: \"post\",\r\n            url: \"/_layouts/15/WebP" +
                        "artContent.aspx/GetSubscribeCount\",\r\n            data: \"\",\r\n            contentT" +
                        "ype: \"application/json;charset=utf-8\",\r\n            dataType: \"json\",\r\n         " +
                        "   success: function (response, textStatus) {\r\n                if (response.d ==" +
                        " \"0\") {\r\n                    $(\"#Subscription_Title\").html(\"Subscription\");\r\n   " +
                        "             } else {\r\n                    $(\"#Subscription_Title\").html(\"Subscr" +
                        "iption( \" + response.d + \" )\");\r\n                }\r\n            },\r\n            " +
                        "error: function (XMLHttpRequest, textStatus, errorThrown) {\r\n                con" +
                        "sole.log(textStatus);\r\n            }\r\n        });\r\n    });\r\n</script>\r\n"));
        }
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private void InitializeControl() {
            this.@__BuildControlTree(this);
            this.Load += new global::System.EventHandler(this.Page_Load);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        protected virtual object Eval(string expression) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        protected virtual string Eval(string expression, string format) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression, format);
        }
    }
}
