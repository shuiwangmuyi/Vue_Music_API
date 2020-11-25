using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unit.Model
{
   public class TB_Error_log
    {

        [SugarColumn(IsPrimaryKey = true)]
        public System.String ErrorID { get; set; }
        public System.String ErrorMessage { get; set; }
        public System.String ErrorTime { get; set; }
        public System.String ErrorTable { get; set; }
        public System.String ErrorMethod { get; set; }
      }
}
