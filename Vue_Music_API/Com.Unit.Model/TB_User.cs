using SqlSugar;

namespace Com.Unit.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class TB_User
    {
        /// <summary>
        /// 
        /// </summary>
        public TB_User()
        {
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public System.Int32 U_Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public System.String U_Name { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public System.DateTime? U_Birthday { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public System.String U_Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public System.String U_Tel { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public System.DateTime? U_Regtime { get; set; }

        /// <summary>
        /// 用户账户
        /// </summary>
        public System.String U_Account { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public System.String U_PassWord { get; set; }
    }
}
