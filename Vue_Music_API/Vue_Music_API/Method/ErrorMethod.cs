using Com.Unit.API;
using Com.Unit.Model;
using log4net;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vue_Music_API.Controllers;

namespace Vue_Music_API.Method
{
    public class ErrorMethod
    {      
        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");//这里的 logerror 和 log4net.config 里的名字要一样
        public void WriteErrorLog(Exception ex,string method,string errorTable)
        {
            try
            {
                
                SqlSugarClient db = DBSugar.MySqlInstance();
                TB_Error_log error = new TB_Error_log();
                error.ErrorID = Guid.NewGuid().ToString("D");
                error.ErrorMessage = ex.Message;
                error.ErrorMethod = method;
                error.ErrorTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                error.ErrorTable = errorTable;
                db.Saveable<TB_Error_log>(error).ExecuteCommand();               
                logerror.Error("异常为:" + ex.Message + "========================方法是：" + method + "========================数据表为：" + errorTable);
            }
            
            catch(Exception e)
            {
                logerror.Error("异常为:"+e.Message+"========================方法是："+method+ "========================数据表为：" + errorTable);
                
            }

        }
    }
}
