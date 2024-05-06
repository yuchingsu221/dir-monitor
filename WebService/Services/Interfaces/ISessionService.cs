using Domain.Models.RelationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Services.Interfaces
{
    public interface ISessionService
    {
        public void CleanExpiredSession();

        public void CleanSession(string token);

        public SessionModel CheckSession(string token, bool checkDeviceBinding = true);

    }
}
