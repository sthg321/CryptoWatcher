using System.Linq.Expressions;
using AaveClient;
using AaveClient.UiPoolDataProvider;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Uniswap;
using CryptoWatcher.Core;
using CryptoWatcher.Data;
using CryptoWatcher.Host.Configs;
using CryptoWatcher.Host.Extensions;
using CryptoWatcher.Host.Integrations;
using CryptoWatcher.Host.Services;
using CryptoWatcher.PoolHistorySyncFeature;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniswapClient.Extensions;
using UniswapClient.UniswapV4;
using UniswapProvider = CryptoWatcher.Application.Uniswap.UniswapProvider;

var xx = new UiPoolDataProviderFetcher();

await xx.GetUserReservesDataAsync(AaveNetwork.Sonic, "0xeb9191d780c0aB6Ab320C5F05E41ebF81f14255f");
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ExternalServicesConfig>(builder.Configuration.GetSection(nameof(ExternalServicesConfig)));

builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<ExternalServicesConfig>>().Value);

builder.Services.AddConfiguredDatabase(builder.Configuration.GetConnectionString("Postgres")!);

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddHybridCache();

builder.Services.AddHangfire(configuration => configuration.UseRecommendedSerializerSettings().UseInMemoryStorage())
    .AddHangfireServer();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TokenEnricher>();

builder.Services.AddUniswapClient();

builder.Services.AddScoped<IPoolHistorySyncRepositoryFacade, PoolHistorySyncRepositoryFacade>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<PoolHistorySyncService>();

builder.Services.AddSingleton<IUniswapMath, UniswapMath>();
builder.Services.AddSingleton<UniswapProvider>();

builder.Services.AddHttpClient<CoinGeckoTokenPriceProvider>((provider, client) =>
    client.BaseAddress = provider.GetRequiredService<ExternalServicesConfig>().CoinGecko);

builder.Services.AddHttpClient<UniswapV4ClientOld>((provider, client) =>
    client.BaseAddress = provider.GetRequiredService<ExternalServicesConfig>().Uniswap);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CryptoWatcherDbContext>();

    db.Database.Migrate();

    var service = scope.ServiceProvider.GetRequiredService<PoolHistorySyncService>();

    Expression<Func<Task>> x = () => service.SyncAsync(CancellationToken.None);

    var recurringManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManagerV2>();
    recurringManager.AddOrUpdate("pool_history", x, Cron.Hourly);
    recurringManager.TriggerJob("pool_history");
}

app.MapGet("/report", async (ExcelService excelService, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to) =>
{
    var repot = await excelService.ExportPoolInfoToExcelAsync(from, to);

    return TypedResults.File(repot, fileDownloadName: "report.xlsx");
});

app.Run();