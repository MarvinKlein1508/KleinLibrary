namespace KleinLibrary.Emails
{
    public class Email
    {
        public string Betreff { get; set; } = string.Empty;
        public string Inhalt { get; set; } = string.Empty;

        public List<string> Empfänger { get; set; } = new();
        public List<string> CC { get; set; } = new();
        public List<string> BCC { get; set; } = new();
        public bool HtmlMail { get; set; } = true;

        public List<EmailAnhang> Anhänge { get; set; } = new();
    }
}
