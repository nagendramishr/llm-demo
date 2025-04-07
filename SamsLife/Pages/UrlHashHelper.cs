using System.Security.Cryptography;
using System.Text;

public class UrlHashHelper
{    
    private readonly IConfiguration configuration;
    private readonly string secretKey;

    public UrlHashHelper(IConfiguration config)
    {
        configuration = config;
        secretKey = configuration["Hash"]!;
        
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentException("Secret key is not configured.");
        }
    }

    public string GenerateHash(string boardId)
    {
        var encoding = new UTF8Encoding();
        byte[] keyBytes = encoding.GetBytes(secretKey);
        byte[] messageBytes = encoding.GetBytes(boardId);

        using var hmacsha256 = new HMACSHA256(keyBytes);
        byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
        return Convert.ToBase64String(hashMessage)
                      .Replace("+", "-")
                      .Replace("/", "_")
                      .Replace("=", "");
    }

    public bool VerifyHash(string boardId, string hash)
    {
        var expectedHash = GenerateHash(boardId);
        return expectedHash == hash;
    }
}