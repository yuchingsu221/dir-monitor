using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WebService.Models.AppRequest;

namespace WebService.Models.AppResponse
{
    [DataContract]
    public class LoginRsModel
    {
        /// <summary>
        /// Login Token
        /// </summary>
        [DataMember]
        public string Token { get; set; }
    }
}
