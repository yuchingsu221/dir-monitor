using Domain.UnitOfWork;

namespace WebService.Utility
{
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// 建立目標資料庫的連線
        /// </summary>
        /// <param name="connectionName">欲建立的連線設定名稱</param>
        /// <returns></returns>
        IUnitOfWork Create(string connectionName);
    }
}
