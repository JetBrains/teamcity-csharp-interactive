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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

static class Bot
{
    public static IEnumerable<PollAnswer> Run(IHost host, string token, string messageToSend, TimeSpan timeout, string question, params string[] options)
    {
        var start = DateTime.Now.ToUniversalTime();
        var pollAnswers = new List<PollAnswer>();
        var usersCount = 0;
        using var finish = new ManualResetEvent(false);
        new TelegramBotClient(token).StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));
        finish.WaitOne(timeout);
        return pollAnswers;

        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case ApiRequestException apiRequestException: host.Error(apiRequestException.Message, $"Bot{apiRequestException.ErrorCode}");
                    break;

                default: host.Error(exception.ToString(), "Bot");
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

                                await botClient.SendPollAsync(
                                    chatId:update.Message.Chat.Id,
                                    question: question,
                                    options: options,
                                    allowsMultipleAnswers: false,
                                    isAnonymous: false,
                                    type:PollType.Regular,
                                    closeDate: (start + timeout).ToUniversalTime(),
                                    cancellationToken: cancellationToken);

                                usersCount++;
                                break;
                        }
                    }

                    break;

                case UpdateType.PollAnswer:
                    pollAnswers.Add(update.PollAnswer);
                    if (pollAnswers.Count == usersCount)
                    {
                        finish.Set();
                    }

                    break;
            }
        }
    }    
}

var answers = Bot.Run(
    Host,
    token,
    $"The <a href='{Args[0]}'>build #{Props["build.number"]} \"{Props["teamcity.buildConfName"]}\"</a> has been almost completed.",
    TimeSpan.FromMinutes(timeout),
    "We are ready to deploy?",
    "Yes, deploy.",
    "No, abort this build.")
    .Where(i => i.OptionIds.Length == 1)
    .Select(i => (approved: i.OptionIds[0] == 0, user: $"{i.User.FirstName} {i.User.LastName}"))
    .ToList();

var approvingUsers = answers.Where(i => i.approved).Select(i => i.user).ToList();
var cancelingUsers = answers.Where(i => !i.approved).Select(i => i.user).ToList();

if (cancelingUsers.Any())
{
    Host.Error($"Canceled by {string.Join(",", cancelingUsers)}.", "Bot");
}
else
{
    if (approvingUsers.Any())
    {
        Host.WriteLine($"Approved by {string.Join(",", approvingUsers)}.");
    }
    else
    {
        Host.Error("Has no answer, canceled.", "Bot");
    }
}