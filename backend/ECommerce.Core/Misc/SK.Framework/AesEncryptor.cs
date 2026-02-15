namespace SK.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Provides URL-safe AES encryption and decryption functionality
/// </summary>
public class AesEncryptor
{
    private const int Iterations = 5000;
    private const int KeySize = 256;
    private const int BlockSize = 128;

    private readonly byte[] _saltBytes;

    public AesEncryptor(string salt)
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


        // Generate a random IV
        byte[] iv = RandomNumberGenerator.GetBytes(BlockSize / 8);

        // Encrypt the data
        byte[] encrypted;
        using (MemoryStream memoryStream = new())
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
                using (StreamWriter writer = new(cryptoStream))
                {
                    writer.Write(plainText);
                }

                encrypted = memoryStream.ToArray();
            }
        }

        // Combine IV and encrypted data
        byte[] combined = new byte[iv.Length + encrypted.Length];
        Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
        Buffer.BlockCopy(encrypted, 0, combined, iv.Length, encrypted.Length);

        // Convert to URL-safe Base64 string
        string base64 = Convert.ToBase64String(combined);
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

        byte[] combined = Convert.FromBase64String(normalizedBase64);

        // Extract IV and ciphertext
        byte[] iv = new byte[BlockSize / 8];
        byte[] cipherBytes = new byte[combined.Length - iv.Length];
        Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(combined, iv.Length, cipherBytes, 0, cipherBytes.Length);

        // Generate the same key using PBKDF2
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: _saltBytes,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize / 8
        );

        // Decrypt the data
        string plaintext;
        using (MemoryStream memoryStream = new(cipherBytes))
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;// sherifr: outdated method

            using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);
            plaintext = reader.ReadToEnd();
        }

        return plaintext;
    }

    /// <summary>
    /// Encrypts an integer value
    /// </summary>
    /// <param name="value">The integer to encrypt</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>URL-safe encrypted string</returns>
    public string EncryptInt(int value, string password)
    {
        return EncryptString(value.ToString(), password);
    }

    /// <summary>
    /// Decrypts an encrypted integer value
    /// </summary>
    /// <param name="encryptedText">The URL-safe encrypted string</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>The decrypted integer value</returns>
    public int DecryptInt(string encryptedText, string password)
    {
        string decrypted = DecryptString(encryptedText, password);
        if (int.TryParse(decrypted, out int result))
            return result;
        throw new FormatException("The decrypted text is not a valid integer.");
    }

    /// <summary>
    /// Encrypts a long value
    /// </summary>
    /// <param name="value">The long to encrypt</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>URL-safe encrypted string</returns>
    public string EncryptLong(long value, string password)
    {
        return EncryptString(value.ToString(), password);
    }

    /// <summary>
    /// Decrypts an encrypted long value
    /// </summary>
    /// <param name="encryptedText">The URL-safe encrypted string</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>The decrypted long value</returns>
    public long DecryptLong(string encryptedText, string password)
    {
        string decrypted = DecryptString(encryptedText, password);
        if (long.TryParse(decrypted, out long result))
            return result;

        throw new FormatException("The decrypted text is not a valid long.");
    }

    /// <summary>
    /// Encrypts a short value
    /// </summary>
    /// <param name="value">The short to encrypt</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>URL-safe encrypted string</returns>
    public string EncryptShort(short value, string password)
    {
        return EncryptString(value.ToString(), password);
    }

    /// <summary>
    /// Decrypts an encrypted short value
    /// </summary>
    /// <param name="encryptedText">The URL-safe encrypted string</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>The decrypted short value</returns>
    public short DecryptShort(string encryptedText, string password)
    {
        string decrypted = DecryptString(encryptedText, password);
        if (short.TryParse(decrypted, out short result))
            return result;
        throw new FormatException("The decrypted text is not a valid short.");
    }
}