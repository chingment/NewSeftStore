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
    class Program
    {
        public static AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            byte[] publicInfoByte = Convert.FromBase64String(s);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入   
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }
        public static AsymmetricKeyParameter GetPrivateKeyParameter(string s)
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
            String input = "{\"user\":\"test\",\"companyId\":\"test\"}";

            // 公钥
            String publicKey1 = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCuTexchfJUXLJR5cSevbQutT9dYj95tW8NuH/Bc5JZwGn/WOr1Vw6jg7ncVTilPRjIS5XqmcTFm6H0qa/uZ3Vn1Mh+XZkfX6hcxmeTQm9vkv3mb4hpWRFhfri3gcbmkQr+DnklqoI7TGWcuYQt47BeppGeeKT7WYblz+lZvaqepQIDAQAB";
            // 私钥
            String privateKey2 = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAK5N7FyF8lRcslHlxJ69tC61P11iP3m1bw24f8FzklnAaf9Y6vVXDqODudxVOKU9GMhLleqZxMWbofSpr+5ndWfUyH5dmR9fqFzGZ5NCb2+S/eZviGlZEWF+uLeBxuaRCv4OeSWqgjtMZZy5hC3jsF6mkZ54pPtZhuXP6Vm9qp6lAgMBAAECgYEApMJLdXm3gj7M39UMcfBnbO8uIhtIXMc/XfzT5gxUfjn+97sY/Sd5Ut6kxLxZevexgULRRpxq/08JW7c58WQh+J+ORdoqPwEyvINcvvAaFtJ0ydIhTi3I4yfmgmgqCdC7stuS6kDB+LO+8nRpNwBEhBIZzzgZcD6qc4sXdS1gD9ECQQDHiBpQl8rJkyTwEV9J0h4bE8oBSuHrIyVRv4VW/l11rBwYh9NWeB+CZy9x6IzsbNOApopUEdLBr00PWGrejkpvAkEA36IgeXEJB6t3+0JwrqvvaQXH94JPHZDNvExQ1fMXlYEmqZEXzANkLu1syVYwTs8FsM/hosdTZ8dBhF43zGTCKwJASNKswANWeNFiZtgATiII6NsFHAmngLk4Eqjy0nhNxffF3VIdWO7ImUBtuYYlgNiLLOYbkGlc4WHInzQm9Qk7swJBAJGVUd69H0vG7Iy9a+3KMEkGYm9WfXqZ0dVLOTSO1EOXmDu7IOrHKmkCV1earEghrWq1agY2DK36oUQysdB1p5ECQQCT+PDLPTmvJ8OtyQknPOyFDyG/yJB7MsT6Ylr9Rhna0+PqSfgY2hvxK6aHam3sMscPCRYjHI/ptYpHgznhY8uz";

            //// 一般使用公钥加密，私钥解密
            //// 如果只要加密则只需要入参公钥
            ////        CustomKeyPair keyPair = CustomKeyPair.of(publicKey, null);
            //CustomKeyPair keyPair = CustomKeyPair.of(publicKey, privateKey);
            //RSAOperate rsaOperate = DefaultRSAOperate.from(keyPair);

            //String encry = rsaOperate.encryptByPublicKey(input);
            //Console.WriteLine("加密结果:" + encry);
            //String decry = rsaOperate.decryptByPrivateKey(encry);
            //Console.WriteLine("解密结果:" + decry);
            //Console.WriteLine("结果相同:" + input.Equals(decry));



            //生成密钥对  
            //RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            //RsaKeyGenerationParameters rsaKeyGenerationParameters = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new Org.BouncyCastle.Security.SecureRandom(), 1024, 25);
            //rsaKeyPairGenerator.Init(rsaKeyGenerationParameters);//初始化参数  


            //AsymmetricCipherKeyPair keyPair = rsaKeyPairGenerator.GenerateKeyPair();
            AsymmetricKeyParameter publicKey = GetPublicKeyParameter(publicKey1);
            AsymmetricKeyParameter privateKey = GetPrivateKeyParameter(privateKey2);

            SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);

            Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();
            byte[] publicInfoByte = asn1ObjectPublic.GetEncoded();
            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            byte[] privateInfoByte = asn1ObjectPrivate.GetEncoded();

            //这里可以将密钥对保存到本地  
            Console.WriteLine("PublicKey:\n" + Convert.ToBase64String(publicInfoByte));
            Console.WriteLine("PrivateKey:\n" + Convert.ToBase64String(privateInfoByte));

            //加密、解密  
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入  
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(SubjectPublicKeyInfo.GetInstance(pubKeyObj));
            IAsymmetricBlockCipher cipher = new RsaEngine();
            cipher.Init(true, pubKey);//true表示加密  
            //加密  
            string data = "{\"user\":\"test\",\"companyId\":\"test\"}";
            Console.WriteLine("\n明文：" + data);
            byte[] encryptData = cipher.ProcessBlock(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
            Console.WriteLine("密文:" + Convert.ToBase64String(encryptData));
            //解密  
            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
            cipher.Init(false, priKey);//false表示解密  
            string decryptData = Encoding.UTF8.GetString(cipher.ProcessBlock(encryptData, 0, encryptData.Length));
            Console.WriteLine("解密后数据：" + decryptData);
            Console.Read();



        }
    }
}
