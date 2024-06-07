using WebApiCursova.Models;
using Newtonsoft.Json;

namespace WebApiCursova.Clients
{
    public class HeroClient
    {
        private static string _address = "https://api.opendota.com/api/";
        private HttpClient _client;

        public HeroClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_address);
        }

        public async Task<Hero> GetHeroes()
        {
            var response = await _client.GetAsync("heroes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var heroesList = JsonConvert.DeserializeObject<List<Heroes>>(content);

            var hero = new Hero { Heroes = heroesList };
            return hero;
        }
    }
}

