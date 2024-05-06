using System;

namespace Domain.Models.Push
{
    public class PushData
    {
        public string AppCode { get; set; }
        public string UserCode { get; set; }
        public string DeviceType { get; set; }
        public string DeviceId { get; set; }
        public string DevicePushToken { get; set; }
        public string DeviceName { get; set; }
        public int DeviceBadge { get; set; }
        public string MessageId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Url { get; set; }
        public string StartDate { get; set; }
        public string AppSenderId { get; set; }
        public string AppAuthToken { get; set; }

        public PushData()
        {
            AppCode = "DIR";
            Title = string.Empty;
            StartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}