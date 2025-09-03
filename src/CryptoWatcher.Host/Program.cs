using System.Text.Json;
using System.Text.Json.Serialization;
using CryptoWatcher.Host.Extensions;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.Extensions;
using CryptoWatcher.Infrastructure.Hyperliquid;
using CryptoWatcher.Infrastructure.Uniswap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

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

builder.Services.AddInfrastructure();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});

var app = builder.Build();

app.UseTickerQ();

app.MapGet("/report",
    async (IUniswapExcelReportService uniswapExcelReportService, IHyperliquidExcelService hyperliquidExcelService,
        [FromQuery] bool poolReport,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to) =>
    {
        var repot = poolReport
            ? await uniswapExcelReportService.ExportPoolInfoToExcelAsync(from, to)
            : await hyperliquidExcelService.CreateReportAsync(from, to);

        return TypedResults.File(repot, fileDownloadName: "report.xlsx");
    });

app.Run();