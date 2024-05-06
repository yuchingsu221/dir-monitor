using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Jobs.Models.AppRequest
{
    [DataContract]
    public class MailSendRqModel
    {
        /// <summary>
        /// 客戶ID
        /// </summary>
        [Required]
        [DataMember]
        public string CustId { get; set; }
        /// <summary>
        /// 信件主旨
        /// </summary>
        [Required]
        [DataMember]
        public string MailSubject { get; set; }
        /// <summary>
        /// 信件內容
        /// </summary>
        [Required]
        [DataMember]
        public string MailContent { get; set; }
        /// <summary>
        /// 收件者
        /// </summary>
        [Required]
        [DataMember]
        public string Receiver { get; set; }
        /// <summary>
        /// 發送類型
        /// </summary>
        [Required]
        [DataMember]
        public int? Type { get; set; }
        /// <summary>
        /// 附加檔案1
        /// </summary>
        [DataMember]
        public byte[] MEFileStream1 { get; set; }
        /// <summary>
        /// 附加檔案名稱1
        /// </summary>
        [DataMember]
        public string MEFileName1 { get; set; }
        /// <summary>
        ///申請
        /// </summary>
        [DataMember]
        public string METMnemonic { get; set; }
    }
}
