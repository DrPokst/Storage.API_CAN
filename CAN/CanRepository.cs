using Newtonsoft.Json;
using Storage.API.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Storage.API.CAN
{
    public class CanRepository : ICanRepository
    {
        public HttpClient httpClient { get; set; }

        public async Task<Rxmsg> SetReelLocation()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://192.168.8.98/api/Can/setlocation"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Byte[]>(apiResponse);
                    Rxmsg msg = new Rxmsg
                    {
                        DLC = 8, //nenaudojama
                        ID = 2,  //nenaudojama
                        Msg = result
                    };
                    return msg;
                }
            }
        }

        public async Task<bool> TakeOutReel(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://192.168.8.98/api/Can/takeout" + "?id=" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Byte[]>(apiResponse);
                    return true;
                }
            }

        }
    }
}
