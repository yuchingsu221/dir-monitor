using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebService.Models.AppRequest;
using WebService.Models.AppResponse;

namespace WebService.Services.Interfaces
{
    public interface ILoginService
    {
        Task<LoginRsModel> Login(BackendLoginRequest request);
        public string Logout(LogoutRqModel rqModel);
        public Task<bool> TestConnect();
    }
}
