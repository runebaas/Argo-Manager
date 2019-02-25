using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgoManager.Lib;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArgoManager.Controllers.v1.tunnel
{
    [Route("v1/tunnel")]
    [ApiController]
    public class TunnelController : ControllerBase
    {
        private readonly IDockerManager _dockerManager;

        public TunnelController(IDockerManager dockerManager)
        {
            _dockerManager = dockerManager;
        }

        [HttpPost]
        public async Task<ActionResult<TunnelInformationDto>> CreateNewTunnel([FromBody] NewTunnelRequestDto newTunnelRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var host = $"{newTunnelRequest.Protocol.ToString().ToLower()}://{newTunnelRequest.Host}";

            var newTunnel = await _dockerManager.CreateArgoContainer(host, newTunnelRequest.Target);
            
            return Ok(MapContainerResponseToTunnelInfo(newTunnel));
        }

        [HttpGet]
        public async Task<ActionResult<IList<TunnelInformationDto>>> ListTunnels()
        {
            var containers = await _dockerManager.GetAllContainers();

            var tunnels = containers
                .Where(e => e.Names.Any(n => n.StartsWith("/argo-")))
                .Select(MapContainerResponseToTunnelInfo)
                .ToList();

            return Ok(tunnels);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> CloseTunnel(string id)
        {
            try
            {
                await _dockerManager.RemoveContainer(id);
                return NoContent();
            }
            catch (DockerContainerNotFoundException)
            {
                return NotFound("Container Not found");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private TunnelInformationDto MapContainerResponseToTunnelInfo(ContainerListResponse containerResponse)
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