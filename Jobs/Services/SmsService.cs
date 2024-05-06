using CommonLibrary.Util;
using DataLayer.Http;
using Domain.Defines;
using Domain.Models.Config;
using Jobs.Models.AppRequest;
using Jobs.Models.AppResponse;
using Jobs.Services.Interfaces;
using System;
using System.Net.Http;

namespace Jobs.Services
{
    public class SmsService : ISmsService
    {
        private readonly ISendHttpRequest _SendHttpRequest;
        private readonly WebServiceSetting _Settings;

        public SmsService(WebServiceSetting setting, ISendHttpRequest sendHttpRequest)
        {
            _Settings = setting;
            _SendHttpRequest = sendHttpRequest;
        }

        public SendSMSRsModel SendSMS(SmsContentModel request)
        {
            string clientTimestamp = DateTime.UtcNow.AddHours(7).ToString("O");
            string clientRefNo = BaseUtility.GetGuid();

            SendSMSRqModel sendSMSRqModel = new SendSMSRqModel();
            sendSMSRqModel.Header.ApplicationName = _Settings.SMSSetting.ApplicationName;
            sendSMSRqModel.Header.ClientTimestamp = clientTimestamp;
            sendSMSRqModel.Header.ClientRefNo = clientRefNo;
            sendSMSRqModel.Content = request;

            SendSMSRsModel sendSMSRsModel = _SendHttpRequest.SendJsonRequest<SendSMSRqModel, SendSMSRsModel>(ExternalServiceEnum.SendSMS, "/SMS/CmCare", HttpMethod.Post, sendSMSRqModel, null);

            sendSMSRsModel.ClientTimestamp = clientTimestamp;
            sendSMSRsModel.ClientRefNo = clientRefNo;

            return sendSMSRsModel;
        }
    }
}
