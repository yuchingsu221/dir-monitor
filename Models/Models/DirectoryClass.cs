using System.Collections.Generic;

namespace Models.Models
{
    public class DirectoryClass
    {
        /// <summary>
        /// 資料夾名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否為資料夾
        /// </summary>
        public bool IsFolder { get; set; }
        /// <summary>
        /// 該資料夾底下之資料夾
        /// </summary>
        public List<DirectoryClass> Directories { get; set; }
        /// <summary>
        /// 該資料夾底下之檔案
        /// </summary>
        public List<FileToJson> Files { get; set; }
    }
}