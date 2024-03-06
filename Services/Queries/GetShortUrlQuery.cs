using MediatR;
using URLShortener.Entities;

namespace URLShortener.Services.Queries
{
    public class GetShortUrlQuery(string code, string origin) : IRequest<ShortenedUrl?>
    {
        public string Code { get; set; } = code;

        public string Origin { get; set; } = origin;
    }
}
