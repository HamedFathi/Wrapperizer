using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Wrapperizer.Extensions.Caching.Behaviors
{
    public sealed class CacheBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TResponse : class
    {
        private readonly IDistributedCache _distributedCache;

        public CacheBehaviour(IDistributedCache distributedCache)
            =>
                _distributedCache = distributedCache
                                    ?? throw new ArgumentNullException(nameof(distributedCache),
                                        "Please configure distributed cache in DI container of the application.");

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var bytes = await _distributedCache.GetAsync(typeof(TRequest).Name, cancellationToken);

            return (bytes != null)
                ? JsonSerializer.Deserialize<TResponse>(bytes)
                : await CallAndCache(cancellationToken, next).ConfigureAwait(false);
        }

        private async Task<TResponse> CallAndCache(CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next().ConfigureAwait(false);

            var json = JsonSerializer.SerializeToUtf8Bytes<TResponse>(response);

            await _distributedCache.SetAsync(typeof(TRequest).Name, json, cancellationToken)
                .ConfigureAwait(false);

            return response;
        }
    }
}
