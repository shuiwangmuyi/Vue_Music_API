using Com.Unit.API;
using Com.Unit.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vue_Music_API.Method
{
    
    public class InforMethod
    {
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("infolog");
        ErrorMethod error = new ErrorMethod();
        public void WriteInforLog(string message,string method, string errorTable)
        {
            loginfo.Error("方法是：" + method + "========================数据表为：" + errorTable);         
            try
            {
                SqlSugarClient db = DBSugar.MySqlInstance();
                TB_Info_log info_Log = new TB_Info_log();
                info_Log.InfoID = Guid.NewGuid().ToString("D");
                info_Log.InfoMessage = message;
                 info_Log.InfoMethod = method;
                info_Log.InfoTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                info_Log.InfoTable = errorTable;
                db.Saveable<TB_Info_log>(info_Log).ExecuteCommand();
            }
            catch (Exception ex)
            {
                error.WriteErrorLog(ex, method, errorTable);
            }
           
        }
    }
}
