using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Jobs.Models.AppResponse
{
    [DataContract]
    public class SendSMSRsModel
    {
        /// <summary>
        /// 結果代碼
        /// </summary>
        [DataMember]
        public string ResultCode { get; set; }

        /// <summary>
        /// 結果訊息
        /// </summary>
        [DataMember]
        public string ResultMessage { get; set; }

        /// <summary>
        /// 結果資訊
        /// </summary>
        [DataMember]
        public ResultModel Result { get; set; }

        /// <summary>
        /// 原傳入資訊
        /// </summary>
        [DataMember]
        public OriginalModel Original { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 發送時間
        /// </summary>
        [DataMember]
        public string ClientTimestamp { get; set; }

        /// <summary>
        /// 用戶端自編唯一值序號
        /// </summary>
        [DataMember]
        public string ClientRefNo { get; set; }

        public SendSMSRsModel()
        {
            Result = new ResultModel();
            Original = new OriginalModel();
        }
    }

    public class ResultModel
    {
        [DataMember]
        public int MessageId { get; set; }
        [DataMember]
        public string MessageContent { get; set; }
    }

    public class OriginalModel
    {
        [DataMember]
        public string MobileNo { get; set; }
        [DataMember]
        public string CampaignCode { get; set; }
        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string Variables { get; set; }
        [DataMember]
        public string LanguageCode { get; set; }
    }
}
