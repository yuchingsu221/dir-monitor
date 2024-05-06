using System.Collections.Generic;

namespace Models.Models
{
    public class Protected
    {
        public int Id { get; set; }
        public bool IsProtected { get; set; }
        public string ProjectType { get; set; }
        public string SiteName { get; set; }

        // Navigation properties
        public List<DirConfig> DirConfigs { get; set; }  // One-to-many relationship with DirConfig
        public List<ErrorSendTo> ErrorSendTos { get; set; }  // One-to-many relationship with ErrorSendTo
    }
}
