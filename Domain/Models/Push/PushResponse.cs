using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Push
{
    public class PushResponse
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public string ReturnCode { get; set; }
        public bool Success { get; set; }
        public string CultureId { get; set; }
    }
}
