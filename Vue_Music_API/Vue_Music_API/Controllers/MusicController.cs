using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Com.Unit.API;
using Com.Unit.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using Vue_Music_API.Controllers;

namespace VueproAPI.Controllers
{
    // [Route("api/[controller]")]
    // [ApiController]    
    public class MusicController : BaseController<TB_SINGER>
    {

    





        [HttpGet]
        public void GetMusic(string name)
        {
      
            string url =$"http://m.kugou.com/singer/list/88?json=true";
            RestApiVisitHelper helper = new RestApiVisitHelper();
            string json = helper.Get(url);
            JObject jobject = JObject.Parse(json);
            string _data = jobject["singers"].ToString();
            var af = JsonConvert.DeserializeObject(_data);
            JArray array = new JArray();
            if (af is JArray)
                array = JsonConvert.DeserializeObject<JArray>(_data);
            else
                array = JsonConvert.DeserializeObject<JArray>("[" + _data + "]");

            foreach (var data in array)
            {
                var afaf = data["list"].ToString();
                jobject = JObject.Parse(afaf);               
               var afafa=  JsonConvert.DeserializeObject(jobject["info"].ToString());
                JArray array1 = new JArray();

                if (afafa is JArray)
                    array1 = JsonConvert.DeserializeObject<JArray>(jobject["info"].ToString());
                else
                    array1 = JsonConvert.DeserializeObject<JArray>("[" + jobject["info"].ToString() + "]");
                foreach (var data1 in array1)
                {
                    TB_SINGER tb = new TB_SINGER
                    {
                        SingerId = data1["singerid"].ToString(),
                        SingerName = data1["singername"].ToString(),
                        SingerImg = data1["imgurl"].ToString()
                    };
                    db.Saveable<TB_SINGER>(tb).ExecuteCommand();
                }

            }
             

        }
        
    }
}