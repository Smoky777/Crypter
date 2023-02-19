using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO.Compression;

namespace Crypter
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void BtnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TxtExe.Text = ofd.FileName;
            }
        }


        private void BtnCrypt_Click(object sender, EventArgs e)
        {
            string inputFile = TxtExe.Text;
            string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + "_monfichier.exe");

            // Define the encryption key
            byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
                                      0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20 };

            byte[] inputBytes = File.ReadAllBytes(inputFile);

            // Encrypt the input file using the encryption key
            byte[] encrypted = EncryptBytes(inputBytes, key);

            // Write the encrypted bytes to a new file with "_monfichier.exe" extension
            
            using (MemoryStream ms = new MemoryStream())
            {
                // Add the "DECOMPRESS" signature to the beginning of the decompression data
                ms.Write(Encoding.ASCII.GetBytes("DECOMPRESS"), 0, "DECOMPRESS".Length);

                // Add the encryption key to the decompression data
                ms.Write(key, 0, key.Length);

                // Add the encrypted data to the decompression data
                ms.Write(encrypted, 0, encrypted.Length);

                // Compress the decompression data using GZip
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gs.Write(ms.GetBuffer(), 0, (int)ms.Length);
                }

                // Write the compressed data to the output file
                File.WriteAllBytes(outputFile, ms.ToArray());

                MessageBox.Show("Cryptage terminé");
            }

            // Delete the original file
            File.Delete(inputFile);
        }



        private byte[] EncryptBytes(byte[] inputBytes, byte[] key)
        {

            // Create an Aes object with the specified key and IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Create a random initialization vector (IV) to use with the algorithm
                aesAlg.GenerateIV();

                // Create an encryptor to perform the stream transform
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Write the initialization vector to the beginning of the stream
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                        // Write the encrypted data to the stream
                        csEncrypt.Write(inputBytes, 0, inputBytes.Length);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }


        public static byte[] DecryptStringFromByte(byte[] cipherText, byte[] key)
        {
            byte[] decrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                byte[] iv = new byte[16];
                Array.Copy(cipherText, iv, 16);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    ICryptoTransform decoder = aes.CreateDecryptor();
                    using (CryptoStream csDecryptor = new CryptoStream(msDecrypt, decoder, CryptoStreamMode.Read))
                    using (BinaryReader brReader = new BinaryReader(csDecryptor))
                    {
                        decrypted = brReader.ReadBytes(cipherText.Length - 16);
                    }
                }
            }

            return decrypted;
        }
    }

}

 
