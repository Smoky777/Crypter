using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Crypter
{
    public partial class Form1 : Form
    {
        private byte[] key = ASCIIEncoding.ASCII.GetBytes("thisismyfuckikey");
        private byte[] IV = ASCIIEncoding.ASCII.GetBytes("thisismytestkeys");

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
            string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "encrypted.exe");
            string inputFileHash = ComputeFileHash(inputFile); // Calcul du hash du fichier
            EncryptFile(inputFile, outputFile);

            string outFileHash = ComputeFileHash(outputFile);
            if (inputFileHash != outFileHash)
            {
                MessageBox.Show("Le fichier a été modifié après son chiffrement !");
                return;
            }

            MessageBox.Show("Le fichier a été chiffré et vérifié avec succès.");

            System.Diagnostics.Process.Start(outputFile);

            MessageBox.Show("Encryption Done");
        }


        public void EncryptFile(string inputFile, string outputFile)
        {
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsEncrypted = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    RijndaelManaged aes = new RijndaelManaged();
                    aes.Key = key;
                    aes.IV = IV;
                    // Écrire la clé et le vecteur d'initialisation au début du fichier
                    fsEncrypted.Write(key, 0, key.Length);
                    fsEncrypted.Write(IV, 0, IV.Length);
                    using (CryptoStream cs = new CryptoStream(fsEncrypted, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, bytesRead);
                        }
                        cs.FlushFinalBlock();
                    }
                }
            }
        }

        public void DecryptFile(string inputFile, string outputFile)
        {
            // Lire la clé de chiffrement du début du fichier
            byte[] key = new byte[16];
            byte[] iv = new byte[16];
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                fsInput.Read(key, 0, key.Length);
                fsInput.Read(iv, 0, iv.Length);
            }
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                // Ignorer les 32 premiers octets (la clé et le vecteur d'initialisation)
                fsInput.Seek(32, SeekOrigin.Begin);
                using (FileStream fsDecrypted = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    RijndaelManaged aes = new RijndaelManaged();
                    aes.Key = key;
                    aes.IV = iv;
                    using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsDecrypted.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }


        public string ComputeFileHash(string filePath)
        {
            using (var stream = new BufferedStream(File.OpenRead(filePath), 100000))
            {
                var sha256 = new SHA256Managed();
                byte[] hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
