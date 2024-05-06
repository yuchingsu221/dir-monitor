using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Domain.Models.RelationDB
{
    public class GetImageDataJob : GetImageData
    {
        public DateTime DocFinishDate { get; set; }
    }
}
