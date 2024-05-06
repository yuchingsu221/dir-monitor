using CommonLibrary;
using Domain.UnitOfWork;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;


namespace WebService.Utility
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ILogger<UnitOfWorkFactory> _logger;
        public UnitOfWorkFactory()
        {

        }

        public IUnitOfWork Create(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                _logger.LogError("UnitOfWorkFactory | 資料庫連線建立失敗，連線字串不可為空");
                Guarder.Throw(ErrorCodeEnum.EXCUTE_ERR_CODE);
            }
            IDbConnection connection = new NpgsqlConnection(connectionName);

            return new UnitOfWork(connection);
        }
    }
}
