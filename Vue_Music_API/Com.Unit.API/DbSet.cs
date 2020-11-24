using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unit.API
{
    public class DbSet<T> : SimpleClient<T> where T : class, new()
    {
        public DbSet(ISqlSugarClient context) : base(context)
        {
        }
        //定义自己的方法
        public ISugarQueryable<T> sugarTable()
        {
            return Context.Queryable<T>();
        }

    }
}
