namespace Crypter
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnFile = new System.Windows.Forms.Button();
            this.TxtExe = new System.Windows.Forms.TextBox();
            this.BtnCrypt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnFile
            // 
            this.BtnFile.Location = new System.Drawing.Point(555, 51);
            this.BtnFile.Name = "BtnFile";
            this.BtnFile.Size = new System.Drawing.Size(161, 23);
            this.BtnFile.TabIndex = 0;
            this.BtnFile.Text = "FILE";
            this.BtnFile.UseVisualStyleBackColor = true;
            this.BtnFile.Click += new System.EventHandler(this.BtnFile_Click);
            // 
            // TxtExe
            // 
            this.TxtExe.Location = new System.Drawing.Point(46, 51);
            this.TxtExe.Name = "TxtExe";
            this.TxtExe.Size = new System.Drawing.Size(503, 20);
            this.TxtExe.TabIndex = 1;
            // 
            // BtnCrypt
            // 
            this.BtnCrypt.Location = new System.Drawing.Point(261, 140);
            this.BtnCrypt.Name = "BtnCrypt";
            this.BtnCrypt.Size = new System.Drawing.Size(288, 23);
            this.BtnCrypt.TabIndex = 2;
            this.BtnCrypt.Text = "CRYPT";
            this.BtnCrypt.UseVisualStyleBackColor = true;
            this.BtnCrypt.Click += new System.EventHandler(this.BtnCrypt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BtnCrypt);
            this.Controls.Add(this.TxtExe);
            this.Controls.Add(this.BtnFile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnFile;
        private System.Windows.Forms.TextBox TxtExe;
        private System.Windows.Forms.Button BtnCrypt;
    }
}

