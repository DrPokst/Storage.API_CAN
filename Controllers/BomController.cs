using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Storage.API.DTOs;
using Storage.API_CAN.Services;

namespace Storage.API_CAN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BomController : ControllerBase
    {
        private readonly IBomService _repo;
        public BomController(IBomService repo)
        {
            _repo = repo;

        }

        [HttpPost]
        public async Task<IActionResult> AddBom([FromForm] BomForCreationDto bomForCreationDto)
        {
            int lookForMnf = 0;
            int lookForMnfQuantity = 7;
            var fileName = bomForCreationDto.File.FileName;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var mnfNumbers = _repo.GetPartNumber(fileName, lookForMnf);
            var mnfPartQuantity = _repo.GetNumber(fileName, lookForMnfQuantity);

            return Ok(mnfPartQuantity);




            /*
            int lookForMnf = 0;
            int lookForMnfQuantity = 7;
            var file = bomForCreationDto.File;

            var filePath = Path.GetTempFileName();
            
            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            List<string> mnfNumbers = _repo.GetPartNumber(filePath, lookForMnf);
            List<string> mnfPartQuantity = _repo.GetNumber(filePath, lookForMnfQuantity);

            return Ok("viskas ok");
            */

            /* photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!componentsFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            componentsFromRepo.Photos.Add(photo);

            if(await _repo.SaveAll())
            {
                
                var photoToReturn = _mapper.Map<PhotosForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { componentId = componentId, id = photo.Id}, photoToReturn);
            }
                
            return BadRequest("Could not add the photo"); */
        }
    }
}