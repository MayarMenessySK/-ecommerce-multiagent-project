using System;
using System.IO;
using System.Security.Cryptography;

namespace SK.Framework;

public class SimpleAES
{
    // Change this key per project at https://www.random.org/cgi-bin/randbyte?nbytes=32&format=d
    private readonly byte[] Key = new byte[32] { 35, 219, 136, 181, 231, 209, 54, 203, 60, 90, 14, 105, 37, 204, 12, 147, 59, 60, 70, 28, 70, 52, 227, 196, 58, 2, 43, 182, 107, 216, 159, 241, };

    // Change this key per project https://www.random.org/cgi-bin/randbyte?nbytes=16&format=d
    private readonly byte[] Vector = new byte[16] { 146, 59, 215, 128, 126, 229, 190, 76, 8, 98, 138, 149, 226, 35, 24, 141 };

    private readonly ICryptoTransform EncryptorTransform, DecryptorTransform;
    private readonly System.Text.UTF8Encoding UTFEncoder;

    public SimpleAES()
    {
        //This is our encryption method
        var rm = new RijndaelManaged();

        //Create an encryptor and a decryptor using our encryption method, key, and vector.
        EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
        DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

        //Used to translate bytes to text and vice versa
        UTFEncoder = new System.Text.UTF8Encoding();
    }

    /// <summary>
    /// ----------- The commonly used methods ------------------------------    
    /// Encrypt some text and return a string suitable for passing in a URL.
    /// </summary>
    public string EncryptToString(string TextValue)
    {
        return ByteArrToString(Encrypt(TextValue));
    }

    /// <summary>
    /// Encrypt some text and return an encrypted byte array.
    /// </summary>
    public byte[] Encrypt(string TextValue)
    {
        //Translates our text value into a byte array.
        Byte[] bytes = UTFEncoder.GetBytes(TextValue);

        //Used to stream the data in and out of the CryptoStream.
        var memoryStream = new MemoryStream();

        /*
         * We will have to write the unencrypted bytes to the stream,
         * then read the encrypted result back from the stream.
         */
        #region Write the decrypted value to the encryption stream
        var cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        #endregion

        #region Read encrypted value back out of the stream
        memoryStream.Position = 0;
        byte[] encrypted = new byte[memoryStream.Length];
        memoryStream.Read(encrypted, 0, encrypted.Length);
        #endregion

        //Clean up.
        cs.Close();
        memoryStream.Close();

        return encrypted;
    }

    /// <summary>
    /// The other side: Decryption methods
    /// </summary>
    public string DecryptString(string EncryptedString)
    {
        return Decrypt(StrToByteArray(EncryptedString));
    }

    /// <summary>
    /// Decryption when working with byte arrays.
    /// </summary>
    public string Decrypt(byte[] EncryptedValue)
    {
        #region Write the encrypted value to the decryption stream
        var encryptedStream = new MemoryStream();

        var decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
        decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
        decryptStream.FlushFinalBlock();
        #endregion

        #region Read the decrypted value from the stream.
        encryptedStream.Position = 0;
        Byte[] decryptedBytes = new Byte[encryptedStream.Length];
        encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
        encryptedStream.Close();
        #endregion
        return UTFEncoder.GetString(decryptedBytes);
    }

    /// <summary>
    /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
    ///      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    ///      return encoding.GetBytes(str);
    /// However, this results in character values that cannot be passed in a URL.  So, instead, I just
    /// lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
    ///</summary>
    public byte[] StrToByteArray(string str)
    {
        if (str.Length == 0)
            throw new Exception("Invalid string value in StrToByteArray");

        byte val;
        byte[] byteArr = new byte[str.Length / 3];
        int i = 0;
        int j = 0;
        do
        {
            val = byte.Parse(str.Substring(i, 3));
            byteArr[j++] = val;
            i += 3;
        }
        while (i < str.Length);
        return byteArr;
    }

    /// <summary>
    /// Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
    ///      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
    ///      return enc.GetString(byteArr);
    /// </summary>
    public string ByteArrToString(byte[] byteArr)
    {
        byte val;
        string tempStr = "";
        for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
        {
            val = byteArr[i];
            if (val < 10)
                tempStr += "00" + val.ToString();
            else if (val < 100)
                tempStr += "0" + val.ToString();
            else
                tempStr += val.ToString();
        }
        return tempStr;
    }
}
