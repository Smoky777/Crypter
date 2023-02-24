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
        Byte[] key = ASCIIEncoding.ASCII.GetBytes("thisismyfuckikey");
        Byte[] IV = ASCIIEncoding.ASCII.GetBytes("thisismytestkeys");


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

            EncryptFile(inputFile, outputFile);

            string inFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "encrypted.exe");
            string outFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "decrypted.exe");

            DecryptFile(inFile, outFile);

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
                    using (CryptoStream cs = new CryptoStream(fsEncrypted, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        fsInput.CopyTo(cs);
                    }
                }
            }
        }

        public void DecryptFile(string inputFile, string outputFile)
        {
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsEncrypted = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    RijndaelManaged aes = new RijndaelManaged();
                    aes.Key = key;
                    aes.IV = IV;
                    using (CryptoStream cs = new CryptoStream(fsEncrypted, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        fsInput.CopyTo(cs);
                    }
                }
            }
        }
    }

}