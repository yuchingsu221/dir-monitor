using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ModifyFileObject : ErrorFileObject
    {
        /// <summary>
        /// 新增/刪除/編輯
        /// </summary>
        public string Type { get; set; }
    }

    public enum TypeEnum
    {
        /// <summary>
        /// 新增
        /// </summary>
        Create = 0,
        /// <summary>
        /// 刪除
        /// </summary>
        Delete = 1,
        /// <summary>
        /// 編輯
        /// </summary>
        Update = 2
    }
}
