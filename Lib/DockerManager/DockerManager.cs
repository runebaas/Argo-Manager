using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArgoManager.Controllers.V1.Tunnels.Tunnel;
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
                    "--no-tls-verify"
                },
                Image = "runebaas/argo-tunnel:latest",
                Env = new List<string>()
                {
                    $"TUNNEL_URL={host}",
                    $"TUNNEL_HOSTNAME={target}",
                    "TUNNEL_LOGLEVEL=debug",
//                    "TUNNEL_LOGLEVEL=error",
                    "TUNNEL_ORIGIN_CERT=/data/cert.pem"
                },
                HostConfig = new HostConfig
                {
                    DNS = new List<string> {"1.1.1.1", "1.0.0.1"},
                    Mounts = new List<Mount>
                    {
                        new Mount
                        {
                            Target = "/data/cert.pem",
                            Source = "/home/daan/.cloudflared/cert.pem",
                            ReadOnly = true,
                            Type = "bind"
                        }
                    },
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        // Figure out the right port...
                        {"32444/tcp", new List<PortBinding>
                            {
                                new PortBinding()
                                {
                                    HostIP = "127.0.0.1",
                                    HostPort = "32444"
                                }   
                            }
                        }
                    }
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

        public async Task<IEnumerable<ContainerListResponse>> GetAllContainers()
        {
            var allContainers = await _client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            return allContainers.Where(e => e.Names.Any(n => n.StartsWith("/argo-")));
        }

        public async Task RemoveContainer(string containerId)
        {
            await _client.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            await _client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { RemoveVolumes = true});
        }

        public async Task<List<LogItemDto>> GetContainerLogs(string containerId, int numberOfLines)
        {
            var containerLogsParameters = new ContainerLogsParameters
            {
                Tail = numberOfLines.ToString(),
                Follow = false,
                Since = null,
                ShowStdout = true,
                ShowStderr = true,
                Timestamps = false
            };
            
            var stream = await _client.Containers.GetContainerLogsAsync(containerId, containerLogsParameters);

            var logs = new List<LogItemDto>();
            using (var reader = new StreamReader(stream))
            {
                string line;
                // ToDo: Optimize this regex for SPEED
                var rgx = new Regex("(time=\"(?<Time>[\\d|\\-|TZ|\\:]+)\")?\\s(level=(?<Level>panic|fatal|error|warning|info|debug))?\\s(msg=\"(?<Message>.+)\")?", RegexOptions.ExplicitCapture);
                
                while ((line = reader.ReadLine()) != null)
                {
                    var matches = rgx.Match(line);
                    logs.Add(new LogItemDto
                    {
                        Date = matches.Groups["Time"].Captures.First().Value,
                        Level = matches.Groups["Level"].Captures.First().Value,
                        Message = matches.Groups["Message"].Captures.First().Value,
                    });
                }
            }
            return logs;
        }
    }
}
