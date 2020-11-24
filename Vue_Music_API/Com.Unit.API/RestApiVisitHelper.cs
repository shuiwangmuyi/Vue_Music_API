using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace Com.Unit.API
{
    public class RestApiVisitHelper
    {// 鉴权 token 的请求头属性名称
        public String TokenHeaderName { get; set; }
        // 默认的鉴权 token 信息
        public String DefaultToken { get; set; }


        public RestApiVisitHelper()
        {

        }

        // 构造时设置鉴权 token 的请求头属性名称
        public RestApiVisitHelper(String tokenHeaderName)
        {
            TokenHeaderName = tokenHeaderName;
        }

        // 构造时设置鉴权 token 的请求头属性名称，以及默认的 token 值
        public RestApiVisitHelper(String tokenHeaderName, String defaultToken)
        {
            TokenHeaderName = tokenHeaderName;
            DefaultToken = defaultToken;
        }

        public String Get(string apiUrl, Hashtable queryParams)
        {
            return Get(System.Web.HttpUtility.UrlEncode(apiUrl), queryParams, DefaultToken);
        }

        private String Get(string apiUrl, Hashtable queryParams, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                String result = webClientObj.DownloadString(apiUrl);
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }
        /// <summary>
        /// 通过API地址获取数据
        /// </summary>
        /// <param name="url">API URL地址</param>
        /// <returns></returns>
        public string Get(string url)//传入网址
        {
            string result;
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] strData = MyWebClient.DownloadData(url);
            MemoryStream ms = new MemoryStream(strData);
            using (StreamReader sr = new StreamReader(ms, Encoding.Default))//Encoding.GetEncoding("GB2312")
            {
                result = sr.ReadLine();
            }

            MyWebClient.Dispose();
            return result;
        }

        /// <summary>
        /// Post Api 返回结果文本，使用默认的鉴权 token
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="queryParams"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public String Post(string apiUrl, Hashtable queryParams, JObject body)
        {
            return Post(apiUrl, queryParams, body, DefaultToken);
        }

        /// <summary>
        /// Post Api 返回结果文本
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="queryParams"></param>
        /// <param name="body"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public String Post(string apiUrl, Hashtable queryParams, JObject body, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                webClientObj.Headers[HttpRequestHeader.ContentType] = "application/json";
                String result = webClientObj.UploadString(apiUrl, "POST", body.ToString(Newtonsoft.Json.Formatting.None));
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }

        public String Post(string apiUrl, JObject body)
        {
            System.Net.WebClient webClientObj = CreateWebClient(DefaultToken);

            // apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                webClientObj.Headers[HttpRequestHeader.ContentType] = "application/json";
                String result = webClientObj.UploadString(apiUrl, "POST", body.ToString(Newtonsoft.Json.Formatting.None));
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }

        public String Put(string apiUrl, Hashtable queryParams, JObject body)
        {
            return Put(apiUrl, queryParams, body, DefaultToken);
        }

        public String Put(string apiUrl, Hashtable queryParams, JObject body, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                String result = webClientObj.UploadString(apiUrl, "PUT", body.ToString(Newtonsoft.Json.Formatting.None));
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }


        private String _Delete(string apiUrl, Hashtable queryParams, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);

            try
            {
                String result = webClientObj.UploadString(apiUrl, "DELETE", "");
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }

        #region 私有方法

        // 创建 WebClient 并设置好 token 信息
        private WebClient CreateWebClient(String token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            System.Net.WebClient webClientObj = new System.Net.WebClient();
            webClientObj.Headers.Add("Accept", "application/json");
            if (!String.IsNullOrEmpty(TokenHeaderName) && !String.IsNullOrEmpty(token))
            {
                webClientObj.Headers.Add(TokenHeaderName, token);
            }
            webClientObj.Encoding = Encoding.UTF8;
            return webClientObj;
        }

        // 将查询参数格式化拼接到 url 上形成最终的访问地址
        private String FormatUrl(String apiUrl, Hashtable queryParams)
        {
            String queryString = "";
            foreach (var k in queryParams.Keys)
            {
                if (!String.IsNullOrEmpty(queryString))
                {
                    queryString += "&";
                }
                queryString += String.Format("{0}={1}", k, queryParams[k]);
            }
            if (!String.IsNullOrEmpty(queryString))
            {
                apiUrl += "?" + queryString;
            }
            return apiUrl;
        }

        // 异常时返回的信息：应该根据实际需要进行返回
        private String WhenError(Exception e)
        {
            JObject result = new JObject();
            result["err_code"] = -1;
            if (e is WebException)
            {
                var we = (WebException)e;
                if (we.Response != null)  // 如果有输出则仍然返回实际输出
                {
                    return new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    result["err_msg"] = we.Message;
                }
            }
            else
            {
                result["err_msg"] = e.Message;
            }
            return result.ToString(Newtonsoft.Json.Formatting.None);
        }

        #endregion
    }
}
