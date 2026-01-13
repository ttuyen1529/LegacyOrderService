using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LegacyOrderService.Core.Observability;

public sealed class LoggingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly ILogger<LoggingBehavior<TReq, TRes>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TReq, TRes>> logger)
        => _logger = logger;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var name = typeof(TReq).Name;
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName}: {@Request}", name, request);
        try
        {
            var res = await next();
            _logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", name, sw.ElapsedMilliseconds);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed {RequestName}", name);
            throw;
        }
    }
}
