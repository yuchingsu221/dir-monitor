using CommonLibrary.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Http
{
    public class HttpHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRequestUrl"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        //public static string GetApiResponseByMethod(string strRequestUrl, string method)
        //{
        //    string strTag = "GetApiResponseByMethod";
        //    string strResposne = string.Empty;

        //    try
        //    {
        //        LogUtility.LogInfo(strTag, $"Method: [{method}] | Url: {strRequestUrl}");
        //        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strRequestUrl);
        //        req.Method = method;
        //        req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

        //        using (WebResponse wr = req.GetResponse())
        //        {
        //            using (StreamReader myStreamReader = new StreamReader(
        //                wr.GetResponseStream(), Encoding.UTF8))
        //            {
        //                strResposne = myStreamReader.ReadToEnd();
        //            }
        //            wr.Close();
        //        }

        //    }
        //    catch (Exception ex) when (ex is Exception)
        //    {
        //        LogUtility.LogError(strTag, ex);
        //        throw ex;
        //    }

        //    LogUtility.LogInfo(strTag, $"Url: {strRequestUrl} | Out: {strResposne}");
        //    return strResposne;
        //}

        //private static string UrlConverter(string oriUrl)
        //{
        //    if (oriUrl.Contains("+")) { oriUrl.Replace("+", "%2B"); };
        //    if (oriUrl.Contains(" ")) { oriUrl.Replace(" ", "%20"); };
        //    if (oriUrl.Contains("/")) { oriUrl.Replace("/", "%2F"); };
        //    if (oriUrl.Contains("?")) { oriUrl.Replace("?", "%3F"); };
        //    if (oriUrl.Contains("%")) { oriUrl.Replace("%", "%25"); };
        //    if (oriUrl.Contains("#")) { oriUrl.Replace("#", "%23"); };
        //    if (oriUrl.Contains("&")) { oriUrl.Replace("&", "%26"); };
        //    if (oriUrl.Contains("=")) { oriUrl.Replace("=", "%3D"); };
        //    var newUrl = oriUrl;
        //    return newUrl;
        //}

        /// <summary>
        /// HTTP POST JSON FORMAT
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string PostJson(string url, string data, Dictionary<string, string> headers = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };

                // 加入header
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }


                var response = httpClient.SendAsync(request).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("HTTP POST JSON Error! reason:\r\n", ex);
                throw ex;
            }
        }

        // http post application/x-www-form-urlencoded
        public static string GetApiResponse(string strRequestUrl, NameValueCollection getParam)
        {
            try
            {
                var param = new List<KeyValuePair<string, string>>();

                if (getParam != null)
                {
                    for (int i = 0; i < getParam.Count; i++)
                        param.Add(new KeyValuePair<string, string>(getParam.GetKey(i), getParam.Get(i)));
                }

                var request = new HttpRequestMessage(HttpMethod.Post, strRequestUrl)
                {
                    Content = new FormUrlEncodedContent(param)
                };

                var response = httpClient.SendAsync(request).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("HTTP POST FORM DATA Error! reason:\r\n", ex);
                throw ex;
            }

        }

        public static string GetXmlApiResponse(string strRequestUrl,string xmlStr)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient client = new HttpClient(clientHandler);

                StringContent content = new StringContent(xmlStr, Encoding.UTF8, "text/xml");
                var response = client.PostAsync(strRequestUrl, content).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("HTTP GET FORM DATA Error! reason:\r\n", ex);
                throw ex;
            }

        }

    }
}
