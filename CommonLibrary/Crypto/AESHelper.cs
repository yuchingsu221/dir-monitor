using CommonLibrary.Util;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonLibrary.Crypto
{
    public class AESHelper
    {
        public const string AES_CBC_PKCS7Padding = "AES/CBC/PKCS7Padding";
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">待加密原文資料</param>
        /// <param name="key">金鑰</param>
        /// <param name="iv">偏移量，ECB模式不用填寫！</param>
        /// <param name="algorithm">密文演算法</param>
        /// <returns>密文資料</returns>
        public static string Encrypt(string original, string ase_K, string aes_I, string algorithm = "")
        {
            var data = Encoding.UTF8.GetBytes(original);
            var key = Encoding.UTF8.GetBytes(ase_K);
            var iv = Encoding.UTF8.GetBytes(aes_I);

            if (string.IsNullOrWhiteSpace(algorithm))
            {
                algorithm = AES_CBC_PKCS7Padding;
            }

            var cipher = CipherUtilities.GetCipher(algorithm);

            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iv));

            var result = cipher.DoFinal(data);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">待解密資料</param>
        /// <param name="key">金鑰</param>
        /// <param name="iv">偏移量，ECB模式不用填寫！</param>
        /// <param name="algorithm">密文演算法</param>
        /// <returns>未加密原文資料</returns>
        public static string Decrypt(string original, string ase_K, string aes_I, string algorithm = "")
        {

            var data = Convert.FromBase64String(original);
            var key = Encoding.UTF8.GetBytes(ase_K);
            var iv = Encoding.UTF8.GetBytes(aes_I);

            if (string.IsNullOrWhiteSpace(algorithm))
            {
                algorithm = AES_CBC_PKCS7Padding;
            }

            var cipher = CipherUtilities.GetCipher(algorithm);

            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iv));

            var result = cipher.DoFinal(data);

            return Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// AES 256 解密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="ase_K"></param>
        /// <param name="aes_I"></param>
        /// <returns></returns>
        public static string Decrypt256(string original, string ase_K, string aes_I)
        {
            //RijndaelManaged rDel = null;
            try
            {
                //var keyArray = Encoding.UTF8.GetBytes(ase_K);
                //var ivArray = Encoding.UTF8.GetBytes(aes_I);
                //var originalArray = Convert.FromBase64String(original);

                //rDel = new RijndaelManaged
                //{
                //    KeySize = 256,
                //    Key = keyArray,
                //    IV = ivArray,
                //    Mode = CipherMode.CBC,
                //    Padding = PaddingMode.PKCS7
                //};

                //using ICryptoTransform cTransform = rDel.CreateDecryptor();
                //var resultArray = cTransform.TransformFinalBlock(originalArray, 0, originalArray.Length);

                //return Encoding.UTF8.GetString(resultArray);

                return Decrypt(original, ase_K, aes_I, null);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("AES 256 Decrypt Error!", ex);
                throw;
            }
            //finally
            //{
            //    if (rDel != null) { rDel.Clear(); }
            //}
        }



        /// <summary>
        ///  AES 256 加密
        /// </summary>
        /// <param name="strOriginal"></param>
        /// <param name="strKey"></param>
        /// <param name="strIV"></param>
        /// <returns></returns>
        public static string Encrypt256(string original, string ase_K, string aes_I)
        {
            RijndaelManaged rDel = null;

            try
            {
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(original);
                byte[] keyArray = Encoding.UTF8.GetBytes(ase_K);
                byte[] IvArray = Encoding.UTF8.GetBytes(aes_I);

                rDel = new RijndaelManaged
                {
                    KeySize = 256,
                    Key = keyArray,
                    IV = IvArray,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("AES 256 Encrypt Error!", ex);
                throw;
            }
            finally
            {
                if (rDel != null)
                {
                    rDel.Clear();
                }
            }
        }

        /// <summary>
        ///  AES 256 GCM 加密 結果為base64(密文+TAG)
        /// </summary>
        /// <param name="strOriginal"></param>
        /// <param name="strKey"></param>
        /// <param name="strIV"></param>
        /// <returns></returns>
        public static string AES256GcmEncrypt(string original, string ase_K, string aes_I)
        {
            try
            {
                var toEncryptArray = Encoding.UTF8.GetBytes(original); // 原文
                var keyArray = Encoding.UTF8.GetBytes(ase_K);
                var ivArray = Encoding.UTF8.GetBytes(aes_I);

                var cliper = new byte[toEncryptArray.Length];    // 密文長度
                var tag = new byte[16];
                byte[] resultArray;

                using (var aes = new AesGcm(keyArray))
                {
                    aes.Encrypt(ivArray, toEncryptArray, cliper, tag);
                    resultArray = new byte[cliper.Length + tag.Length];

                    Buffer.BlockCopy(cliper, 0, resultArray, 0, cliper.Length);
                    Buffer.BlockCopy(tag, 0, resultArray, cliper.Length, tag.Length);
                }

                return Convert.ToBase64String(resultArray);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("AES 256 GCM Encrypt Error!", ex);
                throw;
            }
        }

        /// <summary>
        /// AES 256 GCM 解密 base64(密文+TAG)解開後分別存兩個byte[]再進行解密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="ase_K"></param>
        /// <param name="aes_I"></param>
        /// <returns></returns>
        public static string AES256GcmDecrypt(string original, string ase_K, string aes_I)
        {
            try
            {
                var keyArray = Encoding.UTF8.GetBytes(ase_K);
                var ivArray = Encoding.UTF8.GetBytes(aes_I);
                var originalArray = Convert.FromBase64String(original);

                var cliper = new byte[originalArray.Length - 16];
                var tag = new byte[16];
                var resultArray = new byte[originalArray.Length - 16];

                Buffer.BlockCopy(originalArray, 0, cliper, 0, originalArray.Length - 16);
                Buffer.BlockCopy(originalArray, cliper.Length, tag, 0, tag.Length);

                using (var aes = new AesGcm(keyArray))
                {
                    aes.Decrypt(ivArray, cliper, tag, resultArray);
                }

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("AES 256 GCM Decrypt Error!", ex);
                throw;
            }
        }
    }
}
