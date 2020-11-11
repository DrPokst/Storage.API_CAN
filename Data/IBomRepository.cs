using System.Threading.Tasks;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.Data
{
    public interface IBomRepository
    {
        Task<BomList> RegisterBomList (BomList bomList);
        Task<BomName> RegisterBomName (BomName bomName);

    }
}