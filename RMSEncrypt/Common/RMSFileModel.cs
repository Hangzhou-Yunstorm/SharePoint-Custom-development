using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RMSFileModel
    {
        public string Author { get; set; }
        /// <summary>
        /// 文件ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 文件ID
        /// </summary>
        public string FID { get; set; }
        /// <summary>
        /// 完全控制
        /// </summary>
        public bool FIsFullControl { get; set; }
        /// <summary>
        /// 只读
        /// </summary>
        public bool FIsRead { get; set; }
        /// <summary>
        /// 打印
        /// </summary>
        public bool FIsPrint { get; set; }
        /// <summary>
        /// 另存
        /// </summary>
        public bool FIsSave { get; set; }
        /// <summary>
        /// 编辑
        /// </summary>
        public bool FIsEdit { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FTitle { get; set; }
        /// <summary>
        /// 在线URL路径（源文件路径）
        /// </summary>
        public string FUrlSourcePath { get; set; }
        /// <summary>
        /// 在线URL路径（加密后的文件路径）
        /// </summary>
        public string FUrlRMSPath { get; set; }
        /// <summary>
        /// 应用到用户
        /// </summary>
        public FieldUserValue[] Users { get; set; }
        /// <summary>
        /// 加密状态
        /// </summary>
        public string FState { get; set; }
        /// <summary>
        /// 加密错误分类
        /// </summary>
        public string FErrorMessage { get; set; }
        /// <summary>
        /// 加密详情
        /// </summary>
        public string FContent { get; set; }
        /// <summary>
        /// 本地物理路径（源文件）
        /// </summary>
        public string FSourcePath { get; set; }
        /// <summary>
        /// 本地物理路径（加密后的文件）
        /// </summary>
        public string FRMSPath { get; set; }
        /// <summary>
        /// 是否下次不再加密
        /// </summary>
        public bool IsFalse { get; set; }
        /// <summary>
        /// 作者Email，用于失败通知
        /// </summary>
        public string AuthorEmail { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public string UploadTime { get; set; }
    }
}
