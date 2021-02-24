using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExternalSystem
{
    public class TreeFolder
    {
        /// <summary>
        /// 树Id
        /// </summary>
        public string fId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 树返回Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 父树Id
        /// </summary>
        public string pId { get; set; }
        /// <summary>
        /// 分享人
        /// </summary>
        public string sharer { get; set; }
        /// <summary>
        /// 分享时间
        /// </summary>
        public string shareTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public string expiration { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public bool canWrite { get; set; }
        /// <summary>
        /// 默认类型（0：分享文件夹；1：默认文件夹）
        /// </summary>
        public int defaultType { get; set; }
        /// <summary>
        /// 是否父节点
        /// </summary>
        public bool isParent { get; set; }
    }
}