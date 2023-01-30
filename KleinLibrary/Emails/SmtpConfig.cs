namespace KleinLibrary.Emails
{
    public class SmtpConfig
    {
        public string Host { get; set; } = String.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public bool SSL { get; set; } = true;
        public string AbsenderName { get; set; } = string.Empty;
        public string AbsenderEmail { get; set; } = string.Empty;
        public string ReplyTo { get; set; } = string.Empty;

    }
}
