using CommonLibrary;
using CommonLibrary.Util;
using DataLayer.RelationDB.Interfaces;
using DataLayer.Repository.Interfaces;
using Domain.Models.RelationDB;
using System;
using System.Linq;
using WebService.Services.Interfaces;

namespace WebService.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionDBAccess _SessionDBAccess;
        private readonly ILoginDBAccess _LoginDBAccess;

        public SessionService(ISessionDBAccess sessionDBAccess, ILoginDBAccess loginDBAccess)
        {
            _SessionDBAccess = sessionDBAccess;
            _LoginDBAccess = loginDBAccess;
        }

        public SessionModel CheckSession(string token, bool checkDeviceBinding = true)
        {
            var session = _SessionDBAccess.GetSessionByToken(token);

            // 如果不為空, 延長時間(不影響後續流程
            if (session != null)
            {
                if (session.Status == 0)
                {
                    // 1037
                    Guarder.Throw(ErrorCodeEnum.SESSION_FORCE_LOGOUT);
                }

                _SessionDBAccess.ExtendSessionExpiredTime(token);
                var CUSTSession = new CUST_SessionModel
                {
                    SESSIONID = session.Token,
                    LASTACTIONTIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    XID = session.CustId
                };

                //// 多補, 更新B2C_SESSION的資料
                //if (!_LoginDBAccess.UpdateB2CSessionActionTime(b2cSession))
                //{
                //    // 1037
                //    Guarder.Throw(ErrorCodeEnum.SESSION_FORCE_LOGOUT);
                //}
            }
            else
            {
                // 1016
                Guarder.Throw(ErrorCodeEnum.SESSION_NOT_FOUND_ERR_CODE);
            }


            //if (checkDeviceBinding)
            //{
            //    var userDevicebinding = _LoginDBAccess.GetUserDeviceBindingByCustId(session.CustId).FirstOrDefault();

            //    if (userDevicebinding == null)
            //    {
            //        Guarder.Throw(ErrorCodeEnum.CMS_UNBIND_DEVICE_ERR_CODE);
            //    }
            //}


            return session;
        }

        public void CleanSession(string token)
        {
            LogUtility.LogInfo($"清除Session，Token : {token}");
            _SessionDBAccess.CleanSession(token);
        }

        public void CleanExpiredSession()
        {
            // 先撈出所有逾時的資料
            var sessions = _SessionDBAccess.GetExpiredSessions();

            var chunkSessions = BaseUtility.ChunkBy(sessions).ToList();

            foreach (var session in chunkSessions)
            {
                var custIds = session.Select(x => x.CustId).ToList();
                // 刪除過期的Session
                _SessionDBAccess.CleanExpiredSessions(session.ToList());

                //// 撈出所有逾時的userAccess
                //var userAccessList = _LoginDBAccess.GetUserAccessByCustIds(custIds);
                //// 更新所有的UserAccess SignOn Flag 
                //_LoginDBAccess.UpdateAllUserAccessSignOnFlag(userAccessList, "N");
            }
        }
    }
}
