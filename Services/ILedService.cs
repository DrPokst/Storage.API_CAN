using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Storage.API.Models;

namespace Storage.API.Services
{
    public interface ILedService
    {
        Task<bool> TurnOnLed(int id);
        Task<bool> TurnOffLed(int id);
        Task<Rxmsg> SetReelLocation();
        Task<bool> TakeOutReel(int id);
        Task<bool> TurnOnAll(String color);
        Task<bool> TurnOffAll();
    }
}
