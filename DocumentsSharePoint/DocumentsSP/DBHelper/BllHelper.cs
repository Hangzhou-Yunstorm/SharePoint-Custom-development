using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DocumentsSP
{
    public class BllHelper
    {
        /// <summary>
        /// 根据部门Id获取所有子部门Id
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        private string GetCIdByPId(string pId)
        {
            string result = string.Empty;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("with TAB as(");
                strSql.Append(" select DEPTID from T_OPS_DEPT where DEPTID = @DEPTID union all");
                strSql.Append(" select b.DEPTID  from TAB a,T_OPS_DEPT b where b.DH_SUPERIOR_DEPT=a.DEPTID and [STATUS]='A'");
                strSql.Append(" )select * from TAB");

                SqlParameter[] parameters = {
                    new SqlParameter("@DEPTID", SqlDbType.VarChar,20)
                };
                parameters[0].Value = pId;

                DataSet ds = DBHelperSql.Query(strSql.ToString(), parameters);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    List<string> cIds = new List<string>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var cId = row["DEPTID"];
                        if (cId != null && !string.IsNullOrEmpty(cId.ToString()))
                        {
                            cIds.Add(cId.ToString());
                        }
                    }
                    if (cIds.Count > 0)
                    {
                        result = string.Join(",", cIds.ToArray());
                    }
                }
            }
            catch
            { }
            return result;
        }

        /// <summary>
        /// 获取海外全员
        /// </summary>
        /// <returns>海外全员</returns>
        public List<GroupUser> GetAllUsers()
        {
            return GetUsersByPId(PubConstant.ParentDepartId);
        }

        /// <summary>
        /// 根据部门ID获取所有用户（包括所有子部门）
        /// </summary>
        /// <param name="pId">部门Id</param>
        /// <returns>用户列表</returns>
        public List<GroupUser> GetUsersByPId(string pId)
        {
            List<GroupUser> gUsers = new List<GroupUser>();
            if (string.IsNullOrEmpty(pId))
            {
                return gUsers;
            }
            string cIds = pId;
            try
            {
                cIds = GetCIdByPId(pId);
            }
            catch
            { }
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("SELECT [EMPLID] ,[NAME] ,[EMAIL_ADDR] FROM [T_OPS_EMPL] where [DEPTID] IN (" + cIds + ") and [EMPL_RCD]=0 and [HR_STATUS]='A'");
                DataSet ds = DBHelperSql.Query(strSql.ToString());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["EMPLID"] != null && row["EMPLID"].ToString() != "")
                        {
                            GroupUser user = new GroupUser();

                            user.Account = row["EMPLID"].ToString();
                            if (row["NAME"] != null && row["NAME"].ToString() != "")
                            {
                                user.Name = row["NAME"].ToString();
                            }
                            if (row["EMAIL_ADDR"] != null && row["EMAIL_ADDR"].ToString() != "")
                            {
                                user.Email = row["EMAIL_ADDR"].ToString();
                            }
                            gUsers.Add(user);
                        }
                    }
                }
            }
            catch
            {
            }
            return gUsers;
        }

    }
}
