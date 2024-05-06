using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ErrorFileObject
    {
        /// <summary>
        /// 檔案位置
        /// </summary>
        public string DirectoryName { get; set; }
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 檔案建立時間
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 檔案大小
        /// </summary>
        public long Length { get; set; }
    }
}
