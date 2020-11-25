using Com.Unit.Model;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vue_Music_API.Method;

namespace Vue_Music_API.Controllers
{
    public class MusicTypeController :  BaseController<TB_Categorys>
    {
        ErrorMethod error = new ErrorMethod();
        InforMethod infor = new InforMethod();
        /// <summary>
        ///   进入首页，音乐分类
        /// </summary>
        /// <returns></returns>    
        [HttpPost]
        public string GetMusicTypeData()
        {
           
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "";            
            try
            {
                infor.WriteInforLog("查询音乐分类", "GetMusicTypeData()", "TB_Categorys");
                var music = _sugarTable.Where(it => it.TypeClass == "TB_Music");
                infor.WriteInforLog($"执行的SQL语句为{music.ToSql()}", "GetMusicTypeData()", "TB_Categorys");
                message = music.ToJson();
                infor.WriteInforLog($"获取音乐分类的总值：{music.Count()}", "GetMusicTypeData()", "TB_Categorys");
                total = '"' + $"{music.Count()}" + '"';          
            }
            catch(Exception ex) 
            {
                error.WriteErrorLog(ex, "GetMusicTypeData()", "TB_Music");     
                code = '"' + "500" + '"';
                msg = '"'+ "error" + '"' ;
                message = '"' + '"'+"";               
            }
            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
             + "msg" + '"' + $":{msg}," + '"'
             + "data" + '"' + $":{message},"+'"'+"Total"+'"'+$":{total}}}]";
        }


    }
}
