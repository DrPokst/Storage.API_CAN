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
using System.ComponentModel;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public SearchController(ISearchRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
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
        public async Task<IActionResult> GetComponents([FromQuery]ComponentParams componentParams)
        {
            var components = await _repo.GetComponents(componentParams);
            var componentsToReturn = _mapper.Map<IEnumerable<ComponetsForListDto>>(components);

            Response.AddPagination(components.CurrentPage, components.PageSize, components.TotalCount, components.TotalPages);

            return Ok(componentsToReturn);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetMnfs()
        {
            var components = await _repo.GetMnfs();
            var componentsToReturn = _mapper.Map<IEnumerable<ComponetsForListDto>>(components);

            return Ok(componentsToReturn);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetComponents(int id)
        {
            var components = await _repo.GetComponents(id);
            var componentsToReturn = _mapper.Map<ComponetsForListDto>(components);

            return Ok(componentsToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComponent(int id, ComponentForUpdateDto componentForUpdateDto)
        {
            // if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //     return Unauthorized();

            var componentsFromRepo = await _repo.GetComponents(id);

            _mapper.Map(componentForUpdateDto, componentsFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("registercomponent")]
        public async Task<IActionResult> RegisterComponent([FromForm]ComponetsForRegisterDto ComponetsForRegisterDto)
        {
            var file = ComponetsForRegisterDto.file;
            
            var uploadResult = new ImageUploadResult();

            if (await _repo.MnFExists(ComponetsForRegisterDto.Mnf.ToUpper()))
                return BadRequest("Component already exists");

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

           ComponetsForRegisterDto.PublicId = uploadResult.PublicId;
            

            var ComponentasToCreate = new Componentas
                {
                    Mnf = ComponetsForRegisterDto.Mnf.ToUpper(),
                    Manufacturer = ComponetsForRegisterDto.Manufacturer,
                    Detdescription = ComponetsForRegisterDto.Detdescription,
                    BuhNr = ComponetsForRegisterDto.BuhNr.ToUpper(),
                    Size = ComponetsForRegisterDto.Size.ToUpper(),
                    Type = ComponetsForRegisterDto.Type,
                    Nominal = ComponetsForRegisterDto.Nominal.ToUpper(),
                    Furl = ComponetsForRegisterDto.Furl,
                    Durl = ComponetsForRegisterDto.Durl,
                    Murl = ComponetsForRegisterDto.Murl,
                    Created = DateTime.Now
                };

            var createComponent = await _repo.RegisterComponents(ComponentasToCreate);


            var PhotoToCreate = new Photo
            {
                PublicId = ComponetsForRegisterDto.PublicId,
                IsMain = true,
                Url = uploadResult.Uri.ToString(),
                ComponentasId = ComponentasToCreate.Id

            };
            

           var createPhoto = await _repo.RegisterPhoto(PhotoToCreate);



            return StatusCode(201);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var componentFromRepo = await _repo.GetComponents(id);

            var photoFromRepo = await _repo.GetPhotoCID(componentFromRepo.Id);

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
                _repo.Delete(componentFromRepo);
            }

            if (componentFromRepo != null)
            {
                _repo.Delete(componentFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return Ok();
            }


            return BadRequest("Failed to delete");

        }


    }
}