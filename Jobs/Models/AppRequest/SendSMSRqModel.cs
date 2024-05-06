using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Jobs.Models.AppRequest
{
    [DataContract]
    public class SendSMSRqModel
    {
        [Required]
        [DataMember]
        public SmsHeaderModel Header { get; set; }

        [Required]
        [DataMember]
        public SmsContentModel Content { get; set; }

        public SendSMSRqModel()
        {
            Header = new SmsHeaderModel();
            Content = new SmsContentModel();
        }
    }

    public class SmsHeaderModel
    {
        [Required]
        [DataMember]
        public string ApplicationName { get; set; }
        [Required]
        [DataMember]
        public string ClientRefNo { get; set; }
        [Required]
        [DataMember]
        public string ClientTimestamp { get; set; }
    }

    public class SmsContentModel
    {
        /// <summary>
        /// 客戶的手機號碼(含國碼)
        /// </summary>
        [Required]
        [DataMember]
        public string MobileNo { get; set; }

        /// <summary>
        /// 客戶ID
        /// </summary>
        [Required]
        [DataMember]
        public string CustomerId { get; set; }

        /// <summary>
        /// 客戶姓名
        /// </summary>
        [Required]
        [DataMember]
        public string CustomerName { get; set; }

        /// <summary>
        /// 套印的變數值清單，"key1=value1&key2=value2"
        /// </summary>
        [Required]
        [DataMember]
        public string Variables { get; set; }

        /// <summary>
        /// 預設為AP Server上的語系設定
        /// </summary>
        [Required]
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 活動代碼
        /// </summary>
        [Required]
        [DataMember]
        public string CampaignCode { get; set; }
    }
}
