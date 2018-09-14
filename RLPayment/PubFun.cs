using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Landi.FrameWorks;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace RLPayment
{
    public class PubFun
    {
        /// <summary>
        /// 将结构体对象组包成json字符串
        /// </summary>
        /// <param name="stuObj"></param>
        /// <returns></returns>
        public static string GetJsonString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// 解析json字符串为对象
        /// </summary>
        /// <param name="transType"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T GetJsonObject<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }


        public static string Encrypt(string pToEncrypt)
        {
            return Encrypt(pToEncrypt, ConfigFile.ReadConfig("AppData", "DesKey"));
        }


        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中
            //原来使用的UTF8编码，我改成Unicode编码了，不行
            byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
            //byte[]  inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt); 

            //建立加密对象的密钥和偏移量
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法
            //使得输入密码必须输入英文文本
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            //创建其支持存储区为内存的流
            MemoryStream ms = new MemoryStream();
            //将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //Write  the  byte  array  into  the  crypto  stream 
            //(It  will  end  up  in  the  memory  stream) 
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            //用缓冲区的当前状态更新基础数据源或储存库，随后清除缓冲区
            cs.FlushFinalBlock();
            //Get  the  data  back  from  the  memory  stream,  and  into  a  string 
            byte[] EncryptData = (byte[]) ms.ToArray();
            return System.Convert.ToBase64String(EncryptData, 0, EncryptData.Length);
        }


        public static string Decrypt(string pToDecrypt)
        {
            return Decrypt(pToDecrypt, ConfigFile.ReadConfig("AppData", "DesKey"));
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns> 
        public static string Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //Put  the  input  string  into  the  byte  array 
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

            //建立加密对象的密钥和偏移量，此值重要，不能修改
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //Flush  the  data  through  the  crypto  stream  into  the  memory  stream 
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
