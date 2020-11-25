using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Com.Unit.API
{
    public class EmailUnit
    {
        public string SendCode(string EmailName)
        {
            string mailFrom = "773408602@qq.com";//发送者账号账号
            string activeCode = Guid.NewGuid().ToString().Substring(0, 6);//验证码
            string MessageBody = "您的验证码为：" + "【" + activeCode + "】" + "请您在1分钟之内进行验证，若非本人操作，请忽视此短信";
            MailAddress MessageFrom = new MailAddress(mailFrom);
            string MailTo = EmailName;//接收者邮箱
            string MessageSubject = "音乐验证码";
            if (Send(MessageFrom, MailTo, MessageSubject, MessageBody))
            {
                return activeCode;
            }
            else
            {
                return "";
            }
        }

        public bool Send(MailAddress MessageFrom, string MessageTo, string MessageSubject, string MessageBody)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = MessageFrom;
                message.To.Add(MessageTo);//收件人邮箱            
                message.Subject = MessageSubject;//主题        
                message.Body = MessageBody;//正文
                message.IsBodyHtml = true;//html格式
                message.Priority = MailPriority.High;//优先级
                SmtpClient sc = new SmtpClient();
                sc.Host = "smtp.qq.com";//服务器
                sc.Port = 25;//端口
                sc.Credentials = new System.Net.NetworkCredential("773408602@qq.com", "rxqnyhtperjebedd");//登录（账号和授权码）
                sc.Send(message);//发送邮件
                return true;
            }
            catch (ArgumentNullException ex)
            {
                //  Response.Write("<script>alert('" + ex.Message + "')</script>");
                return false;
            }
            //from 为 empty。或 recipients 为 empty。
            catch (ArgumentException ex)
            {
                //Response.Write("<script>alert('" + ex.Message + "')</script>");
                return false;
            }
            //已释放此对象。
            catch (ObjectDisposedException ex)
            {
                //Response.Write("<script>alert('" + ex.Message + "')</script>");
                return false;
            }
            /*
             此 smtpclient 有一个 sendasync 调用正在进行。
             - 或 - deliverymethod 属性设置为 network，且 host 为 null。
             - 或 - deliverymethod 属性设置为 network，host 等于空字符串 ("")。
             - 或 - deliverymethod 属性被设为 network 且 port 为零、负数或大于 65,535。
             */
            catch (InvalidOperationException ex)
            {
                //Response.Write("<script>alert('" + ex.Message + "')</script>");
                return false;
            }
            //message 未能传递给 to、cc 或 bcc 中的一个收件人。       
            catch (SmtpFailedRecipientException ex)
            {
                //Response.Write("<script>alert('" + ex.Message + "')</script>");
                return false;
            }
            /*
             * 连接到 smtp 服务器失败。
             * - 或 - 身份验证失败。
             * - 或 - 操作超时。 - 或 - enablessl 设置为 true，但 deliverymethod 属性设置为 specifiedpickupdirectory 或 pickupdirectoryfromiis。
             * - 或 - enablessl 设置为 true,，但 smtp 邮件服务器不在对 ehlo 命令的响应中播发 starttls。
             * */
            catch (SmtpException ex)
            {
                // Response.Write("<script>alert('" + ex.Message + "')</script>");
                return false;
            }
            catch (Exception ex)
            {
                //   Response.Write("<script>alert('" + ex.Message + "')</script>");
                return false;
            }
        }
    }
}
