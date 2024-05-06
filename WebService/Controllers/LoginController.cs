using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebService.Models.AppRequest;
using WebService.Models.AppResponse;
using WebService.Services.Interfaces;

namespace WebService.Controllers
{
    [ApiController]
    public class LoginController : BaseController
    {
        private readonly ILoginService _LoginService;
        public LoginController(ILoginService loginService)
        {
            _LoginService = loginService;
        }

        /// <summary>
        /// 登入/取得 JWT token
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 驗證google recaptcha token是否合法
        /// 2. 根據request account，進入帳號主表<see cref="AuthorizationAccount"/> 取得資料
        /// 3. 比對table 中加密的密碼是否一致
        /// 4. 根據role id 取得授權function，加入至jwt token
        /// ### 必要篩選條件
        /// account 標記刪除為 false
        /// </remarks>
        /// <response code="422">
        /// Unprocessable entity <br/>
        /// Error Code: <br/>  
        /// 0000, 0041, 0042
        /// </response>
        /// <param name="request">登入請求</param>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<LoginRsModel> Login([FromBody, Required] BackendLoginRequest request)
            => await _LoginService.Login(request);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="rqModel"></param>
        /// <returns></returns>
        [HttpPost("Logout")]
        public string Logout([FromBody][Bind("Token")] LogoutRqModel rqModel)
        => _LoginService.Logout(rqModel);

        [HttpPost("TestConnect")]
        public async Task<bool> TestConnect()
            => await _LoginService.TestConnect();
    }
}
