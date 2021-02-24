using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 日志Model
    /// </summary>
    public class ErrorMessageModel
    {
        /// <summary>
        /// 错误标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 错误时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 错误详情
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 错误文件完整URL路径
        /// </summary>
        public string FilePath { get; set; }
    }
}
