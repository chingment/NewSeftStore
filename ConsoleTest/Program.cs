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
        static void Main(string[] args)
        {
            String input = "{\"channelUserCode\":\"15989287032\",\"userName\":\"15989287032\",\"phone\":\"15989287032\",\"idName\":\"\",\"email\":\"\",\"channelCompanyCode\":\"\"}";

            // 公钥
            String publicKey1 = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDAlyY859V3tM804vEFdefW9O3MLbgvzS/cLBIp1TZ+kH+HVDYlAXe/FoDDsg58Znltz+2mbK5qv1SyOY1Y3W8MJD7AIgsq1SvuSzWOXPzG8OJxCOWo+WEKBteVCXtAiS3igxrQb3QwJ8JoJkhAViT1RUxv94RPgQIAGx4xb7R3nwIDAQAB";
            // 私钥
            String privateKey2 = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAK5N7FyF8lRcslHlxJ69tC61P11iP3m1bw24f8FzklnAaf9Y6vVXDqODudxVOKU9GMhLleqZxMWbofSpr+5ndWfUyH5dmR9fqFzGZ5NCb2+S/eZviGlZEWF+uLeBxuaRCv4OeSWqgjtMZZy5hC3jsF6mkZ54pPtZhuXP6Vm9qp6lAgMBAAECgYEApMJLdXm3gj7M39UMcfBnbO8uIhtIXMc/XfzT5gxUfjn+97sY/Sd5Ut6kxLxZevexgULRRpxq/08JW7c58WQh+J+ORdoqPwEyvINcvvAaFtJ0ydIhTi3I4yfmgmgqCdC7stuS6kDB+LO+8nRpNwBEhBIZzzgZcD6qc4sXdS1gD9ECQQDHiBpQl8rJkyTwEV9J0h4bE8oBSuHrIyVRv4VW/l11rBwYh9NWeB+CZy9x6IzsbNOApopUEdLBr00PWGrejkpvAkEA36IgeXEJB6t3+0JwrqvvaQXH94JPHZDNvExQ1fMXlYEmqZEXzANkLu1syVYwTs8FsM/hosdTZ8dBhF43zGTCKwJASNKswANWeNFiZtgATiII6NsFHAmngLk4Eqjy0nhNxffF3VIdWO7ImUBtuYYlgNiLLOYbkGlc4WHInzQm9Qk7swJBAJGVUd69H0vG7Iy9a+3KMEkGYm9WfXqZ0dVLOTSO1EOXmDu7IOrHKmkCV1earEghrWq1agY2DK36oUQysdB1p5ECQQCT+PDLPTmvJ8OtyQknPOyFDyG/yJB7MsT6Ylr9Rhna0+PqSfgY2hvxK6aHam3sMscPCRYjHI/ptYpHgznhY8uz";


        }
    }
}
