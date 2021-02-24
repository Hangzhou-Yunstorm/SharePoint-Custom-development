using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SyncExternalUserWS
{
    public class SymmCrypt
    {

        private SymmetricAlgorithm mobjCryptoService = new DESCryptoServiceProvider();

        /// <summary>
        /// 获取Key的字节
        /// </summary>
        /// <param name="keyString">Key字符串</param>
        /// <returns>转换后的字节形式</returns>
        private byte[] GetKeyBytes(string keyString)
        {
            string sTemp = "";
            if (mobjCryptoService.LegalKeySizes.Length > 0)
            {
                int lessSize = 0;
                int moreSize = mobjCryptoService.LegalKeySizes[0].MinSize;
                while (keyString.Length * 8 > moreSize)
                {
                    lessSize = moreSize;
                    moreSize += mobjCryptoService.LegalKeySizes[0].SkipSize;
                    System.Threading.Thread.Sleep(0);
                }
                sTemp = keyString.PadRight(moreSize / 8, '_');
            }
            else
                sTemp = keyString;
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 获取IV向量的字节
        /// </summary>
        /// <param name="ivString">IV向量字符串</param>
        /// <returns>转换后的字节形式</returns>
        private byte[] GetIvBytes(string ivString)
        {
            ivString += "diksk.sl";
            ivString = ivString.Substring(0, this.mobjCryptoService.IV.Length);
            return ASCIIEncoding.ASCII.GetBytes(ivString);
        }

        /// <summary>
        /// 加密
        /// </summary>
        public string DESEnCode(string sourceString, string keyString, string ivString)
        {

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(sourceString);

            des.Key = GetKeyBytes(keyString);
            des.IV = GetIvBytes(ivString);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }

            return ret.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        public string DESDeCode(string sourceString, string keyString, string ivString)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[sourceString.Length / 2];
            for (int x = 0; x < sourceString.Length / 2; x++)
            {
                int i = (Convert.ToInt32(sourceString.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = GetKeyBytes(keyString);
            des.IV = GetIvBytes(ivString);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Encoding.Default.GetString(ms.ToArray());
        }

    }
}