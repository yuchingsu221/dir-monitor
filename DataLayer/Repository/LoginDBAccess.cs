using Dapper;
using DataLayer.Repository.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebService.Models.DBModels;
using WebService.Utility;

namespace DataLayer.Repository
{
    public class LoginDBAccess : ILoginDBAccess
    {
        private readonly IDbConnection _connection;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public LoginDBAccess(IDbConnection connection, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _connection = connection;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<MemberDto> QueryMemberInfoByAccount(string account)
        {
            using (var _unitOfWork = _unitOfWorkFactory.Create(_connection.ConnectionString))
            {
                var sql = @"
                            SELECT 
                                [Content]
                            FROM [DealerXml] WITH(NOLOCK)
                            WHERE [IsDelete] = 0
                            ";

                var param = new DynamicParameters();
                var res = await _unitOfWork.Connection.QueryAsync<MemberDto>(sql, param);

                return res.ToList()[0];
            }
        }

        public async Task<string> TestConnect()
        {
            using (var _unitOfWork = _unitOfWorkFactory.Create(_connection.ConnectionString))
            {
                var sql = @"
                           select 1 
                            ";

                var param = new DynamicParameters();
                var res = await _unitOfWork.Connection.QueryAsync<string>(sql, param);

                return res.ToList()[0];
            }
        }



        //public Task<MemberDto> QueryMemberInfoByAccount(string account)
        //{
        //    IDapper dapper = new MsSqlHelper(_Settings.RelationDB.DIR_ConnectionString);
        //    DynamicParameters bag = new DynamicParameters();
        //    var cmd = new StringBuilder();
        //    cmd.AppendLine(@"select * from account WHERE (1=1) and isDeleted = 0");
        //    cmd.AppendLine("and accountId = @_AccountId");
        //    bag.Add("_AccountId", account);

        //    return dapper.Query<MemberDto>(cmd.ToString(), bag);
        //}
    }
}
