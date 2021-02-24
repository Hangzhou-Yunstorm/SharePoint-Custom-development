
namespace SyncExternalUserWS
{
    public class UserModel
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        public string LOGONID { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PASSWORD { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string NAME { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string REGION { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string COUNTRY { get; set; }
    }
}
