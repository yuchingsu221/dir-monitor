using Jobs.Models.AppRequest;
using Jobs.Models.AppResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jobs.Services.Interfaces
{
    public interface ISmsService
    {
        public SendSMSRsModel SendSMS(SmsContentModel request);
    }
}
