using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Push
{
    public class PushReqeust
    {
        public string Route { get; set; }
        public string HttpMethod { get; set; }
        public PushData Data { get; set; }
        public bool IsRedirect { get; set; }

        // 共用參數
        public PushReqeust()
        {
            Route = "PushService/SendPush";
            HttpMethod = "POST";
            IsRedirect = true;
        }
    }
}
