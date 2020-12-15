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
using Vue_Music_API.Method;
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
        [HttpPost]
        public string GetCaptcha(string email)
        {      
            string message = "", status = '"' + "200" + '"', msg = '"' + "OK" + '"', total = '"' + '"' + "";
            var result=_sugarTable.Where(it => it.U_Email.Contains(email)).Any();
            if (!result)
            {
                infor.WriteInforLog("邮箱已注册", "GetCaptcha", "");
                status = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' +"邮箱已注册"+ '"';
                return $"[{{" + '"' + "status" + '"' + $":{status}," + '"'
                 + "msg" + '"' + $":{msg}," + '"'
                 + "captcha" + '"' + ":" + '"' + $"{message}" + '"' + "," + '"' + "Total" + '"' + ":" + '"' + '"' + "}]";
            }

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
        [HttpPost]
        public string userResgister(string email,string userPass)
        {
            string message = "", code = '"' + "200" + '"', msg = '"' + "OK" + '"', total = "";
            ErrorMethod error = new ErrorMethod();
            InforMethod infor = new InforMethod();
            Random random = new Random();
            TB_User user = new TB_User();
            while (true)
            {
               user.U_Id = random.Next(10000000, 100000000);
                if (_sugarTable.Where(it => it.U_Id == user.U_Id).Any())
                    break;
            }
            user.U_Email = email;
            user.U_PassWord = userPass;
            while (true)
            {
                user.U_Account = "music_" + Guid.NewGuid().ToString().Substring(0, 6);
                if (_sugarTable.Where(it => it.U_Account == user.U_Account).Any())
                    break;
            }
            try
            {
                db.Saveable<TB_User>(user).ExecuteCommand();
                message = '"' + $"账号为：{ user.U_Account}" + '"';
            }
            catch(Exception e){
                code = '"' + "500" + '"';
                msg = '"' + "error" + '"';
                message = '"' + '"' + "";
                error.WriteErrorLog(e, "GetMusicTypeName()", "TB_Music");
            }
            return $"[{{" + '"' + "code" + '"' + $":{code}," + '"'
            + "msg" + '"' + $":{msg}," + '"'
            + "data" + '"' + $":{message}," + '"' + "Total" + '"' + $":{total}}}]";
        }
        #endregion

        #region 验证码
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

     
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userAccount">账号/邮箱/手机号</param>
        /// <param name="userPass">密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Controllers/Login")]
        public string UserLogin(string userAccount,string userPass,string code)
        {
            var tb_User= db.Queryable<TB_User>().Where(it =>
             (it.U_Tel.Equals(userAccount) || it.U_Account.Equals(userAccount)
             || it.U_Email.Equals(userAccount))
             && it.U_PassWord.Equals(userPass));         
            if (tb_User.Any())//判断用户是否登录成功
            {
                GetToken(tb_User.First().U_Account, tb_User.First().U_Email);
                UserLoginInfor._User = tb_User.First();//方便用户信息         
            }
            return "";
        }

        private void GetToken(string account,string email)
        {
            // 定义用户信息
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account),
                new Claim(JwtRegisteredClaimNames.Email, email),
            };

            // 和 Startup 中的配置一致
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdABCD1234abcdABCD1234"));

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "server",
                audience: "client007",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine(jwtToken);
        }
    }
}
