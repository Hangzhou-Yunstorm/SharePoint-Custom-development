using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DahuaWeb
{
    public class FileJsonModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 下载链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string FID { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }
}