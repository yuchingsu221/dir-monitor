using CommonLibrary.Crypto;
using CommonLibrary.Util;
using Domain.Models.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using WebService.Models.AppResponse;
using WebService.Models.Defines;
using CommonLibrary;

namespace WebService.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {

        private static WebServiceSetting _Setting;

        public ExceptionFilter(WebServiceSetting setting)
        {
            _Setting = setting;
        }

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            var errInfo = ex.Message;
            ErrorDefine errors;

            if (ex is CustomExceptionHandler)
            {
                var errorInfo = errInfo.Split("||");
                errors = ErrorDefine.GetErrorDefine((ErrorCodeEnum)int.Parse(errorInfo[0]));

                if (!string.IsNullOrWhiteSpace(errorInfo[1]))
                {
                    errors.ErrorMsg = errorInfo[1];
                }

            }
            else
            {
                errors = ErrorDefine.GetErrorDefine(ErrorCodeEnum.EXCUTE_ERR_CODE);
                LogUtility.LogError("發生錯誤\r\n", ex);
            }

            var request = context.HttpContext.Request;
            var requestNo = request.Headers["OU"].ToString();
            var response = context.HttpContext.Response;
            var aesKeyId = response.Headers["CRYPTO-ID"].ToString();
            var controller = context.RouteData.Values["Controller"].ToString();
            var action = context.RouteData.Values["Action"].ToString();
            var tag = $"{controller}.{action}";
            var aesKeyValue = CryptoDefine.GetKeyValue(aesKeyId);

            // 設定回應資訊
            var result = new BaseResponseModel<string>
            {
                RtnCode = ((int)errors.ErrorCode).ToString(),
                RtnMsg = errors.ErrorMsg
            };
            var bussinessResult = JsonConvert.SerializeObject(result);
            LogUtility.LogInfo($"[OU={requestNo}]\r\nRS={bussinessResult}", tag);

            if (tag.Equals("Global.Key", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ContentResult
                {
                    Content = bussinessResult,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                return;
            }

            if (!_Setting.EnableEncrypt)
            {
                context.Result = new ContentResult
                {
                    Content = bussinessResult,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                return;
            }

            context.Result = new ContentResult
            {
                Content = AESHelper.AES256GcmEncrypt(bussinessResult, aesKeyValue.AKey, CryptoDefine.A_IV),
                ContentType = "text/plain",
                StatusCode = 200
            };

        }
    }
}
