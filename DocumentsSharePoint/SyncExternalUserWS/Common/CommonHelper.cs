using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SyncExternalUserWS
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取ClientContext
        /// </summary>
        /// <returns>ClientContext</returns>
        public static ClientContext GetClientContext()
        {
            ClientContext context = new ClientContext(Constant.webUrl);
            try
            {
                context.Credentials = new NetworkCredential(Constant.loginName, Constant.psw, Constant.domain);
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "GetClientContext");
            }
            return context;
        }

        /// <summary>
        /// 获取更新数据
        /// </summary>
        /// <returns>更新人员</returns>
        public static List<UserModel> GetUsers()
        {
            XMLHelper.SetLog("GetUsers start.", "GetUsers");
            List<UserModel> users = new List<UserModel>();
            try
            {
                var txtPath = Constant.LastTimePath;
                string lasttime = ReadLasttime(txtPath);

                SymmCrypt symmCrypt = new SymmCrypt();
                var key = Constant.SCKey;

                var token = key + lasttime;
                var tokenMD5 = MD5Encrypt32(token);

                string ltime = symmCrypt.DESEnCode(lasttime, key, key);

                var request = (HttpWebRequest)WebRequest.Create(Constant.GlobleWebSync);

                var postData = "token=" + tokenMD5;
                postData += "&lasttime=" + ltime;
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


                JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
                int status = Convert.ToInt32(jo["status"]);
                var dataJson = jo["data"];

                if (status == 0 && dataJson != null && !string.IsNullOrEmpty(dataJson.ToString()))
                {
                    var dataList = jo["data"].ToList();

                    foreach (JToken pJson in dataList)
                    {
                        var pData = ((JProperty)(pJson)).Value.ToString();
                        var people = JsonConvert.DeserializeObject<UserModel>(pData);
                        users.Add(people);
                    }
                }

                // 修改最后更新时间
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var newlasttime = Convert.ToInt64(ts.TotalSeconds).ToString();
                System.IO.File.WriteAllText(txtPath, newlasttime);
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "GetUsers");
            }
            XMLHelper.SetLog("GetUsers end. users'count : " + users.Count, "GetUsers");
            return users;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="token">加密前</param>
        /// <returns>加密后</returns>
        private static string MD5Encrypt32(string token)
        {
            string tk = "";
            try
            {
                string cl = token;

                MD5 md5 = MD5.Create(); //实例化一个md5对像
                                        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
                // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                for (int i = 0; i < s.Length; i++)
                {
                    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                    tk = tk + s[i].ToString("x2");
                }
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message + " token:" + token, "MD5Encrypt32");
            }
            return tk;
        }

        /// <summary>
        /// 获取最后更新时间
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>最后更新时间</returns>
        private static string ReadLasttime(string path)
        {
            string lasttime = string.Empty;
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    lasttime += line;
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message + " path:" + path, "ReadLasttime");
            }

            if (lasttime == null || string.IsNullOrEmpty(lasttime))
            {
                TimeSpan tsl = DateTime.UtcNow.AddMonths(-1) - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                lasttime = Convert.ToInt64(tsl.TotalSeconds).ToString();
            }
            return lasttime;
        }

    }
}