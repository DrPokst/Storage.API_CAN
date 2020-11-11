using System.Collections.Generic;
using System.Text;

namespace Storage.API_CAN.Services
{
    public interface IBomService
    {
        List<string> GetPartNumber(string filePath, int lookForMnf);
        List<string> GetNumber(string filePath, int lookForMnfQuantity);
    }
}