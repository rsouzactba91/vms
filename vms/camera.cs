using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vms
{
    public class Camera
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? UrlRtsp { get; set; }
        public string? UrlHD { get; set; }
        public string? UrlSD { get; set; }
        public bool? Ativa { get; set; }

        public Camera()
        {
            Ativa = true;
        }

        public Camera(int id, string nome, string urlRtsp, string urlHD, string urlSD)
        {
            Id = id;
            Nome = nome;
            UrlRtsp = urlRtsp;
            UrlHD = urlHD;
            UrlSD = urlSD;
            Ativa = true;
        }

        public override string ToString()
        {
            return $"{Id} - {Nome}";
        }
    }
}
