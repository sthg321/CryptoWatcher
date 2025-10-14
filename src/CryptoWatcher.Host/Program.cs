using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Host.Extensions;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;
using CryptoWatcher.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<ExternalServicesConfig>(builder.Configuration.GetSection(nameof(ExternalServicesConfig)));

builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<ExternalServicesConfig>>().Value);

builder.Services.Configure<AaveConfig>(builder.Configuration.GetSection(nameof(AaveConfig)));

builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<AaveConfig>>().Value);

builder.Services.AddConfiguredDatabase(builder.Configuration.GetConnectionString("Postgres")!);

builder.Services
    .AddStackExchangeRedisCache(options => options.Configuration = builder.Configuration.GetConnectionString("Redis"))
    .AddHybridCache();

builder.Services.AddTickerQ(optionsBuilder =>
{
    optionsBuilder.SetInstanceIdentifier("CryptoWatcher");
    optionsBuilder.AddDashboard();
    optionsBuilder.AddOperationalStore<CryptoWatcherDbContext>(optionBuilder =>
    {
        optionBuilder.UseModelCustomizerForMigrations();
    });
});

builder.Services.AddInfrastructure();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    if (!app.Environment.IsDevelopment())
    {
        scope.ServiceProvider.GetRequiredService<CryptoWatcherDbContext>().Database.Migrate();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseTickerQ();

async Task<FileStreamHttpResult> Handler(IPlatformDailyReportFacade reportFacade,
    IRepository<Wallet> walletRepository,
    string platform,
    [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
{
    var wallets = await walletRepository.ListAsync();
    var excelReport = platform switch
    {
        "uniswap" => await reportFacade.CreateUniswapReportAsync(wallets, from, to),
        "hyperliquid" => await reportFacade.CreateHyperliquidReportAsync(wallets, from, to),
        "aave" => await reportFacade.CreateAaveReportAsync(wallets, from, to),
        _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
    };

    return TypedResults.File(excelReport.Report, fileDownloadName: $"{excelReport.FileName}.xlsx");
}

app.MapGet("/report/{platform}", Handler);

app.MapGet("/report/total", TotalReportHandler);

app.MapPost("/uniswap/sync-block/{blockNumber}", async (IUniswapCashFlowBlockRangeSynchronizer sync,
    CryptoWatcherDbContext dbContext,
    BigInteger blockNumber) =>
{
    var chain = await dbContext.UniswapChainConfigurations
        .Include(configuration => configuration.LiquidityPoolPositions)
        .ThenInclude(positions => positions.Wallet)
        .FirstAsync();

    await sync.SynchronizeBlockRangeAsync(chain, blockNumber, blockNumber, false);
});

app.MapPatch("/uniswap/sync-block/{blockNumber}", async (IUniswapCashFlowBlockRangeSynchronizer sync,
    CryptoWatcherDbContext dbContext,
    BigInteger blockNumber) =>
{
    await using var tr = await dbContext.Database.BeginTransactionAsync();
    try
    {
        var now = DateTime.UtcNow;
        
        await dbContext.UniswapChainConfigurations
            .ExecuteUpdateAsync(calls =>
                calls.SetProperty(configuration => configuration.LastProcessedBlock, blockNumber));
        
        await dbContext.UniswapChainConfigurations
            .ExecuteUpdateAsync(calls =>
                calls.SetProperty(configuration => configuration.LastProcessedBlockUpdatedAt, now));

        await tr.CommitAsync();
    }
    catch (Exception)
    {
        await tr.RollbackAsync();
        throw;
    }
 
});

async Task<FileStreamHttpResult> TotalReportHandler(IDailySummaryReportProvider reportProvider,
    IRepository<Wallet> walletRepository,
    [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
{
    var wallets = await walletRepository.ListAsync();

    var now = DateTime.Now;
    var monthStart = new DateTime(now.Year, now.Month, 1);
    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

    if (!from.HasValue || !to.HasValue)
    {
        from = DateOnly.FromDateTime(monthStart);
        to = DateOnly.FromDateTime(monthEnd);
    }

    var result = await reportProvider.CreateDailySummaryReportAsync(wallets, from.Value, to.Value);

    return TypedResults.File(result, fileDownloadName: "report.xlsx");
}

app.Run();