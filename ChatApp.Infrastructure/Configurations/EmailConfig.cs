namespace ChatApp.Infrastructure.Configurations
{
    public class EmailConfig
    {
        public static string ConfigName = "MailSettings";
        public string? DefaultSender { get; set; }
        public string? Password { get; set; }
        public string? DisplayName { get; set; }
        public string? Provider { get; set; }
        public int Port { get; set; }
        public string? AppUrl { get; set; }
    }
}
