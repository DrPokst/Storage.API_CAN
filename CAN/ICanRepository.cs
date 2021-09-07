using Storage.API.Models;
using System.Threading.Tasks;

namespace Storage.API.CAN
{
    public interface ICanRepository
    {
        Task<Rxmsg> SetReelLocation();
        Task<bool> TakeOutReel(int id);
    }
}
