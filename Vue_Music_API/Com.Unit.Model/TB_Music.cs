using SqlSugar;

namespace Com.Unit.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class TB_Music
    {
        /// <summary>
        /// 
        /// </summary>
        public TB_Music()
        {
        }

        /// <summary>
        /// 音乐ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String M_Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public System.String M_Name { get; set; }

        /// <summary>
        /// 歌手
        /// </summary>
        public System.String M_Author { get; set; }

        /// <summary>
        /// 歌词
        /// </summary>
        public System.String M_Words { get; set; }

        /// <summary>
        /// 热度
        /// </summary>
        public System.Int32 M_Hot { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public System.DateTime? M_Time { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public System.String M_Img { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public System.String M_Address { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public System.String M_Type { get; set; }
    }
}
