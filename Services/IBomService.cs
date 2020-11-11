using System.Collections.Generic;
using System.IO;
using System.Text;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.Services
{
    public interface IBomService
    {
        List<string> GetPartNumber(string filePath, int lookForMnf);
        List<string> GetNumber(string filePath, int lookForMnfQuantity);
        List<BomList> GetDataFromFile(Stream stream);
    }
}