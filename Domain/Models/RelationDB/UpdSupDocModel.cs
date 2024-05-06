using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain.Models.RelationDB
{
    [DataContract]
    public class UpdSupDocModel
    {
        [DataMember]
        public UpdContentModel Content { get; set; }
    }

    public class UpdContentModel
    {
        [DataMember]
        public string AppNo { get; set; }
        [DataMember]
        public DateTime DocFinishDate { get; set; }
        [DataMember]
        public List<SupModel> SupDocInfo { get; set; }
    }

    public class SupModel
    {
        [DataMember]
        public string DocType  { get; set; }
        [DataMember]
        public string DocImage { get; set; }
    }

    public class BaseId : SupModel
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string SupCaseNum { get; set; }
        [DataMember]
        public DateTime DocFinishDate { get; set; }
    }

    public class AddOC : SupModel
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string CaseNum { get; set; }
    }
}
