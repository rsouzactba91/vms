namespace vms
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbcameras = new ListBox();
            btnPlay = new Button();
            btnDeletar = new Button();
            lbgravacoes = new ListBox();
            SuspendLayout();
            // 
            // lbcameras
            // 
            lbcameras.FormattingEnabled = true;
            lbcameras.ItemHeight = 15;
            lbcameras.Location = new Point(12, 16);
            lbcameras.Name = "lbcameras";
            lbcameras.Size = new Size(237, 184);
            lbcameras.TabIndex = 0;
            lbcameras.SelectedIndexChanged += lbcameras_SelectedIndexChanged;
            // 
            // btnPlay
            // 
            btnPlay.Location = new Point(100, 229);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(75, 23);
            btnPlay.TabIndex = 1;
            btnPlay.Text = "Executar";
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnDeletar
            // 
            btnDeletar.Location = new Point(212, 229);
            btnDeletar.Name = "btnDeletar";
            btnDeletar.Size = new Size(75, 23);
            btnDeletar.TabIndex = 2;
            btnDeletar.Text = "Deletar";
            btnDeletar.UseVisualStyleBackColor = true;
            btnDeletar.Click += btnDeletar_Click;
            // 
            // lbgravacoes
            // 
            lbgravacoes.FormattingEnabled = true;
            lbgravacoes.ItemHeight = 15;
            lbgravacoes.Location = new Point(273, 16);
            lbgravacoes.Name = "lbgravacoes";
            lbgravacoes.Size = new Size(237, 184);
            lbgravacoes.TabIndex = 3;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(522, 264);
            Controls.Add(lbgravacoes);
            Controls.Add(btnDeletar);
            Controls.Add(btnPlay);
            Controls.Add(lbcameras);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListBox lbcameras;
        private Button btnPlay;
        private Button btnDeletar;
        private ListBox lbgravacoes;
    }
}