using Microsoft.EntityFrameworkCore.Storage;
using Models.Models.Context;
using System.Data;
using System;
using System.Threading.Tasks;

namespace Domain.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        IDbConnection _connection = null;
        IDbTransaction _transaction = null;

        IDbConnection IUnitOfWork.Connection => _connection;

        IDbTransaction IUnitOfWork.Transaction => _transaction;

        public UnitOfWork(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void Begin()
        {
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        public void Dispose()
        {
        }
    }
}
