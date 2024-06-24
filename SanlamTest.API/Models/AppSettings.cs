namespace SanlamTest.API.Models
{
    public class AppSettings
    {

        public Jwt Jwt { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public string AllowedHosts { get; set; }
    }
    public class Jwt
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
    }

    public class Connectionstrings
    {
        public string SanlamDb { get; set; }
    }

}