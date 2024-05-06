using CommonLibrary.Util;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommonLibrary.Crypto
{
    public class RSAHelper
    {

        #region microsoft rsa 2048
        //public static void CreateRsaKey()
        //{
        //    RSACryptoServiceProvider rsa = null;
        //    try
        //    {
        //        rsa = new RSACryptoServiceProvider(2048);
        //        var publicKey = rsa.ToXmlString(false);
        //        var privateKey = rsa.ToXmlString(true);

        //    }
        //    catch (Exception ex) when (ex is Exception)
        //    {
        //        LogUtility.LogError("RSA使用私鑰解密失敗", ex);
        //        throw;
        //    }
        //    finally
        //    {
        //        if (rsa != null)
        //        {
        //            rsa.Clear();
        //        }
        //    }
        //}

        public static string Decrypt(string privateKey, string content)
        {
            RSACryptoServiceProvider rsa = null;
            try
            {
                rsa = new RSACryptoServiceProvider(2048);
                rsa.FromXmlString(privateKey);

                var result = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(content), RSAEncryptionPadding.OaepSHA1));

                return result;
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("RSA使用私鑰解密失敗", ex);
                throw;
            }
            finally
            {
                if (rsa != null)
                {
                    rsa.Clear();
                }
            }
        }

        public static string Encrypt(string publicKey, string content)
        {
            RSACryptoServiceProvider rsa = null;
            try
            {
                //var rsa = ImportPublicKey(publicKey);
                rsa = new RSACryptoServiceProvider(2048);
                rsa.FromXmlString(publicKey);

                var result = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(content), RSAEncryptionPadding.OaepSHA1));

                return result;
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("RSA使用公鑰加密失敗", ex);
                throw;
            }
            finally
            {
                if (rsa != null)
                {
                    rsa.Clear();
                }
            }
        }
        #endregion

        #region Rsa 2048 (Bouncy Castle)
        private const string privateContentStartWord = "-----BEGIN DIR_MONITOR RP KEY-----";
        private const string privateContentEndWord = "-----END DIR_MONITOR RP KEY-----";

        private const string publicContentStartWord = "-----BEGIN PUBLIC KEY-----";
        private const string publicContentEndWord = "-----END PUBLIC KEY-----";

        // RSA加解密:
        // 1024位的证书，加密时最大支持117个字节，解密时为128；
        // 2048位的证书，加密时最大支持245个字节，解密时为256。


        ///// <summary>
        ///// BouncyCastle, Decrypt, 無分段處理
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="privateContent"></param>
        ///// <returns></returns>
        //public static string Rsa2048Decrypt(string context, string privateContent)
        //{
        //    context = context.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        //    //非對稱加密演算法，加解密用  
        //    var engine = new Pkcs1Encoding(new RsaEngine());

        //    engine.Init(false, GetPrivateKeyParameter(privateContent));
        //    var byteData = Convert.FromBase64String(context);
        //    var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);

        //    return Encoding.UTF8.GetString(ResultData);

        //}

        ///// <summary>
        ///// BouncyCastle, Encrypt, 無分段處理
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="publicContent"></param>
        ///// <returns></returns>
        //public static string Rsa2048Encrypt(string context, string publicContent)
        //{
        //    //非對稱加密演算法，加解密用  
        //    var engine = new Pkcs1Encoding(new RsaEngine());

        //    engine.Init(true, GetPublicKeyParameter(publicContent));
        //    var byteData = Encoding.UTF8.GetBytes(context);
        //    var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);

        //    return Convert.ToBase64String(ResultData);

        //}

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static string EncryptByBouncyCastle(string plainText, string key, bool isPublic = true)
        {
            //非对称加密算法，加解密用  
            var engine = new Pkcs1Encoding(new RsaEngine());
            //加密  
            try
            {
                engine.Init(true, isPublic ? GetPublicKeyParameter(key) : GetPrivateKeyParameter(key));
                byte[] byteData = System.Text.Encoding.UTF8.GetBytes(plainText);

                int inputLen = byteData.Length;
                MemoryStream ms = new MemoryStream();
                int offSet = 0;
                byte[] cache;
                int i = 0;

                while (inputLen - offSet > 0)
                {
                    if (inputLen - offSet > 245) // keysize
                    {
                        cache = engine.ProcessBlock(byteData, offSet, 245);
                    }
                    else
                    {
                        cache = engine.ProcessBlock(byteData, offSet, inputLen - offSet);
                    }
                    ms.Write(cache, 0, cache.Length);
                    i++;
                    offSet = i * 245;
                }
                byte[] encryptedData = ms.ToArray();

                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("(BouncyCastle)RSA使用加密失敗", ex);
                throw;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static string DecryptByBouncyCastle(string cipherText, string key, bool isPublic = false)
        {
            cipherText = cipherText.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            var engine = new Pkcs1Encoding(new RsaEngine());

            try
            {
                engine.Init(false, isPublic ? GetPublicKeyParameter(key) : GetPrivateKeyParameter(key));
                byte[] byteData = Convert.FromBase64String(cipherText);

                int inputLen = byteData.Length;
                MemoryStream ms = new MemoryStream();
                int offSet = 0;
                byte[] cache;
                int i = 0;

                while (inputLen - offSet > 0)
                {
                    if (inputLen - offSet > 256)
                    {
                        cache = engine.ProcessBlock(byteData, offSet, 256);
                    }
                    else
                    {
                        cache = engine.ProcessBlock(byteData, offSet, inputLen - offSet);
                    }
                    ms.Write(cache, 0, cache.Length);
                    i++;
                    offSet = i * 256;
                }
                byte[] encryptedData = ms.ToArray();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex) when (ex is Exception)
            {
                LogUtility.LogError("(BouncyCastle)RSA使用解密失敗", ex);
                throw;
            }
        }

        /// <summary>
        /// 取得實際使用的公鑰資料
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        private static AsymmetricKeyParameter GetPublicKeyParameter(string publicKey)
        {
            if (!publicKey.Contains(publicContentStartWord) || !publicKey.Contains(publicContentEndWord))
                throw new ArgumentNullException("金鑰不合法，請確認");

            publicKey = publicKey.Replace(publicContentStartWord, "");
            publicKey = publicKey.Replace(publicContentEndWord, "");
            publicKey = publicKey.Replace("\r", "").Replace("\n", "").Replace(" ", "");

            var publicInfoByte = Convert.FromBase64String(publicKey);
            var pubKey = PublicKeyFactory.CreateKey(publicInfoByte);

            return pubKey;
        }

        /// <summary>
        /// 取得實際使用的私鑰資料
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        private static AsymmetricKeyParameter GetPrivateKeyParameter(string privateKey)
        {
            if (!privateKey.Contains(privateContentStartWord) || !privateKey.Contains(privateContentEndWord))
                throw new Exception("金鑰不合法，請確認");

            privateKey = privateKey.Replace(privateContentStartWord, "");
            privateKey = privateKey.Replace(privateContentEndWord, "");
            privateKey = privateKey.Replace("\r", "").Replace("\n", "").Replace(" ", "");

            var privateInfoByte = Convert.FromBase64String(privateKey);
            var priKey = PrivateKeyFactory.CreateKey(privateInfoByte);

            return priKey;
        }

        /// <summary>
        /// BouncyCastle, 產出 size為2048的key
        /// Dictionary Key 分別為 publicKey、privateKey
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetRsaPublicAndPrivateContent()
        {
            //RSA金鑰對的構造器  
            var keyGenerator = new RsaKeyPairGenerator();

            //RSA金鑰構造器的引數  
            var param = new RsaKeyGenerationParameters(
                BigInteger.ValueOf(3),
                new SecureRandom(),
                2048,   //金鑰長度  
                5);
            //用引數初始化金鑰構造器  
            keyGenerator.Init(param);
            //產生金鑰對  
            var keyPair = keyGenerator.GenerateKeyPair();
            //獲取公鑰和金鑰  
            var publicKey = keyPair.Public;
            var privateKey = keyPair.Private;

            var subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);

            var asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();
            var publicInfoByte = asn1ObjectPublic.GetEncoded("UTF-8");

            var asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            var privateInfoByte = asn1ObjectPrivate.GetEncoded("UTF-8");

            var publicKeyBase64 = Convert.ToBase64String(publicInfoByte);
            var privateKeyBase64 = Convert.ToBase64String(privateInfoByte);

            var executeTime = DateTime.Now.ToString("HHmmss");

            var result = new Dictionary<string, string>
            {
                { $"{executeTime}_rsa_public_key", GetRsaPemFileContent(publicKeyBase64, false) },
                { $"{executeTime}_rsa_private_key", GetRsaPemFileContent(privateKeyBase64, true) }
            };

            return result;
        }

        /// <summary>
        /// 產出pem format檔案
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="isPrivatekey"></param>
        /// <returns></returns>
        private static string GetRsaPemFileContent(string base64, bool isPrivatekey)
        {
            var keyArray = base64.ToCharArray();
            var outputStream = new StringWriter();

            if (isPrivatekey)
                outputStream.Write($"{privateContentStartWord}\n");
            else
                outputStream.Write($"{publicContentStartWord}\n");

            for (var i = 0; i < keyArray.Length; i += 64)
            {
                outputStream.Write(keyArray, i, Math.Min(64, keyArray.Length - i));
                outputStream.Write("\n"); // 統一使用\n產生就好
            }

            if (isPrivatekey)
                outputStream.Write($"{privateContentEndWord}");
            else
                outputStream.Write($"{publicContentEndWord}");

            return outputStream.ToString();
        }


        public static bool IsPublicKey(string key)
        {
            if (key.Contains(publicContentStartWord) && key.Contains(publicContentEndWord))
                return true;

            if (key.Contains(privateContentEndWord) && key.Contains(privateContentEndWord))
                return false;

            throw new Exception("金鑰格式錯誤");
        }
        #endregion
    }
}
