using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unit.Model
{
    public class TB_SINGER
    {
        public TB_SINGER() { }
        /// <summary>
        /// 歌手
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String SingerId { get; set; }
        /// <summary>
        /// 歌手名称
        /// </summary>
        public System.String SingerName { get; set; }
        /// <summary>
        /// 歌手图片
        /// </summary>
        public System.String SingerImg { get; set; }
    }
}
