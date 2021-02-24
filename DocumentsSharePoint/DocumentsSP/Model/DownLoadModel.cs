namespace DocumentsSP.Model
{
    /// <summary>
    /// 下载返回模型类
    /// </summary>
    public class DownLoadModel
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 下载链接
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 下载名称
        /// </summary>
        public string name { get; set; }
    }
}
