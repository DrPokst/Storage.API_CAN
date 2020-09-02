using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Storage.API.Models;
using Microsoft.EntityFrameworkCore;
using Storage.API.Helpers;

namespace Storage.API.Data
{
    public class ReelRepository : IReelRepository
    {
        private DataContext _context;

        public ReelRepository(DataContext context) => _context = context;
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
             _context.Remove(entity);
        }

        public async Task<Reel[]> GetCompare(int id)
        {   
            int a = 0;
            Reel[] y = new Reel[0];
            Reel[] d = new Reel[0];

            var componentass = await _context.Componentass.FirstOrDefaultAsync(u => u.Id == id);


            for (int i = 1; i < 500; i++)
            {
                var reel = await _context.Reels.Include(p => p.Photos2).FirstOrDefaultAsync(u => u.Id == i);

                if (reel != null)
                {
                    y = y.Concat(new Reel[] { reel }).ToArray();
                }
                

            }
            for (int k = 0; k < y.Length; k++)
            {

                if (y[k].CMnf == componentass.Mnf)
                    {
                    d = d.Concat(new Reel[] { y[k] }).ToArray();
                }
            }
            


            return d;
        }

        public async Task<Photo2> GetPhoto(int Rid)
        {
            var photo = await _context.Photos2.FirstOrDefaultAsync(p => p.ReelId == Rid);
            return photo;
        }

        public async Task<Reel> GetReel(int id)
        {
           var reel = await _context.Reels.Include(p => p.Photos2).Include(b => b.History).FirstOrDefaultAsync(u => u.Id == id);

            return reel;
        }

        public async Task<Reel> GetReelCMnf(string cMnf)
        {
           var reel = await _context.Reels.Include(p => p.Photos2).FirstOrDefaultAsync(u => u.CMnf == cMnf);

            return reel;
        }

        public async Task<PageList<Reel>> GetReels(ReelParams reelParams)
        {
            var reels = _context.Reels.Include(p => p.Photos2).AsQueryable();

            if (reelParams.CMnf != null)
            {
                reels = reels.Where(u => u.CMnf == reelParams.CMnf);
            }

            if (!string.IsNullOrEmpty(reelParams.OrderBy))
            {
                switch (reelParams.OrderBy)
                {
                    case "CMnf":
                        reels = reels.OrderBy(u => u.CMnf);
                        break;
                    case "qty":
                        reels = reels.OrderBy(u => u.QTY);
                        break;
                    default:
                        reels = reels.OrderBy(u => u.Id);
                        break;
                }
            }

            return await PageList<Reel>.CreateAsync(reels, reelParams.PageNumber, reelParams.PageSize);
            
        }

        public async Task<Photo2> RegisterPhoto(Photo2 photo)
        {
            await _context.Photos2.AddAsync(photo);
            await _context.SaveChangesAsync();

            return photo;
        }

        public async Task<Reel> RegisterReel(Reel reel)
        {
            await _context.Reels.AddAsync(reel);
            await _context.SaveChangesAsync();

            return reel;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

       
    }
}