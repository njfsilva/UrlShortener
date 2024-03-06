using MediatR;
using Microsoft.Extensions.Caching.Memory;
using URLShortener.Data;
using URLShortener.Entities;

namespace URLShortener.Services.Queries
{
    public class GetShortUrlQueryHandler(GenericDbContext<ShortenedUrl> dbContext, IMemoryCache cache) : IRequestHandler<GetShortUrlQuery, ShortenedUrl>
    {

        public async Task<ShortenedUrl> Handle(GetShortUrlQuery request, CancellationToken cancellationToken)
        {
            var shortenedUrl = cache.Get<ShortenedUrl>(request.Code) ?? await dbContext.GetSingleAsync(x => x.Code.Equals(request.Code));

            shortenedUrl.UsageCount++;

            if (shortenedUrl.SourcesUrls != null && shortenedUrl.SourcesUrls.All(x => x != request.Origin) && !string.IsNullOrEmpty(request.Origin))
            {
                shortenedUrl.SourcesUrls.Add(request.Origin);
            }

            await dbContext.UpdateAsync(x => x.Id == shortenedUrl.Id, shortenedUrl);

            cache.Set(request.Code, shortenedUrl);

            return shortenedUrl;
        }
    }
}
