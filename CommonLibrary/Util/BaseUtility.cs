using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using HandlebarsDotNet;
using System.Drawing;
using System.Xml.Schema;
using System.Linq;
using Domain.Models;
using System.Globalization;
using CommonLibrary.Crypto;
using Microsoft.Win32;

namespace CommonLibrary.Util
{
    public class BaseUtility
    {
        /// <summary>
        /// 將集合切成多個集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(IEnumerable<T> source, int chunkSize = 1000)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }


        /// <summary>
        /// 轉換STRING 是否為NULL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime? ParseDateTime(string s)
        {
            if (s != null && !string.IsNullOrWhiteSpace(s))
            {
                return Convert.ToDateTime(s);
            }
            return null;
        }


        /// <summary>
        /// 轉換DateTime 是否為NULL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TimeToString(DateTime? dt)
        {
            if (dt != null)
            {
                return Convert.ToDateTime(dt).ToFull();
            }
            return null;
        }



        /// <summary>
        /// 隱碼員工名子(統一隱碼第二個字含中英)
        /// </summary>
        /// <param name="oldName"></param>
        /// <returns></returns>
        public static string NameSecure(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                switch (name.Length)
                {
                    case 1: break;
                    case 2:
                        name = name.Replace(name.Substring(1, 1), "*");
                        break;
                    default:
                        name = name.Replace(name.Substring(1, 2), "*" + name.Substring(2, 1));
                        break;
                }
            }
            return name;
        }       

        /// <summary>
        /// 轉換XML為Class Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strXML"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xml)
        {
            //XmlDocument xdoc = new XmlDocument();

            try
            {
                ///去除UTF-8 BOM
                byte[] bytes = Encoding.UTF8.GetBytes(xml);
                if (bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
                {
                    string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    xml = xml.Remove(0, byteOrderMarkUtf8.Length);
                }

                //xdoc.XmlResolver = null;
                //xdoc.LoadXml(xml);

                // Create the XmlSchemaSet class.
                XmlSchemaSet sc = new XmlSchemaSet();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(sc);
                settings.ValidationType = ValidationType.Schema;
                StringReader sr = new StringReader(xml);
                XmlReader reader = XmlReader.Create(sr, settings);

                //XmlNodeReader reader = new XmlNodeReader(xdoc.DocumentElement);
                XmlSerializer ser = new XmlSerializer(typeof(T));
                object obj = ser.Deserialize(reader);

                return (T)obj;
            }
            catch (Exception ex) when (ex is Exception)
            {
                return default;
            }
        }       

        /// <summary>
        /// 取得Guid字串
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public static string Render(string html, object arg)
        {
            var template = Handlebars.Compile(html);
            var result = template(arg);
            return result;
        }

        public static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static Stream GetJsonStream(string path, string registryPath, string registryName)
        {
            try
            {
                var registryDefaultValue = new { };
                var rsaPrivateKey = Registry.GetValue(registryPath, registryName, registryDefaultValue).ToString();

                var content = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(rsaPrivateKey))
                    return new MemoryStream();

                // 固定做解密，若解密失敗
                // 將RSA KEY 讀出來
                //var rsaKey = File.ReadAllText($"{Directory.GetCurrentDirectory()}/cert/rsakey.xml");
                var result = RSAHelper.DecryptByBouncyCastle(content, rsaPrivateKey);

                //result = "{\"option1\": 1, \"option2\": \"abc\"}";
                var byteResult = Encoding.UTF8.GetBytes(result.TrimEnd());
                var stream = new MemoryStream(byteResult);

                return stream;
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("讀取JSON字串失敗", ex);
                return new MemoryStream();
            }
        }

        public static string ReplaceString(string str)
        {
            string replacedStr = str.Replace(',', '?').Replace('“', '|').Replace("/", "^").Replace("_", "‘_’").Replace("//", "^^");

            return replacedStr;
        }     
    }

    public static class DateTimeExtensions
    {
        public static string ToFull(this DateTime dt)
            => dt.ToString("yyyy/MM/dd HH:mm:ss");
        public static string ToDate(this DateTime dt)
            => dt.ToString("yyyy/MM/dd");
        public static string ToMinute(this DateTime dt)
            => dt.ToString("yyyy/MM/dd HH:mm");
        public static string ToDateNoSymbol(this DateTime dt)
            => dt.ToString("yyyyMMdd");
        public static string ToMinuteNoSymbol(this DateTime dt)
            => dt.ToString("yyyyMMddHHmm");
        public static string ToPincodeFormat(this DateTime dt)
            => dt.ToString("yyyyMMddHHmmss");
        public static int TotalSec(this DateTime dt)
            => Convert.ToInt32(Math.Abs(Math.Round((DateTime.Now - dt).TotalSeconds)));

        public static DateTime TkDateToDateTime(this string dateString)
            => dateString.ToDateTimeByFormat("yyyyMMdd");
        private static DateTime ToDateTimeByFormat(this string dateString, string format)
            => Convert.ToDateTime(DateTime.ParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture));

        /// <summary>
        /// 國家標準時間（英語：National Standard Time，縮寫：NST），亦被稱為臺灣時間、臺北時間
        /// 舊稱中原標準時間（英語：Chungyuan Standard Time，縮寫：CST）
        /// </summary>
        /// <returns></returns>
        public static DateTime NstNow { get => DateTime.UtcNow.AddHours(8); }
        //public static DateTime ToNst(this DateTime dt)
        //    => dt.AddHours(8);

    }
}
