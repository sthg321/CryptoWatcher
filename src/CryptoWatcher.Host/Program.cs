using System.Text.Json;
using System.Text.Json.Serialization;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Host.Extensions;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.CronJobs.Aave;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;
using CryptoWatcher.Infrastructure.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<ExternalServicesConfig>(builder.Configuration.GetSection(nameof(ExternalServicesConfig)));

builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<ExternalServicesConfig>>().Value);

builder.Services.AddConfiguredDatabase(builder.Configuration);

builder.Services
    .AddStackExchangeRedisCache(options => options.Configuration = builder.Configuration.GetConnectionString("Redis"))
    .AddHybridCache();

builder.Services.AddInfrastructure();
builder.Services.AddTelegram(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Hangfire")))
    .UseRecurringJob(typeof(SyncAavePositionsCronJob).Assembly.GetRecurringJobs));

builder.Services.AddHangfireServer();

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CryptoWatcherDbContext>();

    if (!app.Environment.IsDevelopment())
    {
        db.Database.Migrate();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapHangfireDashboardWithNoAuthorizationFilters();

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

app.MapPost("/sync-daily-performance",
    async (IDailyPositionPerformanceCoordinator coordinator, DateOnly from, DateOnly to, CancellationToken ct) =>
    {
        await coordinator.SynchronizeDailyBalanceChangesAsync(from, to, ct);

        return TypedResults.Ok();
    });

app.MapPost("/hyperliquid/sync-positions",
    async (
        CryptoWatcherDbContext dbContext,
        IHyperliquidPositionsSyncService coordinator, DateOnly from, DateOnly to, CancellationToken ct) =>
    {
        var wallets = await dbContext.Wallets.ToArrayAsync(ct);

        foreach (var wallet in wallets)
        {
            await coordinator.SyncPositionsAsync(wallet, from, to, ct);
        }

        return TypedResults.Ok();
    });

app.MapPost("/uniswap/sync-block/{transactionHash}", async (IUniswapPositionFromTransactionUpdater sync,
    CryptoWatcherDbContext dbContext,
    string transactionHash,
    string chainName,
    string walletAddress,
    int protocolVersion) =>
{
    var chains = dbContext.UniswapChainConfigurations
        .First(configuration => configuration.Name == chainName &&
                                configuration.ProtocolVersion == (UniswapProtocolVersion)protocolVersion);

    var hash = TransactionHash.FromString(transactionHash);

    await sync.ApplyEventFromTransactionAsync(chains, new Wallet { Address = EvmAddress.Create(walletAddress) }, hash);
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