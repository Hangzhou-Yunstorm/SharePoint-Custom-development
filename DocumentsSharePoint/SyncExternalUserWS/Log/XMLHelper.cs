using System;
using System.IO;
using System.Xml;

namespace SyncExternalUserWS
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class XMLHelper
    {

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void SetLog(string errorMessage, string method)
        {
            XMLHelper xh = new XMLHelper();
            string path = "E://SyncExternalUserWSLog//";
            ErrorMessageModel emm = new ErrorMessageModel();
            emm.Content = errorMessage;
            emm.Time = DateTime.Now.ToString();
            emm.Title = method;
            xh.CreateLog(emm, path);
        }

        /// <summary>
        /// 创建一条日志
        /// </summary>
        /// <param name="path"></param>
        private void CreateLog(ErrorMessageModel emm, string path)
        {
            try
            {

                DateTime timeNow = DateTime.Now;
                int yearNow = timeNow.Year;
                int monthNow = timeNow.Month;
                path = path + yearNow + "\\" + monthNow + "\\";
                string xmlPath = path + timeNow.ToString("yyyy-MM-dd") + ".xml";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!File.Exists(xmlPath))
                {
                    CreateXml(xmlPath);
                }
                UpdateXml(emm, xmlPath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 创建XML
        /// </summary>
        /// <param name="path"></param>
        private void CreateXml(string xmlPath)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xmlDec);

                XmlElement xmlEle = xmlDoc.CreateElement("", "Logs", "");
                xmlDoc.AppendChild(xmlEle);

                xmlDoc.Save(xmlPath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 更新XML
        /// </summary>
        /// <param name="emm"></param>
        private void UpdateXml(ErrorMessageModel emm, string xmlPath)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);
                XmlNode xmlRoot = xmlDoc.SelectSingleNode("Logs");
                XmlElement xmlEle = xmlDoc.CreateElement("Log");

                XmlElement xmlTitle = xmlDoc.CreateElement("Title");
                xmlTitle.InnerText = emm.Title;
                XmlElement xmlTime = xmlDoc.CreateElement("Time");
                xmlTime.InnerText = emm.Time;
                XmlElement xmlContent = xmlDoc.CreateElement("Content");
                xmlContent.InnerText = emm.Content;

                xmlEle.AppendChild(xmlTitle);
                xmlEle.AppendChild(xmlTime);
                xmlEle.AppendChild(xmlContent);

                xmlRoot.AppendChild(xmlEle);
                xmlDoc.Save(xmlPath);
            }
            catch
            { }
        }
    }
}
