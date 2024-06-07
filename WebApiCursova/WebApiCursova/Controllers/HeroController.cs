using Microsoft.AspNetCore.Mvc;
using WebApiCursova.Clients;
using WebApiCursova.Models;

namespace WebApiCursova.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeroController : ControllerBase
    {
        private readonly HeroClient _heroClient;

        public HeroController(HeroClient heroClient)
        {
            _heroClient = heroClient;
        }

        [HttpGet("Hero")]
        public async Task<IActionResult> GetHeroByLocalizedName(string localizedName)
        {
            var heroes = await _heroClient.GetHeroes();
            var hero = heroes.Heroes.Find(h => h.Localized_name.Equals(localizedName, StringComparison.OrdinalIgnoreCase));

            if (hero == null)
            {
                return NotFound();
            }

            return Ok(hero);
        }
        [HttpGet("AllHeroes")]
        public async Task<IActionResult> GetAllLocalizedNames()
        {
            var heroes = await _heroClient.GetHeroes();
            var localizedNames = heroes.Heroes.Select(h => h.Localized_name).ToList();

            return Ok(localizedNames);
        }
    }

}