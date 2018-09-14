using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;

namespace Landi.FrameWorks
{
    public class Encrypt
    {
        #region MD5 计算
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <returns></returns>
        public static string MD5Encrypt(string encryptString)
        {
            string returnValue;
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                returnValue = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(encryptString))).Replace("-", "");
                md5.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }
        #endregion

        #region DES/3DES加解密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <param name="encryptKey">密匙（8位）</param>
        /// <returns></returns>
        public static byte[] DESEncrypt(byte[] byteEncrypt, byte[] byteKey)
        {
            if (byteKey.Length != 8)
            {
                throw new Exception("Key length error!");
            }

            byte[] result = new byte[0];
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Padding = PaddingMode.None;
                Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
                object obj = t.GetField("Encrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);
                MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
                ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { byteKey, CipherMode.ECB, null, 0, obj });
                result = desCrypt.TransformFinalBlock(byteEncrypt, 0, byteEncrypt.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">密文</param>
        /// <param name="decryptKey">密匙（8位）</param>
        /// <returns></returns>
        public static byte[] DESDecrypt(byte[] byteDecrypt, byte[] byteKey)
        {
            if (byteKey.Length != 8)
            {
                throw new Exception("Key length error!");
            }

            byte[] result = new byte[0];
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Padding = PaddingMode.None;
                Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
                object obj = t.GetField("Decrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);
                MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
                ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { byteKey, CipherMode.ECB, null, 0, obj });
                result = desCrypt.TransformFinalBlock(byteDecrypt, 0, byteDecrypt.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 3DES 加密
        /// </summary>
        /// <param name="encryptString">待加密密文</param>
        /// <param name="encryptKey">密匙(长度必须为24或者16位)</param>
        /// <returns></returns>
        public static byte[] DES3Encrypt(byte[] encryptString, byte[] encryptKey)
        {
            if (encryptKey.Length != 16 && encryptKey.Length != 24)
            {
                throw new Exception("Key length error!");
            }

            byte[] encryptKey1 = new byte[8];
            byte[] encryptKey2 = new byte[8];
            byte[] encryptKey3 = new byte[8];
            if (encryptKey.Length == 16)
            {
                Array.Copy(encryptKey, encryptKey1, 8);
                Array.Copy(encryptKey, 8, encryptKey2, 0, 8);
                Array.Copy(encryptKey, encryptKey3, 8);
            }
            else if (encryptKey.Length == 24)
            {
                Array.Copy(encryptKey, encryptKey1, 8);
                Array.Copy(encryptKey, 8, encryptKey2, 0, 8);
                Array.Copy(encryptKey, 16, encryptKey3,0, 8);
            }

            byte[] returnValue;
            try
            {
                returnValue = DESEncrypt(encryptString, encryptKey1);
                returnValue = DESDecrypt(returnValue, encryptKey2);
                returnValue = DESEncrypt(returnValue, encryptKey3);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;

        }

        /// <summary>
        /// 3DES 解密
        /// </summary>
        /// <param name="decryptString">待解密密文</param>
        /// <param name="decryptKey">密匙(长度必须为24或者16位)</param>
        /// <returns></returns>
        public static byte[] DES3Decrypt(byte[] decryptString, byte[] decryptKey)
        {
            if (decryptKey.Length != 16 && decryptKey.Length != 24)
            {
                throw new Exception("Key length error!");
            }

            byte[] decryptKey1 = new byte[8];
            byte[] decryptKey2 = new byte[8];
            byte[] decryptKey3 = new byte[8];
            if (decryptKey.Length == 16)
            {
                Array.Copy(decryptKey, decryptKey1, 8);
                Array.Copy(decryptKey, 8, decryptKey2, 0, 8);
                Array.Copy(decryptKey, decryptKey3, 8);
            }
            else if (decryptKey.Length == 24)
            {
                Array.Copy(decryptKey, decryptKey1, 8);
                Array.Copy(decryptKey, 8, decryptKey2, 0, 8);
                Array.Copy(decryptKey, 16, decryptKey3, 0, 8);
            }

            byte[] returnValue;
            try
            {
                returnValue = DESDecrypt(decryptString, decryptKey3);
                returnValue = DESEncrypt(returnValue, decryptKey2);
                returnValue = DESDecrypt(returnValue, decryptKey1);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

        /// <summary>
        /// DES加密(字符串)
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <param name="encryptKey">密匙（8位）</param>
        /// <returns></returns>
        public static string DESEncrypt(string encryptString, string encryptKey)
        {
            if (encryptKey.Length != 8)
            {
                throw new Exception("Key length error!");
            }

            byte[] byteEncrypt = Encoding.Default.GetBytes(encryptString);
            byte[] bytekey = Encoding.Default.GetBytes(encryptKey);
            byte[] byteRet = DESEncrypt(byteEncrypt, bytekey);
            return Convert.ToBase64String(byteRet);
        }

        /// <summary>
        /// DES解密(字符串)
        /// </summary>
        /// <param name="decryptString">密文</param>
        /// <param name="decryptKey">密匙（8位）</param>
        /// <returns></returns>
        public static string DESDecrypt(string decryptString, string decryptKey)
        {
            if (decryptKey.Length != 8)
            {
                throw new Exception("Key length error!");
            }

            byte[] byteDecrypt = Convert.FromBase64String(decryptString);
            byte[] byteKey = Encoding.Default.GetBytes(decryptKey);
            byte[] byteRet = DESDecrypt(byteDecrypt, byteKey);
            return Convert.ToString(byteRet);
        }

        /// <summary>
        /// 3DES 加密(字符串)
        /// </summary>
        /// <param name="encryptString">待加密密文</param>
        /// <param name="encryptKey">密匙(长度必须为16/24位)</param>
        /// <returns></returns>
        public static string DES3Encrypt(string encryptString, string encryptKey)
        {
            if (encryptKey.Length != 16 && encryptKey.Length != 24)
            {
                throw new Exception("Key length error!");
            }

            byte[] byteEncrypt = Encoding.Default.GetBytes(encryptString);
            byte[] bytekey = Encoding.Default.GetBytes(encryptKey);
            byte[] byteRet = DES3Encrypt(byteEncrypt, bytekey);
            return Convert.ToBase64String(byteRet);
        }

        /// <summary>
        /// 3DES 解密(字符串)
        /// </summary>
        /// <param name="decryptString">待解密密文</param>
        /// <param name="decryptKey">密匙(长度必须为16/24位)</param>
        /// <returns></returns>
        public static string DES3Decrypt(string decryptString, string decryptKey)
        {
            if (decryptKey.Length != 16 && decryptKey.Length != 24)
            {
                throw new Exception("Key length error!");
            }

            byte[] byteDecrypt = Convert.FromBase64String(decryptString);
            byte[] byteKey = Encoding.Default.GetBytes(decryptKey);
            byte[] byteRet = DESDecrypt(byteDecrypt, byteKey);
            return Convert.ToString(byteRet);
        }
        
        #endregion

        #region RC2 加解密
        /// <summary>
        /// RC2加密
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <param name="encryptKey">密匙(必须为5-16位)</param>
        /// <returns></returns>
        public static string RC2Encrypt(string encryptString, string encryptKey)
        {
            string returnValue;
            try
            {
                byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
                byte[] byteEncryptString = Encoding.Default.GetBytes(encryptString);
                MemoryStream memorystream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memorystream, rc2.CreateEncryptor(Encoding.Default.GetBytes(encryptKey), temp), CryptoStreamMode.Write);
                cryptoStream.Write(byteEncryptString, 0, byteEncryptString.Length);
                cryptoStream.FlushFinalBlock();
                returnValue = Convert.ToBase64String(memorystream.ToArray());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;

        }

        /// <summary>
        /// RC2解密
        /// </summary>
        /// <param name="decryptString">密文</param>
        /// <param name="decryptKey">密匙(必须为5-16位)</param>
        /// <returns></returns>
        public static string RC2Decrypt(string decryptString, string decryptKey)
        {
            string returnValue;
            try
            {
                byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
                byte[] byteDecrytString = Convert.FromBase64String(decryptString);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, rc2.CreateDecryptor(Encoding.Default.GetBytes(decryptKey), temp), CryptoStreamMode.Write);
                cryptoStream.Write(byteDecrytString, 0, byteDecrytString.Length);
                cryptoStream.FlushFinalBlock();
                returnValue = Encoding.Default.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }
        #endregion

        #region AES加解密
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <param name="encryptKey">加密密匙</param>
        /// <returns></returns>
        public static string AESEncrypt(string encryptString, string encryptKey)
        {
            string returnValue;
            byte[] temp = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            Rijndael AESProvider = Rijndael.Create();
            try
            {
                byte[] byteEncryptString = Encoding.Default.GetBytes(encryptString);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, AESProvider.CreateEncryptor(Encoding.Default.GetBytes(encryptKey), temp), CryptoStreamMode.Write);
                cryptoStream.Write(byteEncryptString, 0, byteEncryptString.Length);
                cryptoStream.FlushFinalBlock();
                returnValue = Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnValue;

        }

        /// <summary>
        ///AES 解密
        /// </summary>
        /// <param name="decryptString">待解密密文</param>
        /// <param name="decryptKey">解密密钥</param>
        /// <returns></returns>
        public static string AESDecrypt(string decryptString, string decryptKey)
        {
            string returnValue = "";
            byte[] temp = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            Rijndael AESProvider = Rijndael.Create();
            try
            {
                byte[] byteDecryptString = Convert.FromBase64String(decryptString);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, AESProvider.CreateDecryptor(Encoding.Default.GetBytes(decryptKey), temp), CryptoStreamMode.Write);
                cryptoStream.Write(byteDecryptString, 0, byteDecryptString.Length);
                cryptoStream.FlushFinalBlock();
                returnValue = Encoding.Default.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

        #endregion

    }
}
