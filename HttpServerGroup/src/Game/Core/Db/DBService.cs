using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Db
{
    class DBService
    {
        public static DBService Instance = new DBService();

        private ConnectionStringSettings connMySqlAuthUserStr;
        private ConnectionStringSettings connMySqlGameDataStr;
        private ConnectionStringSettings connPostgreSqlAuthUserStr;
        private ConnectionStringSettings connPostgreSqlGameDataStr;

        // 使用 SqlSugarScope 替代每次创建新实例
        private static SqlSugarScope gameDbForMySql;
        private static SqlSugarScope authDbForMySql;
        private static SqlSugarScope gameDbForPSql;
        private static SqlSugarScope authDbForPSql;


        public void Init()
        {
            connMySqlAuthUserStr = ConfigurationManager.ConnectionStrings["connMySqlAccountStr"];
            connMySqlGameDataStr = ConfigurationManager.ConnectionStrings["connMySqlGameDataStr"];
            connPostgreSqlAuthUserStr = ConfigurationManager.ConnectionStrings["connPostgreSqlAccountStr"];
            connPostgreSqlGameDataStr = ConfigurationManager.ConnectionStrings["connPostgreSqlGameDataStr"];

            // 初始化 GameDB Scope
            gameDbForMySql = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connMySqlGameDataStr.ConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            },
            db =>
            {
                // 添加Sql打印事件
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //Console.WriteLine(sql);
                };
                db.Aop.OnError = (exp) =>
                {
                    //Console.WriteLine($"[PostgreDB] Error: {exp.Message}");
                    //Console.WriteLine($"[PostgreDB] StackTrace: {exp.StackTrace}");
                };
            });

            // 初始化 AuthDB Scope
            authDbForMySql = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connMySqlAuthUserStr.ConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            },
            db =>
            {
                // 添加Sql打印事件
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //Console.WriteLine(sql);
                };
            });


            gameDbForPSql = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connPostgreSqlGameDataStr.ConnectionString,
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            },
            db =>
            {
                // 添加Sql打印事件
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //Console.WriteLine(sql);
                };
            });

            authDbForPSql = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connPostgreSqlAuthUserStr.ConnectionString,
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            },
            db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //Console.WriteLine($"[PostgreDB] SQL: {sql}");
                };

                db.Aop.OnError = (exp) =>
                {
                    //Console.WriteLine($"[PostgreDB] Error: {exp.Message}");
                    //Console.WriteLine($"[PostgreDB] StackTrace: {exp.StackTrace}");
                };
            });
        }

        public SqlSugarScope GetGameInstance()
        {
            return gameDbForMySql = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connMySqlGameDataStr.ConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            },
            db =>
            {
                // 添加Sql打印事件
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //Console.WriteLine(sql);
                };
                db.Aop.OnError = (exp) =>
                {
                    //Console.WriteLine($"[PostgreDB] Error: {exp.Message}");
                    //Console.WriteLine($"[PostgreDB] StackTrace: {exp.StackTrace}");
                };
            }); ;
        }

        public SqlSugarScope GetAuthInstance()
        {
            return authDbForMySql;
        }
    }
}


