using Microsoft.Extensions.Caching.Memory;

namespace URLShortener.Services
{
    using Data;
    using Entities;

    public class UrlShortenService(GenericDbContext<ShortenedUrl> dbContext, IMemoryCache cache)
    {
        public const int UrlSize = 5;
        private static readonly string UpperAlphabet = "ABCDEFGIJKLMNOPQRSTUVWYXZ";
        private static readonly string LowerAlphabet = UpperAlphabet.ToLower();
        private static readonly string Numbers = "1234567890";
        private static readonly string Alphabet = $"{UpperAlphabet}{LowerAlphabet}{Numbers}";

        public async Task<string> GenerateCode()
        {
            while (true)
            {

                var code = string.Empty;
                var random = new Random();

                for (var i = 0; i < UrlSize; i++)
                {
                    var index = random.Next(Alphabet.Length - 1);
                    code += Alphabet[index];
                }

                if (!await dbContext.AnyAsync(x => x.Code.Equals(code)))
                {
                    return code;
                }
            }
        }

        public async Task InsertNew(ShortenedUrl shortenedUrl)
        {
            await dbContext.AddAsync(shortenedUrl);
            cache.Set(shortenedUrl.Code, shortenedUrl);
        }

        public async Task<ShortenedUrl?> GetShortUrl(string code, string origin)
        {
            var shortenedUrl = cache.Get<ShortenedUrl>(code) ?? await dbContext.GetSingleAsync(x => x.Code.Equals(code));

            shortenedUrl.UsageCount++;

            if (shortenedUrl.SourcesUrls != null && shortenedUrl.SourcesUrls.All(x => x != origin) && !string.IsNullOrEmpty(origin))
            {
                shortenedUrl.SourcesUrls.Add(origin);
            }

            await dbContext.UpdateAsync(x => x.Id == shortenedUrl.Id, shortenedUrl);

            cache.Set(code, shortenedUrl);

            return shortenedUrl;
        }
    }
}
