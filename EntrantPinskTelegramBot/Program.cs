using AutoMapper;
using EntrantPinsk.Common.Mapper;
using EntrantPinsk.Model;
using EntrantPinskTelegramBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExperiments
{
    
class Program
    {
       
        public static TelegramBotClient client;
        public static async Task HandleUpdates(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(botClient, update.Message);
                return;
            }
        }
        public static Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken cancellation)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        public static async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup keyboard = 
                    new(new[] 
                    {
                        new KeyboardButton[] { "Базовое", "Среднее общее"},
                        new KeyboardButton[] {"Профессионально - техническое", "Среднее специальное" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.From.FirstName}, " +
                       $"я помогу определиться с выбором места учебы. Какое образование у тебя на данный момент?", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Базовое")
            {
                ReplyKeyboardMarkup keyboard = 
                    new(new[] 
                    {
                        new KeyboardButton[] { "Хочу профессионально - техническое" },
                        new KeyboardButton[] { "Хочу среднее специальное" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Какое образование ты бы хотел получить?", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Среднее общее" || message.Text == "Профессионально - техническое" || message.Text == "Среднее специальное")
            {
                ReplyKeyboardMarkup keyboard = new(new KeyboardButton[] { "Хочу высшее" })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Какое образование ты бы хотел получить?", replyMarkup: keyboard);
                return;
            }

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder.UseSqlServer(connectionString).Options;

            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperProfile()));
            IMapper mapper = mappingConfig.CreateMapper();

            using (ApplicationContext db = new(options))
            {
                db.Database.EnsureCreated();

                var directions = db.Directions
                    .Include(i => i.EducationalInstitution)
                    .ThenInclude(e => e.Education)
                    .AsNoTracking()
                    .ToList();
               List<string> directionsList = new();
                foreach (var direction in directions)
                    if (direction.EducationalInstitution.Education.Name.ToString() == "Профессионально - техническое")
                        directionsList.Add(direction.Name);

                for (int i = 0; i < directionsList.Count; i++)
                    for (int s = i + 1; s < directionsList.Count; s++)
                        if (directionsList[i] == directionsList[s]) directionsList.Remove(directionsList[s]);
                      
                if (message.Text == "Хочу профессионально - техническое")
                {
                    KeyboardButton[][] kbDirections = new KeyboardButton[directionsList.Count][];
                    for (int i = 0, d = 0; i < kbDirections.Length; i++, d++)
                        kbDirections[i] = new KeyboardButton[1] { directionsList[d] };
                    ReplyKeyboardMarkup keyboard = new(kbDirections)
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери направление:", replyMarkup: keyboard);
                    return;
                }



                /* if (message.Text == "Хочу среднее специальное")
                 {
                     KeyboardButton[][] kbDirections = new KeyboardButton[directions.Count][];
                     foreach (var direction in directions)
                     {
                         for (int i = 0, d = 0; i < kbDirections.Length; i++, d++)
                             kbDirections[i] = new KeyboardButton[1] { directions[d].Name };
                     }
                     ReplyKeyboardMarkup keyboard = new(kbDirections)
                     {
                         ResizeKeyboard = true
                     };
                     await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери направление:", replyMarkup: keyboard);
                     return;
                 }*/
            }
            /*
            
            if (message.Text == "Хочу среднее специальное")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Искусство и дизайн" },
                        new KeyboardButton[] { "Здравоохранение" },
                        new KeyboardButton[] { "Педагогика" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери направление:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Хочу высшее")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Финансы и кредит" },
                        new KeyboardButton[] { "Техника и технологии" },
                        new KeyboardButton[] { "Физическая культура. Туризм и гостеприимство" },
                        new KeyboardButton[] { "Естественные науки" },
                        new KeyboardButton[] { "Гуманитарные науки" },
                        new KeyboardButton[] { "Сельское хозяйство. Садово-парковое строительство" },
                        new KeyboardButton[] { "Экономика. Организация и управление на предприятии" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери направление:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Финансы и кредит")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Финансы" },
                        new KeyboardButton[] { "Банковское дело" },
                        new KeyboardButton[] { "Страхование" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери специальность:", replyMarkup: keyboard);
                return;
            }
           
            if (message.Text == "Естественные науки")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Биология (научная и производственная деятельность)" },
                        new KeyboardButton[] { "Биология (биотехнология)" },
                        new KeyboardButton[] { "Аналитическая биохимия" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери специальность:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Гуманитарные науки")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Лингвистическое обеспечение межкультурных коммуникаций (внешнеэкономические связи)"}
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери специальность:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Сельское хозяйство. Садово-парковое строительство")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Ландшафтное проектирование" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери специальность:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Техника и технологии")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Информационные технологии финансово-кредитной системы" },
                        new KeyboardButton[] {"Технология переработки рыбной продукции"}
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери специальность:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Физическая культура. Туризм и гостеприимство")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Физическая культура (лечебная)" },
                        new KeyboardButton[] { "Физическая культура (для дошкольников)" },
                        new KeyboardButton[] { "Оздоровительная и адаптивная физическая культура (оздоровительная)" },
                        new KeyboardButton[] { "Оздоровительная и адаптивная физическая культура (адаптивная)" },
                        new KeyboardButton[] { "Физическая реабилитация и эрготерапия (физическая реабилитация)" },
                        new KeyboardButton[] { "Физическая реабилитация и эрготерапия (эрготерапия)" },
                        new KeyboardButton[] { "Спортивно-педагогическая деятельность (тренерская работа с указанием вида спорта)" },
                        new KeyboardButton[] { "Менеджмент (в сфере международного туризма)" },
                        new KeyboardButton[] { "Предпринимательство в области физической культуры, спорта и международного туризма" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери специальность:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Экономика. Организация и управление на предприятии")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Экономика и управление на предприятии промышленности" },
                        new KeyboardButton[] { "Экономика и управление на предприятии АПК" },
                         new KeyboardButton[] { "Бухгалтерский учет, анализ и аудит в промышленности" },
                        new KeyboardButton[] { "Маркетинг предприятий промышленности" },
                        new KeyboardButton[] { "Аналитическая экономика" },



                        new KeyboardButton[] { "Экономика и управление на малых и средних предприятиях" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери специальность:", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Финансы" || message.Text == "Банковское дело" || message.Text == "Страхование" || message.Text == "Физическая культура (лечебная)" 
                || message.Text == "Физическая культура (для дошкольников)" || message.Text == "Оздоровительная и адаптивная физическая культура (оздоровительная)" 
                || message.Text == "Оздоровительная и адаптивная физическая культура (адаптивная)" || message.Text == "Физическая реабилитация и эрготерапия (физическая реабилитация)" 
                || message.Text == "Физическая реабилитация и эрготерапия (эрготерапия)" || message.Text == "Спортивно-педагогическая деятельность (тренерская работа с указанием вида спорта)" 
                || message.Text == "Менеджмент (в сфере международного туризма)" || message.Text == "Предпринимательство в области физической культуры, спорта и международного туризма" 
                || message.Text == "Информационные технологии финансово-кредитной системы" || message.Text == "Технология переработки рыбной продукции"
                || message.Text == "Лингвистическое обеспечение межкультурных коммуникаций (внешнеэкономические связи)" || message.Text == "Биология (научная и производственная деятельность)"
                || message.Text == "Биология (биотехнология)" || message.Text == "Аналитическая биохимия" || message.Text ==  "Ландшафтное проектирование"
                || message.Text == "Экономика и управление на предприятии промышленности" || message.Text == "Экономика и управление на предприятии АПК"
                || message.Text == "Бухгалтерский учет, анализ и аудит в промышленности" || message.Text == "Маркетинг предприятий промышленности"
                || message.Text == "Аналитическая экономика"
                )
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "УВО \"ПОЛЕССКИЙ ГОСУДАРСТВЕННЫЙ УНИВЕРСИТЕТ\" " +
                    " \nТелефон: (8-0165) 65 00 41" +
                    " \nАдрес: ул. Днепровской флотилии, 23" +
                    " \nEmail: box@polessu.by" +
                    " \nСайт: https://polessu.by");
                return;
            }*/
            await botClient.SendTextMessageAsync(message.Chat, "Введи /start, чтобы начать");
        }


        static void Main(string[] args)
        {
            /*
            
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder.UseSqlServer(connectionString).Options;

            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperProfile()));
            IMapper mapper = mappingConfig.CreateMapper();
            */
            /*
            using (ApplicationContext db = new(options))
            {
                db.Database.EnsureCreated();
                Console.WriteLine("\nList of  Educational Institution:");
                var institutions = db.EducationalInstitutions.ToList();
                foreach (var institution in institutions) Console.WriteLine(institution.Name);
            }
            */
            client = new(Config.Token);
            Console.WriteLine("Запущен бот " + client.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };

            client.StartReceiving(HandleUpdates, HandleError, receiverOptions, cancellationToken: cts.Token);
            Console.ReadLine();
        }
    }
}