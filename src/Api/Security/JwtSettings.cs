namespace ProductApi.Security;

public class JwtSettings
{
    public string Issuer { get; set; } = "ProductApi";
    public string Audience { get; set; } = "ProductApiClients";
    public string SecretKey { get; set; } = "super_secret_dev_key_change_me_please_12345";
    public int ExpirationMinutes { get; set; } = 60;
}
