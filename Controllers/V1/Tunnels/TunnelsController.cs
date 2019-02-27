using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgoManager.Lib;
using ArgoManager.Lib.TunnelInformationMapper;
using Microsoft.AspNetCore.Mvc;

namespace ArgoManager.Controllers.V1.Tunnels
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
            
            return Ok(TunnelInformationMapper.MapContainerResponse(newTunnel));
        }

        [HttpGet]
        public async Task<ActionResult<IList<TunnelInformationDto>>> ListTunnels()
        {
            var containers = await _dockerManager.GetAllContainers();

            var tunnels = containers
                .Select(TunnelInformationMapper.MapContainerResponse)
                .ToList();

            return Ok(tunnels);
        }
    }
}