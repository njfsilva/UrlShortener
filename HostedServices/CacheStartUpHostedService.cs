using Microsoft.Extensions.Caching.Memory;
using URLShortener.Data;
using URLShortener.Entities;

namespace URLShortener.HostedServices
{
    public class CacheStartUpHostedService(IMemoryCache cache, GenericDbContext<ShortenedUrl> dbContext) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var itemsToCache = await dbContext.GetAllAsync();

            foreach (var shortenedUrl in itemsToCache)
            {
                cache.Set(shortenedUrl.Code, shortenedUrl);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cache.Dispose();
            return Task.CompletedTask;
        }
    }
}
