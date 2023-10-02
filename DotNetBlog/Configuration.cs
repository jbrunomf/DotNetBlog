namespace DotNetBlog;

public static class Configuration
{
    public static string JwtKey = "chavejwt";
    public static string ApiKeyName = "api-key";
    public static string ApiKey = "api-key-teste";
    public static SmtpConfiguration Smtp = new(); 
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password  { get; set; }
    }
}