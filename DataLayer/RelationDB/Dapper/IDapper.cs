using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.RelationDB
{
    public interface IDapper
    {
        public List<T> Query<T>(string command, DynamicParameters param = null, int? commandTimeout = 30);
        public long Excute<T>(string command, DynamicParameters param, int? commandTimeout = 30);
        public long Excute(string command, DynamicParameters parameters, int? commandTimeout = 10);
        public long InsertMulti<T>(List<T> param, int? commandTimeout = 30);
        public long Insert<T>(T param, int? commandTimeout = 30);
        public bool UpdateMulti<T>(List<T> param, int? commandTimeout = 30);
        public bool Update<T>(T param, int? commandTimeout = 30);
        public bool DeleteMulti<T>(List<T> param, int? commandTimeout = 30);
        public bool Delete<T>(T param, int? commandTimeout = 30);

        public T Get<T>(object id, string tableName = "", int? commandTimeout = 30) where T : class;
        public List<T> GetAll<T>(string tableName = "", int? commandTimeout = 30) where T : class;

        public int ExcuteSP(string spName, DynamicParameters param, int? commandTimeout = 30);

        public List<T> QuerySP<T>(string spName, object param, int? commandTimeout = 30) where T : class;

        public T InsertAndGetId<T>(string command, object param, int? commandTimeout = 30);

        //public T UpdateAndGetValue<T>(string command, object param, int? commandTimeout = 30);

        public string InsertMulti(Dictionary<DynamicParameters, string> param, int? commandTimeout = 10);
    }
}
