using CryptoWatcher.Infrastructure.Telegram;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace CryptoWatcher.Infrastructure.BackgroundServices;

public class TelegramBackgroundService : BackgroundService
{
    private readonly TelegramBotClient _client;
    private readonly TelegramReportHandler _handler;

    public TelegramBackgroundService(TelegramBotClient client, TelegramReportHandler handler)
    {
        _client = client;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.StartReceiving(_handler, new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message],
            DropPendingUpdates = true
        }, stoppingToken);
    }
}