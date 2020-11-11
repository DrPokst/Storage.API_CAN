using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.API.Data;
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

        public async Task<BomName> GetBomName(string name)
        {
            var bomName = await _context.BomName.FirstOrDefaultAsync(u => u.Name == name);

            return bomName;
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