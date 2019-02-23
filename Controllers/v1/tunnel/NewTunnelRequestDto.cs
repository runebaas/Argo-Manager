using System.ComponentModel.DataAnnotations;

namespace ArgoManager.Controllers.v1.tunnel
{
    public class NewTunnelRequestDto
    {
        [Required]
        public string Host { get; set; }
        
        [Required]
        public string Target { get; set; }

        public ArgoProtocol Protocol { get; set; } = ArgoProtocol.Http;
    }

    public enum ArgoProtocol
    {
        Http,
        Https,
        Ssh
    }
}