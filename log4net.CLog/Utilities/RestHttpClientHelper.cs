using log4net.CLog.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace log4net.CLog.Utilities
{
    public class RestHttpClientHelper 
    {
        const string ContentType = "application/json";
       // const string ContentType = "text/json";

        public static IServiceProvider ServiceProvider { get; set; }
        private static readonly IServiceCollection ServiceCollection = new ServiceCollection();
        private static readonly IHttpClientFactory HttpClientFactory;


        static RestHttpClientHelper()
        {
            ServiceCollection.AddHttpClient();
            ServiceProvider = ServiceCollection.BuildServiceProvider();
            HttpClientFactory = ServiceProvider.GetService<IHttpClientFactory>();
        }


        public async Task<ResponseResult> Post(Uri uri, string reqBoby)
        {
            return await DoRequest(HttpClientFactory, HttpMethod.Post, ContentType, uri, reqBoby);
        }




        /// <summary>
        /// POST,PUT,DELETE请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="reqBody"></param>
        /// <param name="headerDic"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="httpMethod"></param>
        /// <param name="contentType"></param>
        /// <param name="basicAuthorization">basic授权凭据</param>
        /// <param name="bearerAuthorization"></param>
        /// <returns></returns>
        private async Task<ResponseResult> DoRequest(IHttpClientFactory httpClientFactory, HttpMethod httpMethod, string contentType, Uri uri, string reqBody
            , Dictionary<string, string> headerDic=null,string basicAuthorization = null, string bearerAuthorization = null)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }



            Task<ResponseResult> result;
            HttpContent content = new StringContent(reqBody, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType) { CharSet = "utf-8" };
            if (headerDic != null)
            {
                foreach (var item in headerDic)
                {
                    content.Headers.Add(item.Key, item.Value);
                }
            }
            var client = httpClientFactory.CreateClient();

            if (uri.Scheme.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    CheckValidationResult;
            }

            //为request添加basic授权凭据
            if (string.IsNullOrWhiteSpace(basicAuthorization) == false)
            {
                var byteArray = Encoding.ASCII.GetBytes(basicAuthorization);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            //为request添加bearer授权凭证
            if (string.IsNullOrWhiteSpace(bearerAuthorization) == false)
            {
                client.DefaultRequestHeaders.Authorization =
                   new AuthenticationHeaderValue("Bearer", bearerAuthorization);
            }

            if (httpMethod == HttpMethod.Post)
            {
                using (var response = await client.PostAsync(uri, content))
                {
                    result = ReadResponseResult(response);
                }
            }
            else if (httpMethod == HttpMethod.Put)
            {
                using (var response = await client.PutAsync(uri, content))
                {
                    result = ReadResponseResult(response);
                }
            }
            else if (httpMethod == HttpMethod.Delete)
            {
                if (!string.IsNullOrEmpty(reqBody))
                {
                    var message = new HttpRequestMessage();
                    message.Method = HttpMethod.Delete;
                    message.Content = content;
                    message.RequestUri = uri;
                    using (var response = await client.SendAsync(message))
                    {
                        result = ReadResponseResult(response);
                    }
                }
                else
                {
                    using (var response = await client.DeleteAsync(uri))
                    {
                        result = ReadResponseResult(response);
                    }
                }
            }
            else
            {
                using (var response = await client.PostAsync(uri, content))
                {
                    result = ReadResponseResult(response);
                }
            }

            return await result;
        }

        /// <summary>
        /// 读取请求结果
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<ResponseResult> ReadResponseResult(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = new ResponseResult(response.StatusCode, content);
            return result;
        }
        /// <summary>
        /// 检查验证结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true; //总是接受  
        }



    }

}
