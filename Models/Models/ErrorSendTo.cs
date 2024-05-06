using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ErrorSendTo
    {
        public int Id { get; set; }
        public string Email { get; set; }

        // Foreign key
        public int ProtectedId { get; set; }  // Foreign key to Protected

        // Navigation property
        public Protected Protected { get; set; }  // Reference to Protected
    }

}
