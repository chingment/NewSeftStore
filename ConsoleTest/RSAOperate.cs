using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public interface RSAOperate
    {
        /**
 * 公钥加密
 *
 * @param encryptedData 加密串
 */
        string encryptByPublicKey(string encryptedData);

        /**
         * 私钥加密
         *
         * @param encryptedData 加密串
         */
        string encryptByPrivateKey(string encryptedData);

        /**
         * 公钥解密
         *
         * @param decryptData 解密串
         * @return 结果
         */
        string decryptByPublicKey(string decryptData);

        /**
         * 私钥解密
         *
         * @param decryptData 解密串
         * @return 结果
         */
        string decryptByPrivateKey(string decryptData);

        /**
         * 获取私钥
         *
         * @param keyMap 密钥对
         */
        string getPrivateKey(Dictionary<string, object> keyMap);

        /**
         * 获取公钥
         *
         * @param keyMap 密钥对
         */
        string getPublicKey(Dictionary<string, object> keyMap);
    }
}
