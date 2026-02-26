using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace vms
{
    public partial class Form2 : Form
    {
        private Form1 _form1Ref;
        private List<Camera> camerasSelecionadas; // Armazena a lista de câmeras com seus IDs

        public Form2(Form1 form1Ref)
        {
            InitializeComponent();
            _form1Ref = form1Ref;
            camerasSelecionadas = new List<Camera>();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "Busca de Gravações";
            CarregarCameras();
        }

        // 1. Carrega as câmeras com seus IDs (necessário para buscar as gravações certas)
        private void CarregarCameras()
        {
            lbcameras.Items.Clear();
            camerasSelecionadas.Clear();

            // Usando a lista de câmeras do Form1
            Camera cam1 = new Camera(1, "Câmera 1 - Entrada", "rtsp://admin:root@192.168.100.38:554", 
                "rtsp://admin:root@192.168.100.38:554/V_ENC_000",
                "rtsp://admin:root@192.168.100.38:554/V_ENC_001");
            
            Camera cam2 = new Camera(2, "Câmera 2 - Corredor", "rtsp://admin:root@192.168.100.101:554",
                "rtsp://admin:root@192.168.100.101:554/V_ENC_000",
                "rtsp://admin:root@192.168.100.101:554/V_ENC_001");

            camerasSelecionadas.Add(cam1);
            camerasSelecionadas.Add(cam2);

            lbcameras.Items.Add(cam1.Nome ?? "Câmera Desconhecida");
            lbcameras.Items.Add(cam2.Nome ?? "Câmera Desconhecida");
        }

        // 2. Quando você clica na câmera, a função busca os arquivos dela
        private void lbcameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbcameras.SelectedIndex != -1)
            {
                CarregarGravacoes();
            }
        }

        private void CarregarGravacoes()
        {
            lbgravacoes.Items.Clear(); // Limpa a lista de arquivos (ListBox da direita)

            int indexSelecionado = lbcameras.SelectedIndex;
            Camera cameraSelecionada = camerasSelecionadas[indexSelecionado];
            string diretorio = AppDomain.CurrentDomain.BaseDirectory;

            // Busca arquivos que comecem com "camera" + ID (ex: "camera1_*.ts", "camera2_*.ts")
            string padroBusca = $"camera{cameraSelecionada.Id}_*.ts";
            string[] arquivos = Directory.GetFiles(diretorio, padroBusca);

            if (arquivos.Length == 0)
            {
                lbgravacoes.Items.Add("Nenhuma gravação encontrada.");
            }
            else
            {
                foreach (var caminho in arquivos)
                {
                    var info = new FileInfo(caminho);
                    string tamanho = FormatarTamanho(info.Length);
                    string nome = Path.GetFileName(caminho);
                    string criadoEm = info.CreationTime.ToString("dd/MM/yyyy HH:mm");
                    
                    lbgravacoes.Items.Add($"{nome} - {tamanho} - {criadoEm}");
                }
            }
        }

        private string FormatarTamanho(long bytes)
        {
            string[] tamanhos = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < tamanhos.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {tamanhos[order]}";
        }

        // 3. Executa a reprodução no Form1
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (lbgravacoes.SelectedIndex == -1 || (lbgravacoes.SelectedItem?.ToString() ?? "").Contains("Nenhuma"))
            {
                MessageBox.Show("Selecione um arquivo de vídeo para reproduzir.", "Aviso");
                return;
            }

            string itemSelecionado = lbgravacoes.SelectedItem?.ToString() ?? "";
            string nomeArquivo = itemSelecionado.Split('-')[0].Trim(); // Extrai só o nome do arquivo
            string caminhoCompleto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivo);

            if (File.Exists(caminhoCompleto))
            {
                _form1Ref.ReproducirGravacao(caminhoCompleto);
                this.Close(); // Fecha a busca e volta para a tela de vídeo
            }
            else
            {
                MessageBox.Show($"Arquivo não encontrado: {caminhoCompleto}", "Erro");
            }
        }

        // 4. Deleta o arquivo físico do HD
        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (lbgravacoes.SelectedIndex == -1 || (lbgravacoes.SelectedItem?.ToString() ?? "").Contains("Nenhuma")) 
                return;

            string itemSelecionado = lbgravacoes.SelectedItem?.ToString() ?? "";
            string nomeArquivo = itemSelecionado.Split('-')[0].Trim(); // Extrai só o nome do arquivo
            string caminhoCompleto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivo);

            var confirmacao = MessageBox.Show($"Apagar arquivo: {nomeArquivo}?", "Confirmação",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacao == DialogResult.Yes)
            {
                try
                {
                    File.Delete(caminhoCompleto);
                    CarregarGravacoes(); // Recarrega a lista de vídeos
                    MessageBox.Show("Arquivo deletado com sucesso!", "Sucesso");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}");
                }
            }
        }
    }
}