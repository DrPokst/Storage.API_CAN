using System;
using System.Threading.Tasks;
using AutoMapper;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
         private readonly IReelRepository _repo;
        private readonly ISearchRepository _srepo;
        private readonly IMapper _mapper;
        public LocationController(IReelRepository repo, ISearchRepository srepo, IMapper mapper)
        {
            _srepo = srepo;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost("put")]
        public async Task<IActionResult> RegisterLocation(LocationForRegisterDto LocationForRegisterDto)
        {

            
           var ReelsFromRepo = await _repo.GetReel(LocationForRegisterDto.Id);
           var ComponentasFromRepo = await _srepo.GetCompCMnf(ReelsFromRepo.CMnf);

           if (LocationForRegisterDto.QTY == 0) LocationForRegisterDto.QTY = ReelsFromRepo.QTY;
           


           var HistoryToCreate = new History
            {
                Mnf = ReelsFromRepo.CMnf,
                NewLocation = LocationForRegisterDto.Location,
                NewQty = LocationForRegisterDto.QTY,
                OldLocation = ReelsFromRepo.Location,
                OldQty = ReelsFromRepo.QTY,
                ComponentasId = ComponentasFromRepo.Id,
                DateAdded = DateTime.Now,
                ReelId = LocationForRegisterDto.Id
           };

            var createHistory = await _srepo.RegisterHistory(HistoryToCreate);

            _mapper.Map(LocationForRegisterDto, ReelsFromRepo);

            if (await _repo.SaveAll())
                return NoContent();
            
            else 
             return BadRequest("Could notregister location");
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _srepo.GetHistory();
            //var componentsToReturn = _mapper.Map<IEnumerable<ComponetsForListDto>>(components);

            return Ok(history);
        }
    }
}