using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API.Models;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Helpers;
using System.ComponentModel;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System.IO;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentController : ControllerBase
    {
        private readonly ISearchRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public ComponentController(ISearchRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
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
        public async Task<IActionResult> GetComponents([FromQuery] ComponentParams componentParams)
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

        [HttpGet("mnf/{Mnf}")]
        public async Task<IActionResult> GetComponentsMnf(string Mnf)
        {
            var components = await _repo.GetCompCMnf(Mnf);
            var componentsToReturn = _mapper.Map<ComponetsForListDto>(components);

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
        public async Task<IActionResult> RegisterComponent([FromForm] ComponetsForRegisterDto ComponetsForRegisterDto)
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
        [HttpPost("registercomponent/all")]
        public async Task<IActionResult> AddBom([FromForm] BomForCreationDto bomForCreationDto)
        {

            var formFile = bomForCreationDto.File;

            if (formFile == null || formFile.Length <= 0)
            {
                return BadRequest("formfile is empty");
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Not Support file extension");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        var colCount = worksheet.Dimension.Columns;


                        for (int row = 1; row <= rowCount; row++)
                        {

                            var buhnr = worksheet.Cells[row, 1].Value.ToString().Trim();
                            var manufacturer = worksheet.Cells[row, 3].Value.ToString().Trim();
                            var mnf = worksheet.Cells[row, 4].Value.ToString().Trim();
                            var type = worksheet.Cells[row, 5].Value.ToString().Trim();
                            var detdes = worksheet.Cells[row, 6].Value.ToString().Trim();
                            var size = worksheet.Cells[row, 7].Value.ToString().Trim();
                            var nominal = worksheet.Cells[row, 8].Value.ToString().Trim();
                            var Durl = worksheet.Cells[row, 9].Value.ToString().Trim();
                            var Furl = worksheet.Cells[row, 10].Value.ToString().Trim();
                            var Murl = worksheet.Cells[row, 11].Value.ToString().Trim();
                            var photoUrl = worksheet.Cells[row, 12].Value.ToString().Trim();
                            //var publicid = worksheet.Cells[row, 9].Value.ToString().Trim();
                            //var url = worksheet.Cells[row, 10].Value.ToString().Trim();
                            
                            var uploadParams = new ImageUploadParams()
                            {
                                File = new FileDescription(photoUrl),
                               
                            };

                            var uploadResult = _cloudinary.Upload(uploadParams);

                            var publicid = uploadResult.PublicId;
                            var url = uploadResult.Uri.ToString();

                            var ComponentasToCreate = new Componentas
                            {
                                Mnf = mnf,
                                Manufacturer = manufacturer,
                                Detdescription = detdes,
                                BuhNr = buhnr,
                                Size = size,
                                Type = type,
                                Nominal = nominal,
                                Created = DateTime.Now,
                                Furl = Furl,
                                Murl = Murl,
                                Durl = Durl
                            };

                            var createComponent = await _repo.RegisterComponents(ComponentasToCreate);

                            var PhotoToCreate = new Photo
                            {
                                PublicId = publicid,
                                IsMain = true,
                                Url = url,
                                ComponentasId = createComponent.Id

                            };

                            var createPhoto = await _repo.RegisterPhoto(PhotoToCreate);


                        }

                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return Ok();
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