namespace Models.Models
{
    public class SendMailObject : MailSetting
    {
        public string[] Mails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
