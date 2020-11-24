using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Unit.Model
{
    public class TB_Categorys
    {
        public TB_Categorys()
        {
        }
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 TypeId { get; set; }
        public System.String TypeName { get; set; }
        public System.String TypeClass { get; set; }       
    }
}
