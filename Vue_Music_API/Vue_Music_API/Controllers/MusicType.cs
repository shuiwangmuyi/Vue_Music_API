using Com.Unit.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vue_Music_API.Controllers
{
    public class MusicType : BaseController<TB_Categorys>
    {
        /// <summary>
        ///   一进入首页就发送音乐分类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetMusicTypeData()
        {            
            string message = "", code = "200", msg = "OK";
            try
            {
                message = _sugarTable.Where(it => it.TypeClass == "TB_Music").ToJson();
            }
            catch(Exception ex) 
            {
                code = '"' + "500" + '"';
                msg = '"'+ "error" + '"' ;
                message = '"' + '"'+"";
            }
            return $"["+'"'+"code"+'"'+$":{code},"+'"'
                +"msg"+'"'+$":{msg},"+'"'
                +"data"+'"'+$":{message}]";
        }
    }
}
