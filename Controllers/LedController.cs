using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Data;
using Storage.API.Services;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedController : ControllerBase
    {
        private readonly ILedService _ledService;
        private readonly IReelRepository _repo;

        public LedController(ILedService ledService, IReelRepository repo)
        {
            _ledService = ledService;
            _repo = repo;
        }

        [HttpPut("on/{id}")]
        public async Task<IActionResult> TurnOnLed(int id)
        {   
            var reelFromRepo = await _repo.GetReel(id);
            int result = Int32.Parse(reelFromRepo.Location);
            var reel = await _ledService.TurnOnLed(result);
            
            return Ok(reel);
        }
        [HttpPut("off/{id}")]
        public async Task<IActionResult> TurnOffLed(int id)
        {
            var reelFromRepo = await _repo.GetReel(id);
            int result = Int32.Parse(reelFromRepo.Location);
            var reel = await _ledService.TurnOffLed(id);

            return Ok(reel);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPut("on/all")]
        public async Task<IActionResult> TurnOnAll()
        {   
            var reel = await _ledService.TurnOnAll();
            return Ok();
        }
        
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPut("off/all")]
        public async Task<IActionResult> TurnOffAll()
        {   
            var reel = await _ledService.TurnOffAll();
            return Ok();
        }
    }
}