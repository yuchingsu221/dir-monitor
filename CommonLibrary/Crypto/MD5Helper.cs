using CommonLibrary.Util;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonLibrary.Crypto
{
    public class MD5Helper
    {

        public static string ComputeMd5Hash(string input)
        {
            // Create a new instance of the MD5 CryptoServiceProvider object.
            using MD5 md5 = MD5.Create();
            byte[] buffer = md5.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sb = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("x2"));
            }
            return sb.ToString();    
        }
    }
}
