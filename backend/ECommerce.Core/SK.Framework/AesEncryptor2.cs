using System.Security.Cryptography;
using System.Text;

namespace SK.Framework;

/// <summary>
/// This implementation is less secure than AesEncryptor. Use only when you need a repeatable encrypted string for same input.
/// </summary>
public class AesEncryptor2
{
    private const int Iterations = 5000;
    private const int KeySize = 256;
    private readonly byte[] _saltBytes;

    public AesEncryptor2(string salt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(salt, nameof(salt));
        _saltBytes = Encoding.UTF8.GetBytes(salt);
    }

    public string EncryptString(string plainText, string password)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        // Generate key using PBKDF2
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: _saltBytes,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize / 8
        );


        byte[] encrypted;
        using (MemoryStream memoryStream = new())
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
                using (StreamWriter writer = new(cryptoStream))
                {
                    writer.Write(plainText);
                }

                encrypted = memoryStream.ToArray();
            }
        }

        // Convert to URL-safe Base64 string
        string base64 = Convert.ToBase64String(encrypted);
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public string DecryptString(string encryptedText, string password)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            return string.Empty;

        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        // Convert from URL-safe Base64 to normal Base64
        string normalizedBase64 = encryptedText.Replace('-', '+').Replace('_', '/');
        switch (normalizedBase64.Length % 4)
        {
            case 2: normalizedBase64 += "=="; break;
            case 3: normalizedBase64 += "="; break;
        }

        byte[] cipherBytes = Convert.FromBase64String(normalizedBase64);

        // Generate the same key using PBKDF2
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: _saltBytes,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize / 8
        );

        string plaintext;
        using (MemoryStream memoryStream = new(cipherBytes))
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using StreamReader reader = new(cryptoStream);
                plaintext = reader.ReadToEnd();
            }
        }

        return plaintext;
    }

    public string EncryptInt(int value, string password)
    {
        return EncryptString(value.ToString(), password);
    }

    public int DecryptInt(string encryptedText, string password)
    {
        string decrypted = DecryptString(encryptedText, password);
        if (int.TryParse(decrypted, out int result))
            return result;
        throw new FormatException("The decrypted text is not a valid integer.");
    }

    public string EncryptLong(long value, string password)
    {
        return EncryptString(value.ToString(), password);
    }

    public long DecryptLong(string encryptedText, string password)
    {
        string decrypted = DecryptString(encryptedText, password);
        if (long.TryParse(decrypted, out long result))
            return result;

        throw new FormatException("The decrypted text is not a valid long.");
    }

    public string EncryptShort(short value, string password)
    {
        return EncryptString(value.ToString(), password);
    }

    public short DecryptShort(string encryptedText, string password)
    {
        string decrypted = DecryptString(encryptedText, password);
        if (short.TryParse(decrypted, out short result))
            return result;
        throw new FormatException("The decrypted text is not a valid short.");
    }
}