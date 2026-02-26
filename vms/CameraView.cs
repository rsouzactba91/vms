using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System;

namespace vms
{
    /// <summary>
    /// Encapsula uma câmera com seu MediaPlayer, VideoView e Media atual.
    /// Cada câmera tem seus próprios recursos de streaming e gravação.
    /// </summary>
    public class CameraView
    {
        public Camera? Camera { get; set; }
        public MediaPlayer StreamPlayer { get; set; }      // Player para exibição ao vivo
        public MediaPlayer RecorderPlayer { get; set; }    // Player para gravação em background
        public VideoView View { get; set; }                // Componente visual
        public Media? CurrentStreamMedia { get; set; }      // Mídia atual em exibição
        public string CurrentRecordPath { get; set; }      // Arquivo sendo gravado
        
        public CameraView(Camera? camera, MediaPlayer streamPlayer, MediaPlayer recorderPlayer, VideoView view)
        {
            Camera = camera;
            StreamPlayer = streamPlayer;
            RecorderPlayer = recorderPlayer;
            View = view;
            View.MediaPlayer = StreamPlayer;
            CurrentRecordPath = "";
        }

        /// <summary>
        /// Para o streaming e libera recursos de mídia
        /// </summary>
        public void StopStreaming()
        {
            StreamPlayer?.Stop();
            CurrentStreamMedia?.Dispose();
            CurrentStreamMedia = null;
        }

        /// <summary>
        /// Para a gravação em background
        /// </summary>
        public void StopRecording()
        {
            RecorderPlayer?.Stop();
        }

        /// <summary>
        /// Retorna o identificador da câmera
        /// </summary>
        public override string ToString()
        {
            return Camera?.ToString() ?? "Câmera Desconhecida";
        }
    }
}
