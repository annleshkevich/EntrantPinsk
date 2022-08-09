﻿using AutoMapper;
using EntrantPinsk.Common.DTOs;
using EntrantPinsk.Common.Mapper;
using EntrantPinsk.Model;
using EntrantPinsk.Model.DatabaseModels;
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
        const string basic = "Базовое";
        const string secondary = "Среднее общее";
        const string vocational = "Профессионально - техническое";
        const string postSecondary = "Среднее специальное";
        const string higher = "Высшее";
        const string wantEducation = "Какое образование ты бы хотел получить?";
        const string choiceOfDirection = "Выбери направление:";
        const string choiceOfSpecialty = "Выбери специальность:";
        static string education = "";
        static string displayEducationalInstitution = "";
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
                ReplyKeyboardMarkup keyboard = new(new[]
                {
                    new KeyboardButton[] { basic, secondary},
                    new KeyboardButton[] { vocational, postSecondary}
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.From.FirstName}, " +
                       $"я помогу определиться с выбором места учебы. Какое образование у тебя на данный момент?", replyMarkup: keyboard);
                return;
            }
            if (message.Text == basic)
            {
                ReplyKeyboardMarkup keyboard = new(new[]
                {
                    new KeyboardButton[] { $"Хочу {vocational}" },
                    new KeyboardButton[] { $"Хочу {postSecondary}" }
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, wantEducation, replyMarkup: keyboard);
                return;
            }
            if (message.Text == secondary || message.Text == vocational || message.Text == postSecondary)
            {
                ReplyKeyboardMarkup keyboard = new(new KeyboardButton[] { $"Хочу {higher}" })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, wantEducation, replyMarkup: keyboard);
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

                var specialties = db.Specialties
                    .Include(d => d.Direction)
                    .ThenInclude(i => i.EducationalInstitution)
                    .ThenInclude(e => e.Education)
                    .AsNoTracking()
                    .ToList();

                var educations = db.Educations
                    .AsNoTracking()
                    .ToList();

                ReplyKeyboardMarkup ListOfDirections(string educationName)
                {
                    List<string> directionsList = new();
                    foreach (var direction in directions)
                        if (direction.EducationalInstitution.Education.Name.ToString() == educationName)
                            directionsList.Add(direction.Name);

                    for (int i = 0; i < directionsList.Count; i++)
                        for (int s = i + 1; s < directionsList.Count; s++)
                            if (directionsList[i] == directionsList[s]) directionsList.Remove(directionsList[s]);

                    KeyboardButton[][] kbDirections = new KeyboardButton[directionsList.Count][];
                    for (int i = 0, d = 0; i < kbDirections.Length; i++, d++)
                        kbDirections[i] = new KeyboardButton[1] { directionsList[d] };
                    ReplyKeyboardMarkup keyboard = new(kbDirections)
                    {
                        ResizeKeyboard = true
                    };
                    return keyboard;
                }
                ReplyKeyboardMarkup ListOfSpecialties(string directionName, string education)
                {
                    List<string> specialtiesList = new();
                    foreach (var specialty in specialties)
                        if (specialty.Direction.Name == directionName && specialty.Direction.EducationalInstitution.Education.Name == education)
                            specialtiesList.Add(specialty.Name);

                    for (int i = 0; i < specialtiesList.Count; i++)
                        for (int s = i + 1; s < specialtiesList.Count; s++)
                            if (specialtiesList[i] == specialtiesList[s]) specialtiesList.Remove(specialtiesList[s]);

                    KeyboardButton[][] kbSpecialties = new KeyboardButton[specialtiesList.Count][];
                    for (int i = 0, d = 0; i < kbSpecialties.Length; i++, d++)
                        kbSpecialties[i] = new KeyboardButton[1] { specialtiesList[d] };
                    ReplyKeyboardMarkup keyboard = new(kbSpecialties)
                    {
                        ResizeKeyboard = true
                    };
                    return keyboard;
                }
                string DisplayEducationalInstitution(Specialty selectedSpecialty)
                {
                    foreach (var specialty in specialties)
                        if (specialty.Name  == selectedSpecialty.Name)
                        displayEducationalInstitution = $"\"{selectedSpecialty.Direction.EducationalInstitution.Name}\"" +
                                $"\nТелефон: {selectedSpecialty.Direction.EducationalInstitution.Phone}" +
                                $"\nАдрес: {selectedSpecialty.Direction.EducationalInstitution.Address}" +
                                $"\nEmail: {selectedSpecialty.Direction.EducationalInstitution.Email}" +
                                $"\nСайт: {selectedSpecialty.Direction.EducationalInstitution.Website}";
                    return displayEducationalInstitution;
                }
                for (int i = 0; i < educations.Count; i++)
                    if (message.Text == $"Хочу {educations[i].Name}")
                    {
                        education = educations[i].Name;
                        await botClient.SendTextMessageAsync(message.Chat.Id, choiceOfDirection, replyMarkup: ListOfDirections(educations[i].Name));
                        return;
                    }

                for (int i = 0; i < directions.Count; i++)
                    if (message.Text == directions[i].Name)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, choiceOfSpecialty, replyMarkup: ListOfSpecialties(directions[i].Name, education));
                        return;
                    }
                for (int i = 0; i < specialties.Count; i++)
                { 
                    if (message.Text == specialties[i].Name)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, DisplayEducationalInstitution(specialties[i]));
                        return;
                    }
                }
            }

            await botClient.SendTextMessageAsync(message.Chat, "Введи /start, чтобы начать");
        }


        static void Main(string[] args)
        {
            
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