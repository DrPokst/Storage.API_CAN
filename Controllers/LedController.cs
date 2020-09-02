using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Services;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedController : ControllerBase
    {
        private readonly ILedService _ledService;

        public LedController(ILedService ledService)
        {
            _ledService = ledService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> TurnOnLed(int id)
        {
            var reel = await _ledService.TurnOnLed(id);

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