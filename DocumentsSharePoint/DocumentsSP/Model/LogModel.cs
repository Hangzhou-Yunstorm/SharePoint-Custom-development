using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace DocumentsSP.Model
{
    /// <summary>
    /// 日志Model
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Operate { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public SPUser Operater { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 操作对象名称
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// 操作对象类型
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 操作者账户
        /// </summary>
        public string OperatorId { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; set; }
    }
}
