using System.ComponentModel.DataAnnotations;

namespace WebApiCursova.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]

        public string Name { get; set; }

        public double Price { get; set; }

        public string IsInSecretShop { get; set; }

        public string IsInBasicShop { get; set; }

        public string Recipe { get; set; }
    }
}