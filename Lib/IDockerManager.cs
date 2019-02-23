using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace ArgoManager.Lib
{
    public interface IDockerManager
    {
        Task<ContainerListResponse> CreateArgoContainer(string host, string target);
        Task<IList<ContainerListResponse>> GetAllContainers();
        Task RemoveContainer(string containerId);
    }
}