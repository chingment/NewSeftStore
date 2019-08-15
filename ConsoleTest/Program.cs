using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class userInfo
    {
        public string channelUserCode { get; set; }
        public string userName { get; set; }
        public string phone { get; set; }
        public string idName { get; set; }
        public string email { get; set; }
        public string channelCompanyCode { get; set; }
    }

    class Program
    {
        private static AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {

            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            byte[] publicInfoByte = Convert.FromBase64String(s);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入   
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }

        private static AsymmetricKeyParameter GetPrivateKeyParameter(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            byte[] privateInfoByte = Convert.FromBase64String(s);
            // Asn1Object priKeyObj = Asn1Object.FromByteArray(privateInfoByte);//这里也可以从流中读取，从本地导入   
            // PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
            return priKey;
        }

        static void Main(string[] args)
        {
            String input = "{channelUserCode:\"15989287032\",userName:\"15989287032\",phone:\"15989287032\",idName:\"\",email:\"\",channelCompanyCode:\"\"}";

            userInfo u = new userInfo();

            u.channelUserCode = "15989287032";
            u.userName = "15989287032";
            u.phone = "15989287032";
            u.idName = "";
            u.email = "";
            u.channelCompanyCode = "";


            input = Newtonsoft.Json.JsonConvert.SerializeObject(u);

            input = input.Replace("\\", "");
            //// 公钥
            //String publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCuTexchfJUXLJR5cSevbQutT9dYj95tW8NuH/Bc5JZwGn/WOr1Vw6jg7ncVTilPRjIS5XqmcTFm6H0qa/uZ3Vn1Mh+XZkfX6hcxmeTQm9vkv3mb4hpWRFhfri3gcbmkQr+DnklqoI7TGWcuYQt47BeppGeeKT7WYblz+lZvaqepQIDAQAB";
            String publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDTT1ryGLfq5lucyHdzPLbjtcVsgurf5x4Y09U/cTiV85duIk0zQeRTXNyGcMAS92+xV/eGp7IjncwL8QE8JqlclLvuOU3zTdlAQ58lu/JcTcsF6eA6JXb8OJAhmDoug1J77M2GLoqAl0Cf34kavj/r9bAQpWqbk8JlJU3YqIePuwIDAQAB";

            //// 私钥
            String privateKey = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAK5N7FyF8lRcslHlxJ69tC61P11iP3m1bw24f8FzklnAaf9Y6vVXDqODudxVOKU9GMhLleqZxMWbofSpr+5ndWfUyH5dmR9fqFzGZ5NCb2+S/eZviGlZEWF+uLeBxuaRCv4OeSWqgjtMZZy5hC3jsF6mkZ54pPtZhuXP6Vm9qp6lAgMBAAECgYEApMJLdXm3gj7M39UMcfBnbO8uIhtIXMc/XfzT5gxUfjn+97sY/Sd5Ut6kxLxZevexgULRRpxq/08JW7c58WQh+J+ORdoqPwEyvINcvvAaFtJ0ydIhTi3I4yfmgmgqCdC7stuS6kDB+LO+8nRpNwBEhBIZzzgZcD6qc4sXdS1gD9ECQQDHiBpQl8rJkyTwEV9J0h4bE8oBSuHrIyVRv4VW/l11rBwYh9NWeB+CZy9x6IzsbNOApopUEdLBr00PWGrejkpvAkEA36IgeXEJB6t3+0JwrqvvaQXH94JPHZDNvExQ1fMXlYEmqZEXzANkLu1syVYwTs8FsM/hosdTZ8dBhF43zGTCKwJASNKswANWeNFiZtgATiII6NsFHAmngLk4Eqjy0nhNxffF3VIdWO7ImUBtuYYlgNiLLOYbkGlc4WHInzQm9Qk7swJBAJGVUd69H0vG7Iy9a+3KMEkGYm9WfXqZ0dVLOTSO1EOXmDu7IOrHKmkCV1earEghrWq1agY2DK36oUQysdB1p5ECQQCT+PDLPTmvJ8OtyQknPOyFDyG/yJB7MsT6Ylr9Rhna0+PqSfgY2hvxK6aHam3sMscPCRYjHI/ptYpHgznhY8uz";



            //String input = "{\"user\":\"test\",\"companyId\":\"test\"}";

            //// 公钥
            //String publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCuTexchfJUXLJR5cSevbQutT9dYj95tW8NuH/Bc5JZwGn/WOr1Vw6jg7ncVTilPRjIS5XqmcTFm6H0qa/uZ3Vn1Mh+XZkfX6hcxmeTQm9vkv3mb4hpWRFhfri3gcbmkQr+DnklqoI7TGWcuYQt47BeppGeeKT7WYblz+lZvaqepQIDAQAB";
            //// 私钥
            //String privateKey = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAK5N7FyF8lRcslHlxJ69tC61P11iP3m1bw24f8FzklnAaf9Y6vVXDqODudxVOKU9GMhLleqZxMWbofSpr+5ndWfUyH5dmR9fqFzGZ5NCb2+S/eZviGlZEWF+uLeBxuaRCv4OeSWqgjtMZZy5hC3jsF6mkZ54pPtZhuXP6Vm9qp6lAgMBAAECgYEApMJLdXm3gj7M39UMcfBnbO8uIhtIXMc/XfzT5gxUfjn+97sY/Sd5Ut6kxLxZevexgULRRpxq/08JW7c58WQh+J+ORdoqPwEyvINcvvAaFtJ0ydIhTi3I4yfmgmgqCdC7stuS6kDB+LO+8nRpNwBEhBIZzzgZcD6qc4sXdS1gD9ECQQDHiBpQl8rJkyTwEV9J0h4bE8oBSuHrIyVRv4VW/l11rBwYh9NWeB+CZy9x6IzsbNOApopUEdLBr00PWGrejkpvAkEA36IgeXEJB6t3+0JwrqvvaQXH94JPHZDNvExQ1fMXlYEmqZEXzANkLu1syVYwTs8FsM/hosdTZ8dBhF43zGTCKwJASNKswANWeNFiZtgATiII6NsFHAmngLk4Eqjy0nhNxffF3VIdWO7ImUBtuYYlgNiLLOYbkGlc4WHInzQm9Qk7swJBAJGVUd69H0vG7Iy9a+3KMEkGYm9WfXqZ0dVLOTSO1EOXmDu7IOrHKmkCV1earEghrWq1agY2DK36oUQysdB1p5ECQQCT+PDLPTmvJ8OtyQknPOyFDyG/yJB7MsT6Ylr9Rhna0+PqSfgY2hvxK6aHam3sMscPCRYjHI/ptYpHgznhY8uz";

            //        // 一般使用公钥加密，私钥解密
            //        // 如果只要加密则只需要入参公钥
            ////        CustomKeyPair keyPair = CustomKeyPair.of(publicKey, null);
            //        CustomKeyPair keyPair = CustomKeyPair.of(publicKey, privateKey);
            //        RSAOperate rsaOperate = DefaultRSAOperate.from(keyPair);

            //        String encry = rsaOperate.encryptByPublicKey(input);
            //        System.out.println("加密结果:" + encry);
            //        String decry = rsaOperate.decryptByPrivateKey(encry);
            //        
            //        System.out.println("结果相同:" + input.equals(decry));

            RSAForJava rsa = new RSAForJava();
            var key = rsa.GetKey(publicKey, privateKey);

            String encry = rsa.EncryptByPublicKey(input, key.PublicKey);

            Console.WriteLine("加密结果:" + encry);
            String decry = rsa.DecryptByPrivateKey(encry, key.PrivateKey);
            Console.WriteLine("解密结果:" + decry);
        }
    }
}
