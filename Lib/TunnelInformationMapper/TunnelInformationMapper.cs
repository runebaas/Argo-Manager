using System.Linq;
using Docker.DotNet.Models;

namespace ArgoManager.Lib.TunnelInformationMapper
{
    public static class TunnelInformationMapper
    {
        public static TunnelInformationDto MapContainerResponse(ContainerListResponse containerResponse)
        {
            return new TunnelInformationDto
            {
                ID = containerResponse.ID,
                Name = containerResponse.Names.FirstOrDefault()?.Remove(0, 1),
                Host = containerResponse.Labels["Host"] ?? "Unknown",
                Target = containerResponse.Labels["Target"] ?? "Unknown",
                // ToDo: get the status of the tunnel instead of the container
                Status = containerResponse.State
            };
        }
    }
}