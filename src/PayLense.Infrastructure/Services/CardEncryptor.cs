using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using PayLense.Application.Interfaces;

namespace PayLense.Infrastructure.Services;

/// AES-256-GCM. الـ key جاي من config "Encryption:Key" (Base64 لـ 32 byte).
/// الناتج: Base64( nonce[12] + tag[16] + ciphertext )
public class CardEncryptor : ICardEncryptor
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public CardEncryptor(IConfiguration config)
    {
        var base64 = config["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption:Key is missing.");
        _key = Convert.FromBase64String(base64);
        if (_key.Length != 32)
            throw new InvalidOperationException("Encryption:Key must be 32 bytes (Base64).");
    }

    public string Encrypt(string plainText)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var plain = Encoding.UTF8.GetBytes(plainText);
        var cipher = new byte[plain.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plain, cipher, tag);

        var result = new byte[NonceSize + TagSize + cipher.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
        Buffer.BlockCopy(cipher, 0, result, NonceSize + TagSize, cipher.Length);
        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);
        var nonce = data[..NonceSize];
        var tag = data[NonceSize..(NonceSize + TagSize)];
        var cipher = data[(NonceSize + TagSize)..];
        var plain = new byte[cipher.Length];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipher, tag, plain);
        return Encoding.UTF8.GetString(plain);
    }
}
