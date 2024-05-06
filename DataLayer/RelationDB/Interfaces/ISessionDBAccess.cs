using Domain.Models.Config;
using Domain.Models.RelationDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.RelationDB.Interfaces
{
    public interface ISessionDBAccess
    {
        public SessionModel GetSessionByToken(string tokens);
        public void CleanExpiredSessions(List<SessionModel> sessions);
        public void ExtendSessionExpiredTime(string token);
        public void CleanSession(string token);
        public List<SessionModel> GetExpiredSessions();
        public void ExtendTokenExpiredTime(string token);
    }
}
