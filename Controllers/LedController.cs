using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> TurnOnLed(int id)
        {   
            var reelFromRepo = await _repo.GetReel(id);
            int result = Int32.Parse(reelFromRepo.Location);
            var reel = await _ledService.TurnOnLed(result);

            return Ok(reel);
        }
        [HttpGet]
        public async Task<IActionResult> TurnOff(int id)
        {
            var reel = await _ledService.TurnOff(id);

            return Ok(reel);
        }

    }
}