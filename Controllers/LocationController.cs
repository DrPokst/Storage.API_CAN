using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Storage.API.CAN;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API.Helpers;
using Storage.API.Models;
using Storage.API_CAN.DTOs;
using Storage.API_CAN.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IReelRepository _repo;
        private readonly ISearchRepository _srepo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ICanRepository _can;

        public LocationController(IReelRepository repo, ISearchRepository srepo, IMapper mapper, UserManager<User> userManager, ICanRepository can)
        {
            _userManager = userManager;
            _srepo = srepo;
            _mapper = mapper;
            _can = can;
            _repo = repo;
        }

        [HttpPost("put")]
        public async Task<IActionResult> RegisterLocation(LocationForRegisterDto locationForRegisterDto)
        {

            var ReelsFromRepo = await _repo.GetReel(locationForRegisterDto.Id);
            if (ReelsFromRepo == null) return BadRequest("Pagal pateikta ID ritė nerasta");

            var ComponentasFromRepo = await _srepo.GetCompCMnf(ReelsFromRepo.CMnf);

            int likutis = ReelsFromRepo.QTY - locationForRegisterDto.QTY;
            if (likutis <= 0) return BadRequest("Rite tusčia, bandote padeti tuščia pakuotę, nurasote didesni kieki nei buvo uzregistruota riteje");


            var res = Int32.TryParse(ReelsFromRepo.Location, out _);
            if (res == true)
            {
                int result = Int32.Parse(ReelsFromRepo.Location);
                if (result > 0) return BadRequest("Ši ritė turėtų būti padėta į " + ReelsFromRepo.Location + " slotą !!!!!");

            }

            var rxmsg = await _can.SetReelLocation();
            //testo tikslais
            /* Rxmsg rxmsg = new Rxmsg
             {
                 DLC = 0,
                 ID = 2,
                 Msg = new byte[] { 25, 6, 0xFF, 0x00, 0x00, 0xFF, 0xFF, 0xFF }
             };
            */

            int Location = rxmsg.Msg[1] + (rxmsg.Msg[0] * 10);

            var reelByLocation = await _repo.GetByLocation(Location.ToString());

            if (reelByLocation != null) return BadRequest("Ritės vieta jau užimta");

            var user = await _userManager.FindByIdAsync(locationForRegisterDto.UserId);

            var HistoryToCreate = new History
            {
                Mnf = ReelsFromRepo.CMnf,
                NewLocation = Location.ToString(),
                NewQty = likutis,
                OldLocation = ReelsFromRepo.Location,
                OldQty = ReelsFromRepo.QTY,
                ComponentasId = ComponentasFromRepo.Id,
                DateAdded = DateTime.Now,
                ReelId = locationForRegisterDto.Id,
                UserId = user.Id
            };

            var createHistory = await _srepo.RegisterHistory(HistoryToCreate);
            locationForRegisterDto.QTY = likutis;
            locationForRegisterDto.UserId = null;
            locationForRegisterDto.Location = Location.ToString();
            _mapper.Map(locationForRegisterDto, ReelsFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            else
                return BadRequest("Could notregister location");
        }
        [HttpPost("put/withlocation")]
        public async Task<IActionResult> RegisterLocationFromUser(LocationForRegisterDto locationForRegisterDto)
        {

            var ReelsFromRepo = await _repo.GetReel(locationForRegisterDto.Id);
            if (ReelsFromRepo == null) return BadRequest("Pagal pateikta ID ritė nerasta");

            var ComponentasFromRepo = await _srepo.GetCompCMnf(ReelsFromRepo.CMnf);

            int likutis = ReelsFromRepo.QTY - locationForRegisterDto.QTY;
            if (likutis <= 0) return BadRequest("Rite tusčia, bandote padeti tuščia pakuotę, nurasote didesni kieki nei buvo uzregistruota riteje");


            var res = Int32.TryParse(ReelsFromRepo.Location, out _);
            if (res == true)
            {
                int result = Int32.Parse(ReelsFromRepo.Location);
                if (result > 0) return BadRequest("Ši ritė turėtų būti padėta į " + ReelsFromRepo.Location + " slotą !!!!!");

            }

            var user = await _userManager.FindByIdAsync(locationForRegisterDto.UserId);
            
            locationForRegisterDto.UserId = null;

            if (locationForRegisterDto.Location is null)
            {
                locationForRegisterDto.Location = user.UserName;
                locationForRegisterDto.UserId = user.Id.ToString();
            }

            var HistoryToCreate = new History
            {
                Mnf = ReelsFromRepo.CMnf,
                NewLocation = locationForRegisterDto.Location,
                NewQty = likutis,
                OldLocation = ReelsFromRepo.Location,
                OldQty = ReelsFromRepo.QTY,
                ComponentasId = ComponentasFromRepo.Id,
                DateAdded = DateTime.Now,
                ReelId = locationForRegisterDto.Id,
                UserId = user.Id
            };

            var createHistory = await _srepo.RegisterHistory(HistoryToCreate);
            locationForRegisterDto.QTY = likutis;
            
            _mapper.Map(locationForRegisterDto, ReelsFromRepo);

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

            var take = _can.TakeOutReel(result);

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

            reelForTakeDto.Location = user.UserName;
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

            Response.AddPagination(history.CurrentPage, history.PageSize, history.TotalCount, history.TotalPages);

            return Ok(historyToReturn);
        }
    }
}