using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Crypter
{
    public partial class Form1 : Form
    {
        private AesCryptoServiceProvider aes;

        public Form1()
        {
            InitializeComponent();

            aes = new AesCryptoServiceProvider();
            aes.KeySize = 256;

            string keyFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "key");

            if (!File.Exists(keyFile))
            {
                // Initialise le fournisseur de chiffrement AES avec une clé de 256 bits
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.KeySize = 256;
                aes.GenerateKey();
                byte[] keyBytes = aes.Key;

                // Enregistre la clé dans un fichier
                File.WriteAllBytes(keyFile, keyBytes);
            }

            // Charge la clé de chiffrement depuis le fichier "key"
            byte[] key = File.ReadAllBytes(keyFile);

            // Vérifie que la clé a la bonne longueur
            if (key.Length != 32)
            {
                throw new CryptographicException("La clé de chiffrement n'a pas la bonne longueur.");
            }

            // Configure le fournisseur de chiffrement avec la clé chargée depuis le fichier
            aes.Key = key;

            string ivFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "iv");

            if (!File.Exists(ivFile))
            {
                // Initialise le fournisseur de chiffrement AES avec un vecteur d'initialisation de 128 bits
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.KeySize = 256;
                aes.GenerateIV();
                byte[] ivBytes = aes.IV;

                // Enregistre le vecteur d'initialisation dans un fichier
                File.WriteAllBytes(ivFile, ivBytes);
            }


            // Charge le vecteur d'initialisation depuis le fichier "iv"
            byte[] iv = File.ReadAllBytes(ivFile);


            // Vérifie que le vecteur d'initialisation a la bonne longueur
            if (iv.Length != 16)
            {
                throw new CryptographicException("Le vecteur d'initialisation n'a pas la bonne longueur.");
            }

            // Configure le fournisseur de chiffrement avec le vecteur d'initialisation chargé depuis le fichier
            aes.IV = iv;
        }


        private void BtnFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    TxtExe.Text = openFileDialog.FileName;
                }
            }

        }

        private void BtnCrypt_Click(object sender, EventArgs e)
        {
            if (TxtExe.Text == "")
            {
                MessageBox.Show("Please select a file to encrypt.");
                return;
            }

            string inputFile = TxtExe.Text;
            string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + "_encrypted.exe");

            // Lit le fichier à chiffrer
            byte[] data = File.ReadAllBytes(inputFile);

            // Chiffre les données
            byte[] encrypted;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }
                encrypted = ms.ToArray();
            }

            // Lit les clés à inclure dans le fichier chiffré
            byte[] key = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "key"));
            byte[] iv = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "iv"));

            // Concatène les données chiffrées et les clés
            byte[] encryptedWithKeys = new byte[encrypted.Length + key.Length + iv.Length];
            Array.Copy(encrypted, encryptedWithKeys, encrypted.Length);
            Array.Copy(key, 0, encryptedWithKeys, encrypted.Length, key.Length);
            Array.Copy(iv, 0, encryptedWithKeys, encrypted.Length + key.Length, iv.Length);

            // Enregistre les données chiffrées avec les clés dans un fichier
            File.WriteAllBytes(outputFile, encryptedWithKeys);

            MessageBox.Show("Encryption successful.");
        }

    }
}
