using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enum
{
    /// <summary>
    /// 所有的授權功能 (只有子項目)
    /// 中文名稱必須和DB一樣 很重要!!
    /// 中文名稱必須和DB一樣 很重要!! 
    /// 中文名稱必須和DB一樣 很重要!!
    /// </summary>
    public enum AuthorizationFunctionEnum
    {
        [Display(Name = "角色設定")]
        Role = 1,
        [Display(Name = "帳號設定")]
        Account,
    }
}
