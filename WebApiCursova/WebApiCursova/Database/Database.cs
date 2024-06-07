using Npgsql;
using WebApiCursova.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiCursova.Database
{
    public class Database
    {
        public async Task InsertItemAsync(Item item)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(Constants.Connect))
            {
                await con.OpenAsync();

                using (NpgsqlCommand comm = new NpgsqlCommand())
                {
                    comm.Connection = con;
                    comm.CommandText = "INSERT INTO public.\"Dota2Items\"(\"id\", \"name\", \"price\", \"isInSecretShop\", \"isInBasicShop\", \"recipe\") VALUES (@id, @name, @price, @isInSecretShop, @isInBasicShop, @recipe)";
                    comm.Parameters.AddWithValue("@id", item.Id);
                    comm.Parameters.AddWithValue("@name", item.Name);
                    comm.Parameters.AddWithValue("@price", item.Price);
                    comm.Parameters.AddWithValue("@isInSecretShop", item.IsInSecretShop);
                    comm.Parameters.AddWithValue("@isInBasicShop", item.IsInBasicShop);
                    comm.Parameters.AddWithValue("@recipe", item.Recipe);

                    await comm.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Item>> GetItemsAsync()
        {
            var items = new List<Item>();

            using (NpgsqlConnection con = new NpgsqlConnection(Constants.Connect))
            {
                await con.OpenAsync();

                using (NpgsqlCommand comm = new NpgsqlCommand("SELECT * FROM public.\"Dota2Items\" ORDER BY \"id\"", con))
                {
                    using (var reader = await comm.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new Item
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Price = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                                IsInSecretShop = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                IsInBasicShop = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                Recipe = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                            };
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        public async Task DeleteItemAsync(string name)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(Constants.Connect))
            {
                await con.OpenAsync();

                using (NpgsqlCommand comm = new NpgsqlCommand("DELETE FROM public.\"Dota2Items\" WHERE \"name\" = @name", con))
                {
                    comm.Parameters.AddWithValue("@name", name);
                    await comm.ExecuteNonQueryAsync();
                }

                var items = await GetItemsAsync();
                int newId = 1;
                foreach (var item in items)
                {
                    using (NpgsqlCommand comm = new NpgsqlCommand("UPDATE public.\"Dota2Items\" SET \"id\" = @newId WHERE \"id\" = @oldId", con))
                    {
                        comm.Parameters.AddWithValue("@newId", newId);
                        comm.Parameters.AddWithValue("@oldId", item.Id);
                        await comm.ExecuteNonQueryAsync();
                    }
                    newId++;
                }
            }
        }

        public async Task UpdateItemByNameAsync(string oldName, Item updatedItem)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(Constants.Connect))
            {
                await con.OpenAsync();

                using (NpgsqlCommand comm = new NpgsqlCommand())
                {
                    comm.Connection = con;
                    comm.CommandText = "UPDATE public.\"Dota2Items\" SET \"name\" = @newName, \"price\" = @price, \"isInSecretShop\" = @isInSecretShop, \"isInBasicShop\" = @isInBasicShop, \"recipe\" = @recipe WHERE \"name\" = @oldName";
                    comm.Parameters.AddWithValue("@newName", updatedItem.Name);
                    comm.Parameters.AddWithValue("@price", updatedItem.Price);
                    comm.Parameters.AddWithValue("@isInSecretShop", updatedItem.IsInSecretShop);
                    comm.Parameters.AddWithValue("@isInBasicShop", updatedItem.IsInBasicShop);
                    comm.Parameters.AddWithValue("@recipe", updatedItem.Recipe);
                    comm.Parameters.AddWithValue("@oldName", oldName);

                    await comm.ExecuteNonQueryAsync();
                }
            }
        }
    }
}


