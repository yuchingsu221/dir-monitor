using CommonLibrary;
using System.Collections.Generic;
using System.Linq;

namespace WebService.Models.Defines
{
    public class ErrorDefine
    {
        public ErrorCodeEnum ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        public static ErrorDefine GetErrorDefine(ErrorCodeEnum errorCode)
        {
            var errInfo = ErrorCodeAndMsgDefine.ERROR_MAPS.Where(x => x.ErrorCode == errorCode).FirstOrDefault();

            if (errInfo != null)
            {
                var errCode = errInfo.ErrorCode;
                var errMsg = errInfo.ErrorMsg;

                return new ErrorDefine
                {
                    ErrorCode = errCode,
                    ErrorMsg = errMsg
                };
            }

            return new ErrorDefine { ErrorCode = errorCode };
        }
    }

    public class ErrorCodeAndMsgDefine
    {
        public static readonly List<ErrorDefine> ERROR_MAPS = new List<ErrorDefine>
        {
            #region 總類
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.SUCCESS_CODE,
                ErrorMsg = "成功"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.PARAMETER_ERR_CODE,
                ErrorMsg = "參數錯誤，請檢查請求是否正確。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.EXCUTE_ERR_CODE,
                ErrorMsg = "系統忙線中，請稍後再試。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.EXCUTE_ERR_CODE,
                ErrorMsg = "系統忙線中，請稍後再試。"
            },

            #endregion     

            #region 登入
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.LOGIN_FAIL_ERR_CODE,
                ErrorMsg =  "依行內政策禁止行員代他人登入"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.NOT_MEMBER_ERR_CODE,
                ErrorMsg =  "你尚未加入會員。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.FIRST_LOGIN_ERR_CODE,
                ErrorMsg = "您第一次登入，將由「加入會員」功能來啟用密碼及修改您的基本資料。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.MIMA_IS_NOT_CHANGE_WITHIN_30DAYS_ERR_CODE,
                ErrorMsg = "您於申請後30天內未變更密碼,請與客服聯絡,以重新申請密碼。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.MIMA_NOT_BEEN_ACTIVE_ERR_CODE,
                ErrorMsg = "尚未重設啟用密碼，將由「忘記密碼」功能來啟用。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.HAS_NOT_CHANGED_MIMA_FOR_12MONTHS_ERR_CODE,
                ErrorMsg = "您超過12個月未變更密碼，您必須修改密碼後才能啟用。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.MIMA_HAS_BEEN_WRONG_FIVE_TIMES_ERR_CODE,
                ErrorMsg =  "您密碼已錯誤五次，為確保安全，請與客服聯絡，以重新申請密碼。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.USERNAME_MIMA_CANNOT_NULL_ERR_CODE,
                ErrorMsg =  "使用者代碼與使用者密碼不得為空值。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.USERNAME_WRONG_ERR_CODE,
                ErrorMsg = "帳號錯誤(第\"{0}\"次)，請重新簽入。(注意 : 英文字母部份大小寫視為不同)"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.MIMA_WRONG_ERR_CODE,
                ErrorMsg = "密碼錯誤(第\"{0}\"次)，請重新簽入。(注意 : 英文字母部份大小寫視為不同)"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.USER_DUPLICATE_LOGIN_ERR_CODE,
                ErrorMsg =  "使用者重覆簽入，請關閉瀏覽器視窗再行簽入。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.SESSION_NOT_FOUND_ERR_CODE,
                ErrorMsg = "登入逾時，請重新登入。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.LOGIN_FAIL_ERR_CODE,
                ErrorMsg = "登入失敗，請重新登入。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.USER_CODE_NOT_NULL_ERR_CODE,
                ErrorMsg = "使用者代碼不可為空白。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.USER_CODE_REGULAR_ERR_CODE,
                ErrorMsg =  "新使用者代碼需符合 6-20字數的英數字混合，不可包含空白與各種符號"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.USER_CODE_ORI_NEW_SAME_ERR_CODE,
                ErrorMsg = "新舊使用者代碼不可相同"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.USER_CODE_NOT_EQUAL_MIMA_ERR_CODE,
                ErrorMsg = "新使用者代碼不可與密碼相同"
            },
            #endregion           
        };
    }
}