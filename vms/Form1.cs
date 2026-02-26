using System;
using System.Drawing;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace vms
{
    public partial class Form1 : Form
    {
        private LibVLC _libVLC;

        // Lista de câmeras
        private List<Camera> cameras = new();

        // CameraViews - Encapsulam câmera + player de streaming + player de gravação + videoview
        private CameraView cameraView1 = null!;
        private CameraView cameraView2 = null!;

        private System.Windows.Forms.Timer _timerTrackBar;
        bool hd_ativo = false;

        // Variáveis para ngrok
        private Process? _ngrokProcess;
        private string? _ngrokLink;

        public Form1()
        {
            InitializeComponent();
            Core.Initialize();
            _libVLC = new LibVLC();

            // Criar CameraView para câmera 1 (linkedBD a videoView1)
            var streamPlayer1 = new MediaPlayer(_libVLC);
            var recorderPlayer1 = new MediaPlayer(_libVLC);
            streamPlayer1.AspectRatio = "16:9";
            cameraView1 = new CameraView(null, streamPlayer1, recorderPlayer1, videoView1);

            // Criar CameraView para câmera 2 (linkedBD a videoView2)
            var streamPlayer2 = new MediaPlayer(_libVLC);
            var recorderPlayer2 = new MediaPlayer(_libVLC);
            streamPlayer2.AspectRatio = "16:9";
            cameraView2 = new CameraView(null, streamPlayer2, recorderPlayer2, videoView2);

            panel1.Dock = DockStyle.Fill;
            panel1.Resize += Panel1_Resize;

            _timerTrackBar = new System.Windows.Forms.Timer();
            _timerTrackBar.Interval = 1000;
            _timerTrackBar.Tick += TimerTrackBar_Tick;
            _timerTrackBar.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "VMS-R | 0.2";
            this.MaximizeBox = false;
            // Inicializa câmeras
            InicializarCameras();
            ConfigurarInterface();

            // --- AQUI EST� O QUE VOC� PEDIU ---
            // 1. O motor de grava��o liga IMEDIATAMENTE
            IniciarGravacaoBackground();

            // 2. O v�deo aparece na tela (Ao Vivo) por padr�o
            btnLive_Click_1(null, EventArgs.Empty);

            IniciarTunelNuvem();
        }

        private void ConfigurarInterface()
        {
            trackBar1.Location = new Point(100, this.ClientSize.Height - trackBar1.Height - 110);
            trackBar1.Width = this.ClientSize.Width - 200;

            int bottomY = this.ClientSize.Height - 50;
            btnHd.Location = new Point(20, bottomY);
            btnLive.Location = new Point(this.ClientSize.Width / 2 + 10, bottomY);
            btnPlay.Location = new Point(this.ClientSize.Width - 280, bottomY);
            btnPause.Location = new Point(this.ClientSize.Width - 190, bottomY);
            btnGravacoes.Location = new Point(100, bottomY);
            
            // Posicionar e estilizar botão de acesso remoto
            var btnNgrok = this.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnNgrok");
            if (btnNgrok != null)
            {
                btnNgrok.Location = new Point(this.ClientSize.Width - 100, bottomY);
                btnNgrok.Text = "🌐 Link Remoto";
                btnNgrok.ForeColor = SystemColors.ControlText;
            }

            AjustarVideos();
        }

        private void InicializarCameras()
        {
            cameras = new List<Camera>
            {
                new Camera(1, "Câmera 1 - Entrada", "rtsp://admin:root@192.168.100.38:554",
                    "rtsp://admin:root@192.168.100.38:554/V_ENC_000",
                    "rtsp://admin:root@192.168.100.38:554/V_ENC_001"),

                new Camera(2, "Câmera 2 - Corredor", "rtsp://admin:root@192.168.100.101:554",
                    "rtsp://admin:root@192.168.100.101:554/V_ENC_000",
                    "rtsp://admin:root@192.168.100.101:554/V_ENC_001")
            };

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Câmeras inicializadas: {cameras.Count}");
            foreach (var cam in cameras)
            {
                System.Diagnostics.Debug.WriteLine($"  - {cam}");
            }
        }

        private void IniciarGravacaoBackground()
        {
            // Usamos a data e hora para não sobrescrever gravações antigas
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // Usa a lista de câmeras
            var cam1 = cameras[0];
            var cam2 = cameras[1];

            // Configura gravador para câmera 1
            cameraView1.CurrentRecordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"camera1_{timestamp}.ts");
            var m1 = new Media(_libVLC, new Uri(cam1.UrlSD ?? ""));
            m1.AddOption($":sout=#std{{access=file,mux=ts,dst='{cameraView1.CurrentRecordPath}'}}");
            cameraView1.RecorderPlayer.Play(m1);

            // Configura gravador para câmera 2
            cameraView2.CurrentRecordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"camera2_{timestamp}.ts");
            var m2 = new Media(_libVLC, new Uri(cam2.UrlSD ?? ""));
            m2.AddOption($":sout=#std{{access=file,mux=ts,dst='{cameraView2.CurrentRecordPath}'}}");
            cameraView2.RecorderPlayer.Play(m2);

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Gravação iniciada: {cameraView1.CurrentRecordPath}");
        }

        private void btnLive_Click_1(object? sender, EventArgs e) // MODO AO VIVO
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] === CLICOU AO VIVO ===");
            btnLive.ForeColor = Color.Green;
            btnLive.Font = new Font(btnLive.Font, FontStyle.Bold);

            trackBar1.Enabled = false;
            string url1 = hd_ativo ? cameras[0].UrlHD ?? "" : cameras[0].UrlSD ?? "";
            string url2 = hd_ativo ? cameras[1].UrlHD ?? "" : cameras[1].UrlSD ?? "";

            lblestado_1.Text = "AO VIVO (GRAVANDO...)";
            lblestado_1.ForeColor = Color.Red;
            lblEstado_2.Text = lblestado_1.Text;
            lblEstado_2.ForeColor = Color.Red;

            System.Diagnostics.Debug.WriteLine($"URLs: {url1}");
            System.Diagnostics.Debug.WriteLine($"       {url2}");

            AtualizarPlayer(url1, url2);
        }


        // --- MÉTODOS DE APOIO (Sincronia e Ajustes) ---

        private void TimerTrackBar_Tick(object? sender, EventArgs e)
        {
            if (trackBar1.Enabled)
            {
                if (cameraView1.StreamPlayer != null && cameraView1.StreamPlayer.IsPlaying)
                {
                    int posicaoAtual = (int)(cameraView1.StreamPlayer.Position * 100);
                    if (posicaoAtual >= 0 && posicaoAtual <= 100 && posicaoAtual != trackBar1.Value)
                    {
                        trackBar1.Value = posicaoAtual;
                    }
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Enabled && cameraView1.StreamPlayer.Length > 0)
            {
                float posicao = trackBar1.Value / 100f;
                cameraView1.StreamPlayer.Position = posicao;
                if (cameraView2.StreamPlayer.Length > 0) cameraView2.StreamPlayer.Position = posicao;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!trackBar1.Enabled) return;

            if (cameraView1.StreamPlayer.IsPlaying == false)
            {
                cameraView1.StreamPlayer.Play();
            }
            if (cameraView2.StreamPlayer.IsPlaying == false)
            {
                cameraView2.StreamPlayer.Play();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!trackBar1.Enabled) return;

            if (cameraView1.StreamPlayer.IsPlaying)
            {
                cameraView1.StreamPlayer.Pause();
            }
            if (cameraView2.StreamPlayer.IsPlaying)
            {
                cameraView2.StreamPlayer.Pause();
            }
        }

        private void AtualizarPlayer(string url1, string url2)
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] >>> ANTES: cameraView1.StreamPlayer.IsPlaying={cameraView1.StreamPlayer.IsPlaying}");
            cameraView1.StreamPlayer.Stop();
            cameraView2.StreamPlayer.Stop();

            // Dispose dos Media antigos para liberar recursos
            cameraView1.CurrentStreamMedia?.Dispose();
            cameraView2.CurrentStreamMedia?.Dispose();

            System.Threading.Thread.Sleep(300);

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] >>> Criando novo Media para Câmera 1: {url1}");

            cameraView1.CurrentStreamMedia = new Media(_libVLC, new Uri(url1));
            cameraView1.CurrentStreamMedia.AddOption(":rtsp-tcp");
            cameraView1.CurrentStreamMedia.AddOption(":no-rtsp-caching");
            cameraView1.CurrentStreamMedia.AddOption(":rtsp-caching=0");
            cameraView1.CurrentStreamMedia.AddOption(":network-caching=0");
            cameraView1.CurrentStreamMedia.AddOption(":live-caching=0");

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] >>> Criando novo Media para Câmera 2: {url2}");

            cameraView2.CurrentStreamMedia = new Media(_libVLC, new Uri(url2));
            cameraView2.CurrentStreamMedia.AddOption(":rtsp-tcp");
            cameraView2.CurrentStreamMedia.AddOption(":no-rtsp-caching");
            cameraView2.CurrentStreamMedia.AddOption(":rtsp-caching=0");
            cameraView2.CurrentStreamMedia.AddOption(":network-caching=0");
            cameraView2.CurrentStreamMedia.AddOption(":live-caching=0");

            try
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] >>> Play StreamPlayer da Câmera 1");
                cameraView1.StreamPlayer.Play(cameraView1.CurrentStreamMedia);
                System.Threading.Thread.Sleep(200);
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] >>> Play StreamPlayer da Câmera 2");
                cameraView2.StreamPlayer.Play(cameraView2.CurrentStreamMedia);
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✓ Streaming ao vivo iniciado com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✗ ERRO: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void AjustarVideos()
        {
            if (panel1.ClientSize.Width == 0) return;
            int metade = panel1.ClientSize.Width / 2;
            videoView1.Size = new Size(metade, panel1.ClientSize.Height);
            videoView2.Location = new Point(metade, 0);
            videoView2.Size = new Size(metade, panel1.ClientSize.Height);
        }

        private void Panel1_Resize(object? sender, EventArgs e) => AjustarVideos();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // CRUCIAL: Parar os gravadores para fechar o arquivo corretamente
            cameraView1.RecorderPlayer?.Stop();
            cameraView2.RecorderPlayer?.Stop();

            // Parar streaming
            cameraView1.StreamPlayer?.Stop();
            cameraView2.StreamPlayer?.Stop();

            // Limpar Media em cache
            cameraView1.CurrentStreamMedia?.Dispose();
            cameraView2.CurrentStreamMedia?.Dispose();

            // Limpar Players
            cameraView1.StreamPlayer?.Dispose();
            cameraView2.StreamPlayer?.Dispose();
            cameraView1.RecorderPlayer?.Dispose();
            cameraView2.RecorderPlayer?.Dispose();

            // Limpar LibVLC
            _libVLC?.Dispose();

            // Matar ngrok se estiver rodando
            try
            {
                foreach (var p in Process.GetProcessesByName("ngrok"))
                {
                    p.Kill();
                }
            }
            catch { }

            base.OnFormClosing(e);
        }

        private void IniciarTunelNuvem()
        {
            try
            {
                // Limpeza de processos fantasmas
                foreach (var p in Process.GetProcessesByName("ngrok")) { try { p.Kill(); } catch { } }

                var processInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ngrok.exe"),
                    Arguments = "tcp 192.168.100.38:554",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true, // Permite ler o que o ngrok escreve
                    UseShellExecute = false
                };

                _ngrokProcess = Process.Start(processInfo);

                if (_ngrokProcess != null)
                {
                    // Roda a leitura em segundo plano para não travar o Form
                    Task.Run(() => LerOutputNgrok(_ngrokProcess));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao disparar Ngrok: {ex.Message}");
            }
        }

        private void LerOutputNgrok(Process process)
        {
            try
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Procura o padrão do link TCP
                        if (line.Contains("tcp://"))
                        {
                            var match = Regex.Match(line, @"tcp://[0-9a-z.-]+:[0-9]+");
                            if (match.Success)
                            {
                                _ngrokLink = match.Value;

                                // ALERTA VIA MESSAGEBOX (Usa Invoke porque está em outra thread)
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.Text = $"VMS-R | LINK ATIVO: {_ngrokLink}";
                                    
                                    MessageBox.Show($"🌍 ACESSO REMOTO LIBERADO!\n\nLink: {_ngrokLink}\n\nO link foi capturado e está pronto para uso.", 
                                                    "Ngrok Monitor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    
                                    // Atualiza o botão se ele existir
                                    var btnNgrok = this.Controls.Find("btnNgrok", true).FirstOrDefault() as Button;
                                    if (btnNgrok != null) btnNgrok.Text = "🌐 Copiar Link";
                                });
                                break; // Para de ler o log após achar o link
                            }
                        }
                    }
                }
            }
            catch { /* Silencia erros de fechamento */ }
        }

        private void btnHd_Click(object sender, EventArgs e) // Botão HD/SD
        {
            hd_ativo = !hd_ativo;
            btnHd.Text = hd_ativo ? "HD ATIVO" : "SD ATIVO";
            if (!trackBar1.Enabled) btnLive_Click_1(null, EventArgs.Empty);
        }

        private void AbrirGravacoes_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.ShowDialog();
        }

        public void ReproducirGravacao(string caminhoArquivo)
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] === REPRODUZINDO GRAVAÇÃO: {caminhoArquivo} ===");

            btnLive.ForeColor = Color.Gray;

            trackBar1.Enabled = true;
            lblestado_1.Text = "REPRODUÇÃO GRAVADA";
            lblestado_1.ForeColor = Color.DodgerBlue;
            lblEstado_2.Text = lblestado_1.Text;
            lblEstado_2.ForeColor = Color.DodgerBlue;

            // Dispose dos Media antigos
            cameraView1.CurrentStreamMedia?.Dispose();
            cameraView2.CurrentStreamMedia?.Dispose();

            try
            {
                if (File.Exists(caminhoArquivo))
                {
                    cameraView1.CurrentStreamMedia = new Media(_libVLC, caminhoArquivo, FromType.FromPath);
                    cameraView1.StreamPlayer.Play(cameraView1.CurrentStreamMedia);

                    // Se houver segunda camera, tenta reproduzir o par
                    string caminhoCamera2 = caminhoArquivo.Replace("camera1_", "camera2_");
                    if (File.Exists(caminhoCamera2))
                    {
                        cameraView2.CurrentStreamMedia = new Media(_libVLC, caminhoCamera2, FromType.FromPath);
                        cameraView2.StreamPlayer.Play(cameraView2.CurrentStreamMedia);
                    }
                    else
                    {
                        cameraView2.StreamPlayer.Stop();
                    }

                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✓ Gravação iniciada");
                }
                else
                {
                    MessageBox.Show($"Arquivo não encontrado: {caminhoArquivo}", "Erro");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✗ ERRO ao reproduzir: {ex.Message}");
                MessageBox.Show($"Erro ao reproduzir: {ex.Message}", "Erro");
            }
        }

        private void btnNgrok_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_ngrokLink))
            {
                MessageBox.Show("O túnel ainda está sendo estabelecido ou falhou.\nAguarde o aviso na tela.", "Aguarde...");
                return;
            }

            // Se já tem o link, copia e avisa
            Clipboard.SetText(_ngrokLink);
            MessageBox.Show($"✓ Link copiado para o celular!\n\n{_ngrokLink}", "Sucesso");
        }
    }
}