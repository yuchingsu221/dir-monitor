using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebService.Models.AppRequest
{
    [DataContract]
    public class GetLoginStatusRqModel
    {
        /// <summary>
        /// 登入Token
        /// </summary>
        [Required]
        [DataMember]
        public string Token { get; set; }
    }
}
