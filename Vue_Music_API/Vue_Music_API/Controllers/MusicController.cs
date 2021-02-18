using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Com.Unit.API;
using Com.Unit.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using Vue_Music_API.Controllers;
using Vue_Music_API.Method;

namespace VueproAPI.Controllers
{
    public class MusicController : BaseController<TB_Music>
    {
        ErrorMethod error = new ErrorMethod();
        InforMethod infor = new InforMethod();
        [EnableCors("any")]
        //  [Route("[controller]/[Action]")]
        [Route("api/[Action]")]
        //[Authorize(Policy = "MustAdmin")]
        [Authorize]
        [HttpPost]
        public string GetMusicTypeName(string typeName)
        {
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "";
            try
            {
                string TypeId = "";
                try
                {
                    infor.WriteInforLog("根据类型查询音乐", "GetMusicTypeName()", "TB_Categorys");
                    var _types = db.Queryable<TB_Categorys>().Where(it => it.TypeName.Contains(typeName)).Select(it => it.TypeId);
                    infor.WriteInforLog($"执行的SQL语句为{_types.ToSql()}", "GetMusicTypeName()", "TB_Categorys");
                    TypeId= _types.First();
                }
                catch(Exception e)
                {
                    code = '"' + "500" + '"';
                    msg = '"' + "error" + '"';
                    message = '"' + '"' + "";
                    error.WriteErrorLog(e, "GetMusicTypeName()", "TB_Categorys");
                }
                infor.WriteInforLog("根据音乐类型ID查询对应的音乐", "GetMusicTypeName()", "TB_Music");
                var musicNameJson= _sugarTable.Where(it => it.M_Type == TypeId).Distinct();
                infor.WriteInforLog($"执行的SQL语句为{musicNameJson.ToSql()}", "GetMusicTypeName()", "TB_Music");
                message = musicNameJson.ToJson();
                infor.WriteInforLog($"根据音乐类型ID查询对应的音乐的总值：{musicNameJson.Count()}", "GetMusicTypeName()", "TB_Music");
                total = '"' + $"{musicNameJson.Count()}" + '"';               
            }
            catch(Exception e)
            {               
                code = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + '"' + "";
                error.WriteErrorLog(e, "GetMusicTypeName()", "TB_Music");
            }

            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
             + "msg" + '"' + $":{msg}," + '"'
             + "data" + '"' + $":{message}," + '"' + "Total" + '"' + $":{total}}}]";
        }

