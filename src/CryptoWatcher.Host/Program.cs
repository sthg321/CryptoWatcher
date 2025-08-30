using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using AaveClient.Extensions;
using CoinGeckoClient.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Application;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Host.Configs;
using CryptoWatcher.Host.Extensions;
using CryptoWatcher.Host.Integrations;
using CryptoWatcher.Host.Services;
using CryptoWatcher.HyperliquidModule.Abstractions;
using CryptoWatcher.HyperliquidModule.Extensions;
using CryptoWatcher.Infrastructure.Hyperliquid;
using CryptoWatcher.Integrations;
using CryptoWatcher.UniswapModule.Abstractions;
using CryptoWatcher.UniswapModule.Extensions;
using CryptoWatcher.UniswapModule.Services;
using HyperliquidClient.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using UniswapClient.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ExternalServicesConfig>(builder.Configuration.GetSection(nameof(ExternalServicesConfig)));

builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<ExternalServicesConfig>>().Value);

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

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TokenEnricher>();

builder.Services.AddUniswapClient();
builder.Services.AddSingleton<IUniswapProvider, UniswapProvider>();

builder.Services.AddScoped<IPoolHistorySyncRepositoryFacade, PoolHistorySyncRepositoryFacade>();
builder.Services.AddScoped<UniswapExcelService>();
builder.Services.AddScoped<PoolHistorySyncService>();

builder.Services.AddCoinGeckoClient(provider => provider.GetRequiredService<ExternalServicesConfig>().CoinGecko);
builder.Services.AddTransient<ICoinPriceProvider, CoinGeckoCoinPriceProvider>();

builder.Services.AddSingleton<CoinPriceService>();
builder.Services.AddSingleton<CoinNormalizer>();

builder.Services.AddAaveClient();
builder.Services.AddHyperLiquidClient();
builder.Services.AddScoped<IHyperliquidProvider, HyperliquidProvider>();
builder.Services.AddScoped<HyperliquidExcelService>();

builder.Services.AddSingleton<AaveProvider>();

builder.Services
    .AddUniswapModule()
    .AddHyperliquidModule();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});

var app = builder.Build();

app.UseTickerQ();

app.MapGet("/report",
    async (UniswapExcelService uniswapExcelService, HyperliquidExcelService hyperliquidExcelService,
        [FromQuery] bool poolReport,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to) =>
    {
        var repot = poolReport
            ? await uniswapExcelService.ExportPoolInfoToExcelAsync(from, to)
            : await hyperliquidExcelService.CreateReportAsync(from, to);

        return TypedResults.File(repot, fileDownloadName: "report.xlsx");
    });

app.Run();