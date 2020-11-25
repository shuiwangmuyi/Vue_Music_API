using Com.Unit.API;
using Com.Unit.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vue_Music_API.Method;

namespace Vue_Music_API.Controllers
{
    public class LoginController : BaseController<TB_User>
    {
        ErrorMethod error = new ErrorMethod();
        InforMethod infor = new InforMethod();  
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
        //用户注册
        [HttpPost]
        public string userResgister(string email,string userPass)
        {
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
            db.Saveable<TB_User>(user).ExecuteCommand();

            return "";
        }
    }
}
