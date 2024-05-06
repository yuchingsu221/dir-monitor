using com.sun.javadoc;
using Domain.Models.RelationDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebService.Models.DBModels;

namespace DataLayer.Repository.Interfaces
{
    public interface ILoginDBAccess
    {
        /// <summary>
        /// 查詢會員資料
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<MemberDto> QueryMemberInfoByAccount(string account);

        Task<string> TestConnect();
    }
}