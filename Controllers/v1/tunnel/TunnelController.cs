using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgoManager.Lib;
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
            
            return Ok(new TunnelInformationDto
            {
                ID = newTunnel.ID,
                Labels = newTunnel.Labels,
                Names = newTunnel.Names,
                State = newTunnel.State,
            });
        }

        [HttpGet]
        public async Task<ActionResult<IList<TunnelInformationDto>>> ListTunnels()
        {
            var containers = await _dockerManager.GetAllContainers();

            var tunnels = containers.Select(container => new TunnelInformationDto
            {
                ID = container.ID,
                Labels = container.Labels,
                Names = container.Names,
                State = container.State,
            }).ToList();

            return Ok(tunnels);
        }

        [HttpDelete("{id}")]
        public ActionResult CloseTunnel(string id)
        {
            return Accepted();
        }
    }
}