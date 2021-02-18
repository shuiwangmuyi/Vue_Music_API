using Com.Unit.API;
using Com.Unit.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vue_Music_API.Common;
using RestSharp;
using RestSharp.Authenticators;
using Vue_Music_API.Method;
using System.Net;
using HtmlAgilityPack;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vue_Music_API.Controllers
{    
    public class LoginController : BaseController<TB_User>
    {
        ErrorMethod error = new ErrorMethod();
        InforMethod infor = new InforMethod();      
        #region 注册
        /// <summary>
        /// 邮箱注册
        /// code：500：验证码发送失败 
        /// </summary>        
     //   [HttpPost]
        public string GetCaptcha(string email)
        {      
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"', total = '"' + '"' + "";           

            EmailUnit unit = new EmailUnit();     
            string captcha = unit.SendCode(email);//得到验证码           
            if (captcha == "")
            {
                infor.WriteInforLog("获取验证码失败", "GetCaptcha","");
                status = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + "获取验证码失败" + '"';
                return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
                 + "msg" + '"' + $":{msg}," + '"'
                 + "captcha" + '"' + ":" + '"' + $"{message}" + '"' + "," + '"' + "Total" + '"' + ":" + '"' + '"' + "}]";
            }   
            return  $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
                 + "msg" + '"' + $":{msg}," + '"'
                 + "captcha" + '"' + ":"+'"'+$"{captcha}"+ '"'+ "," + '"' + "Total" + '"' + ":" + '"' + '"' + "}]";
        }

        /// <summary>
        /// 用户点击了注册按钮
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        [HttpPost]
        public string userResgister(string email,string userPass,string code)
        {           
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"';  
            ErrorMethod error = new ErrorMethod();          
            InforMethod infor = new InforMethod();           
            //if(code!=CODE)
            //{
            //    msg = '"' + "error" + '"';
            //    message = '"' + "验证错误" + '"';
            //    return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
            //         + "msg" + '"' + $":{msg},"
            //         + '"' + "data" + '"' + ':' + message + "}]";
            //}
            TB_User user = new TB_User();
            user.U_Email = email;
            //验证邮箱是否注册
            var result = _sugarTable.Where(it => it.U_Email.Contains(user.U_Email)).Any();
            if (result)
            {
                infor.WriteInforLog("邮箱已注册", "userResgister", "注册");
                status = '"' + "200" + '"';
                msg = '"' + "error" + '"';
                message = '"' + "邮箱已注册" + '"';
                return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
                 + "msg" + '"' + $":{msg}," + '"'
                 + "captcha" + '"' + ":" + $"{message}" + "}]";
            }
            user.U_PassWord = userPass;
            user.U_ICO = "https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png";
            Random random = new Random();           
            while (true)
            {
               user.U_Id = random.Next(10000000, 100000000);
                if (!_sugarTable.Where(it => it.U_Id == user.U_Id).Any())
                    break;
            } 
            while (true)
            {
                user.U_Account = "music_" + Guid.NewGuid().ToString().Substring(0, 6);
                if (!_sugarTable.Where(it => it.U_Account == user.U_Account).Any())
                    break;
            }
            user.U_Regtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                db.Saveable<TB_User>(user).ExecuteCommand();
                message = '"' + $"注册账号为：{ user.U_Account}" + '"';
            }
            catch(Exception e)
            {
                code = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + "注册失败" + '"';
                error.WriteErrorLog(e, "GetMusicTypeName()", "TB_Music");
            }
            string ret = "[{" + '"' + "status" + '"' + $":{status},"
           + '"' + "msg" + '"' + $":{msg}," + '"'
            + "data" + '"' + $":{message}"+"}]";
            return ret;
        }
        #endregion
     

        private string GetCode()
        {
            int number;
            char code;
            string _captcha = String.Empty;
            Random r = new Random();
            for (int i = 0; i < 4; i++)
            {
                number = r.Next(); //返回非负随机数
                if (number % 2 == 0) //偶数则生成数字，奇数则生成字母
                    code = (char)('0' + (char)(number % 10)); //获取0与9之间的数字，0+num表示0后第num位
                else
                    code = (char)('A' + (char)(number % 26)); //获取A与Z之间的字母，A+num表示A后第num位
                _captcha += code.ToString();
            }
           // CODE = _captcha;
            return _captcha;
        }

        #region 
        /// <summary>
        /// 获取图片验证码
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("[controller]/[Action]")]
        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        [HttpGet]
         public FileContentResult GetImageCode()      
        {
            Random r = new Random();
            string _captcha = GetCode();
            MemoryStream ms = null;
            //验证码颜色集合  
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            //验证码字体集合
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            using (var img = new Bitmap((int)_captcha.Length * 18, 32)) 
            {
                using (var g = Graphics.FromImage(img))
                {
                    g.Clear(Color.White);//背景设为白色
                    //在随机位置画背景点  
                    for (int i = 0; i < 100; i++)
                    {
                        int x = r.Next(img.Width);
                        int y = r.Next(img.Height);
                        g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
                    }
                    //验证码绘制在g中  
                    for (int i = 0; i < _captcha.Length; i++)
                    {
                        int cindex = r.Next(7);//随机颜色索引值  
                        int findex = r.Next(5);//随机字体索引值  
                        Font f = new Font(fonts[findex], 15, FontStyle.Bold);//字体  
                        Brush b = new SolidBrush(c[cindex]);//颜色  
                        int ii = 4;
                        if ((i + 1) % 2 == 0)//控制验证码不在同一高度  
                        {
                            ii = 2;
                        }
                        g.DrawString(_captcha.Substring(i, 1), f, b, 3 + (i * 12), ii);//绘制一个验证字符  
                    }
                    ms = new MemoryStream();//生成内存流对象  
                    img.Save(ms, ImageFormat.Jpeg);//将此图像以Png图像文件的格式保存到流中  
                }
            }
             return File(ms.ToArray(), @"image/png");
        }
        #endregion


        [EnableCors("any")]
        [Route("[controller]/[Action]")]
        [HttpPost]
        public string SeleUserInfo()
        {
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"';
            try
            {
                if (UserLoginInfor._User.U_Birthday == null || UserLoginInfor._User.U_Regtime == null || UserLoginInfor._User.U_Birthday == null)
                    message = '"' + "false" + '"';
                else
                    message = '"' + "true" + '"';
            }
            catch(Exception e)
            {
                status = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + "true" + '"';
                error.WriteErrorLog(e, "UserLogin()", "TB_User");
            }
            string ret = "[{" + '"' + "status" + '"' + $":{status},"
              + '"' + "msg" + '"' + $":{msg}," + '"'
               + "message" + '"' + $":{message}" + "}]";
            return ret;
        }

        [EnableCors("any")]
        [HttpPost]
        [Route("[controller]/[Action]")]
        public string UserInfo()
        {
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"';
            try
            {
                if (UserLoginInfor._User == null)
                {
                    msg = '"' + "error" + '"';
                    message = '"' + "message" + '"' + ':' + '"' + "false" + '"';
                }
                else
                {
                    message ='"'+ "U_Account" + '"'+':'+ '"' + UserLoginInfor._User.U_Account + '"'+','
                        + '"' + "U_Birthday" + '"' + ':' + '"' + UserLoginInfor._User.U_Birthday + '"' + ','
                       + '"' + "U_Email" + '"' + ':' + '"' + UserLoginInfor._User.U_Email + '"' + ','
                        + '"' + "U_ICO" + '"' + ':' + '"' + UserLoginInfor._User.U_ICO + '"' + ','
                       + '"' + "U_Id" + '"' + ':' + '"' + UserLoginInfor._User.U_Id + '"' + ','
                        + '"' + "U_Name" + '"' + ':' + '"' + UserLoginInfor._User.U_Name + '"' + ','
                        + '"' + "U_Tel" + '"' + ':' + '"' + UserLoginInfor._User.U_Tel + '"'+ ','
                        + '"' + "U_Sex" + '"' + ':' + '"' + UserLoginInfor._User.U_Sex + '"';
                }

            }
            catch(Exception e)
            {
                msg = '"' + "error" + '"';
                message = '"' + "message" + '"' + ':' + '"' + "false" + '"';
            }
            return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
               + "msg" + '"' + $":{msg},"
               + '"' + "data" + '"' + ":[{" + $"{message}}}]}}]";
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Route("[controller]/[Action]")]
        public string ModifyUserPass(string password)
        {
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"';
            try
            {
                UserLoginInfor._User.U_PassWord = password;
                db.Saveable<TB_User>(UserLoginInfor._User).ExecuteCommand();
                message = '"' + $"修改成功" + '"';
            }
            catch (Exception e)
            {
                status = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + "修改失败" + '"';
                error.WriteErrorLog(e, "ModifyUserDetail()", "TB_User");
            }
            return "{" + '"' + "status" + '"' + $":{status},"
           + '"' + "msg" + '"' + $":{msg}," + '"'
            + "data" + '"' + $":{message}" + "}";
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Route("[controller]/[Action]")]
        public string ModifyUserDetail(int Id,string ICO,string userAccount,string tel,
                string email,string name,string sex,string birthday,string code)
        {
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"';
            try
            {
                UserLoginInfor._User.U_ICO = ICO;
                UserLoginInfor._User.U_Id = Id;
                UserLoginInfor._User.U_Account = userAccount;
                UserLoginInfor._User.U_Tel = tel;
                UserLoginInfor._User.U_Email = email;
                UserLoginInfor._User.U_Name = name;
                UserLoginInfor._User.U_Sex = sex;
                UserLoginInfor._User.U_Birthday = birthday.Substring(0,10);
                db.Saveable<TB_User>(UserLoginInfor._User).ExecuteCommand();
                message = '"' + $"修改成功" + '"';
            }
            catch(Exception e)
            {
                status = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + "修改失败" + '"';
                error.WriteErrorLog(e, "ModifyUserDetail()", "TB_User");
            }
            return "{" + '"' + "status" + '"' + $":{status},"
           + '"' + "msg" + '"' + $":{msg}," + '"'
            + "data" + '"' + $":{message}" + "}";
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userAccount">账号/邮箱/手机号</param>
        /// <param name="userPass">密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Route("Controllers/Login")]
        public string UserLogin(string userAccount,string userPass,string code,string Ip)
        {
            string key="";
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"';
            try
            {
               // if (code == CODE) { 
                    var tb_User = db.Queryable<TB_User>().Where(it =>
                    (it.U_Tel.Equals(userAccount) || it.U_Account.Equals(userAccount)
                    || it.U_Email.Equals(userAccount))
                    && it.U_PassWord.Equals(userPass));
                    if (tb_User.Any())//判断用户是否登录成功
                    {
                        key = GetToken(userAccount, userPass, code);

                        UserLoginInfor._User = tb_User.First();//方便用户信息     
                        message = '"' + "U_Name" + '"' + ':' + '"' + $"{UserLoginInfor._User.U_Name}" + '"' + ','
                       + '"' + "U_ICO" + '"' + ':' + '"' + $"{UserLoginInfor._User.U_ICO}" + '"' + ','
                       + '"' + "token" + '"' + ':' + '"' + $"Bearer {key}" + '"';
                        string city = GetAddress(Ip);
                        JObject OAddress= (JObject)JsonConvert.DeserializeObject(city);
                        city = OAddress["address"].ToString();
                        var Operators = city.Split(' ');                    
                        string operators = Operators[Operators.Length-1];
                         string City = "";
                         for(int i = 0; i < Operators.Length - 1;i++)
                         {
                             City += Operators[i];
                         }
                        login_log loginlog = new login_log();
                        login_log login= db.Queryable<login_log>().Where(it=> 
                            it.U_Id== UserLoginInfor._User.U_Id
                            ).OrderBy(it=>it.l_logintime,SqlSugar.OrderByType.Desc).First();
                      
                        if (login == null)
                        {
                            loginlog.Login_Id = Guid.NewGuid().ToString();
                            loginlog.Login_Ip = Ip;
                            loginlog.login_address = City;
                            loginlog.Operators = operators;
                            loginlog.l_logintime = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
                            loginlog.l_relogtime = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
                            loginlog.U_Id = UserLoginInfor._User.U_Id;
                        }
                        else
                        {
                        loginlog.Login_Id = Guid.NewGuid().ToString();
                        loginlog.Login_Ip = Ip;
                        loginlog.login_address = City;
                        loginlog.Operators = operators;
                        loginlog.l_logintime = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
                        loginlog.l_relogtime = login.l_logintime;
                        loginlog.U_Id = UserLoginInfor._User.U_Id;
                        }
                        db.Saveable<login_log>(loginlog).ExecuteCommand();
                    }
                    else
                    {
                        msg = '"' + "error" + '"';
                        message = '"' + "账号密码或密码错误" + '"';
                        return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
                             + "msg" + '"' + $":{msg},"
                             + '"' + "data" + '"'+':' + message + "}]";
                    }
                //}
                //else
                //{
                //    msg = '"' + "error" + '"';
                //    message = '"' + "验证错误" + '"';
                //    return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
                //         + "msg" + '"' + $":{msg},"
                //         + '"' + "data" + '"' + ':' + message + "}]";
                //}
            }
            catch(Exception e)
            {
                status = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + '"' + "";
                error.WriteErrorLog(e, "UserLogin()", "TB_User");
            }
            return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
               + "msg" + '"' + $":{msg},"
               + '"' + "data" + '"' + ":[{" + $"{message}}}]}}]";

        }

        private string GetAddress(string Ip)
        {
            string address = "";
            string url = "https://www.ip.cn/api/index?ip="+Ip+"&type=1";
            IRestClient restclient = new RestClient();
            RestRequest restRequest = new RestRequest(url);
            restclient.CookieContainer = new CookieContainer();
            IRestResponse restResponse = restclient.Execute(restRequest);
            if (!restResponse.IsSuccessful)            
                address = "未知";
            else
            {
                address = restResponse.Content;
            }
            return address;
        }

        private string GetToken(string account, string userPass,string code)
        {
            // 定义用户信息
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, account),
                new Claim("code",code)
            };

            // 和 Startup 中的配置一致// laozhanglaozhanglaozhanglaozhang
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EF1DA5B4-C7FA-4240-B997-7D1701BF9BE2"));

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "issuer",
                audience: "audience",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine(jwtToken);
            return jwtToken;
        }
    }
}
