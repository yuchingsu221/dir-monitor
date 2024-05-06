using System;
using System.Collections.Generic;
using System.Linq;

namespace WebService.Models.Defines
{
    public class CryptoDefine
    {
        public static readonly string A_FIXED_K = "7A63215D8F9E2412CD6B0754A83FEDB9"; // APP固定的KEY
        public static readonly string A_IV = "9fcopinis0000"; // 共用的IV 0000
        private static readonly string A_ID = "A_ID_{0}";

        private static readonly List<AesKeyDefine> KEY_LIST = new List<AesKeyDefine>
        {
            #region 給前端的隨機五組KEY
            new AesKeyDefine()
            {
                AId = string.Format(A_ID,1),
                AKey = "7D8B5A962F8C1046E24B72A1C39D058F"
            },
            new AesKeyDefine()
            {
                AId = string.Format(A_ID,2),
                AKey = "2F3A8C9614D78B512E4C60F197E2A345"
            },
            new AesKeyDefine()
            {
                AId = string.Format(A_ID,3),
                AKey = "9F4C72A168E02B137D5A0E34B81C963F"
            },
            new AesKeyDefine()
            {
                AId = string.Format(A_ID,4),
                AKey = "F2C847A51E9B06D3194A38C7B5D6F21E"
            },
            new AesKeyDefine()
            {
                AId = string.Format(A_ID,5),
                AKey = "C1B0E7A82F4D953617E86C2B0F341A8D"
            }
            #endregion
        };

        public static List<AesKeyDefine> GetKeyList()
        {
            return KEY_LIST;
        }

        public static AesKeyDefine GetKeyValue(string keyId)
        {
            var result = KEY_LIST.Where(p => p.AId == keyId).FirstOrDefault();

            return result;
        }

        public static AesKeyDefine GetRandomKeyValue()
        {
            // 先隨機取得一組 KeyID
            int random = int.Parse(DateTime.Now.ToString("fff").ToString());
            var AId = string.Format(A_ID, random % 5 + 1);

            return GetKeyValue(AId);
        }
    }
}
