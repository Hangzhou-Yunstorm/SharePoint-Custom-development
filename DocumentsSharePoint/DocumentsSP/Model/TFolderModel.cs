namespace DocumentsSP.Model
{
    /// <summary>
    /// 文件夹Model
    /// </summary>
    public class TFolderModel
    {
        /// <summary>
        /// 文件夹Id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 文件夹名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 父文件夹ID
        /// </summary>
        public int pId { get; set; }
        /// <summary>
        /// 文件夹链接
        /// </summary>
        public string furl { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool open { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 是否是父节点
        /// </summary>
        public bool isParent { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int order { get; set; }
    }
}