        /// <summary>
        /// 根据音乐的热度进行排序
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        [HttpPost]
        public string GetMusicHotName()
        {
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "";          
            try
            {
                infor.WriteInforLog("取前十条数据", "GetMusicHotName()", "TB_Music");
                var hotName = _sugarTable.OrderBy(it => it.M_Hot, OrderByType.Desc).Distinct().Take(10);
                infor.WriteInforLog($"执行的SQL语句为{hotName.ToSql()}", "GetMusicHotName()", "TB_Music");
                message = hotName.ToJson();
                total = '"' + $"10" + '"';
            }
            catch(Exception e)
            {
                code = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + '"' + "";
                error.WriteErrorLog(e, "GetMusicTypeName()", "TB_Music");
            }
            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
                   + "msg" + '"' + $":{msg}," + '"'
                   + "data" 
                   + '"' + $":{message}," + '"' + "Total" + '"' + $":{total}}}]";
        }
        /// <summary>
        /// 搜索音乐
        /// </summary>
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        [HttpPost]
        public string SeachMusicName(string seachName,string seachType)
        {
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "", name="";
            try
            {
                infor.WriteInforLog("根据搜索信息(在分类表中查询)查询音乐", "SeachMusicName()", "TB_Categorys");
                var category = db.Queryable<TB_Categorys>().Where(it => it.TypeName.Contains(seachName))
                                                 .Select(it => it.TypeId);
                infor.WriteInforLog($"执行的SQL语句为{category.ToSql()}", "SeachMusicName()", "TB_Categorys");
                name=category.First();
            }
            catch(Exception e)
            {
                code = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + '"' + "";
                error.WriteErrorLog(e, "GetMusicTypeName()", "TB_Music");
            }
            try
            {
                //infor.WriteInforLog("根据搜索信息查询音乐并且按照热度排序", "SeachMusicName()", "TB_Music");
                //var seracMusicList = _sugarTable.Where(it => it.M_Author.Contains(seachName)
                //            || it.M_Name.Contains(seachName) || it.M_Words.Contains(seachName)
                //            || it.M_Id.Contains(seachName) || it.M_Type.Contains(name))
                //           .OrderBy(it=>it.M_Hot,OrderByType.Desc).Distinct();
                //infor.WriteInforLog($"执行的SQL语句为{seracMusicList.ToSql()}", "SeachMusicName()", "TB_Music");
                //message = seracMusicList.ToJson();
                //infor.WriteInforLog($"一共查询{seracMusicList.Count()}条数据", "SeachMusicName()", "TB_Music");
                //total = '"' + $"{seracMusicList.Count()}" + '"';
                if (seachType.Contains("first"))
                {
                    infor.WriteInforLog("根据搜索信息查询音乐并且按照热度排序", "SeachMusicName()", "TB_Music");
                    var seracMusicList = _sugarTable.Where(it =>it.M_Name.Contains(seachName)||it.M_Type.Contains(name))
                        .OrderBy(it=>it.M_Hot,OrderByType.Desc).Distinct();
                    infor.WriteInforLog($"执行的SQL语句为{seracMusicList.ToSql()}", "SeachMusicName()", "TB_Music");
                    message = seracMusicList.ToJson();
                    infor.WriteInforLog($"一共查询{seracMusicList.Count()}条数据", "SeachMusicName()", "TB_Music");
                    total = '"' + $"{seracMusicList.Count()}" + '"';
                }
                else if (seachType.Contains("second"))
                {
                    infor.WriteInforLog("根据搜索信息查询歌手并且按照热度排序", "SeachMusicName()", "TB_Music");
                    var seracMusicList = _sugarTable.Where(it => it.M_Author.Contains(seachName))
                        .OrderBy(it => it.M_Hot, OrderByType.Desc).Distinct();
                    infor.WriteInforLog($"执行的SQL语句为{seracMusicList.ToSql()}", "SeachMusicName()", "TB_Music");
                    message = seracMusicList.ToJson();
                    infor.WriteInforLog($"一共查询{seracMusicList.Count()}条数据", "SeachMusicName()", "TB_Music");
                    total = '"' + $"{seracMusicList.Count()}" + '"';
                }
                else if(seachType.Contains("third"))
                {
                    infor.WriteInforLog("根据搜索信息查询歌手并且按照热度排序", "SeachMusicName()", "TB_Music");
                    var seracMusicList = _sugarTable.Where(it => it.M_Words.Contains(seachName))
                        .OrderBy(it => it.M_Hot, OrderByType.Desc).Distinct();
                    infor.WriteInforLog($"执行的SQL语句为{seracMusicList.ToSql()}", "SeachMusicName()", "TB_Music");
                    message = seracMusicList.ToJson();
                    infor.WriteInforLog($"一共查询{seracMusicList.Count()}条数据", "SeachMusicName()", "TB_Music");
                    total = '"' + $"{seracMusicList.Count()}" + '"';
                }
                else
                {
                    infor.WriteInforLog("根据搜索信息查询用户并且按照热度排序", "SeachMusicName()", "TB_User");
                    var seracMusicList = db.Queryable<TB_User>().Where(it => it.U_Name.Contains(seachName))
                        .OrderBy(it => it.U_Regtime, OrderByType.Desc).Distinct();
                    infor.WriteInforLog($"执行的SQL语句为{seracMusicList.ToSql()}", "SeachMusicName()", "TB_Music");
                    message = seracMusicList.ToJson();
                    infor.WriteInforLog($"一共查询{seracMusicList.Count()}条数据", "SeachMusicName()", "TB_Music");
                    total = '"' + $"{seracMusicList.Count()}" + '"';
                }
            }
            catch(Exception e)
            {
                code = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + '"' + "";
                error.WriteErrorLog(e, "SeachMusicName()", "TB_Music");
            }

            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
                 + "msg" + '"' + $":{msg}," + '"'
                 + "data" + '"' + $":{message}," + '"' + "Total" + '"' + $":{total}}}]";
        }
    }
}