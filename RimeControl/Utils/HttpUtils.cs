using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RimeControl.Utils
{
    /// <summary>
    /// C# HTTP请求工具
    /// v 0.0.1
    /// qzly 2019-03-05 19:20
    /// </summary>
    public class HttpUtils
    {

        public static string GetStringData(string url,string paramData)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest request;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    //https 请求
                    request = WebRequest.Create(url) as HttpWebRequest;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request.ProtocolVersion = HttpVersion.Version11;
                    // 这里设置了协议类型。
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                    request.KeepAlive = false;
                    ServicePointManager.CheckCertificateRevocationList = true;
                    ServicePointManager.DefaultConnectionLimit = 100;
                    ServicePointManager.Expect100Continue = false;
                }
                else
                {
                    //http 请求
                    request = (HttpWebRequest)WebRequest.Create(url);
                }
                //设置请求方式，头信息等
                request.Method = "GET";    //使用get方式发送数据
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = null;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                request.Accept = "*/*";

                //要发送的数据
                if (!string.IsNullOrEmpty(paramData))
                {
                    byte[] data = Encoding.UTF8.GetBytes(paramData);
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }

                //获取网页响应结果
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return result;
        }


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受          }
        }
    }
}
