using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class XMLHelper
    {
        /// <summary>
        /// 创建一条日志
        /// </summary>
        /// <param name="path"></param>
        public void CreateLog(Common.ErrorMessageModel emm, string path)
        {
            try
            {
                
                DateTime timeNow = DateTime.Now;
                int yearNow = timeNow.Year;
                string monthNow = Helper.MonthToFullMonth(timeNow.Month);
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
            catch (Exception)
            {
                throw;
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

                XmlElement xmlEle = xmlDoc.CreateElement("", "Log", "");
                xmlDoc.AppendChild(xmlEle);

                xmlDoc.Save(xmlPath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 更新XML
        /// </summary>
        /// <param name="emm"></param>
        private void UpdateXml(Common.ErrorMessageModel emm, string xmlPath)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);
                XmlNode xmlRoot = xmlDoc.SelectSingleNode("Log");
                XmlElement xmlEle = xmlDoc.CreateElement("Error");

                XmlElement xmlTitle = xmlDoc.CreateElement("Title");
                xmlTitle.InnerText = emm.Title;
                XmlElement xmlTime = xmlDoc.CreateElement("Time");
                xmlTime.InnerText = emm.Time;
                XmlElement xmlContent = xmlDoc.CreateElement("Content");
                xmlContent.InnerText = emm.Content;

                XmlElement xmlFilePath = xmlDoc.CreateElement("FilePath");
                xmlFilePath.InnerText = emm.FilePath;

                xmlEle.AppendChild(xmlTitle);
                xmlEle.AppendChild(xmlTime);
                xmlEle.AppendChild(xmlContent);
                xmlEle.AppendChild(xmlFilePath);

                xmlRoot.AppendChild(xmlEle);
                xmlDoc.Save(xmlPath);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
