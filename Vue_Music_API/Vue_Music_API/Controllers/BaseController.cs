using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Unit.API;
using Com.Unit.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Vue_Music_API.Method;

namespace Vue_Music_API.Controllers
{
    [EnableCors("any")]
    [Route("[controller]/[Action]")]
    public class BaseController<T> : ControllerBase
    {
        public static SqlSugarClient db = DBSugar.MySqlInstance();
        public ISugarQueryable<T> _sugarTable { get { return db.Queryable<T>(); } }             
    }
}