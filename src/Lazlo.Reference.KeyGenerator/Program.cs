using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;
using ThreeTwoSix.SDK.Cryptography;
using ThreeTwoSix.SDK.Cryptography.Helpers;

namespace Lazlo.Reference.KeyGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var options = CommandLine.Parser.Default.ParseArguments<ArgOptions>(args);

            var outputPath = options.Value.OutputPath;

            if (options.Value.Pwd.Trim() != options.Value.PwdConfirm.Trim())
            {
                Console.WriteLine("Passwords do NOT match.");
            }

            //Generate a public/private key pair.  
            RSA rsa = RSA.Create(2048);

            var privatePem = PemHelper.ExportKey(rsa.ExportParameters(true), true);

            using SHA256 hasher = SHA256.Create();

            var pwdHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(options.Value.Pwd.Trim()));

            byte[] encrypted;

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                //aes.BlockSize = 256;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = pwdHash;
                aes.GenerateIV();

                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor();
                // Create MemoryStream    
                using (MemoryStream ms = new())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                    // Create StreamWriter and write data to a stream    
                    using (StreamWriter sw = new StreamWriter(cs))
                        sw.Write(privatePem);

                    encrypted = ms.ToArray();

                    File.WriteAllBytes($@"{outputPath}\private-key.data", encrypted);
                    File.WriteAllText($@"{outputPath}\private-key-decrypt.json",
                        JsonConvert.SerializeObject(
                            new
                            {
                                keyCypherText = Convert.ToBase64String(rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1)),
                                iv = Convert.ToBase64String(aes.IV),
                                publicKey = PemHelper.ExportKey(rsa.ExportParameters(false), false)
                            }
                        )
                    );
                }

                ICryptoTransform decryptor = aes.CreateDecryptor();
                // Create the streams used for decryption.    
                using (MemoryStream ms = new(encrypted))
                {
                    // Create crypto stream    
                    using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                    // Read crypto stream    
                    using StreamReader reader = new StreamReader(cs);

                    var plaintext = reader.ReadToEnd();

                    if (plaintext.GetHashCode() == privatePem.GetHashCode())
                    {
                        Console.WriteLine("Encrypted and decrypted with Pwd to verify process.");
                    }
                }
            }



            //File.WriteAllText($@"c:\temp\private-key.pem", privatePem);

            var publicPem = PemHelper.ExportKey(rsa.ExportParameters(false), false);
            File.WriteAllText($@"{outputPath}\public-key.pem", publicPem);
        }
    }
}