using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ArgoManager.Controllers.v1.tunnel;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace ArgoManager.Lib
{
    public class DockerManager : IDockerManager
    {
        private readonly DockerClient _client;

        public DockerManager(DockerClient client)
        {
            _client = client;
        }

        public async Task<ContainerListResponse> CreateArgoContainer(string host, string target)
        {
            var guid = Guid.NewGuid().ToString().Substring(0, 8);

            var config = new CreateContainerParameters
            {
                Name = $"argo-{target}-{guid}",
                Labels = new Dictionary<string, string>
                {
                    {"Creation Date", DateTime.Now.ToUniversalTime().ToLongDateString()},
                    {"Host", host},
                    {"Target", target},
                    {"Managed", "true"},
                },
                Cmd = new List<string>
                {
                    "/cloudflared",
                    "tunnel",
                    "--origincert=/data/cert.pem",
                    "--no-tls-verify",
                    "--loglevel=error",
                    $"--url={host}",
                    $"--hostname={target}"
                },
                Image = "runebaas/argo-tunnel:latest",
                Volumes = new Dictionary<string, EmptyStruct>
                {
                    // ToDo: Figure out how to mount a volume
//                    { "/data/cert.pem" , new EmptyStruct()}
                },
                Env = new List<string>()
                {
                    $"ARGO_HOST={host}",
                    $"ARGO_TARGET={target}"
                },
                HostConfig = new HostConfig
                {
                    DNS = new List<string> {"1.1.1.1", "1.0.0.1"}
                }
            };

            var createdContainer = await _client.Containers.CreateContainerAsync(config);

            // ToDo: Add proper logging
            if (createdContainer.Warnings != null && createdContainer.Warnings.Any())
            {
                foreach (var warning in createdContainer.Warnings)
                {
                    Console.WriteLine(warning);
                }
            }

            await _client.Containers.StartContainerAsync(createdContainer.ID, new ContainerStartParameters());
            
            var allContainers = await GetAllContainers();
            var createdContainerInfo = allContainers.FirstOrDefault(e => e.ID == createdContainer.ID);

            return createdContainerInfo;
        }

        public Task<IList<ContainerListResponse>> GetAllContainers()
        {
            return _client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
        }

        public Task RemoveContainer(string containerId)
        {
            return _client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { RemoveVolumes = true});
        }
    }
}
