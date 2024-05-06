using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Defines
{
    public class RegexDefine
    {
        /// <summary>
        /// 為6-20英數混合，不可包含空白與各種符號
        /// </summary>
        public static readonly string ENG_AND_NUM_AND_SIX_TWENTY = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$";
        /// <summary>
        /// 為6-20英數混合，不可包含空白與各種符號
        /// </summary>
        public static readonly string ENG_AND_NUM_AND_SIX_TWELVE = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,12}$";
        /// <summary>
        /// 為6-20英數混合至少有一個小寫和大寫字母，不可包含空白與各種符號
        /// </summary>
        public static readonly string ENG_LESSTHAN_UP_AND_LOW_AND_NUM_SIX_TWENTY = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{6,20}$";
    }
}
