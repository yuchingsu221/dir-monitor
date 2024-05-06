using CommonLibrary.Crypto;
using CommonLibrary.Util;
using Domain.Models.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.Models.AppResponse;
using WebService.Models.Defines;
using WebService.Services;
using WebService.Models.AppResponse;
using CommonLibrary;

namespace WebService.Filters
{
    public class ResourceFilter : IResourceFilter
    {
        private static readonly object _CountLocker = new object();
        private static int INCounter = 0;
        private static WebServiceSetting _Setting;

        public ResourceFilter(WebServiceSetting setting)
        {
            _Setting = setting;
        }

        /// <summary>
        /// Request
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // TODO: 延長token時效
            var request = context.HttpContext.Request;
            var controller = context.RouteData.Values["Controller"].ToString();
            var action = context.RouteData.Values["Action"].ToString();
            var tag = $"{controller}.{action}";
            var requestContent = string.Empty;
            var requestNo = 0;

            // 記錄此次編號
            lock (_CountLocker)
            {
                INCounter++;
                requestNo = INCounter;
            }

            // ******** 開始判斷
            if (!_Setting.EnableEncrypt)
            {
                switch (request.Method.ToUpper())
                {
                    case "POST":
                        // 讀取Body 
                        request.EnableBuffering();
                        using (var stream = new StreamReader(stream: request.Body,
                                                     encoding: Encoding.UTF8,
                                                     detectEncodingFromByteOrderMarks: false,
                                                     bufferSize: 1024,
                                                     leaveOpen: true))
                        {
                            requestContent = stream.ReadToEndAsync().GetAwaiter().GetResult();
                        }
                        // 將資料放回 body
                        var bytes = Encoding.UTF8.GetBytes(requestContent);
                        request.Body = new MemoryStream(bytes);

                        break;

                    case "GET":
                        requestContent = request.QueryString.Value; // 直接將查詢字串放到
                        break;
                    default:
                        break;
                }

                try
                {
                    // 紀錄Log
                    if (!string.IsNullOrWhiteSpace(requestContent))
                    {
                        if (request.Method.ToUpper() == "POST")
                            requestContent = JObject.Parse(requestContent).ToString(Formatting.None);

                        if (!action.Equals("UploadDoc", StringComparison.OrdinalIgnoreCase))
                            LogUtility.LogInfo($"[IN={requestNo}]\r\nPA={requestContent}", tag);
                    }
                }
                catch (Exception ex) when (ex is Exception)
                {
                    LogUtility.LogError($"請求: {requestContent} 嘗試轉換 JSON 失敗", ex, tag);

                    PacketErrorResult(
                        context,
                        ErrorCodeEnum.EXCUTE_ERR_CODE,
                        requestNo,
                        "",
                        tag);

                    return;
                }
                context.HttpContext.Request.Headers.Add("OU", requestNo.ToString());

                return;
            }
            LogUtility.LogInfo($"[Request Header ={JsonConvert.SerializeObject(request.Headers)}", tag);

            // ***************
            // 走加密流程
            // ***************
            // 如果是 取得加密API，紀錄明文
            if (tag.Equals("Global.Key", StringComparison.OrdinalIgnoreCase))
            {
                // 讀取資料
                request.EnableBuffering();
                using (var stream = new StreamReader(stream: request.Body,
                                                     encoding: Encoding.UTF8,
                                                     detectEncodingFromByteOrderMarks: false,
                                                     bufferSize: 1024,
                                                     leaveOpen: true))
                {
                    requestContent = stream.ReadToEndAsync().GetAwaiter().GetResult();
                }
                // 將解密成功的資料放回 body
                var bytes = Encoding.UTF8.GetBytes(requestContent);
                request.Body = new MemoryStream(bytes);

                LogUtility.LogInfo($"[IN={requestNo}]\r\nPA={requestContent}", tag);

                context.HttpContext.Request.Headers.Add("OU", requestNo.ToString());
                return;
            }

            // 先決定好, Response的加密用哪種編號(隨機), 若下面邏輯都OK, 再改回原來的aId
            var aesKeyValue = CryptoDefine.GetRandomKeyValue();
            context.HttpContext.Response.Headers.Add("CRYPTO-ID", aesKeyValue.AId);
            context.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "crypto-id");

