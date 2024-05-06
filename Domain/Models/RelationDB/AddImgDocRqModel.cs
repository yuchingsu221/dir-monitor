using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain.Models.RelationDB
{
    [DataContract]
    public class AddImgDocRqModel
    {
        [DataMember]
        public AddImgDocContentModel Content { get; set; }
    }

    public class AddImgDocContentModel
    {
        [DataMember]
        public string AppNo { get; set; }
        [DataMember]
        public List<AddImgDocInfoModel> ImageInfo { get; set; }
    }

    public class AddImgDocInfoModel
    {
        [DataMember]
        public string DocType  { get; set; }
        [DataMember]
        public string DocImage { get; set; }
    }
}
