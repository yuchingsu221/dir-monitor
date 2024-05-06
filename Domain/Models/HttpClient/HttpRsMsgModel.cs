using System;
using System.Net.Http;

namespace Domain.Models.HttpClient
{
    public class HttpRsMsgModel
    {
        public HttpResponseMessage Response { get; set; }
        public Guid Guid { get; set; }
    }
}
