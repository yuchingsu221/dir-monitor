using CommonLibrary;
using DataLayer.Repository.Interfaces;
using Domain.Models.Config;
using Domain.UnitOfWork;
using Org.BouncyCastle.Ocsp;
using System.Threading.Tasks;
using WebService.Models.AppRequest;
using WebService.Models.AppResponse;
using WebService.Models.Defines;
using WebService.Services.Interfaces;

namespace WebService.Services
{
    public class LoginService : ILoginService
    {
        private readonly WebServiceSetting _Settings;
        private readonly ILoginDBAccess _LoginDBAccess;
        private readonly ISessionService _SessionService;
        private readonly JwtHelper _JwtHelper;
        private readonly IUnitOfWork _UnitOfWork;

        public LoginService(
            WebServiceSetting settings,
            ILoginDBAccess loginDBAccess,
            ISessionService sessionService,
            JwtHelper jwtHelper,
            IUnitOfWork unitOfWork)
        {
            _Settings = settings;
            _LoginDBAccess = loginDBAccess;
            _SessionService = sessionService;
            _JwtHelper = jwtHelper;
            _UnitOfWork = unitOfWork;
        }

        public async Task<LoginRsModel> Login(BackendLoginRequest request)
        {
            //request.Password = RSAHelper.Decrypt(_Settings.EncryptKeys.RSA.Private,request.Password);
            // 帳號
            //var account = await _UnitOfWork.AuthorizationAccount.FirstOrDefaultAsync(a => a.Account == request.Account && !a.Deleted);
            if ("yuchingsu221" != request.Account)
            {
                Guarder.Throw(ErrorCodeEnum.LOGIN_FAIL_ERR_CODE);
            }
            //bool passwordError = request.Password != account.Password;
            if ("850221" != request.Password)
            {
                Guarder.Throw(ErrorCodeEnum.LOGIN_FAIL_ERR_CODE);
            }

            // 角色
            // 授權功能


            //查出帳號資訊
            var test = await _LoginDBAccess.TestConnect();




            var result = new LoginRsModel { Token = _JwtHelper.GenerateToken(0, request.Account) };

            return result;
        }

        public string Logout(LogoutRqModel rqModel)
        {
            var session = _SessionService.CheckSession(rqModel.Token, false);

            if (session == null)
            {
                Guarder.Throw(ErrorCodeEnum.SESSION_NOT_FOUND_ERR_CODE);
            }

           // var userAccess = _LoginDBAccess.GetUserAccessByCustId(session.CustId).FirstOrDefault();

            // 清除 Session
            _SessionService.CleanSession(rqModel.Token);

            //// 設定 B2C_UserAccess
            //userAccess.SIGNONFLAG = "N";
            //_LoginDBAccess.UpdateUserAccessSignOnFlag(userAccess);

            //try
            //{
            //    var b2cSession = new B2C_SessionModel
            //    {
            //        SESSIONID = session.Token,
            //        LOGOFFTIME = DateTime.Now,
            //        LASTACTIONTIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            //        XID = userAccess.XID
            //    };

            //    _LoginDBAccess.UpdateB2CSessionActionTimeAndLogOffTime(b2cSession);
            //}
            //catch (Exception ex) when (ex is Exception)
            //{
            //    LogUtility.LogError("新增或更新B2C_SESSION失敗.", ex);
            //}

            return string.Empty;
        }

        public BaseResponseModel<string> GetLoginStatus(GetLoginStatusRqModel rqModel)
        {
            var session = _SessionService.CheckSession(rqModel.Token);
            if (session == null)
            {
                Guarder.Throw(ErrorCodeEnum.SESSION_NOT_FOUND_ERR_CODE);
            }
            if (session.Status == 0) // Session被強制踢出
            {
                Guarder.Throw(ErrorCodeEnum.SESSION_FORCE_LOGOUT);
            }
            return new BaseResponseModel<string> { };
        }

        public async Task<bool> TestConnect()
        {
            var test = await _LoginDBAccess.TestConnect();

            if (test == null)
            {
                return false;
            }

            return true;
        }
    }
}