using MediatR;
using URLShortener.Entities;

namespace URLShortener.Services.Commands
{
    public class CreateNewCodeCommand(ShortenedUrl shortenedUrl) : IRequest
    {
        public ShortenedUrl ShortenedUrl { get; set; } = shortenedUrl;
    }
}
