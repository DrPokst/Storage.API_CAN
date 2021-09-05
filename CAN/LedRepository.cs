using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Storage.API.CAN
{
    public class LedRepository : ILedRepository
    {
        public async Task<bool> TurnOffAll()
        {
            var link = "http://192.168.8.98/api/Led/off/all";
            await HttpRequest(link); 
            return true;
        }

        public async Task<bool> TurnOffLed(int id)
        {
            var link = "http://192.168.8.98/api/Led/off" + "?id=" + id;
            await HttpRequest(link); 
            return true;
        }

        public async Task<bool> TurnOnAll(string color)
        {
            var link = "http://192.168.8.98/api/Led/on/all" + "?color=" + color;
            await HttpRequest(link); 
            return true;
        }

        public async Task<bool> TurnOnLed(int id, string color)
        {
            var link = "http://192.168.8.98/api/Led/on" + "?id=" + id + "&color=" + color;
            await HttpRequest(link); 
            return true;
        }

        private async Task<bool> HttpRequest(string link){
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(link))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Byte[]>(apiResponse);
                    return true;
                }
            }
        }
    }
}
