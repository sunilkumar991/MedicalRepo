using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Nop.Plugin.Payments.OKDollar
{
    public class AESDecrypt
    {
        private UTF8Encoding _enc;
        private RijndaelManaged _rcipher;
        private byte[] _key, _pwd, _ivBytes, _iv;
        public AESDecrypt()
        {
            _enc = new UTF8Encoding();
            _rcipher = new RijndaelManaged();
            _rcipher.Mode = CipherMode.CBC;
            _rcipher.Padding = PaddingMode.PKCS7;
            _rcipher.KeySize = 256;
            _rcipher.BlockSize = 128;
            _key = new byte[32];
            _iv = new byte[_rcipher.BlockSize / 8]; //128 bit / 8 = 16 bytes
            _ivBytes = new byte[16];
        }
        public string DecryptStringAES(string cipherText, string iVector, string key)
        {
            //return EncryptDecrypt(cipherText, key, EncryptMode.DECRYPT, iVector);


            return DecryptString(cipherText, iVector,key);

            //char[] newchiptes = cipherText.ToCharArray()
            //return DecryptStringFromBytes(cipherText, key, iVector);


        }

        public static string DecryptString(string cipherText, string iVector, string key)
        {
            var decriptedFromJavascript = "";
            try
            {
                var keybytes = Encoding.UTF8.GetBytes(key);
                var iv = Encoding.UTF8.GetBytes(iVector);
                var encrypted = Convert.FromBase64String(cipherText);
                decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return decriptedFromJavascript;
        }
        private String EncryptDecrypt(string _inputText, string _encryptionKey, EncryptMode _mode, string _initVector)
        {
            string _out = ""; // output string
            //_encryptionKey = MD5Hash (_encryptionKey);
            _pwd = Encoding.UTF8.GetBytes(_encryptionKey);
            _ivBytes = Encoding.UTF8.GetBytes(_initVector);

            int len = _pwd.Length;
            if (len > _key.Length)
            {
                len = _key.Length;
            }
            int ivLenth = _ivBytes.Length;

            if (ivLenth > _iv.Length)
            {
                ivLenth = _iv.Length;
            }

            Array.Copy(_pwd, _key, len);
            Array.Copy(_ivBytes, _iv, ivLenth);
            _rcipher.Key = _key;
            _rcipher.IV = _iv;

            if (_mode.Equals(EncryptMode.ENCRYPT))
            {
                //encrypt
                byte[] plainText = _rcipher.CreateEncryptor()
                    .TransformFinalBlock(_enc.GetBytes(_inputText), 0, _inputText.Length);
                _out = Convert.ToBase64String(plainText);
            }
            if (_mode.Equals(EncryptMode.DECRYPT))
            {
                //decrypt
                byte[] plainText =
                    _rcipher.CreateDecryptor()
                        .TransformFinalBlock(Convert.FromBase64String(_inputText), 0,
                            Convert.FromBase64String(_inputText).Length);
                _out = _enc.GetString(plainText);
            }
            _rcipher.Dispose();
            return _out; // return encrypted/decrypted string
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }
            string plaintext = null;
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }
            return plaintext;
        }

        public string EncryptStringToBytes_Aes(string prm_text_to_encrypt, string prm_key, string prm_iv)
        {
            var sToEncrypt = prm_text_to_encrypt;

            var myRijndael = new RijndaelManaged()
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
                KeySize = 128,
                BlockSize = 128
            };

            var key = Encoding.ASCII.GetBytes(prm_key);
            var IV = Encoding.ASCII.GetBytes(prm_iv);

            var encryptor = myRijndael.CreateEncryptor(key, IV);

            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            var toEncrypt = Encoding.ASCII.GetBytes(sToEncrypt);

            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            var encrypted = msEncrypt.ToArray();

            return (Convert.ToBase64String(encrypted));

        }
        private enum EncryptMode
        {
            ENCRYPT,
            DECRYPT
        };
    }
}