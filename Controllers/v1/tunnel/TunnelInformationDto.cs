using System.Collections.Generic;

namespace ArgoManager.Controllers.v1.tunnel
{
    public class TunnelInformationDto
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Host { get; set; }
        
        public string Target { get; set; }
        
        public string Status { get; set; }
    }
}