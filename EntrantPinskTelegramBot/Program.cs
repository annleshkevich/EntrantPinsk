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
                        new KeyboardButton[] { "Хочу среднее специальное" },
                        new KeyboardButton[] { "Хочу высшее" }
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
            if (message.Text == "Хочу профессионально - техническое")
            {
                ReplyKeyboardMarkup keyboard =
                    new(new[]
                    {
                        new KeyboardButton[] { "Легкая промышленностью. Швейное производство" },
                        new KeyboardButton[] { "Промышленное и гражданское строительство" },
                        new KeyboardButton[] { "Техника и технологии" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери направление:", replyMarkup: keyboard);
                return;
            }
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
                        new KeyboardButton[] { "Естественные науки" },
                        new KeyboardButton[] { "Гуманитарные науки" },
                        new KeyboardButton[] { "Физическая культура. Туризм и гостеприимство" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери направление:", replyMarkup: keyboard);
                return;
            }
            await botClient.SendTextMessageAsync(message.Chat, "Введи /start, чтобы начать");
        }

        static void Main(string[] args)
        {
            
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
            }
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