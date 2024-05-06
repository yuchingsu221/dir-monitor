using System;


namespace CommonLibrary
{
    public class Guarder
    {
        public static void Throw(ErrorCodeEnum errorCode, string customErrorMsg = "", Exception ex = null)
        {
            if (ex == null) // 如果是客製化錯誤訊息
                throw new CustomExceptionHandler(errorCode, customErrorMsg);
            else // 一般錯誤攔截
                throw ex;
        }
    }
}
