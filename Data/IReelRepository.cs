using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Storage.API.Helpers;
using Storage.API.Models;


namespace Storage.API.Data
{
    public interface IReelRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<PageList<Reel>> GetReels(ReelParams reelParams);
         Task<Reel> GetReel(int id);
         Task<Reel> GetByLocation(string Location);
         Task<Reel> GetReelCMnf(string cMnf);
         Task<IQueryable<Reel>> GetReelByMnf(int id);
         Task<Photo2> RegisterPhoto(Photo2 photo);
         Task<Photo2> GetPhoto(int Rid);
         Task<Reel> RegisterReel(Reel reel);
         

    }
}