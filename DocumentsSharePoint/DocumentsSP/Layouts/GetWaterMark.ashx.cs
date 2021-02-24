using System;
using Microsoft.SharePoint;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DocumentsSP.Layouts
{
    public partial class GetWaterMark : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/png";
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.CacheControl = "no-cache";
            var loginNameList = SPContext.Current.Web.CurrentUser.LoginName.Split('\\');
            var userId = loginNameList[loginNameList.Length - 1];
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd");
            byte[] buffer = GetPng(userId + " " + dateNow);
            context.Response.BinaryWrite(buffer);
        }

        /// <summary>
        /// 生成水印图片
        /// </summary>
        /// <param name="waterWords">水印文字</param>
        /// <returns></returns>
        private byte[] GetPng(string waterWords)
        {
            int width = 700;
            int height = 200;

            Bitmap bm = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage(bm);
            g.FillRectangle(new SolidBrush(Color.FromArgb(0, 255, 255, 255)), new Rectangle(0, 0, width, height));
            Font crFont = new Font("Microsoft YaHei", 40, FontStyle.Italic);
            SizeF crSize = g.MeasureString(waterWords, crFont);
            int strX = (int)((float)width - crSize.Width) / 2;
            int strY = (int)((float)height - crSize.Height) / 2;

            g.TranslateTransform(0, strY); //设置旋转中心为文字中心
            g.RotateTransform((float)(-10)); //旋转


            g.DrawString(waterWords, crFont, new SolidBrush(Color.FromArgb(100, 245, 245, 245)), strX, strY);
            MemoryStream ms = new MemoryStream();
            bm.Save(ms, ImageFormat.Png);
            return ms.GetBuffer();
        }

        #endregion
    }
}
