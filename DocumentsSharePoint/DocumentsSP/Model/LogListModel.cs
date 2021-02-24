namespace DocumentsSP.Model
{
    /// <summary>
    /// 日志Model
    /// </summary>
    public class LogListModel
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
        /// 时间
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; set; }
    }
}
