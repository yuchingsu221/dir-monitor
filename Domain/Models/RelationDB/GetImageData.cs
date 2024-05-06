using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain.Models.RelationDB
{
    public class GetImageData
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string CaseNum { get; set; }
        public string SupCaseNum { get; set; }
        public string Photo { get; set; }
    }
}
