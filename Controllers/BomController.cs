using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Storage.API.CAN;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API_CAN.Data;
using Storage.API_CAN.DTOs;
using Storage.API_CAN.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Storage.API_CAN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BomController : ControllerBase
    {

        private readonly IBomRepository _repo;
        private readonly ISearchRepository _search;
        private readonly DataContext _context;
        private readonly ICanRepository _can;

        public BomController(IBomRepository repo, ISearchRepository search, DataContext context, ICanRepository can)
        {
            _can = can;
            _repo = repo;
            _search = search;
            _context = context;
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
                        var colCount = worksheet.Dimension.Columns;
                        var buhNrCol = 0;
                        var qtyCol = 0;
                        var mnfCol = 0;
                        var name = worksheet.Cells[1, 1].Value.ToString().Trim();

                        var tikrinimas = await _repo.GetBomName(name);
                        if (tikrinimas != null) return BadRequest("Toks bomas jau yra ikeltas");
                        for (int col = 1; col <= colCount; col++)
                        {
                            var res = worksheet.Cells[2, col].Value.ToString().Trim();
                            if (res == "Buh.Nr.") buhNrCol = col;
                            if (res == "QTY") qtyCol = col;
                            if (res == "Manufacturer Part Number") mnfCol = col;
                        }
                        if (buhNrCol == 0) return BadRequest("Nerastas buhalterinio nr. stuleplis, patikslinkite langelio pavadinima i Buh.Nr. ");
                        if (qtyCol == 0) return BadRequest("Nerastas kiekio stulpelis, patikslinkite langelio pavadinima i QTY");
                        if (mnfCol == 0) return BadRequest("Nerastas gamintojo kodo stuleplis, patikslinkite i Manufacturer Part Number");

                        var bomName = new BomName
                        {
                            Name = name,
                            DateAdded = DateTime.Now,
                            LastModified = DateTime.Now
                        };

                        var result = await _repo.RegisterBomName(bomName);


                        for (int row = 3; row <= rowCount; row++)
                        {

                            var buhNr = worksheet.Cells[row, buhNrCol].Value.ToString().Trim();
                            var manufPartNr = worksheet.Cells[row, mnfCol].Value.ToString().Trim();
                            var componentasId = 0;

                            var res = await _search.GetComponentBuhNr(buhNr);
                            if (res != null)
                            {
                                manufPartNr = res.Mnf;
                                componentasId = res.Id;
                            }




                            listas.Add(new BomList
                            {
                                BuhNr = buhNr,
                                Qty = int.Parse(worksheet.Cells[row, qtyCol].Value.ToString().Trim()),
                                BomNameId = result.Id,
                                ComponentasId = componentasId,
                                ManufPartNr = manufPartNr

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
        [HttpGet]
        public async Task<IActionResult> GetBomNames()
        {
            var bomNames = await _repo.GetBomNames();

            return Ok(bomNames);
        }
        [HttpGet("{name}")]
        public async Task<IActionResult> GetBomList(string name)
        {
            var bomList = await _repo.GetBomList(name);

            return Ok(bomList);
        }
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteBom(string name)
        {
            var bomName = await _repo.GetBomName(name);
            if (bomName != null)
            {
                _search.Delete(bomName);
            }

            if (await _search.SaveAll())
            {
                return Ok();
            }


            return BadRequest("Failed to delete");

        }

        [HttpGet("{name}/check/{xQty}")]
        public async Task<IActionResult> GetBomListXQty(string name, int xQty)
        {
            var bomList = await _repo.GetBomListXQty(xQty, name);
            var listas = new List<BomListWithXQtyForListDto>();
            var suma = 0;

            foreach (var item in bomList)
            {
                var componentas = await _context.Componentass.Include(u => u.Reels).FirstOrDefaultAsync(u => u.BuhNr == item.BuhNr);
                if (componentas != null)
                {
                    foreach (var itemas in componentas.Reels)
                    {
                        suma = suma + itemas.QTY;
                        int result = Int32.Parse(itemas.Location);
                        var reel = await _can.TakeOutReel(result);
                    }
                }
                listas.Add(new BomListWithXQtyForListDto
                {
                    Id = item.Id,
                    BuhNr = item.BuhNr,
                    xQty = item.Qty,
                    QtyInDb = suma,
                    BomNameId = item.BomNameId

                });
                suma = 0;
            }
            return Ok(listas);
        }
    }
}