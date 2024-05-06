using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebService.Models.AppResponse
{
    [DataContract]
    public class AspNetResultModel
    {
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public List<string> ContentTypes { get; set; }
        [DataMember]
        public object StatusCode { get; set; }
    }
}
