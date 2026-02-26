namespace vms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            videoView1 = new LibVLCSharp.WinForms.VideoView();
            videoView2 = new LibVLCSharp.WinForms.VideoView();
            panel1 = new Panel();
            lblEstado_2 = new Label();
            lblestado_1 = new Label();
            btnHd = new Button();
            trackBar1 = new TrackBar();
            btnLive = new Button();
            btnPlay = new Button();
            btnPause = new Button();
            btnGravacoes = new Button();
            btnNgrok = new Button();
            ((System.ComponentModel.ISupportInitialize)videoView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)videoView2).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // videoView1
            // 
            videoView1.BackColor = Color.Black;
            videoView1.Location = new Point(0, 3);
            videoView1.MediaPlayer = null;
            videoView1.Name = "videoView1";
            videoView1.Size = new Size(0, 0);
            videoView1.TabIndex = 0;
            videoView1.Text = "videoView1";
            // 
            // videoView2
            // 
            videoView2.BackColor = Color.Black;
            videoView2.Location = new Point(359, 3);
            videoView2.MediaPlayer = null;
            videoView2.Name = "videoView2";
            videoView2.Size = new Size(0, 0);
            videoView2.TabIndex = 1;
            videoView2.Text = "videoView2";
            // 
            // panel1
            // 
            panel1.Controls.Add(lblEstado_2);
            panel1.Controls.Add(lblestado_1);
            panel1.Controls.Add(videoView1);
            panel1.Controls.Add(videoView2);
            panel1.Location = new Point(32, 36);
            panel1.Name = "panel1";
            panel1.Size = new Size(735, 314);
            panel1.TabIndex = 2;
            // 
            // lblEstado_2
            // 
            lblEstado_2.AutoSize = true;
            lblEstado_2.Location = new Point(669, 3);
            lblEstado_2.Name = "lblEstado_2";
            lblEstado_2.Size = new Size(47, 15);
            lblEstado_2.TabIndex = 3;
            lblEstado_2.Text = "Ao vivo";
            // 
            // lblestado_1
            // 
            lblestado_1.AutoSize = true;
            lblestado_1.Location = new Point(20, 3);
            lblestado_1.Name = "lblestado_1";
            lblestado_1.Size = new Size(47, 15);
            lblestado_1.TabIndex = 2;
            lblestado_1.Text = "Ao vivo";
            // 
            // btnHd
            // 
            btnHd.Location = new Point(52, 415);
            btnHd.Name = "btnHd";
            btnHd.Size = new Size(75, 23);
            btnHd.TabIndex = 3;
            btnHd.Text = "HD";
            btnHd.UseVisualStyleBackColor = true;
            btnHd.Click += btnHd_Click;
            // 
            // trackBar1
            // 
            trackBar1.BackColor = Color.IndianRed;
            trackBar1.Location = new Point(32, 364);
            trackBar1.Maximum = 100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(735, 45);
            trackBar1.TabIndex = 4;
            trackBar1.TickFrequency = 10;
            // 
            // btnLive
            // 
            btnLive.Location = new Point(368, 415);
            btnLive.Name = "btnLive";
            btnLive.Size = new Size(75, 23);
            btnLive.TabIndex = 6;
            btnLive.Text = "Ao vivo";
            btnLive.UseVisualStyleBackColor = true;
            btnLive.Click += btnLive_Click_1;
            // 
            // btnPlay
            // 
            btnPlay.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPlay.Location = new Point(205, 415);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(29, 23);
            btnPlay.TabIndex = 7;
            btnPlay.Text = ">";
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnPause
            // 
            btnPause.Font = new Font("Segoe UI Black", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            btnPause.Location = new Point(240, 415);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(31, 23);
            btnPause.TabIndex = 8;
            btnPause.Text = "||";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // btnGravacoes
            // 
            btnGravacoes.Location = new Point(287, 415);
            btnGravacoes.Name = "btnGravacoes";
            btnGravacoes.Size = new Size(75, 23);
            btnGravacoes.TabIndex = 9;
            btnGravacoes.Text = "Gravações";
            btnGravacoes.UseVisualStyleBackColor = true;
            btnGravacoes.Click += AbrirGravacoes_Click;
            // 
            // btnNgrok
            // 
            btnNgrok.Location = new Point(449, 415);
            btnNgrok.Name = "btnNgrok";
            btnNgrok.Size = new Size(75, 23);
            btnNgrok.TabIndex = 10;
            btnNgrok.Text = "Link Ngrok";
            btnNgrok.UseVisualStyleBackColor = true;
            btnNgrok.Click += btnNgrok_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnNgrok);
            Controls.Add(btnGravacoes);
            Controls.Add(btnPause);
            Controls.Add(btnPlay);
            Controls.Add(btnLive);
            Controls.Add(trackBar1);
            Controls.Add(btnHd);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "VMS-R";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)videoView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)videoView2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoView1;
        private LibVLCSharp.WinForms.VideoView videoView2;
        private System.Windows.Forms.Panel panel1;
        private Button btnHd;
        private TrackBar trackBar1;
        private Button btnLive;
        private Label lblestado_1;
        private Label lblEstado_2;
        private Button btnPlay;
        private Button btnPause;
        private Button btnGravacoes;
        private Button btnNgrok;
    }
}