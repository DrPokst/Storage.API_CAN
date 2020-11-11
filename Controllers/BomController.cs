using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Storage.API.DTOs;
using Storage.API_CAN.Data;
using Storage.API_CAN.Models;
using Storage.API_CAN.Services;

namespace Storage.API_CAN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BomController : ControllerBase
    {
        
        private readonly IBomRepository _repo;

        public BomController(IBomRepository repo)
        {
            _repo = repo;
            

        }

        [HttpPost]
        public async Task<IActionResult> AddBom([FromForm] BomForCreationDto bomForCreationDto)
        {

            var formFile = bomForCreationDto.File;

            var listas = new List<BomList>();

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
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    var name = worksheet.Cells[1, 1].Value.ToString().Trim();

                    var tikrinimas = await _repo.GetBomName(name);
                    if (tikrinimas != null) return BadRequest("Toks bomas jau yra ikeltas");

                    var bomName = new BomName
                        {
                            Name = name,
                            DateAdded = DateTime.Now,
                            LastModified = DateTime.Now
                        };

                    var result = await _repo.RegisterBomName(bomName);

                    for (int row = 3; row <= rowCount; row++)
                    {
                       
                       listas.Add(new BomList
                                {
                                    BuhNr = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                    Qty = int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()),
                                    BomNameId = result.Id
                                    
                                });
                    }
                            
                    var reg = await _repo.RegisterBomList(listas);
                }
            }
            }
            catch (System.Exception)
            {
                throw;
            }
            return Ok(listas);
        }
    }
}