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
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://192.168.8.98/api/Led/off/all"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Byte[]>(apiResponse);
                    return true;
                }
            }

        }

        public async Task<bool> TurnOffLed(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://192.168.8.98/api/Led/off" + "?id=" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Byte[]>(apiResponse);
                    return true;
                }
            }

        }

        public async Task<bool> TurnOnAll(string color)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://192.168.8.98/api/Led/on/all" + "?color=" + color))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Byte[]>(apiResponse);
                    return true;
                }
            }

        }

        public async Task<bool> TurnOnLed(int id, string color)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://192.168.8.98/api/Led/on" + "?id=" + id + "&color=" + color))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Byte[]>(apiResponse);
                    return true;
                }
            }

        }
    }
}
