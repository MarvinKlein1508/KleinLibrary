namespace KleinLibrary.Emails
{
    public struct EmailResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public EmailResponse(bool success = false, string message = "")
        {
            Success = success;
            Message = message;
        }
    }
}
