using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Config
{
    public class WebServiceSetting
    {
        public RelationDB RelationDB { get; set; }
        public bool EnableSwagger { get; set; }
        public EncryptKeysModel EncryptKeys { get; set;}
        public JwtSettings JwtSettings { get; set; }
        public int SessionExpireDuration { get; set; }
        public bool EnableEncrypt { get; set; }
        public string MailSender { get; set; }
        public string EMAIL_IMG_URL { get; set; }
        public string HttpWebRequestHost { get; set; }
        public SendNowApiSetting SendNowApiSetting { get; set; }
        public string SignTermsCaseNum { get; set; }
        //----------------CMS-----------------------------------        
        public PushSettings PushSettings { get; set; }
        public int AgreeTermsCountdown { get; set; }
        public int OTPSecend { get; set; }
        public TrueIdSetting TrueIdSetting { get; set; }      
        public SMSSetting SMSSetting { get; set; }      
        public int UploadImgLimit { get; set; }
        public int TokenExpireDuration { get; set; }
        //public WebServiceSetting()
        //{

        //}
        /// <summary>
        /// return Detionary 
        /// </summary>
        public Dictionary<string, string> InitDictionary()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Global.key".ToLower(), "StatusAndNoticeSupdoc");
            //result.Add("Global.key".ToLower(), "Dynamiclink");
            result.Add("StatusAndNoticeSupdoc".ToLower(), "");
            result.Add("Dynamiclink".ToLower(), "");
            return result;
        }
    }

    public class SendNowApiSetting
    {
        public string Url { get; set; }
    }

    public class SMSSetting
    {
        public string Url { get; set; }
        public string ApplicationName { get; set; }
        public string FailCampaignCode { get; set; }
        public string SuccessCampaignCode { get; set; }
        public string OtpCampaignCode { get; set; }
        public string OtpFailTooManyTimeCampaignCode { get; set; }
        public string LoginSuccessCampaignCode { get; set; }
        public string GraphicLoginSuccessCampaignCode { get; set; }
        public string BiometricsLoginSuccessCampaignCode { get; set; }
        public string BindingSuccessCampaignCode { get; set; }
        public string RemittanceTransferCampaignCode { get; set; }
    }

    public class TrueIdSetting
    {
        public string Host { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
    }

    public class PushSettings
    {
        public string Url { get; set; }
        public string HeaderKey { get; set; }
        public string SenderId { get; set; }
        public string ServerKey { get; set; }
    }

    public class RelationDB
    {       
        public string DIR_ConnectionString { get; set; }       
    }

    public class EncryptKeysModel
    {
        public AESGCMModel AESGCM { get; set; }
        public AESModel AES { get; set; }
        public DESModel DES { get; set; }
        public RSAModel RSA { get; set; }
    }

    public class AESGCMModel
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }

    public class AESModel
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }

    public class DESModel
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }

    public class RSAModel
    {
        public string Public { get; set; }
        public string Private { get; set; }
    }

    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string SignKey { get; set; }
    }
}
