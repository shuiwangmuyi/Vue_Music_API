using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unit.API
{
    public class ToJson
    {
        public JArray GetJsonConvert(string restult,string data)
        {
            JObject jobject = JObject.Parse(restult);
            string _data = jobject[data].ToString();
            var obj = JsonConvert.DeserializeObject(data);
            JArray json = new JArray();
            if (obj is JArray)
                json = JsonConvert.DeserializeObject<JArray>(data);
            else
                json = JsonConvert.DeserializeObject<JArray>("[" + data + "]");
            return json;
        }

        //[HttpGet]
        public void GetMusic(string name)
        {

            string url = $"http://m.kugou.com/singer/list/88?json=true";
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
                var afafa = JsonConvert.DeserializeObject(jobject["info"].ToString());
                JArray array1 = new JArray();

                if (afafa is JArray)
                    array1 = JsonConvert.DeserializeObject<JArray>(jobject["info"].ToString());
                else
                    array1 = JsonConvert.DeserializeObject<JArray>("[" + jobject["info"].ToString() + "]");
                foreach (var data1 in array1)
                {
                    //TB_SINGER tb = new TB_SINGER
                    //{
                    //    SingerId = data1["singerid"].ToString(),
                    //    SingerName = data1["singername"].ToString(),
                    //    SingerImg = data1["imgurl"].ToString()
                    //};
                    //db.Saveable<TB_SINGER>(tb).ExecuteCommand();
                }

            }


        }
    }
}
