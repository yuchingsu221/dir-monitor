using Models.Models;
using System.Collections.Generic;

namespace WebService.Models.AppRequest
{
    public class Config
    {
        /// <summary>
        /// log 存放目錄
        /// </summary>
        public string Logs { get; set; }
        /// <summary>
        /// 發信相關設定值
        /// </summary>
        public MailSetting MailSettings { get; set; }
        /// <summary>
        /// 欲掃描之指定目錄資訊
        /// </summary>
        public List<DirConfig> Dirs { get; set; }
    }
}
