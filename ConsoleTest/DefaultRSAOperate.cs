using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class DefaultRSAOperate : RSAOperate
    {
        /**
   * 加密算法RSA
   */
        private static readonly string KEY_ALGORITHM = "RSA";

        /**
         * 签名算法
         */
        private static readonly string SIGNATURE_ALGORITHM = "MD5withRSA";

        /**
         * 获取公钥的key
         */
        private static readonly string PUBLIC_KEY = "RSAPublicKey";

        /**
         * 获取私钥的key
         */
        private static readonly string PRIVATE_KEY = "RSAPrivateKey";

        /**
         * RSA最大加密明文大小
         */
        private static readonly int MAX_ENCRYPT_BLOCK = 117;

        /**
         * RSA最大解密密文大小
         */
        private static readonly int MAX_DECRYPT_BLOCK = 128;

        // 密钥对数据
        private CustomKeyPair customKeyPair;

        private DefaultRSAOperate(CustomKeyPair customKeyPair)
        {
            this.customKeyPair = customKeyPair;
        }

        public static DefaultRSAOperate from(CustomKeyPair customKeyPair)
        {
            return new DefaultRSAOperate(customKeyPair);
        }

        /**
         * 公钥加密
         *
         * @param encryptedData 加密串
         */
        public string encryptByPublicKey(string encryptedData)
        {
            return encrypt(encryptedData, 0);
        }

        /**
         * 私钥加密
         *
         * @param encryptedData 加密串
         */
        public string encryptByPrivateKey(string encryptedData)
        {

            return encrypt(encryptedData, 1);
        }

        /**
         * 公钥解密
         *
         * @param decryptData 解密串
         * @return 结果
         */
        public string decryptByPublicKey(string decryptData)
        {

            return decrypt(decryptData, 0);
        }

        /**
         * 私钥解密
         *
         * @param decryptData 解密串
         * @return 结果
         */
        public string decryptByPrivateKey(string decryptData)
        {

            return decrypt(decryptData, 1);
        }

        /**
         * 公钥加密
         *
         * @param encryptedData 加密串
         * @param type          0公钥/1私钥
         */
        private string encrypt(string encryptedData, int type)
        {
            if (string.IsNullOrEmpty(encryptedData))
            {
                return "";
            }

            byte[] decodedData;
            try
            {
                if (0 == type)
                {
                    decodedData = encryptByPublicKey(Encoding.UTF8.GetBytes(encryptedData));
                }
                else
                {
                    decodedData = encryptByPrivateKey(Encoding.UTF8.GetBytes(encryptedData));
                }
            }
            catch (Exception e)
            {
                return "";
            }

            return Base64.EncodeBase64(Encoding.UTF8, decodedData);
        }

        /**
         * 解密
         *
         * @param decryptData 解密串
         * @param type        0公钥/1私钥
         */
        private String decrypt(string decryptData, int type)
        {
            if (string.IsNullOrEmpty(decryptData))
            {
                return "";
            }


            byte[] data = Convert.FromBase64String(decryptData);
            byte[] decodedData;
            try
            {
                if (0 == type)
                {
                    decodedData = decryptByPublicKey(data);
                }
                else
                {
                    decodedData = decryptByPrivateKey(data);
                }
            }
            catch (Exception e)
            {
                return "";
            }

            return System.Text.Encoding.UTF8.GetString(decodedData);
        }

        /**
         * 私钥解密
         *
         * @param encryptedData 已加密数据
         */
        private byte[] decryptByPrivateKey(byte[] encryptedData)
        {
            //byte[] keyBytes = Convert.FromBase64String(customKeyPair.PrivateKey);

            //PKCS8EncodedKeySpec pkcs8KeySpec = new PKCS8EncodedKeySpec(keyBytes);
            //KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
            //Key privateK = keyFactory.generatePrivate(pkcs8KeySpec);
            //Cipher cipher = Cipher.getInstance(keyFactory.getAlgorithm());
            //cipher.init(Cipher.DECRYPT_MODE, privateK);

            //return blockEncrypt(encryptedData, cipher, MAX_DECRYPT_BLOCK);
            return null;
        }

        /**
         * 公钥解密
         *
         * @param encryptedData 已加密数据
         */
        private byte[] decryptByPublicKey(byte[] encryptedData)
        {
            //byte[]
            //keyBytes = org.apache.commons.codec.binary.Base64.decodeBase64(customKeyPair.getPublicKey());
            //X509EncodedKeySpec x509KeySpec = new X509EncodedKeySpec(keyBytes);
            //KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
            //Key publicK = keyFactory.generatePublic(x509KeySpec);
            //Cipher cipher = Cipher.getInstance(keyFactory.getAlgorithm());
            //cipher.init(Cipher.DECRYPT_MODE, publicK);

            //return blockEncrypt(encryptedData, cipher, MAX_DECRYPT_BLOCK);
            return null;
        }

        /**
         * 公钥加密
         *
         * @param data 源数据
         */
        private byte[] encryptByPublicKey(byte[] data)
        {
            //    byte[]
            //keyBytes = org.apache.commons.codec.binary.Base64.decodeBase64(customKeyPair.getPublicKey());
            //    X509EncodedKeySpec x509KeySpec = new X509EncodedKeySpec(keyBytes);
            //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
            //    Key publicK = keyFactory.generatePublic(x509KeySpec);
            //    // 对数据加密
            //    Cipher cipher = Cipher.getInstance(keyFactory.getAlgorithm());
            //    cipher.init(Cipher.ENCRYPT_MODE, publicK);

            //    return blockEncrypt(data, cipher, MAX_ENCRYPT_BLOCK);
            return null;
        }

        /**
         * 私钥加密
         *
         * @param data 源数据
         */
        private byte[] encryptByPrivateKey(byte[] data)
        {
            //    byte[]
            //keyBytes = org.apache.commons.codec.binary.Base64.decodeBase64(customKeyPair.getPrivateKey());
            //    PKCS8EncodedKeySpec pkcs8KeySpec = new PKCS8EncodedKeySpec(keyBytes);
            //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
            //    Key privateK = keyFactory.generatePrivate(pkcs8KeySpec);
            //    Cipher cipher = Cipher.getInstance(keyFactory.getAlgorithm());
            //    cipher.init(Cipher.ENCRYPT_MODE, privateK);

            //    return blockEncrypt(data, cipher, MAX_ENCRYPT_BLOCK);
            return null;
        }

        /**
         * 分段加密
         */
        //private byte[] blockEncrypt(byte[] data, Cipher cipher, int maxBlock)
        //{
        //    //    int inputLen = data.length;
        //    //    int offSet = 0;
        //    //    ByteArrayOutputStream out = new ByteArrayOutputStream();
        //    //    byte[] cache;
        //    //    int i = 0;
        //    //    while (inputLen - offSet > 0)
        //    //    {
        //    //        if (inputLen - offSet > maxBlock)
        //    //        {
        //    //            cache = cipher.doFinal(data, offSet, maxBlock);
        //    //        }
        //    //        else
        //    //        {
        //    //            cache = cipher.doFinal(data, offSet, inputLen - offSet);
        //    //        }
        //    //    out.write(cache, 0, cache.length);
        //    //        i++;
        //    //        offSet = i * maxBlock;
        //    //    }

        //    //    byte[] decryptedData = out.toByteArray();
        //    //out.close();

        //    //    return decryptedData;
        //    return null;
        //}

        /**
         * 生成密钥对(公钥和私钥)
         */
        private Dictionary<String, Object> genKeyPair()
        {
            //KeyPairGenerator keyPairGen = KeyPairGenerator.getInstance(KEY_ALGORITHM);
            //keyPairGen.initialize(1024);
            //KeyPair keyPair = keyPairGen.generateKeyPair();
            //RSAPublicKey publicKey = (RSAPublicKey)keyPair.getPublic();
            //RSAPrivateKey privateKey = (RSAPrivateKey)keyPair.getPrivate();
            //Map<String, Object> keyMap = new HashMap<>(2);
            //keyMap.put(PUBLIC_KEY, publicKey);
            //keyMap.put(PRIVATE_KEY, privateKey);
            //return keyMap;
            return null;
        }

        /**
         * 用私钥对信息生成数字签名
         *
         * @param data       已加密数据
         * @param privateKey 私钥(BASE64编码)
         */
        private String sign(byte[] data, String privateKey)
        {
            //    byte[]
            //keyBytes = org.apache.commons.codec.binary.Base64.decodeBase64(privateKey);
            //    PKCS8EncodedKeySpec pkcs8KeySpec = new PKCS8EncodedKeySpec(keyBytes);
            //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
            //    PrivateKey privateK = keyFactory.generatePrivate(pkcs8KeySpec);
            //    Signature signature = Signature.getInstance(SIGNATURE_ALGORITHM);
            //    signature.initSign(privateK);
            //    signature.update(data);

            //    return new String(org.apache.commons.codec.binary.Base64.decodeBase64(signature.sign()));
            return null;
        }

        /**
         * 校验数字签名
         *
         * @param data      已加密数据
         * @param publicKey 公钥(BASE64编码)
         * @param sign      数字签名
         */
        private bool verify(byte[] data, String publicKey, String sign)
        {
            //    byte[]
            //keyBytes = org.apache.commons.codec.binary.Base64.decodeBase64(publicKey);
            //    X509EncodedKeySpec keySpec = new X509EncodedKeySpec(keyBytes);
            //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
            //    PublicKey publicK = keyFactory.generatePublic(keySpec);
            //    Signature signature = Signature.getInstance(SIGNATURE_ALGORITHM);
            //    signature.initVerify(publicK);
            //    signature.update(data);
            //    return signature.verify(org.apache.commons.codec.binary.Base64.decodeBase64(sign));
            return false;
        }

        /**
         * 获取私钥
         *
         * @param keyMap 密钥对
         */

        public string getPrivateKey(Dictionary<string, object> keyMap)
        {
            //Key key = (Key)keyMap.get(PRIVATE_KEY);
            //return new String(org.apache.commons.codec.binary.Base64.decodeBase64(key.getEncoded()));
            return null;
        }

        /**
         * 获取公钥
         *
         * @param keyMap 密钥对
         */
        public string getPublicKey(Dictionary<string, object> keyMap)
        {
            //Key key = (Key)keyMap.get(PUBLIC_KEY);
            //return new String(org.apache.commons.codec.binary.Base64.decodeBase64(key.getEncoded()));
            return null;
        }
    }
}
