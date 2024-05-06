using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WebService.Models.AppRequest
{
    [DataContract]
    public class ChangePwdRqModel
    {
        /// <summary>
        /// Token
        /// </summary>
        [Required]
        [DataMember]
        public string Token { get; set; }
        /// <summary>
        /// 原使用者密碼
        /// </summary>
        [Required]
        [DataMember]
        public string OrigUserPwd { get; set; }
        /// <summary>
        /// 新使用者密碼
        /// </summary>
        [Required]
        [DataMember]
        public string NewUserPwd { get; set; }
    }
}
