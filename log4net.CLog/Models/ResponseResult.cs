using System;
using System.Net;

namespace log4net.CLog.Models
{
    /// <summary>
    ///响应的结果
    /// </summary>
    public class ResponseResult
    {
        /// <summary>
        /// http状态码
        /// </summary>
        public HttpStatusCode HttpStatus { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 获取请求是否成功
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                var statusCode = (int)HttpStatus;
                return statusCode >= 200 && statusCode < 400;
            }
        }

        /// <summary>
        /// 构造Json响应结果
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="content"></param>
        public ResponseResult(HttpStatusCode httpStatus, string content)
        {
            if ((int)httpStatus >= 300)
            {
                throw new  Exception($"请求失败：{content} HttpStatusCode:{(int)httpStatus}");
            }
            HttpStatus = httpStatus;
            Content = content;
        }
    }
}
