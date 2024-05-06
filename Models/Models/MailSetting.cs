namespace Models.Models
{
    public class MailSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SendName { get; set; }
        public string SendFrom { get; set; }
        public string UserName { get; set; }
        public string UserMima { get; set; }
        public double FileSize { get; set; }
        public bool IsSSL { get; set; }
    }
}
