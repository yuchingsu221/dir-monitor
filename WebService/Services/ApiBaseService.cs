using WebService.Models.Defines;
using WebService.Models.AppResponse;
using CommonLibrary;

namespace WebService.Services
{
    public class ApiBaseService
    {
        /// <summary>
        /// 統一處理 reponse 格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static T BuildResponsePacket<T>(
            T content,
            ErrorCodeEnum errorCode = ErrorCodeEnum.SUCCESS_CODE)
            where T : BaseResponseModel<string>, new()
        {
            var error = ErrorDefine.GetErrorDefine(errorCode);

            if (errorCode == ErrorCodeEnum.SUCCESS_CODE)
                content.RtnCode = "0000";
            else
                content.RtnCode = ((int)error.ErrorCode).ToString();

            content.RtnMsg = error.ErrorMsg;

            return content;
        }
    }
}
