using System;
using System.Linq;
using System.Threading.Tasks;
using ArgoManager.Lib;
using ArgoManager.Lib.TunnelInformationMapper;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;

namespace ArgoManager.Controllers.V1.Tunnels.Tunnel
{
    [Route("v1/tunnel/{id}")]
    [ApiController]
    public class TunnelInformationController : ControllerBase
    {
        private readonly IDockerManager _dockerManager;

        public TunnelInformationController(IDockerManager dockerManager)
        {
            _dockerManager = dockerManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetTunnelInfo(string id)
        {
            var all = await _dockerManager.GetAllContainers();
            var c = all.FirstOrDefault(e => e.ID == id);
            if (c == null)
            {
                return BadRequest("Container not found");
            }

            return Ok(TunnelInformationMapper.MapContainerResponse(c));
        }
        
        [HttpDelete]
        public async Task<ActionResult> CloseTunnel(string id)
        {
            try
            {
                await _dockerManager.RemoveContainer(id);
                return NoContent();
            }
            catch (DockerContainerNotFoundException)
            {
                return NotFound("Container not found");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs(string id)
        {
            try
            {
                var logs = await _dockerManager.GetContainerLogs(id, 50);
                return Ok(logs);
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
    }
}