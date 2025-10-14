namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.UniswapAppApiClient.Contracts;

internal record GetPositionsResponse(IReadOnlyCollection<Position> Positions);