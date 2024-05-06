using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.RelationDB
{
    [Table("CUST_SESSION")]
    public class CUST_SessionModel
    {
        public string SESSIONID { get; set; }
        public string SESSIONPROFILE { get; set; }
        public int DIRTYFLAG { get; set; }
        public DateTime? LOGONTIME { get; set; }
        public string LASTACTIONTIME { get; set; }
        public DateTime? LOGOFFTIME { get; set; }
        public string ROLECODE { get; set; }
        public DateTime? TIMEOUTTIME { get; set; }
        public string USERID { get; set; }
        public string XID { get; set; }
        public string MMA_SESSIONID { get; set; }
    }
}
