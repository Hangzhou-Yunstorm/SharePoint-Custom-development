namespace DocumentsSP.Model
{
    /// <summary>
    /// 部门model
    /// </summary>
    public class DepartModel
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartName { get; set; }
        /// <summary>
        /// 部门路径
        /// </summary>
        public string DepartPath { get; set; }
        /// <summary>
        /// 上级部门名称
        /// </summary>
        public string DepartParentName { get; set; }
        /// <summary>
        /// 上级部门id
        /// </summary>
        public int DepartParentId { get; set; }
    }
}
