using System.Threading.Tasks;

namespace Storage.API.CAN
{
    public interface ILedRepository
    {
        Task<bool> TurnOnLed(int id, string color);
        Task<bool> TurnOffLed(int id);
        Task<bool> TurnOnAll(string color);
        Task<bool> TurnOffAll();
    }
}
