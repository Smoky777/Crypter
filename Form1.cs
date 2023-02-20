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
using System.Runtime.Serialization.Formatters.Binary;

namespace Crypter
{
    public partial class Form1 : Form
    {
        private const string KeyString = "0123456789ABCDEF0123456789ABCDEF";
        private const string IVString = "0123456789ABCDEF";
        byte[] key = Encoding.ASCII.GetBytes(KeyString);
        byte[] iv = Encoding.ASCII.GetBytes(IVString);
        private const int headerSize = 20; // la taille du header du fichier chiffré

        [Serializable]
        struct FileHeader
        {
            public string Signature; // une signature pour indiquer qu'il s'agit bien d'un fichier chiffré
            public byte[] Key; // la clé de chiffrement, de taille 16 octets (128 bits)
        }



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

            // Chiffre le fichier d'entrée en utilisant la clé de chiffrement
            EncryptFile(inputFile, outputFile, key, iv);

            // Déchiffre le fichier de sortie en utilisant la clé de chiffrement
            DecryptFile(outputFile, key, iv);

            MessageBox.Show("Cryptage terminé");
        }





        private void EncryptFile(string inputFile, string outputFile, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Définit l'en-tête du fichier
                FileHeader header;
                header.Signature = "CRYPTER_V1.0";
                header.Key = key;

                // Lit le fichier d'entrée
                byte[] inputBytes = File.ReadAllBytes(inputFile);

                // Chiffre le fichier d'entrée en utilisant la clé de chiffrement
                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                    // Write the file header and encrypted bytes to the output file
                    // Écrit l'en-tête et le contenu chiffré dans le fichier de sortie
                    using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(outputStream, header);
                        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                }
            }
        }

        private void DecryptFile(string inputFile, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Read the encrypted bytes from the input file
                byte[] encryptedBytes;
                using (FileStream fs = new FileStream(inputFile, FileMode.Open))
                {
                    // Ignore the first 7 bytes ("DECRYPT" signature and key size)
                    fs.Seek(7, SeekOrigin.Begin);

                    // Read the key size
                    byte[] keySizeBytes = new byte[sizeof(int)];
                    fs.Read(keySizeBytes, 0, keySizeBytes.Length);
                    int keySize = BitConverter.ToInt32(keySizeBytes, 0);

                    // Read the key
                    byte[] keyBytes = new byte[keySize];
                    fs.Read(keyBytes, 0, keyBytes.Length);


                    // Ajuster la longueur de la clé si nécessaire
                    if (keyBytes.Length != aesAlg.KeySize / 8)
                    {
                        Array.Resize(ref keyBytes, aesAlg.KeySize / 8);
                    }

                    // Définir la clé pour l'algorithme AES
                    aesAlg.Key = keyBytes;


                    // Decrypt the rest of the file
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                    {
                        using (MemoryStream msDecrypt = new MemoryStream())
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                            {
                                csDecrypt.CopyTo(msDecrypt);
                                encryptedBytes = msDecrypt.ToArray();
                            }
                        }
                    }
                }

                // Decrypt the bytes
                byte[] decryptedBytes = DecryptBytes(encryptedBytes, key, iv);

                // Save the decrypted bytes to the original file
                File.WriteAllBytes(inputFile, decryptedBytes);
            }
        }

        private byte[] DecryptBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(cipherText, 0, cipherText.Length);
                    }

                    return msDecrypt.ToArray();
                }
            }
        }



    }

}



 
