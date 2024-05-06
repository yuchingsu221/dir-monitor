using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebService.Models.AppRequest
{
    public class BackendLoginRequest
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Account { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
