using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ErrorFiles
    {
        /// <summary>
        /// 不在白名單內之檔案資訊
        /// </summary>
        public List<ErrorFileObject> Files { get; set; }
    }
}
