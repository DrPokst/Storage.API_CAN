using System.Threading.Tasks;
using Storage.API.Data;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Storage.API.DTOs;
using Storage.API.Models;
using System.Collections.Generic;

namespace Storage.API.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class CompariseController : ControllerBase
    {
        private readonly IReelRepository _repo;
        private readonly ISearchRepository _srepo;
        private readonly IMapper _mapper;

       public CompariseController(IReelRepository repo, ISearchRepository srepo, IMapper mapper)
        {
            _srepo = srepo;
            _mapper = mapper;
            _repo = repo;
        }

        public IMapper Mapper { get; }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompare(int id)  //surasti komponento visus reelus pagal MNF 
        {
            var compare = await _repo.GetCompare(id);
            var compareToReturn = _mapper.Map<IEnumerable<ReelsForListDto>>(compare);
            return Ok(compareToReturn);
        }

    }
}