            // header檢查, 是否有帶上隨機的KeyID
            if (!request.Headers.TryGetValue("CRYPTO-ID", out StringValues cryptoId))
            {
                PacketErrorResult(
                    context,
                    ErrorCodeEnum.PARAMETER_ERR_CODE,
                    requestNo,
                    aesKeyValue.AKey,
                    tag);
                return;
            }

            // 在檢查是否可以根據id取得加密資料
            var requestAesKeyValue = CryptoDefine.GetKeyValue(cryptoId.ToString());

            if (requestAesKeyValue == null)
            {
                PacketErrorResult(
                    context,
                    ErrorCodeEnum.PARAMETER_ERR_CODE,
                    requestNo,
                    aesKeyValue.AKey,
                    tag);
                return;
            }

            switch (request.Method.ToUpper())
            {
                case "POST":
                    // 讀取加密資料
                    request.EnableBuffering();

                    using (var stream = new StreamReader(stream: request.Body,
                                                      encoding: Encoding.UTF8,
                                                      detectEncodingFromByteOrderMarks: false,
                                                      bufferSize: 1024,
                                                      leaveOpen: true))
                    {
                        requestContent = stream.ReadToEndAsync().GetAwaiter().GetResult();
                    }
                    try
                    {
                        // 解密資料
                        requestContent = AESHelper.AES256GcmDecrypt(
                            requestContent,
                            requestAesKeyValue.AKey,
                            CryptoDefine.A_IV);
                    }
                    catch (Exception ex) when (ex is Exception)
                    {
                        LogUtility.LogError("讀取Body失敗", ex);

                        PacketErrorResult(
                            context,
                            ErrorCodeEnum.EXCUTE_ERR_CODE,
                            requestNo,
                            aesKeyValue.AKey,
                            tag);

                        return;
                    }

                    // 將解密成功的資料放回 body
                    var bytes = Encoding.UTF8.GetBytes(requestContent);
                    request.Body = new MemoryStream(bytes);
                    break;

                case "GET":
                    requestContent = request.QueryString.Value; // 直接將查詢字串放到
                    break;
                default:
                    break;
            }

            try
            {
                // 紀錄Log
                if (!string.IsNullOrWhiteSpace(requestContent))
                {
                    if (request.Method.ToUpper() == "POST")
                        requestContent = JObject.Parse(requestContent).ToString(Formatting.None);

                    if (!action.Equals("UploadDoc", StringComparison.OrdinalIgnoreCase))
                        LogUtility.LogInfo($"[IN={requestNo}]\r\nPA={requestContent}", tag);
                }
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError($"請求: {requestContent} 嘗試轉換 JSON 失敗", ex, tag);

                PacketErrorResult(
                    context,
                    ErrorCodeEnum.EXCUTE_ERR_CODE,
                    requestNo,
                    aesKeyValue.AKey,
                    tag);

                return;
            }

            context.HttpContext.Request.Headers.Add("OU", requestNo.ToString());
            context.HttpContext.Response.Headers.Remove("CRYPTO-ID");
            context.HttpContext.Response.Headers.Add("CRYPTO-ID", requestAesKeyValue.AId);
        }

        /// <summary>
        /// Response
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        /// <summary>
        /// 直接組錯誤訊息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestNo"></param>
        /// <param name="aesKey"></param>
        /// <param name="tag"></param>
        private void PacketErrorResult(ResourceExecutingContext context, ErrorCodeEnum errorCode, int requestNo, string aesKey, string tag)
        {
            var result = ApiBaseService.BuildResponsePacket(new BaseResponseModel<string>(), errorCode);
            var response = JsonConvert.SerializeObject(result);

            if (!_Setting.EnableEncrypt)
            {
                context.Result = new ContentResult
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };

            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = AESHelper.AES256GcmEncrypt(response, aesKey, CryptoDefine.A_IV),
                    ContentType = "text/plain",
                    StatusCode = 200
                };
            }

            LogUtility.LogInfo($"[OU={requestNo}]\r\nRS={response}", tag);
        }
    }
}
