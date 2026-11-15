namespace PayLense.Application.Interfaces;

public interface ICardEncryptor
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
