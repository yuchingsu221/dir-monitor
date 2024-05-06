using CommonLibrary.Util;
using Domain.Defines;
using Domain.Models.HttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Http
{
    public class SendHttpRequest : ISendHttpRequest
    {
        private readonly IHttpClientFactory _clientFactory;
        public SendHttpRequest(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public TResponse SendJsonRequest<TRequest, TResponse>(
            ExternalServiceEnum serviceType,
            string uri,
            HttpMethod method,
            TRequest body = null,
            Dictionary<string, string> headers = null,
            bool isBearer = true) where TRequest : class
        {
            try
            {
                HttpRsMsgModel httpRsMsg = SendJsonRequest<TRequest>(serviceType, uri, method, body, headers, isBearer);
                
                var responseTime = DateTime.UtcNow.AddHours(8);
                string responseString = httpRsMsg.Response.Content.ReadAsStringAsync().Result;
                LogUtility.LogInfo($"Send TResponse body responseString: {responseString}");
                var resp = JsonConvert.DeserializeObject<TResponse>(responseString);

                LogUtility.LogInfo($"Send TResponse body ({httpRsMsg.Guid}): {JsonConvert.SerializeObject(resp)}");
                return resp;
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("SendJsonRequest Error!", ex);
                throw ex;
            }
        }

        public HttpRsMsgModel SendJsonRequest<TRequest>(
            ExternalServiceEnum serviceType,
            string uri,
            HttpMethod method,
            TRequest body = null,
            Dictionary<string, string> headers = null,
            bool isBearer = true) where TRequest : class
        {
            try
            {
                var sendId = Guid.NewGuid();
                var client = _clientFactory.CreateClient(serviceType.ToString());
                var response = new HttpResponseMessage();
                var responseString = string.Empty;
                var content = string.Empty;
                HttpRsMsgModel httpRsMsg = new HttpRsMsgModel();

                if (body != null)
                    content = JsonConvert.SerializeObject(body);

                var stringContent = new StringContent(content, Encoding.UTF8, "application/json");


                if (headers != null)
                {
                    LogUtility.LogInfo($"Send Request Header({sendId}): {string.Join(", ", headers)}");
                    foreach (var header in headers)
                    {
                        if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                        {
                            if (header.Value.Contains(' ') && !isBearer)
                            {
                                var authInfo = header.Value.Split(' ');
                                var schema = authInfo.Length != 2 ? string.Empty : authInfo[0];
                                var param = authInfo.Length != 2 ? authInfo[0] : authInfo[1];
                                var p = Encoding.UTF8.GetBytes($"{schema}:{param}");
                                string val = Convert.ToBase64String(p);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", val);
                                break;
                            }
                            else if (isBearer)
                            {
                                if (header.Value.Contains(' '))
                                {
                                    var authInfo = header.Value.Split(' ');
                                    var schema = authInfo.Length != 2 ? string.Empty : authInfo[1];
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(schema);
                                }
                                else
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", header.Value);
                            }
                            else
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(header.Value);
                            }
                        }
                        else if (header.Key.Equals("Content-Type"))
                        {
                            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        }
                        else
                        {
                            stringContent.Headers.Add(header.Key, header.Value);
                        }
                    }
                }

                var request = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri($"{client.BaseAddress}{uri}"),
                    Content = stringContent
                };

                client.Timeout = TimeSpan.FromMinutes(2);

                    LogUtility.LogInfo($"Send Request body ({sendId}): {content}");

                LogUtility.LogInfo($"Send Request url ({sendId}): {request.RequestUri}");

                response = client.SendAsync(request).Result;
                LogUtility.LogInfo($"Send Response ({sendId}): {JsonConvert.SerializeObject(response)}");

                return new HttpRsMsgModel()
                {
                    Response = response,
                    Guid = sendId
                };
            }
            catch (Exception ex) when (ex is Exception)
            {
                if (ex.InnerException is TimeoutException)
                {
                    ex = ex.InnerException;
                    LogUtility.LogInfo("Timeout");
                }
                else if (ex is TaskCanceledException)
                {
                    if ((ex as TaskCanceledException).CancellationToken == null || (ex as TaskCanceledException).CancellationToken.IsCancellationRequested == false)
                    {
                        ex = new TimeoutException("Timeout occurred");
                        LogUtility.LogInfo("TaskCanceled");
                    }
                }
                LogUtility.LogError("SendJsonRequest Error!", ex);
                throw ex;
            }
        }

        //public string TrimEndSlah(string context)
        //{
        //    return context.end
        //}
    }
}
