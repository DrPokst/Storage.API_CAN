using System.Collections.Generic;
using System.Threading.Tasks;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.Data
{
    public interface IBomRepository
    {
        Task<List<BomList>> RegisterBomList (List<BomList> bomList);
        Task<BomName> RegisterBomName (BomName bomName);
        Task<BomName> GetBomName (string name);
        Task<IEnumerable<BomName>> GetBomNames();
        Task<IEnumerable<BomList>> GetBomList(string name);
        Task<IEnumerable<BomList>> GetBomListXQty( int xQty, string name);

    }
}