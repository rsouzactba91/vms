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
            btnHd = new Button();
            ((System.ComponentModel.ISupportInitialize)videoView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)videoView2).BeginInit();
            panel1.SuspendLayout();
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
            panel1.Controls.Add(videoView1);
            panel1.Controls.Add(videoView2);
            panel1.Location = new Point(32, 36);
            panel1.Name = "panel1";
            panel1.Size = new Size(735, 314);
            panel1.TabIndex = 2;
            // 
            // btnHd
            // 
            btnHd.Location = new Point(56, 380);
            btnHd.Name = "btnHd";
            btnHd.Size = new Size(75, 23);
            btnHd.TabIndex = 3;
            btnHd.Text = "HD";
            btnHd.UseVisualStyleBackColor = true;
            btnHd.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnHd);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Gerenciador de Câmeras IP";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)videoView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)videoView2).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoView1;
        private LibVLCSharp.WinForms.VideoView videoView2;
        private System.Windows.Forms.Panel panel1;
        private Button btnHd;
    }
}