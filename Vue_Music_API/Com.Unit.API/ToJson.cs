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
    }
}
