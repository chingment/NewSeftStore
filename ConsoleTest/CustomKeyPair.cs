using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class CustomKeyPair
    {
        private string _publicKey;

        private string _privateKey;

        public string PublicKey
        {
            get
            {
                return _publicKey;
            }
            set
            {
                _publicKey = value;
            }
        }

        public string PrivateKey
        {
            get
            {
                return _privateKey;
            }
            set
            {
                _privateKey = value;
            }
        }

        public CustomKeyPair(string publicKey, string privateKey)
        {
            this._publicKey = publicKey;
            this._privateKey = privateKey;
        }
        public static CustomKeyPair of(string publicKey, string privateKey)
        {
            return new CustomKeyPair(publicKey, privateKey);
        }
    }
}
