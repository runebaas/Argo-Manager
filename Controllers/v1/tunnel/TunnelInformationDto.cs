using System.Collections.Generic;

namespace ArgoManager.Controllers.v1.tunnel
{
    public class TunnelInformationDto
    {
        public string ID { get; set; }

        public IList<string> Names { get; set; }

        public IDictionary<string, string> Labels { get; set; }

        public string State { get; set; }
    }
}