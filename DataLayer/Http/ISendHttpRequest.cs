using Domain.Defines;
using Domain.Models.HttpClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DataLayer.Http
{
    public interface ISendHttpRequest
    {
        /// <summary>
        /// 發送JSON請求
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        TResponse SendJsonRequest<TRequest, TResponse>(ExternalServiceEnum serviceType, string uri, HttpMethod method, TRequest body = null, Dictionary<string, string> headers = null, bool isBearer = true) where TRequest : class;

        HttpRsMsgModel SendJsonRequest<TRequest>(ExternalServiceEnum serviceType, string uri, HttpMethod method, TRequest body, Dictionary<string, string> headers = null, bool isBearer = true) where TRequest : class;
    }
}
