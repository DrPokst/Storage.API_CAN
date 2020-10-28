using System;
using System.Threading.Tasks;
using AutoMapper;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API.Models;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Services;
using Storage.API_CAN.Helpers;
using System.Collections.Generic;
using Storage.API.Helpers;
using Storage.API_CAN.DTOs;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IReelRepository _repo;
        private readonly ISearchRepository _srepo;
        private readonly IMapper _mapper;
        private readonly ILedService _ledService;
        private readonly UserManager<User> _userManager;
        public LocationController(ILedService ledService, IReelRepository repo, ISearchRepository srepo, IMapper mapper, UserManager<User> userManager)
        {
            _userManager = userManager;
            _srepo = srepo;
            _mapper = mapper;
            _repo = repo;
            _ledService = ledService;
        }


        [HttpPost("put")]
        public async Task<IActionResult> RegisterLocation(LocationForRegisterDto LocationForRegisterDto)
        {

            var ReelsFromRepo = await _repo.GetReel(LocationForRegisterDto.Id);
            var ComponentasFromRepo = await _srepo.GetCompCMnf(ReelsFromRepo.CMnf);

            int likutis = ReelsFromRepo.QTY - LocationForRegisterDto.QTY;

            if (likutis <= 0) return BadRequest("Rite tusčia, bandote padeti tuščia pakuotę, nurasote didesni kieki nei buvo uzregistruota riteje");


            var rxmsg = await _ledService.SetReelLocation();
            int Location = rxmsg.Msg[1] + ((rxmsg.Msg[0] - 1) * 30);


            //var secret = "super secret key";
            // var TokenData = ReadJwtToken(LocationForRegisterDto, secret);

            var HistoryToCreate = new History
            {
                Mnf = ReelsFromRepo.CMnf,
                NewLocation = Location.ToString(),
                NewQty = likutis,
                OldLocation = ReelsFromRepo.Location,
                OldQty = ReelsFromRepo.QTY,
                ComponentasId = ComponentasFromRepo.Id,
                DateAdded = DateTime.Now,
                ReelId = LocationForRegisterDto.Id,
                UserId = 1
            };


            LocationForRegisterDto.QTY = likutis;

            var createHistory = await _srepo.RegisterHistory(HistoryToCreate);
            LocationForRegisterDto.Location = Location.ToString();
            _mapper.Map(LocationForRegisterDto, ReelsFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            else
                return BadRequest("Could notregister location");
        }

        [HttpPost]
        public async Task<IActionResult> TakeOutReel(ReelForTakeDto reelForTakeDto)
        {
            var reelFromRepo = await _repo.GetReel(reelForTakeDto.ReelId);
            var ComponentasFromRepo = await _srepo.GetCompCMnf(reelFromRepo.CMnf);
            var user = await _userManager.FindByNameAsync(reelForTakeDto.Username);

            int result = Int32.Parse(reelFromRepo.Location);

            var take = _ledService.TakeOutReel(result);

            var HistoryToCreate = new History
            {
                Mnf = reelFromRepo.CMnf,
                NewLocation = reelForTakeDto.Username,
                NewQty = reelFromRepo.QTY,
                OldLocation = reelFromRepo.Location,
                OldQty = reelFromRepo.QTY,
                ComponentasId = ComponentasFromRepo.Id,
                DateAdded = DateTime.Now,
                ReelId = reelFromRepo.Id,
                UserId = user.Id
            };

            reelForTakeDto.Location = "0";
            reelForTakeDto.UserId = user.Id;

            var createHistory = await _srepo.RegisterHistory(HistoryToCreate);
            
            _mapper.Map(reelForTakeDto, reelFromRepo);


            if (await _repo.SaveAll())
                return NoContent();

            else
                return BadRequest("Could notregister location");

        }


        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] HistoryParams historyParams)
        {
            var history = await _srepo.GetHistory(historyParams);
            var historyToReturn = _mapper.Map<IEnumerable<HistoryForListDto>>(history);
            //var componentsToReturn = _mapper.Map<IEnumerable<ComponetsForListDto>>(components);

            Response.AddPagination(history.CurrentPage, history.PageSize, history.TotalCount, history.TotalPages);


            return Ok(historyToReturn);
        }
    }
}