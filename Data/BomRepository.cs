using System.Threading.Tasks;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.Data
{
    public class BomRepository : IBomRepository
    {
        public Task<BomList> RegisterBomList(BomList bomList)
        {
            throw new System.NotImplementedException();
        }

        public Task<BomName> RegisterBomName(BomName bomName)
        {
            throw new System.NotImplementedException();
        }
    }
}