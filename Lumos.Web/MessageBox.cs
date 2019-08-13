using System;


namespace Lumos.Web
{

    public enum MessageBoxType
    {
        Warn = 1,
        Success = 2,
        Failure = 3,
        Exception = 4
    }

    public class MessageBox
    {
        public string No { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPopup { get; set; }
        public string ErrorStackTrace { get; set; }
        public bool IsTop { get; set; }
        public string RedirectUrl { get; set; }
        public MessageBoxType Type { get; set; }
    }
}
