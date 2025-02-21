﻿using Microsoft.AspNetCore.Mvc;
using Storage.API.CAN;
using Storage.API.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedController : ControllerBase
    {
        private readonly IReelRepository _repo;
        private readonly ILedRepository _led;

        public LedController(IReelRepository repo, ILedRepository led)
        {
            _repo = repo;
            _led = led;
        }

        [HttpPut("on/{id}")]
        public async Task<IActionResult> TurnOnLed(int id)
        {
            var reelFromRepo = await _repo.GetReel(id);
            int result = Int32.Parse(reelFromRepo.Location);
            var reel = await _led.TurnOnLed(result, "039be5");

            return Ok(reel);
        }
        [HttpPut("off/{id}")]
        public async Task<IActionResult> TurnOffLed(int id)
        {
            var reelFromRepo = await _repo.GetReel(id);
            int result = Int32.Parse(reelFromRepo.Location);
            var reel = await _led.TurnOffLed(result);
            return Ok(reel);
        }

        [HttpPut("on/all/{color}")]
        public async Task<IActionResult> TurnOnAll(string color)
        {
            var reel = await _led.TurnOnAll(color);
            return Ok();
        }

        [HttpPut("off/all")]
        public async Task<IActionResult> TurnOffAll()
        {
            var reel = await _led.TurnOffAll();
            return Ok();
        }


        [HttpPost("test")]
        public async Task<IActionResult> TurnOnTest()
        {
            for (int i = 1; i < 150; i++)
            {
                for (int a = 1; a < 11; a++)
                {
                    await _led.TurnOnLed(i * 10 + a, "039be5");
                    Thread.Sleep(170);
                }

                for (int z = 1; z < 11; z++)
                {
                    await _led.TurnOffLed(i * 10 + z);
                }

            }
            return Ok();
        }

    }
}