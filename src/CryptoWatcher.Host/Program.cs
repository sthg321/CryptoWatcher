using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using AaveClient;
using AaveClient.Extensions;
using CoinGeckoClient;
using CoinGeckoClient.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Application;
using CryptoWatcher.Application.Uniswap;
using CryptoWatcher.Core;
using CryptoWatcher.Data;
using CryptoWatcher.Entities;
using CryptoWatcher.Host.Configs;
using CryptoWatcher.Host.Extensions;
using CryptoWatcher.Host.Integrations;
using CryptoWatcher.Host.Services;
using CryptoWatcher.Integrations;
using CryptoWatcher.PoolHistorySyncFeature;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniswapClient.Extensions;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ExternalServicesConfig>(builder.Configuration.GetSection(nameof(ExternalServicesConfig)));

builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<ExternalServicesConfig>>().Value);

builder.Services.AddConfiguredDatabase(builder.Configuration.GetConnectionString("Postgres")!);

builder.Services
    .AddStackExchangeRedisCache(options => options.Configuration = builder.Configuration.GetConnectionString("Redis"))
    .AddHybridCache();

builder.Services
    .AddHangfire(configuration => configuration.UseRecommendedSerializerSettings().UseInMemoryStorage())
    .AddHangfireServer();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TokenEnricher>();

builder.Services.AddUniswapClient();
builder.Services.AddSingleton<IUniswapProvider, UniswapProvider>();

builder.Services.AddScoped<IPoolHistorySyncRepositoryFacade, PoolHistorySyncRepositoryFacade>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<PoolHistorySyncService>();

builder.Services.AddSingleton<IUniswapMath, UniswapMath>();
builder.Services.AddSingleton<IUniswapProvider, UniswapProvider>();

builder.Services.AddCoinGeckoClient(provider => provider.GetRequiredService<ExternalServicesConfig>().CoinGecko);
builder.Services.AddTransient<ICoinPriceProvider, CoinGeckoCoinPriceProvider>();

builder.Services.AddSingleton<CoinPriceService>();
builder.Services.AddSingleton<CoinNormalizer>();

builder.Services.AddAaveClient();
builder.Services.AddSingleton<AaveProvider>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});

var app = builder.Build();

Console.WriteLine(app.Environment.EnvironmentName);
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CryptoWatcherDbContext>();

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