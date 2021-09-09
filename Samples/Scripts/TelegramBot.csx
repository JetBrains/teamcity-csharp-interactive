if(!Props.TryGetValue("telegram.bot.token", out var token))
{
    throw new ArgumentException($"Required TeamCity parameter system.telegram.bot.token was not specified, see https://telegrambots.github.io/book/1/quickstart.html#bot-father");
}

if(!Props.TryGetValue("telegram.bot.poll.timeout", out var timeoutStr) || !int.TryParse(timeoutStr, out var timeout))
{
    throw new ArgumentException($"Required TeamCity parameter system.telegram.bot.poll.timeout (poll timeout in minutes) was not specified");
}

#r "nuget: Telegram.Bot, 15.6.0"
#r "nuget: Telegram.Bot.Extensions.Polling, 0.2.0"

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

static class Bot
{
    public static PollAnswer Run(string token, string messageToSend, TimeSpan timeout, string question, params string[] options)
    {
        var start = DateTime.Now.ToUniversalTime();
        var pollMessages = new List<Message>();
        PollAnswer pollAnswer = default;
        using var finish = new ManualResetEvent(false);
        new TelegramBotClient(token).StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));
        finish.WaitOne(timeout);
        return pollAnswer;

        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case ApiRequestException apiRequestException: Error(apiRequestException.Message, $"Bot{apiRequestException.ErrorCode}");
                    break;

                default: Error(exception.ToString(), "Bot");
                    break;
            }
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message.Type == MessageType.Text)
                    {
                        var message = update.Message.Text;
                        switch (message.ToLowerInvariant())
                        {
                            case "/start":
                                await botClient.SendTextMessageAsync(
                                    update.Message.Chat.Id,
                                    messageToSend,
                                    parseMode: ParseMode.Html,
                                    disableNotification: true,
                                    cancellationToken: cancellationToken);
                                var pollMessage = await botClient.SendPollAsync(
                                    chatId:update.Message.Chat.Id,
                                    question: question,
                                    options: options,
                                    allowsMultipleAnswers: false,
                                    isAnonymous: false,
                                    type:PollType.Regular,
                                    closeDate: (start + timeout).ToUniversalTime(),
                                    cancellationToken: cancellationToken);

                                pollMessages.Add(pollMessage);
                                break;
                        }
                    }

                    break;

                case UpdateType.PollAnswer:
                    if (pollMessages.Count > 0)
                    {
                        foreach (var message in pollMessages)
                        {
                            await botClient.StopPollAsync(message.Chat.Id, message.MessageId, cancellationToken: cancellationToken);
                        }

                        pollAnswer = update.PollAnswer;
                        pollMessages.Clear();
                        finish.Set();
                    }

                    break;
            }
        }
    }
}

var result = Bot.Run(token, $"The <a href='{Args[0]}'>build #{Props["build.number"]} \"{Props["teamcity.buildConfName"]}\"</a> has been almost completed.", TimeSpan.FromMinutes(timeout), "We are ready to deploy?", "Yes, deploy.", "No, abort this build.");

switch (result?.OptionIds?.Length == 1 ? result.OptionIds[0] : int.MaxValue)
{
    case 0: WriteLine($"Approved by {result.User.FirstName} {result.User.LastName}. Deploy starting ...", Success);
        break;
    
    case 1: Error($"Aborted by {result.User.FirstName} {result.User.LastName}. Deploy is canceled.", "Bot_Aborted");
        break;
    
    default: Error("Has no answer. Deploy is canceled.", "Bot_NoAnswer");
        break;
}