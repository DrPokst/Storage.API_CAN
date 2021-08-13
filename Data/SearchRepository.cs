using System.Collections.Generic;
using System.Threading.Tasks;
using Storage.API.Models;
using Microsoft.EntityFrameworkCore;
using Storage.API.Helpers;
using System.ComponentModel;
using System.Linq;
using Storage.API_CAN.Helpers;

namespace Storage.API.Data
{
    public class SearchRepository : ISearchRepository
    {
        private DataContext _context;

        public SearchRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Componentas> GetComponents(int id)
        {
            var componentass = await _context.Componentass.Include(p => p.Photos)
                                                          .Include(b => b.History)
                                                          .Include(r => r.Reels)
                                                          .FirstOrDefaultAsync(u => u.Id == id);

            return componentass;
        }
        public async Task<IEnumerable<Componentas>> GetMnfs()
        {
            var componentass = _context.Componentass.AsQueryable();

            return componentass;
        }
        public async Task<Componentas> GetCompCMnf(string cMnf)
        {
            var reel = await _context.Componentass.Include(p => p.Photos)
                                                  .Include(b => b.History)
                                                  .Include(r => r.Reels)
                                                  .FirstOrDefaultAsync(u => u.Mnf == cMnf);

            return reel;
        }
        public async Task<PageList<Componentas>> GetComponents(ComponentParams componentParams)
        {
            var componentass = _context.Componentass.Include(p => p.Photos).AsQueryable();

            if (componentParams.Size != null)
            {
                componentass = componentass.Where(u => u.Size == componentParams.Size);
            }

            if (componentParams.Type != null)
            {
                componentass = componentass.Where(u => u.Type == componentParams.Type);
            }

            if (componentParams.Mnf != null)
            {
                componentass = from u in componentass
                               where u.Mnf.StartsWith(componentParams.Mnf)
                               select u;
   

            }
            if (componentParams.Nominal != null)
            {
                componentass = from u in componentass
                               where u.Nominal.StartsWith(componentParams.Nominal)
                               select u;
            }
            if (componentParams.BuhNr != null)
            {
                componentass = componentass.Where(u => u.BuhNr == componentParams.BuhNr);
            }

            if (!string.IsNullOrEmpty(componentParams.OrderBy))
            {
                switch (componentParams.OrderBy)
                {
                    case "created":
                        componentass = componentass.OrderByDescending(u => u.Created);
                        break;
                    default:
                        componentass = componentass.OrderBy(u => u.Id);
                        break;
                }
            }

            return await PageList<Componentas>.CreateAsync(componentass, componentParams.PageNumber, componentParams.PageSize);
        }

        public async Task<Componentas> RegisterComponents(Componentas componentas)
        {   
            await _context.Componentass.AddAsync(componentas);
            await _context.SaveChangesAsync();

            return componentas;

        }
        public async Task<Photo> RegisterPhoto(Photo photo)
        {

            await _context.Photos.AddAsync(photo);
            await _context.SaveChangesAsync();

            return photo;

        }
        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MnFExists(string Mnf)
        {
            
           if (await _context.Componentass.AnyAsync(x => x.Mnf == Mnf))
            return true;
            
            return false;
        }

        public async Task<Photo> GetPhotoCID(int Cid)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.ComponentasId == Cid);
            return photo;
        }

        public async Task<History> RegisterHistory(History history)
        {
            await _context.History.AddAsync(history);
            await _context.SaveChangesAsync();

            return history;
        }

        public async Task<PageList<History>> GetHistory(HistoryParams historyParams)
        {
            var history = _context.History.AsQueryable();

            if (historyParams.Mnf != null)
            {
                history  = history.Where(u => u.Mnf == historyParams.Mnf);
            }

            if (historyParams.ReelId != 0 )
            {
                history  = history.Where(u => u.ReelId == historyParams.ReelId);
            }

             if (historyParams.OldLocation!= null)
            {
                history  = history.Where(u => u.OldLocation == historyParams.OldLocation);
            }
             if (historyParams.NewLocation!= null)
            {
                history  = history.Where(u => u.NewLocation == historyParams.NewLocation);
            }

            history = history.OrderByDescending(u => u.DateAdded);

            if (!string.IsNullOrEmpty(historyParams.OrderBy))
            {
                switch (historyParams.OrderBy)
                {
                    case "Mnf":
                        history = history.OrderBy(u => u.Mnf);
                        break;
                    case "id":
                        history = history.OrderBy(u => u.ReelId);
                        break;
                    default:
                        history = history.OrderBy(u => u.DateAdded);
                        break;
                }
            }

            return await PageList<History>.CreateAsync(history, historyParams.PageNumber, historyParams.PageSize);
        }
        public async Task<bool> MnfExists(string Mnf)
        {
            if (await _context.Componentass.AnyAsync(x => x.Mnf == Mnf))
                return true;

            return false;

        }

        public async Task<Componentas> GetComponentBuhNr(string buhNr)
        {
            var comp = await _context.Componentass.Where(u => u.BuhNr == buhNr).FirstOrDefaultAsync();
            return comp;
        }
    }
}