using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API.Helpers;
using Storage.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Storage.API.Controllers
{
    
    [Route("api/search/{componentId}/photos")]
    [ApiController]    public class PhotosController : ControllerBase
    { 
        private readonly ISearchRepository _repo;
        private readonly IReelRepository _repo2;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(ISearchRepository repo, IReelRepository repo2, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;
            _repo2 = repo2;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName = "drpokst1",           // reikia pakeisti del saugumo negali visi matyti.... 
                _cloudinaryConfig.Value.ApiKey = "753448745425474",         // reikia pakeisti del saugumo negali visi matyti.... 
                _cloudinaryConfig.Value.ApiSecret = "x8KHxQjnrGXKcu9WOfxU2ddkGqE"     // reikia pakeisti del saugumo negali visi matyti.... 

            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            
            var photo = _mapper.Map<PhotosForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForComponent(int componentId, [FromForm]PhotosForCreationDto photoForCreationDto)
        {
            var componentsFromRepo = await _repo.GetComponents(componentId);
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();

            if  (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(300).Height(300).Crop("fill")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!componentsFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            componentsFromRepo.Photos.Add(photo);

            if(await _repo.SaveAll())
            {
                
                var photoToReturn = _mapper.Map<PhotosForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { componentId = componentId, id = photo.Id}, photoToReturn);
            }
                
            return BadRequest("Could not add the photo");
        }
     
    
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int componentId, int id)
        {
            var componentFromRepo = await _repo.GetComponents(componentId);

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");

        }

    }
}