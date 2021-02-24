using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SyncGroups
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
            catch (Exception ex)
            {
                CommonHelper.SendErrorGroupMail();
                XMLHelper.SetLog(ex.Message, "BllHelper_GetCIdByPId");
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取海外全员
        /// </summary>
        /// <returns>海外全员</returns>
        public List<GroupUser> GetAllUsers()
        {
            return GetUsersByPId(Constant.rootDepartId);
        }

        /// <summary>
        /// 获取所有子级部门集合
        /// </summary>
        /// <returns></returns>
        public List<Depart> GetAllSubDeparts(string parentDepartId)
        {
            List<Depart> departs = new List<Depart>();
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("SELECT [DEPTID],[DESCR],[DEPT_ENG] FROM [DHDATAMID].[dbo].[T_OPS_DEPT] where [DH_SUPERIOR_DEPT]=@DH_SUPERIOR_DEPT and [STATUS]='A'");

                SqlParameter[] parameters = {
                    new SqlParameter("@DH_SUPERIOR_DEPT", SqlDbType.VarChar,20)
                };
                parameters[0].Value = parentDepartId;

                DataSet ds = DBHelperSql.Query(strSql.ToString(), parameters);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Depart depart = new Depart();
                        var cId = row["DEPTID"];
                        if (cId != null && !string.IsNullOrEmpty(cId.ToString()))
                        {
                            depart.DepartId = cId.ToString();
                        }
                        var cName = row["DESCR"];
                        if (cName != null && !string.IsNullOrEmpty(cName.ToString()))
                        {
                            depart.DepartName = cName.ToString();
                        }
                        var eName = row["DEPT_ENG"];
                        if (eName != null && !string.IsNullOrEmpty(eName.ToString()))
                        {
                            depart.DepartName_En = eName.ToString();
                        }
                        departs.Add(depart);
                    }
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "BllHelper_GetAllSubDeparts");
            }
            return departs;
        }

        /// <summary>
        /// 获取部门集合通过Id集合
        /// </summary>
        /// <returns>部门Id</returns>
        public List<Depart> GetDepartsByIds(string departIds)
        {
            List<Depart> departs = new List<Depart>();
            if (string.IsNullOrEmpty(departIds))
            {
                return departs;
            }
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("SELECT [DEPTID],[DESCR],[DEPT_ENG] FROM [DHDATAMID].[dbo].[T_OPS_DEPT] where [DEPTID] IN (" + departIds + ") and [STATUS]='A'");

                DataSet ds = DBHelperSql.Query(strSql.ToString());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Depart depart = new Depart();
                        var cId = row["DEPTID"];
                        if (cId != null && !string.IsNullOrEmpty(cId.ToString()))
                        {
                            depart.DepartId = cId.ToString();
                        }
                        var cName = row["DESCR"];
                        if (cName != null && !string.IsNullOrEmpty(cName.ToString()))
                        {
                            depart.DepartName = cName.ToString();
                        }
                        var eName = row["DEPT_ENG"];
                        if (eName != null && !string.IsNullOrEmpty(eName.ToString()))
                        {
                            depart.DepartName_En = eName.ToString();
                        }
                        departs.Add(depart);
                    }
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "BllHelper_GetAllSubDeparts");
            }
            return departs;
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
            string cIds = string.Empty;
            try
            {
                cIds = GetCIdByPId(pId);
                if (string.IsNullOrEmpty(cIds))
                {
                    cIds = pId;
                }
            }
            catch
            {
                return gUsers;
            }
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
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "BllHelper_GetUsersByPId");
            }
            return gUsers;
        }

    }
}
