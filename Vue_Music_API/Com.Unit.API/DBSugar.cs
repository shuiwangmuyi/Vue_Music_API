using SqlSugar;
using System;
using System.Linq;

namespace Com.Unit.API
{
    public class DBSugar
    {
        public static SqlSugarClient db;
        public static string conStr { get; set; }
        public static SqlSugarClient MySqlInstance(/*string ConnectionStringsName*/)
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = conStr,
                DbType = DbType.MySql, //必填
                IsAutoCloseConnection = true,//默认false)是否自动释放数据库，设为true我们不需要close或者Using的操作
                InitKeyType = InitKeyType.Attribute
            });
            put(db);
            return db;
        }
        public static void put(SqlSugarClient db)
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
        }
        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
}
