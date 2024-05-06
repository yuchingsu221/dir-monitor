using Domain.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CommonLibrary
{
    public class JwtHelper
    {
        private readonly IConfiguration Configuration;
        private readonly WebServiceSetting _Settings;

        /// <summary>
        /// key 授權功能中文,
        /// value 授權功能英文
        /// </summary>
        private static Dictionary<string, string> _authorizationFunctionNameDict = null;

        public JwtHelper(
            IConfiguration configuration,
            WebServiceSetting settings)
        {
            this.Configuration = configuration;
            _Settings = settings;
        }
        public string GenerateToken(int accountId, string account)
        {
            var issuer = _Settings.JwtSettings.Issuer;
            var signKey = _Settings.JwtSettings.SignKey;

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>();
            #region 一些claim範例
            // 在 RFC 7519 規格中(Section#4)，總共定義了 7 個預設的 Claims，我們應該只用的到兩種！
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, issuer));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "The Audience"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            #endregion
            claims.Add(new Claim("account", account));
            claims.Add(new Claim("accountId", accountId.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

            // 角色
            //claims.Add(new Claim("roles", "Admin"));
            //claims.Add(new Claim("roles", "Users"));

            // 授權功能
            if (_authorizationFunctionNameDict is null)
            {
                // 組dict
                _authorizationFunctionNameDict = new Dictionary<string, string>();
                var enumType = typeof(AuthorizationFunctionEnum);
                foreach (var functionName in Enum.GetNames(enumType))
                {
                    var enumItem = (AuthorizationFunctionEnum)Enum.Parse(typeof(AuthorizationFunctionEnum), functionName);
                    var memberInfos = enumType.GetMember(enumItem.ToString());
                    var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                    var valueAttributes =
                          enumValueMemberInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
                    var name = ((DisplayAttribute)valueAttributes[0]).Name;
                    _authorizationFunctionNameDict.Add(name, functionName);
                }
            }

            //foreach (var keyValuePair in authorizatedDict)
            //{
            //    var key = keyValuePair.Key;

            //    if (!_authorizationFunctionNameDict.ContainsKey(key))
            //    {
            //        continue;
            //    }

            //    var functionEnglistName = _authorizationFunctionNameDict[keyValuePair.Key];
            //    // value = readonly
            //    claims.Add(new Claim(functionEnglistName, keyValuePair.Value.ToString()));
            //}

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 key 不能太短，至少要 16 字元以上
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Audience = issuer, // 由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
                //NotBefore = DateTime.Now, // 預設值就是 DateTime.Now
                //IssuedAt = DateTime.Now, // 預設值就是 DateTime.Now
                Expires = DateTime.Now.AddMonths(6),
                Issuer = issuer,
                Subject = userClaimsIdentity,
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
