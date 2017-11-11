using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Encryption
    {
        public string HashString(string mytext)
        {
            SHA256 myAlg = SHA256.Create(); // initialize the algorithm instance

            byte[] input = Encoding.UTF32.GetBytes(mytext); // converting from string to byte[]

            byte[] digest = myAlg.ComputeHash(input); // hashing byte[] >> base64 types

            return Convert.ToBase64String(digest); // converting back from byte[] to string
        }
    }
}
