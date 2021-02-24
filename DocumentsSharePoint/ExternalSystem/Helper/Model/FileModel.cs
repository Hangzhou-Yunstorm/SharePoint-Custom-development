namespace ExternalSystem
{
    public class FileModel
    {
        /// <summary>
        /// 文件Id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 文件Id
        /// </summary>
        public string PId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 隐藏文件名
        /// </summary>
        public string HideName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 分享人
        /// </summary>
        public string Sharer { get; set; }
        /// <summary>
        /// 分享时间
        /// </summary>
        public string ShareTime { get; set; }
        /// <summary>
        /// 类型（0：文件；1：文件夹）
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 默认类型（0：分享文件夹；1：默认文件夹）
        /// </summary>
        public int DefaultType { get; set; }
        /// <summary>
        /// 文件类型图标
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public string Expiration { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public bool CanWrite { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string Size { get; set; }
    }
}