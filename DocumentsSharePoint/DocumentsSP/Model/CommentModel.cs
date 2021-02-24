namespace DocumentsSP.Model
{
    /// <summary>
    /// 评论Model
    /// </summary>
    public class CommentModel
    {
        /// <summary>
        /// 评论人
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string CommentText { get; set; }
        /// <summary>
        /// 评论时间
        /// </summary>
        public string Time { get; set; }
    }
}
