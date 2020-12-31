using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class MyDESCryptoUtil
    {
        private const string PickupCode_KEY_64 = "VavicXbv";//注意了，是8个字符，64位
        private const string PickupCode_IV_64 = "VavicXbv";
        private static string Encode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_IV_64);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }
        private static string Decode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(PickupCode_IV_64);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        public static string BuildQrcode2PickupCode(string pCode)
        {
            string code = MyDESCryptoUtil.Encode(pCode);
            string value = "pickupcode@v2:" + code;
            return value;
        }
        public static string DecodeQrcode2PickupCode(string pCode)
        {
            if (pCode.IndexOf("pickupcode@v2:") < 0)
                return null;

            string code = pCode.Split(':')[1];
            string value = MyDESCryptoUtil.Decode(code);
            return value;
        }

        public static string BuildQrcode2CouponWtCode(string pCode)
        {
            string code = MyDESCryptoUtil.Encode(pCode);
            string value = "couponwtcode@v2:" + code;
            return value;
        }
        public static string DecodeQrcode2CouponWtCode(string pCode)
        {
            if (pCode.IndexOf("couponwtcode@v2:") < 0)
                return null;

            string code = pCode.Split(':')[1];
            string value = MyDESCryptoUtil.Decode(code);
            return value;
        }
    }
}
