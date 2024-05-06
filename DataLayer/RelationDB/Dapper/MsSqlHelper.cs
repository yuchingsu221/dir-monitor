using CommonLibrary.Util;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace DataLayer.RelationDB
{
    public class MsSqlHelper : IDapper
    {
        private readonly string CONN_STRING = string.Empty;

        public MsSqlHelper(string connectionString)
        {
            CONN_STRING = connectionString;
        }

        // 自己寫sql處理select
        public List<T> Query<T>(string command, DynamicParameters param = null, int? commandTimeout = 10)
        {
            var result = new List<T>();
            var retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    result = conn.Query<T>(command, param, null, true, commandTimeout).ToList();

                    if (result == null || result.Count == 0)
                    {
                        result = new List<T>();
                    }

                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                        LogUtility.LogDBError(command, logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return result;
        }

        public long Excute(string command, DynamicParameters parameters, int? commandTimeout = 10)
        {
            long count = 0;
            var retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    count = conn.Execute(command, parameters, null, commandTimeout);
                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = parameters != null ? JsonConvert.SerializeObject(parameters, Formatting.None) : null;
                        LogUtility.LogDBError(command, logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return count;
        }

        // 自己寫sql處理 insert/ update/ delete
        public long Excute<T>(string command, DynamicParameters param, int? commandTimeout = 10)
        {
            long count = 0;
            var retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    count = conn.Execute(command, param, null, commandTimeout);
                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        //var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                        LogUtility.LogDBError(command, null, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return count;
        }

        // 新增資料, 使用dapper延伸套件
        public long InsertMulti<T>(List<T> param, int? commandTimeout = 10)
        {
            long count = 0;

            //var conn = new SqlConnection(CONN_STRING);
           
            //if (conn.State == ConnectionState.Closed)
            //{
            //    conn.Open();
            //}

            using (SqlConnection conn = new SqlConnection(CONN_STRING))
            {
                //if (conn.State == ConnectionState.Closed)
                //{
                //    conn.Open();
                //}

                var transaction = conn.BeginTransaction();

                try
                {
                    count = conn.Insert(param, transaction, commandTimeout);
                    transaction.Commit();
                }
                catch (Exception ex) when (ex is Exception)
                {
                    var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                    LogUtility.LogDBError($"INSERT TABLE {param.FirstOrDefault().ToString()}", logParam, ex);
                    transaction.Rollback();
                }
            }
            //var transaction = conn.BeginTransaction();

            //try
            //{
            //    count = conn.Insert(param, transaction, commandTimeout);
            //    transaction.Commit();
            //}
            //catch (Exception ex) when (ex is Exception)
            //{
            //    var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
            //    LogUtility.LogDBError($"INSERT TABLE {param.FirstOrDefault().ToString()}", logParam, ex);
            //    transaction.Rollback();
            //}
            //finally
            //{
            //    transaction.Dispose();
            //    conn.Close();
            //    conn.Dispose();
            //}

            return count;
        }

        public long Insert<T>(T param, int? commandTimeout = 10)
        {
            long count = 0;
            var retry = 0;

            while (retry < 3)
            {
                var singleParam = new List<T> { param };
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    count = conn.Insert(singleParam, null, commandTimeout);
                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                        LogUtility.LogDBError($"INSERT TABLE {param}", logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return count;
        }

        // 編輯資料, 使用dapper延伸套件
        public bool UpdateMulti<T>(List<T> param, int? commandTimeout = 10)
        {
            var success = false;
            var retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    success = conn.Update(param, null, commandTimeout);

                    retry = 3;
                    success = true;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                        LogUtility.LogDBError($"UPDATE TABLE {param.FirstOrDefault().ToString()}", logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return success;
        }

        // 編輯資料, 使用dapper延伸套件
        public bool Update<T>(T param, int? commandTimeout = 10)
        {
            var success = false;
            var retry = 0;

            while (retry < 3)
            {
                var singleParam = new List<T> { param };
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    success = conn.Update(singleParam, null, commandTimeout);

                    retry = 3;
                    success = true;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                        LogUtility.LogDBError($"UPDATE TABLE {param}", logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return success;
        }

        // 刪除資料, 使用dapper延伸套件
        public bool DeleteMulti<T>(List<T> param, int? commandTimeout = 10)
        {
            var success = false;
            var retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    success = conn.Delete(param, null, commandTimeout);

                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                        LogUtility.LogDBError($"DELETE TABLE {param.FirstOrDefault().ToString()}", logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return success;
        }

        // 刪除資料, 使用dapper延伸套件
        public bool Delete<T>(T param, int? commandTimeout = 10)
        {
            var success = false;
            var retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);
                var singleParam = new List<T> { param };

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    success = conn.Delete(singleParam, null, commandTimeout);
                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = param != null ? JsonConvert.SerializeObject(param, Formatting.None) : null;
                        LogUtility.LogDBError($"DELETE TABLE {param}", logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return success;
        }

        // 根據id取回資料, 資料為唯一
        public T Get<T>(object id, string tableName = "", int? commandTimeout = 10) where T : class
        {
            T result = default;
            int retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    result = conn.Get<T>(id, null, commandTimeout);

                    if (result == null)
                    {
                        result = default;
                    }

                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        var logParam = id != null ? JsonConvert.SerializeObject(id, Formatting.None) : null;
                        LogUtility.LogDBError($"SELECT TABLE {tableName}", logParam, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return result;
        }

        // 取得資料表的所有資料.
        public List<T> GetAll<T>(string tableName = "", int? commandTimeout = 10) where T : class
        {
            List<T> result = new List<T>();
            int retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    result = conn.GetAll<T>(null, commandTimeout).ToList();

                    if (result == null)
                    {
                        result = new List<T>();
                    }

                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        LogUtility.LogDBError($"SELECT TABLE {tableName}", null, ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return result;
        }

        public int ExcuteSP(string spName, DynamicParameters param, int? commandTimeout = 10)
        {
            int affectRows = 0;
            int retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    affectRows = conn.Execute(spName, param, null, commandTimeout, commandType: CommandType.StoredProcedure);

                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        LogUtility.LogDBError($"EXCUTE SP: {spName}", JsonConvert.SerializeObject(param, Formatting.None), ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return affectRows;
        }

        public List<T> QuerySP<T>(string spName, object param, int? commandTimeout = 10) where T : class
        {
            List<T> result = new List<T>();
            int retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);
                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // // conn.Open();

                    result = conn.Query<T>(
                        spName,
                        param,
                        null,
                        true,
                        commandTimeout,
                        commandType: CommandType.StoredProcedure).ToList();


                    if (result == null)
                    {
                        result = new List<T>();
                    }

                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        LogUtility.LogDBError($"QUERY SP: {spName}", JsonConvert.SerializeObject(param, Formatting.None), ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return result;
        }

        public T InsertAndGetId<T>(string command, object param, int? commandTimeout = 10)
        {
            T result = default;
            int retry = 0;

            while (retry < 3)
            {
                var conn = new SqlConnection(CONN_STRING);

                try
                {
                    //if (conn.State == ConnectionState.Closed)
                    // conn.Open();

                    result = conn.QuerySingle<T>(command, param, null, commandTimeout);

                    retry = 3;
                }
                catch (Exception ex) when (ex is Exception)
                {
                    Thread.Sleep(1);
                    retry++;

                    if (retry >= 3)
                    {
                        LogUtility.LogDBError($"INSERT : {command}", JsonConvert.SerializeObject(param, Formatting.None), ex);

                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return result;
        }

        //public T UpdateAndGetValue<T>(string command, object param, int? commandTimeout = 10)
        //{
        //    T result = default;
        //    int retry = 0;

        //    while (retry < 3)
        //    {
        //        var conn = new SqlConnection(CONN_STRING);

        //        try
        //        {
        //            result = conn.QuerySingle<T>(command, param, null, commandTimeout);

        //            retry = 3;
        //        }
        //        catch (Exception ex) when (ex is Exception)
        //        {
        //            Thread.Sleep(1);
        //            retry++;

        //            if (retry >= 3)
        //            {
        //                LogUtility.LogDBError($"Update : {command}", JsonConvert.SerializeObject(param, Formatting.None), ex);

        //                throw;
        //            }
        //        }
        //        finally
        //        {
        //            conn.Close();
        //            conn.Dispose();
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// 對多張不同Table做異動
        /// </summary>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public string InsertMulti(Dictionary<DynamicParameters, string> param, int? commandTimeout = 10)
        {
            string result = "";
            string cmdStr = "";

            using (SqlConnection conn = new SqlConnection(CONN_STRING))
            {
                try
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (KeyValuePair<DynamicParameters, string> kvp in param)
                            {
                                cmdStr = $"SQL Script: {kvp.Value}\n Parameters: {JsonConvert.SerializeObject(kvp.Key)}\n";
                                conn.Execute(kvp.Value, kvp.Key, transaction, commandTimeout);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex) when (ex is Exception)
                        {
                            LogUtility.LogDBError(cmdStr, null, ex);
                            result = cmdStr;
                            transaction.Rollback();
                        }
                        finally
                        {
                            close(transaction);
                        }
                    }
                }
                catch (Exception ex) when (ex is Exception)
                {
                    LogUtility.LogDBError(cmdStr, null, ex);
                }
                finally
                {
                    close(conn);
                }
            }

            return result;
        }

        public void close(SqlConnection conn)
        {
            if (conn != null)
            {
                try
                {
                    conn.Close();
                }
                catch (SqlException e)
                {
                    LogUtility.LogError("SqlConnection Close Error", e);
                }
            }
        }

        public void close(SqlTransaction transaction)
        {
            if (transaction != null)
            {
                try
                {
                    transaction.Dispose();
                }
                catch (SqlException e)
                {
                    LogUtility.LogError("SqlTransaction Dispose Error", e);
                }
            }
        }
    }
}

