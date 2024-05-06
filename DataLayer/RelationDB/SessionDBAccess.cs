using CommonLibrary.Util;
using Dapper;
using DataLayer.RelationDB.Interfaces;
using Domain.Models.Config;
using Domain.Models.RelationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.RelationDB
{
    public class SessionDBAccess : ISessionDBAccess
    {
        private readonly WebServiceSetting _Settings;

        public SessionDBAccess(WebServiceSetting settings)
        {
            _Settings = settings;
        }

        /// <summary>
        /// 通過token取得的 session資料
        /// </summary>
        /// <returns></returns>
        public SessionModel GetSessionByToken(string token)
        {
            IDapper dapper = new MsSqlHelper(_Settings.RelationDB.DIR_ConnectionString);
            var command =
                "SELECT Token, Data, ExpiredTime, CustId, [Status], [Region]   FROM session " +
                "WHERE token=@_Token ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("_Token", token);
       
            return dapper.Query<SessionModel>(command, parameters).FirstOrDefault();
        }

        public List<SessionModel> GetExpiredSessions()
        {
            IDapper dapper = new MsSqlHelper(_Settings.RelationDB.DIR_ConnectionString);
            var command = "SELECT Token, Data, ExpiredTime, CustId, [Status] FROM [Session] " +
                   "WHERE expiredTime <= @_ExpiredTime ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("_ExpiredTime", DateTime.Now);

            return dapper.Query<SessionModel>(command, parameters);
        }

        public void CleanExpiredSessions(List<SessionModel> sessions)
        {
            try
            {
                IDapper dapper = new MsSqlHelper(_Settings.RelationDB.DIR_ConnectionString);
                var command = "DELETE FROM [Session] " +
                    "WHERE token in @_Tokens";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("_Tokens", sessions.Select(x => x.Token).ToList());
                

                dapper.Excute(command, parameters);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogInfo("CleanExpiredSessions 異常");
            }
        }

        public void ExtendSessionExpiredTime(string token)
        {
            try
            {
                IDapper dapper = new MsSqlHelper(_Settings.RelationDB.DIR_ConnectionString);
                var command = "UPDATE [Session] SET " +
                    "expiredTime = @_NewTime " +
                    "WHERE token = @_Token ";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("_NewTime", DateTime.Now.AddSeconds(_Settings.SessionExpireDuration));
                parameters.Add("_Token", token);

                dapper.Excute(command, parameters);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogInfo("ExtendSessionExpiredTime 異常");
            }
        }        

        public void CleanSession(string token)
        {
            try
            {
                IDapper dapper = new MsSqlHelper(_Settings.RelationDB.DIR_ConnectionString);
                var command = "DELETE FROM [Session] " +
                               "WHERE token = @_Token ";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("_Token", token);

                dapper.Excute(command, parameters);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogInfo("CleanSession 異常");
            }
        }

        public void ExtendTokenExpiredTime(string token)
        {
            try
            {
                IDapper dapper = new MsSqlHelper(_Settings.RelationDB.DIR_ConnectionString);
                var command = "UPDATE [Session] SET " +
                    "expiredTime = @_NewTime " +
                    "WHERE token = @_Token ";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("_NewTime", DateTime.Now.AddSeconds(_Settings.TokenExpireDuration));
                parameters.Add("_Token", token);

                dapper.Excute(command, parameters);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogInfo("ExtendTokenExpiredTime 異常");
            }
        }
    }
}
