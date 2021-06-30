using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.API.Data;
using Storage.API_CAN.DTOs;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.Data
{
    public class BomRepository : IBomRepository
    {
        private readonly DataContext _context;
        public BomRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BomList>> GetBomList(string name)
        {
            var bomName = await _context.BomName.Include(u => u.BomList).FirstOrDefaultAsync(u => u.Name == name);
            var bomList = await _context.BomList.Where(u => u.BomNameId == bomName.Id).ToListAsync();

            return bomList;
        }

        public async Task<IEnumerable<BomList>> GetBomListXQty(int xQty, string name)
        {
            var bomName = await _context.BomName.Include(u => u.BomList).FirstOrDefaultAsync(u => u.Name == name);
            var bomList = await _context.BomList.Where(u => u.BomNameId == bomName.Id).ToListAsync();
           // var bomList2 = bomList;
           // var suma = 0;

            foreach (var item2 in bomList)
            {
                item2.Qty = item2.Qty * xQty;
            }
            /*foreach (var item in bomList)
            {
                var componentas = await _context.Componentass.Include(u => u.Reels).FirstOrDefaultAsync(u => u.BuhNr == item.BuhNr);
                if (componentas != null)
                {
                    foreach (var itemas in componentas.Reels)
                    {
                        suma = suma + itemas.QTY;
                    }
                    item.Qty = suma;
                }
                suma = item.Qty;

            }*/


            return bomList;
        }

        public async Task<BomName> GetBomName(string name)
        {
            var bomName = await _context.BomName.Include(u => u.BomList).FirstOrDefaultAsync(u => u.Name == name);

            return bomName;
        }
        public async Task<IEnumerable<BomName>> GetBomNames()
        {
            var bomNames = await _context.BomName.ToListAsync();

            return bomNames;
        }

        public async Task<List<BomList>> RegisterBomList(List<BomList> bomList)
        {
            foreach (var element in bomList)
            {
                await _context.BomList.AddAsync(element);
            }
            
            await _context.SaveChangesAsync();

            return bomList;
        }

        public async Task<BomName> RegisterBomName(BomName bomName)
        {
            await _context.BomName.AddAsync(bomName);
            await _context.SaveChangesAsync();

            return bomName;
        }
    }
}