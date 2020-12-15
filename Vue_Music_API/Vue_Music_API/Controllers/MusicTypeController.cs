using Com.Unit.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
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
                error.WriteErrorLog(ex, "GetMusicTypeData()", "TB_Categorys");     
                code = '"' + "500" + '"';
                msg = '"'+ "error" + '"' ;
                message = '"' + '"'+"";               
            }
            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
             + "msg" + '"' + $":{msg}," + '"'
             + "data" + '"' + $":{message},"+'"'+"Total"+'"'+$":{total}}}]";
        }


        [HttpPost]
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        public string GetMusictypeSubnav()
        {
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "";
            message = "[{" + '"' + "id" + '"' + ":" + '"' + "10001" + '"' + "," + '"' + "discover" + '"' + ":" + '"' + "推荐" + '"' + "," + '"' + "path" + '"' + ':' + '"' + "/find" + '"' + "}," +
                "{" + '"' + "id" + '"' + ":" + '"' + "10002" + '"' + "," + '"' + "discover" + '"' + ":" + '"' + "排行榜" + '"' + "," + '"' + "path" + '"' + ':' + '"' + "/topList" + '"' + "},"
                + "{" + '"' + "id" + '"' + ":" + '"' + "10003" + '"' + "," + '"' + "discover" + '"' + ":" + '"' + "歌单" + '"' + "," + '"' + "path" + '"' + ':' + '"' + "/playList" + '"' + "},"
                + "{" + '"' + "id" + '"' + ":" + '"' + "10004" + '"' + "," + '"' + "discover" + '"' + ":" + '"' + "歌手" + '"' + "," + '"' + "path" + '"' + ':' + '"' + "/artist" + '"' + "}]";

            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
             + "msg" + '"' + $":{msg}," + '"'
             + "data" + '"' + $":{message}," + '"' + "Total" + '"' + $":{0}}}]";
        }
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        [HttpPost]
        public string GetMusicSingerId(string SingerClass)
        {
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "";
            try
            {
                var DB = db.Queryable<TB_SINGER>().Where(it => it.SingerClass.Contains(SingerClass))
                    .OrderBy(it=>it.SingeRanking,OrderByType.Asc);
                message=DB.ToJson();
                total = '"' + $"{DB.Count()}" + '"';            }
            catch(Exception ex)
            {
                error.WriteErrorLog(ex, "GetMusicSingerId()", "TB_SINGER");
                code = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + '"' + "";
            }
            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
                         + "msg" + '"' + $":{msg}," + '"'
                         + "data" + '"' + $":{message}," + '"' + "Total" + '"' + $":{total}}}]";
        }

        [HttpPost]
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        public string GetNameId(string id)
        {
            id = id as string;
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "";
            switch (id)
            {
                case "10001":                   
                  
                    break;
                case "10002":
                    infor.WriteInforLog("查询排行榜", "GetNameId()", "TB_Music");
                    var DB = db.Queryable<TB_Music>().OrderBy(it => it.M_Hot, OrderByType.Desc)
                        .OrderBy(it=>it.M_Time,OrderByType.Desc)
                        .Distinct().Take(100);
                    infor.WriteInforLog($"执行的SQL语句为{DB.ToSql()}", "GetNameId()", "TB_Music");
                    message = DB.ToJson();
                    total = '"' + $"{DB.Count()}" + '"';
                    break;
                case "10003":

                    break;
                case "10004":
                    infor.WriteInforLog("查询歌手排行榜,并查询不为空的数据", "GetNameId()", "TB_Music");
                    var DB1 = db.Queryable<TB_SINGER>()
                        .Select(it=>new {
                            // it.SingerId,
                            it.SingerClass
                    }).Where(it=> SqlFunc.HasValue(it.SingerClass)&&!(it.SingerClass.Contains("热门")))
                        .OrderBy(it=>it.SingerClass,OrderByType.Asc).Distinct();
                    infor.WriteInforLog($"执行的SQL语句为{DB1.ToSql()}", "GetNameId()", "TB_Music");
                    message = DB1.ToJson();
                    total = '"' + $"{DB1.Count()}" + '"';
                    break;
            }
            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
            + "msg" + '"' + $":{msg}," + '"'
            + "data" + '"' + $":{message}," + '"' + "Total" + '"' + $":{total}}}]";
        }
    }
}
