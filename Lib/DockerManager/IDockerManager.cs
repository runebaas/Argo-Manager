using System.Collections.Generic;
using System.Threading.Tasks;
using ArgoManager.Controllers.V1.Tunnels.Tunnel;
using Docker.DotNet.Models;

namespace ArgoManager.Lib
{
    public interface IDockerManager
    {
        Task<ContainerListResponse> CreateArgoContainer(string host, string target);
        Task<IEnumerable<ContainerListResponse>> GetAllContainers();
        Task RemoveContainer(string containerId);
        Task<List<LogItemDto>> GetContainerLogs(string containerId, int numberOfLines);
    }
}