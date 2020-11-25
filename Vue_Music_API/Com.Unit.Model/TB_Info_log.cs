using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unit.Model
{
    public class TB_Info_log
    {
        public TB_Info_log() { }
        [SugarColumn(IsPrimaryKey = true)]
        public System.String InfoID { get; set; }
        public System.String InfoMessage { get; set; }
        public System.String InfoTime { get; set; }
        public System.String InfoTable { get; set; }
        public System.String InfoMethod { get; set; }
    }
}
