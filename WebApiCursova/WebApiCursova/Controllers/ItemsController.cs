using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCursova.Models;
using WebApiCursova.Database;

namespace WebApiCursova.Controllers
{
    [Route("/api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly Database.Database _database = new Database.Database();

        [HttpGet]
        public async Task<IEnumerable<Item>> Get()
        {
            return await _database.GetItemsAsync();
        }

        [HttpGet("ItemName")]
        public async Task<IActionResult> Get(string Name)
        {
            var items = await _database.GetItemsAsync();
            var item = items.SingleOrDefault(p => p.Name == Name);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpDelete("DeleteItem")]
        public async Task<IActionResult> Delete(string Name)
        {
            var items = await _database.GetItemsAsync();
            var item = items.SingleOrDefault(p => p.Name == Name);
            if (item == null)
            {
                return NotFound(new { Message = "Item not found" });
            }

            await _database.DeleteItemAsync(Name);
            return Ok(new { Message = "Deleted Successfully" });
        }

        [HttpPost("AddItem")]
        public async Task<IActionResult> Post(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _database.InsertItemAsync(item);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        [HttpPut("UpdateItem/{oldName}")]
        public async Task<IActionResult> Put(string oldName, [FromBody] Item updatedItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var items = await _database.GetItemsAsync();
            var storedItem = items.SingleOrDefault(p => p.Name == oldName);
            if (storedItem == null)
            {
                return NotFound(new { Message = "Item not found" });
            }

            await _database.UpdateItemByNameAsync(oldName, updatedItem);
            return Ok(new { Message = "Updated Successfully" });
        }
    }
}