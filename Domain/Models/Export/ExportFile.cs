using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Export
{
    public class ExportFile
    {
        public byte[] File { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }
    }
}
