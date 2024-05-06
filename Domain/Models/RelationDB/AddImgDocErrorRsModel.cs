using System.Runtime.Serialization;

namespace Domain.Models.RelationDB
{
    [DataContract]
    public class AddImgDocErrorRsModel
    {
        [DataMember]
        public string UploadMsg { get; set; }
        [DataMember]
        public int Resultcode { get; set; }
        [DataMember]
        public string ResultMessage { get; set; }
        [DataMember]
        public object Result { get; set; }
    }
}
