using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure.Excel.Overall.Uniswap;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;
using CryptoWatcher.Infrastructure.Extensions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Application.Services;
using CryptoWatcher.Modules.Morpho.Application.Services;
using CryptoWatcher.Modules.Uniswap.Application.Models.Reports;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace CryptoWatcher.Infrastructure.Telegram;

public class TelegramReportHandler : IUpdateHandler
{
    private readonly ILogger<TelegramReportHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TelegramReportHandler(ILogger<TelegramReportHandler> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var wallets = await scope.ServiceProvider.GetRequiredService<IRepository<Wallet>>()
            .ListAsync(cancellationToken);

        var dailyReportFacade = scope.ServiceProvider.GetRequiredService<IPlatformDailyReportFacade>();

        var (from, to) = DateTime.Now.GetCurrentMonthRange();

        if (update.Message!.Text == "/status")
        {
            var walletAddresses = wallets.Select(x => x.Address).ToArray();
            var morphoService = scope.ServiceProvider.GetRequiredService<MorphoPositionsStatusService>();
            var aaveService = scope.ServiceProvider.GetRequiredService<AaveAccountStatusService>();
            var status = await morphoService.GetPositionsStatusAsync(walletAddresses,
                DateOnly.FromDateTime(DateTime.Now), cancellationToken);

            var aaveHf =
                await aaveService.Test(walletAddresses, DateOnly.FromDateTime(DateTime.Now), cancellationToken);

            await botClient.SendMessage(update.Message!.From!.Id,
                MorphoPositionStatusMessageCreator.CreateMessageFromModels(status),
                cancellationToken: cancellationToken);

            await botClient.SendMessage(update.Message!.From!.Id, $"Aave Health Factor: {Math.Round(aaveHf, 2)}",
                cancellationToken: cancellationToken);
            return;
        }

        var excelReport = update.Message!.Text switch
        {
            "/uniswap" => await dailyReportFacade.CreateUniswapReportAsync(wallets, null, null, cancellationToken),
            "/hyperliquid" => await dailyReportFacade.CreateHyperliquidReportAsync(wallets, null, null,
                cancellationToken),
            "/aave" => await dailyReportFacade.CreateAaveReportAsync(wallets, null, null, cancellationToken),
            "/uniswap_overall" => await scope.ServiceProvider.GetRequiredService<UniswapOverallExcelReportService>()
                .CreateReportAsync(wallets, from, to, cancellationToken)
        };

        await botClient.SendDocument(update.Message!.From!.Id,
            InputFile.FromStream(excelReport.Report, excelReport.FileName),
            cancellationToken: cancellationToken);
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unable to handle updated");

        return Task.CompletedTask;
    }
}