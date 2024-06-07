using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WebApiCursova.Models;  

namespace TelegramBot
{
    class TgBot
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("7014750616:AAFKuwsnjwmlhv5oC4H05R_VsAI1g1sdImM");
        private static readonly HttpClient HttpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            if (!await CheckApiConnectionAsync("https://localhost:7077/api/hero/AllHeroes"))
            {
                Console.WriteLine("API недоступне. Перевірте конфігурацію.");
                return;
            }

            var cts = new System.Threading.CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            Bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);

            var me = await Bot.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, System.Threading.CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message!.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                if (messageText.StartsWith("/hero "))
                {
                    var heroName = messageText.Substring(6).Trim();
                    var heroInfo = await GetHeroInfo(heroName);
                    await botClient.SendTextMessageAsync(chatId, heroInfo, cancellationToken: cancellationToken);
                }
                else if (messageText.StartsWith("/item "))
                {
                    var itemName = messageText.Substring(6).Trim();
                    var itemInfo = await GetItemInfo(itemName);
                    await botClient.SendTextMessageAsync(chatId, itemInfo, cancellationToken: cancellationToken);
                }
                else if (messageText.StartsWith("/deleteitem "))
                {
                    var itemName = messageText.Substring(12).Trim();
                    var deleteResponse = await DeleteItem(itemName);
                    await botClient.SendTextMessageAsync(chatId, deleteResponse, cancellationToken: cancellationToken);
                }
                else
                {
                    switch (messageText.ToLower())
                    {
                        case "/start":
                            var startMessage = "Привіт, я Telegram Bot Dota 2 Items. Я можу надати інформацію про героїв та предмети з гри Dota 2.\n" +
                                               "Для подальшого використання знадобляться команди, наведені нижче: \n" +
                                               "/hero (Hero Name) - шукати героя за іменем \n" +
                                               "/allheroes - переглянути список імен всіх героїв \n" +
                                               "/item (Item Name) - шукати інформацію про предмет за іменем \n" +
                                               "/allitems - переглянути список назв всіх предметів \n" +
                                               "/deleteitem (Item Name) - видалити предмет за іменем \n" +
                                               "Пояснення отриманих даних про атрибути героїв:\n" +
                                               "Основні атрибути: str - Основним атрибутом є сила (Strength)\n" +
                                               "Основні атрибути: agi - Основним атрибутом є спритність (Agility)\n" +
                                               "Основні атрибути: int - Основним атрибутом є інтелект (Intelligence)\n" +
                                               "Основні атрибути: all - Не має основного атрибуту (Універсал)\n";
                            await botClient.SendTextMessageAsync(chatId, startMessage, cancellationToken: cancellationToken);
                            break;

                        case "/allheroes":
                            var heroesMessage = await GetAllHeroes();
                            await botClient.SendTextMessageAsync(chatId, "Список усіх героїв:\n" + heroesMessage, cancellationToken: cancellationToken);
                            break;

                        case "/allitems":
                            var itemsMessage = await GetAllItems();
                            await botClient.SendTextMessageAsync(chatId, "Список усіх предметів:\n" + itemsMessage, cancellationToken: cancellationToken);
                            break;

                            // Інші команди можна додати аналогічним чином
                    }
                }
            }
        }
        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        private static async Task<string> GetAllHeroes()
        {
            var response = await HttpClient.GetAsync("https://localhost:7077/api/hero/AllHeroes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var heroesList = JsonConvert.DeserializeObject<List<string>>(content);
            return string.Join("\n", heroesList);
        }

        private static async Task<string> GetHeroInfo(string heroName)
        {
            var response = await HttpClient.GetAsync($"https://localhost:7077/api/hero/Hero?localizedName={Uri.EscapeDataString(heroName)}");
            if (!response.IsSuccessStatusCode)
            {
                return $"Героя з ім'ям '{heroName}' не знайдено.";
            }

            var content = await response.Content.ReadAsStringAsync();
            var hero = JsonConvert.DeserializeObject<Heroes>(content);
            return $"Характеристики даного героя: \n" +
                   $"Ім'я: {hero.Localized_name}\n" +
                   $"Основний атрибут: {hero.Primary_attr}\n" +
                   $"Тип атаки: {hero.Attack_type}\n" +
                   $"Ролі: {string.Join(", ", hero.Roles)}";
        }

        private static async Task<bool> CheckApiConnectionAsync(string url)
        {
            try
            {
                var response = await HttpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        private static async Task<string> GetAllItems()
        {
            var response = await HttpClient.GetAsync("https://localhost:7077/api/items");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var itemsList = JsonConvert.DeserializeObject<List<Item>>(content);
            return string.Join("\n", itemsList.Select(item => item.Name));
        }

        private static async Task<string> GetItemInfo(string itemName)
        {
            var response = await HttpClient.GetAsync($"https://localhost:7077/api/items/ItemName?Name={Uri.EscapeDataString(itemName)}");
            if (!response.IsSuccessStatusCode)
            {
                return $"Предмет з ім'ям '{itemName}' не знайдено.";
            }

            var content = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<Item>(content);
            return $"Інформація про предмет:\n" +
                   $"Назва: {item.Name}\n" +
                   $"Ціна: {item.Price}\n" +
                   $"У секретному магазині: {item.IsInSecretShop}\n" +
                   $"У базовому магазині: {item.IsInBasicShop}\n" +
                   $"Рецепт: {item.Recipe}";
        }
        private static async Task<string> DeleteItem(string itemName)
        {
            var response = await HttpClient.DeleteAsync($"https://localhost:7077/api/items/DeleteItem?Name={Uri.EscapeDataString(itemName)}");
            if (!response.IsSuccessStatusCode)
            {
                return $"Предмет з ім'ям '{itemName}' не знайдено або не вдалося видалити.";
            }

            var content = await response.Content.ReadAsStringAsync();
            return $"Предмет '{itemName}' успішно видалено.";
        }
    }
}