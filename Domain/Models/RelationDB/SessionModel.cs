using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.RelationDB
{
    [Table("[session]")]
    public class SessionModel
    {
        [ExplicitKey]
        public string Token { get; set; }
        public string Data { get; set; }
        public DateTime ExpiredTime{ get; set; }
        public string CustId { get; set;}
        public int Status { get; set; }
        public string Region { get; set; }
    }
}
