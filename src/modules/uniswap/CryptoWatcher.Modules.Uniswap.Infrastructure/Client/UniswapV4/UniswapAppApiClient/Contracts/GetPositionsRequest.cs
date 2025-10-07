namespace UniswapClient.UniswapV4.UniswapAppApiClient.Contracts;

internal record GetPositionsRequest(
    string Address,
    IReadOnlyCollection<int> ChainIds,
    IReadOnlyCollection<string> ProtocolVersions,
    IReadOnlyCollection<string> PositionStatuses,
    int PageSize,
    string PageToken,
    bool IncludeHidden);