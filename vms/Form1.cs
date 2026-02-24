using System;
using System.Drawing;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace vms
{
    public partial class Form1 : Form
    {
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer1;
        private MediaPlayer _mediaPlayer2;

        public Form1()
        {
            InitializeComponent();

            // 1. Inicializa o Core e a biblioteca
            Core.Initialize();
            _libVLC = new LibVLC();

            _mediaPlayer1 = new MediaPlayer(_libVLC);
            _mediaPlayer2 = new MediaPlayer(_libVLC);

            // --- O SEGREDO PARA A IMAGEM NÃO ESTICAR ---
            // Trava a proporção em Widescreen (16:9)
            _mediaPlayer1.AspectRatio = "16:9";
            _mediaPlayer2.AspectRatio = "16:9";

            // Vincula os Players aos controles da tela
            videoView1.MediaPlayer = _mediaPlayer1;
            videoView2.MediaPlayer = _mediaPlayer2;

            panel1.Dock = DockStyle.Fill;
            panel1.Resize += Panel1_Resize;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            AjustarVideos();

            // --- CONFIGURAÇÃO DA CÂMERA 1 (IP .38) ---
            string urlCam1 = "rtsp://admin:root@192.168.100.38:554/V_ENC_000";
            var media1 = new Media(_libVLC, new Uri(urlCam1));
            media1.AddOption(":rtsp-tcp");             // Força TCP
            media1.AddOption(":network-caching=1000"); // Buffer de 1s

            // --- CONFIGURAÇÃO DA CÂMERA 2 (IP .105) ---
            // Mantido V_ENC_000 (HD). Se o PC travar, mude para V_ENC_001
            string urlCam2 = "rtsp://admin:root@192.168.100.105:554/V_ENC_000";
            var media2 = new Media(_libVLC, new Uri(urlCam2));
            media2.AddOption(":rtsp-tcp");
            media2.AddOption(":network-caching=1000");

            // Inicia a reprodução das duas simultaneamente
            _mediaPlayer1.Play(media1);
            _mediaPlayer2.Play(media2);

            // Arranca a magia do acesso externo!
            await IniciarTunelNuvem();
        }
        private async Task IniciarTunelNuvem()
        {
            try
            {
                // 1. Arranca o Ngrok de forma invisível no background
                ProcessStartInfo psi = new ProcessStartInfo();
                // Isso faz o C# procurar o ngrok.exe na mesma pasta onde o seu VMS está rodando, seja no seu PC, no pen drive ou no PC do professor.
                psi.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ngrok.exe");
                // Apontamos o Ngrok para o IP e porta da Câmara 1 na tua rede local
                psi.Arguments = "tcp 192.168.100.38:554";
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                Process.Start(psi);

                // 2. Aguarda 3 segundos para dar tempo do túnel conectar aos servidores globais
                await Task.Delay(3000);

                // 3. O Ngrok cria uma "API local". Vamos ler o link público que ele gerou!
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync("http://127.0.0.1:4040/api/tunnels");

                    // Usamos Regex para apanhar o link gerado (Ex: tcp://0.tcp.sa.ngrok.io:14567)
                    Match match = Regex.Match(json, @"tcp://[a-zA-Z0-9.-]+:\d+");

                    if (match.Success)
                    {
                        string linkNgrok = match.Value; // Ex: tcp://2.tcp.ngrok.io:12345

                        // Substituímos o "tcp://" por "rtsp://" e adicionamos o teu login e password
                        string linkParaO5G = linkNgrok.Replace("tcp://", "rtsp://admin:root@") + "/V_ENC_000";

                        MessageBox.Show("O teu túnel P2P foi criado com sucesso!\n\nCopia este link e abre no VLC do teu telemóvel no 5G:\n\n" + linkParaO5G,
                                        "TCC - Túnel Ativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao criar o túnel. O ngrok.exe está na pasta do projeto?\n" + ex.Message);
            }
        }

        private void Panel1_Resize(object? sender, EventArgs e)
        {
            AjustarVideos();
        }

        private void AjustarVideos()
        {
            if (panel1.ClientSize.Width == 0 || panel1.ClientSize.Height == 0) return;

            // Divide o painel ao meio verticalmente (Lado a Lado)
            int metadeLargura = panel1.ClientSize.Width / 2;
            int alturaTotal = panel1.ClientSize.Height;

            videoView1.Location = new Point(0, 0);
            videoView1.Size = new Size(metadeLargura, alturaTotal);

            videoView2.Location = new Point(metadeLargura, 0);
            videoView2.Size = new Size(metadeLargura, alturaTotal);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Limpeza das câmeras
            _mediaPlayer1?.Stop();
            _mediaPlayer1?.Dispose();
            _mediaPlayer2?.Stop();
            _mediaPlayer2?.Dispose();
            _libVLC?.Dispose();

            // --- MATA O PROCESSO FANTASMA DO NGROK ---
            foreach (var process in Process.GetProcessesByName("ngrok"))
            {
                process.Kill();
            }

            base.OnFormClosing(e);
        }
    }
}