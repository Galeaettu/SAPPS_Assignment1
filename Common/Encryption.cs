using System;
using System.Collections.Generic;
using System.IO;
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

        public SymmetricParameters GenerateSymmetricParameters(string input)
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(input, new byte[] { 20, 255, 1, 16, 54, 78, 68, 10, 52, 38 });

            Rijndael myAlg = Rijndael.Create();

            SymmetricParameters myParams = new SymmetricParameters()
            {
                SecretKey = rfc.GetBytes(myAlg.KeySize / 8),
                IV = rfc.GetBytes(myAlg.BlockSize / 8)
            };

            return myParams;
        }

        public string EncryptString(string input, string passwordToBeUsedInSecreKey)
        {
            SymmetricParameters myParameters = GenerateSymmetricParameters(passwordToBeUsedInSecreKey);

            Rijndael myAlg = Rijndael.Create();
            myAlg.Key = myParameters.SecretKey;
            myAlg.IV = myParameters.IV;

            byte[] clearDataAsBytes = Encoding.UTF32.GetBytes(input); //Converting user input to byte[]

            MemoryStream msClearData = new MemoryStream(clearDataAsBytes);

            CryptoStream cs = new CryptoStream(msClearData, myAlg.CreateEncryptor(), CryptoStreamMode.Read);

            MemoryStream msEncryptedData = new MemoryStream();
            cs.CopyTo(msEncryptedData);

            byte[] encryptedDataAsBytes = msEncryptedData.ToArray();
            string encrypteddata = Convert.ToBase64String(encryptedDataAsBytes); //converting the cryptographic data to string

            encrypteddata = encrypteddata.Replace('=', '|');
            encrypteddata = encrypteddata.Replace('/', '_');
            encrypteddata = encrypteddata.Replace('+', '*');

            return encrypteddata;
        }

        public string DecryptString(string input, string passwordToBeUsedInSecreKey)
        {
            input = input.Replace('|', '=');
            input = input.Replace('_', '/');
            input = input.Replace('*', '+');

            SymmetricParameters myParameters = GenerateSymmetricParameters(passwordToBeUsedInSecreKey);

            Rijndael myAlg = Rijndael.Create();
            myAlg.Key = myParameters.SecretKey;
            myAlg.IV = myParameters.IV;

            byte[] encryptedDataAsBytes = Convert.FromBase64String(input); //Converting user input to byte[]

            MemoryStream msEncryptedData = new MemoryStream(encryptedDataAsBytes);

            CryptoStream cs = new CryptoStream(msEncryptedData, myAlg.CreateDecryptor(), CryptoStreamMode.Read);

            MemoryStream msClearData = new MemoryStream();
            cs.CopyTo(msClearData);

            byte[] clearDataAsBytes = msClearData.ToArray();
            string clearData = Encoding.UTF32.GetString(clearDataAsBytes); //converting the cryptographic data to string

            return clearData;
        }

        public AsymmetricParameters GenerateAsymmetricParameters() //call this method when a user registers and store pubic/private key in db
        {
            //RSA DSA
            RSA myAlg = RSA.Create();
            AsymmetricParameters myParameters = new AsymmetricParameters()
            {
                PublicKey = myAlg.ToXmlString(false),
                PrivateKey = myAlg.ToXmlString(true)
            };
            return myParameters;
        }

        public string EncryptAsymmetricallyString(string input, string publicKey)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(publicKey);

            byte[] inputAsBytes = Encoding.UTF32.GetBytes(input);

            byte[] cipher = myAlg.Encrypt(inputAsBytes, true);

            string encryptedText = Convert.ToBase64String(cipher);

            return encryptedText;
        }

        public string DecryptAsymmetricallyString(string cipher, string privateKey)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(privateKey);

            byte[] cipherAsBytes = Convert.FromBase64String(cipher);

            byte[] clearDataAsBytes = myAlg.Decrypt(cipherAsBytes, true);

            string clearData = Encoding.UTF32.GetString(clearDataAsBytes);

            return clearData;

        }

        public MemoryStream EncryptSymmetricallyFile(Stream input, SymmetricParameters myParameters)
        {
            Rijndael myAlg = Rijndael.Create();
            myAlg.Key = myParameters.SecretKey;
            myAlg.IV = myParameters.IV;

            CryptoStream cs = new CryptoStream(input, myAlg.CreateEncryptor(), CryptoStreamMode.Read);

            MemoryStream msEncryptedData = new MemoryStream();
            cs.CopyTo(msEncryptedData);

            return msEncryptedData;
        }

        public MemoryStream EncryptAsymmetricallyKey(byte[] key, string publicKey)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(publicKey);

            byte[] cipher = myAlg.Encrypt(key, true);

            MemoryStream msEncrytedData = new MemoryStream(cipher);//to convert from byte[] to memorystream
            msEncrytedData.Position = 0;

            return msEncrytedData;
        }

        public MemoryStream DecryptAsymmetricallyKey(byte[] key, string privateKey)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(privateKey);

            byte[] clearDataAsBytes = myAlg.Decrypt(key, true);

            return new MemoryStream(clearDataAsBytes);
        }

        public MemoryStream DecryptSymmetricallyFile(Stream input, SymmetricParameters myParameters)
        {
            Rijndael myAlg = Rijndael.Create();
            myAlg.Key = myParameters.SecretKey;
            myAlg.IV = myParameters.IV;

            CryptoStream cs = new CryptoStream(input, myAlg.CreateDecryptor(), CryptoStreamMode.Read);

            MemoryStream msEncryptedData = new MemoryStream();
            cs.CopyTo(msEncryptedData);

            return msEncryptedData;
        }

        public Stream HybridEncryptFile(Stream article, string username, string publickey)
        {
            //1. Generate the secret key and the IV from the username
            //>> SymmetricParameters
            SymmetricParameters myParameters = GenerateSymmetricParameters(username);

            //2.Encrypt the file using the methodEncryptSymmetricallyFile with the parameters from step 1.
            Stream encrypted = EncryptSymmetricallyFile(article, myParameters);

            //3. encrypt the secret key and the iv using the public key and using the method EncryptAsymmetricallyKey
            Stream encryptedSecretKey = EncryptAsymmetricallyKey(myParameters.SecretKey, publickey);
            Stream encryptedIV = EncryptAsymmetricallyKey(myParameters.IV, publickey);

            //4. you store the encrypted secret key(from no.3) + the encrypted iv (from no.3) + encrypted file content (from no.2)

            MemoryStream msEncryptedFile = new MemoryStream();
            encryptedSecretKey.CopyTo(msEncryptedFile);
            encryptedIV.CopyTo(msEncryptedFile);
            encrypted.Position = 0;
            encrypted.CopyTo(msEncryptedFile);

            return msEncryptedFile;
        }

        public byte[] HybridDecryptFile(Stream encryptedArticle, string privateKey)
        {
            //1. Read the secret key //128 bytes
            byte[] encSecretKey = new byte[128];
            encryptedArticle.Read(encSecretKey, 0, 128);
            //2. Read the IV //128 bytes
            byte[] encIv = new byte[128];
            encryptedArticle.Read(encIv, 0, 128);
            //3. Read the rest of the file
            byte[] encArticle = new byte[encryptedArticle.Length - encryptedArticle.Position];
            encryptedArticle.Read(encArticle, 0, encArticle.Length);
            //4. Decrypt the secretkey using the privatekey
            MemoryStream decryptKey = DecryptAsymmetricallyKey(encSecretKey, privateKey);

            //5.Decrypt the iv using the secretkey
            MemoryStream decryptIV = DecryptAsymmetricallyKey(encIv, privateKey);
            //6. decrypt the file content read in no.3 using the decrypted secret key and decrypted iv
            MemoryStream decryptFile = DecryptSymmetricallyFile(new MemoryStream(encArticle), new SymmetricParameters { SecretKey = decryptKey.ToArray(), IV = decryptIV.ToArray() });

            return decryptFile.ToArray();
        }

        public byte[] HashBytes(byte[] file)
        {
            SHA256 myAlg = SHA256.Create(); //initialize the algorithm instance

            byte[] digest = myAlg.ComputeHash(file); //hashing byte[] >> base64 bytes

            return digest;
        }

        public string DigitalSign(Stream file, string privateKey) // of the user who is uplaoding the document
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey); //privatekey from db

            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Position = 0;
            byte[] digest = HashBytes(ms.ToArray());

            byte[] sigature = rsa.SignHash(digest, "SHA256");

            return Convert.ToBase64String(sigature);
        }

        public bool DigitalVerify(Stream file, string publicKey, string signature)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey); //publicKey from db

            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Position = 0;
            byte[] digest = HashBytes(ms.ToArray());

            return rsa.VerifyHash(digest, "SHA256", Convert.FromBase64String(signature));
        }
    }

    public class SymmetricParameters
    {
        public byte[] SecretKey { get; set; }
        public byte[] IV { get; set; }
    }

    public class AsymmetricParameters
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
