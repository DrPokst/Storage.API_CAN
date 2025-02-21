using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Helpers;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReelController : ControllerBase
    {

        private readonly IReelRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        private readonly DataContext _context;

        public ReelController(DataContext context, IReelRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _context = context;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            _repo = repo;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName = "drpokst1",           // reikia pakeisti del saugumo negali visi matyti.... 
                _cloudinaryConfig.Value.ApiKey = "753448745425474",         // reikia pakeisti del saugumo negali visi matyti.... 
                _cloudinaryConfig.Value.ApiSecret = "x8KHxQjnrGXKcu9WOfxU2ddkGqE"     // reikia pakeisti del saugumo negali visi matyti.... 

            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet]
        public async Task<IActionResult> GetReels([FromQuery] ReelParams reelParams)
        {
            var reels = await _repo.GetReels(reelParams);
            var reelsToReturn = _mapper.Map<IEnumerable<ReelsForListDto>>(reels);

            Response.AddPagination(reels.CurrentPage, reels.PageSize, reels.TotalCount, reels.TotalPages);

            return Ok(reelsToReturn);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReel(int id)
        {
            var reel = await _repo.GetReel(id);
            var reelToReturn = _mapper.Map<ReelsForListDto>(reel);

            return Ok(reelToReturn);
        }
        [HttpGet("PagalMnf/{Mnf}")]
        public async Task<IActionResult> GetReelCMnf(string Mnf)
        {
            var reel = await _repo.GetReelCMnf(Mnf);
            var reelToReturn = _mapper.Map<ReelsForListDto>(reel);

            return Ok(reelToReturn);
        }
        [HttpGet("ListByLocation/{Location}")]
        public async Task<IActionResult> GetListByLocation(string Location)
        {
            var reels = await _repo.GetFeederList(Location);
            
            return Ok(reels);
        }
        [HttpPost("registerreel")]
        public async Task<IActionResult> RegisterReel([FromForm] ReelForRegisterDto reelForRegisterDto)
        {
            var file = reelForRegisterDto.file;

            var uploadResult = new ImageUploadResult();

            if (reelForRegisterDto.CMnf == null) return BadRequest("No manufacturer number ");    // pratestuoti paskui ar viskas ok, ne veliau bus ikelta 

            if (file != null)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(300).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            if (reelForRegisterDto.URL != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(@"data:image/png;base64," + reelForRegisterDto.URL),
                    Transformation = new Transformation().Width(500).Height(300).Crop("fill").Gravity("face")
                };

                uploadResult = _cloudinary.Upload(uploadParams);
            }


            reelForRegisterDto.PublicId = uploadResult.PublicId;

            var componentass = await _context.Componentass.FirstOrDefaultAsync(u => u.Mnf == reelForRegisterDto.CMnf);

            var ReelToCreate = new Reel
            {
                CMnf = reelForRegisterDto.CMnf,
                QTY = reelForRegisterDto.QTY,
                ComponentasId =componentass.Id,
                Location = "0"
            };

            var CreateReel = await _repo.RegisterReel(ReelToCreate);

            var PhotoToCreate = new Photo2
            {
                PublicId = reelForRegisterDto.PublicId,
                IsMain = true,
                Url = uploadResult.Uri.ToString(),
                ReelId = ReelToCreate.Id
            };

            var createPhoto = await _repo.RegisterPhoto(PhotoToCreate);

            return StatusCode(201);
        }
        [HttpPost("registerreel/stringlocation")]
        public async Task<IActionResult> RegisterReelWithStringLocation([FromForm] ReelForRegisterWithLocationDto reelForRegisterDto)
        {
            var file = reelForRegisterDto.file;

            var uploadResult = new ImageUploadResult();

            if (reelForRegisterDto.CMnf == null) return BadRequest("No manufacturer number ");    // pratestuoti paskui ar viskas ok, ne veliau bus ikelta 

            if (file != null)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(300).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            if (reelForRegisterDto.URL != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(@"data:image/png;base64," + reelForRegisterDto.URL),
                    Transformation = new Transformation().Width(500).Height(300).Crop("fill").Gravity("face")
                };

                uploadResult = _cloudinary.Upload(uploadParams);
            }


            reelForRegisterDto.PublicId = uploadResult.PublicId;

            var componentass = await _context.Componentass.FirstOrDefaultAsync(u => u.Mnf == reelForRegisterDto.CMnf);

            var ReelToCreate = new Reel
            {
                CMnf = reelForRegisterDto.CMnf,
                QTY = reelForRegisterDto.QTY,
                ComponentasId = componentass.Id,
                Location = reelForRegisterDto.Location
            };

            var CreateReel = await _repo.RegisterReel(ReelToCreate);

            var PhotoToCreate = new Photo2
            {
                PublicId = reelForRegisterDto.PublicId,
                IsMain = true,
                Url = uploadResult.Uri.ToString(),
                ReelId = ReelToCreate.Id
            };

            var createPhoto = await _repo.RegisterPhoto(PhotoToCreate);

            return StatusCode(201);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReel(int id, ReelForUpdateDto reelForUpdateDto)
        {
            // if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //     return Unauthorized();

            var reelFromRepo = await _repo.GetReel(id);

            _mapper.Map(reelForUpdateDto, reelFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReel(int id)
        {
            var reelFromRepo = await _repo.GetReel(id);

            var photoFromRepo = await _repo.GetPhoto(reelFromRepo.Id);

            /*
             if (photoFromRepo.PublicId != null)
             {
                 var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                 var result = _cloudinary.Destroy(deleteParams);

                 if (result.Result == "ok")
                 {
                     _repo.Delete(photoFromRepo);
                 }
                 _repo.Delete(reelFromRepo);
             }

             if (photoFromRepo.PublicId == null)
             {
                 _repo.Delete(photoFromRepo);
                 _repo.Delete(reelFromRepo);
             }

              */

            if (photoFromRepo == null)
            {
                _repo.Delete(reelFromRepo);
            }

            if (photoFromRepo != null)
            {
                _repo.Delete(reelFromRepo);
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return Ok();
            }


            return BadRequest("Failed to delete");

        }

    }
}