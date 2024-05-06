using CommonLibrary.Util;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonLibrary.Crypto
{
    public class SHAHelper
    {
        public static string GetSHA256String(string original)
        {
            try
            {
                var encoding = new UTF8Encoding();
                var messageBytes = encoding.GetBytes(original);

                using (var sha256 = new SHA256CryptoServiceProvider())
                {
                    var hashMessage = sha256.ComputeHash(messageBytes);
                    var result = Convert.ToBase64String(hashMessage);

                    return result;
                }
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("GetSHA256String失敗", ex);
                throw;
            }

        }

        public static string ComputeSha256Hash(string input)
        {
            // Create a new instance of the SHA256CryptoServiceProvider object.
            using SHA256 aha256 = SHA256.Create();   
            byte[] data = aha256.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sb = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();    
        }
    }
}
