using Newtonsoft.Json;
using System;
using Formatting = Newtonsoft.Json.Formatting;

namespace Models.Models
{
    public class FileToJson
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否為資料夾
        /// </summary>
        public bool IsFolder { get; set; }
        /// <summary>
        /// 檔案建立時間
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 檔案更新時間
        /// </summary>
        public DateTime UpdatedTime { get; set; }
        /// <summary>
        /// 檔案大小
        /// </summary>
        public long Length { get; set; }
    }
}