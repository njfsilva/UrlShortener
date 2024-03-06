using MediatR;
using Microsoft.Extensions.Caching.Memory;
using URLShortener.Data;
using URLShortener.Entities;

namespace URLShortener.Services.Commands
{
    public class CreateNewCodeCommandHandler(GenericDbContext<ShortenedUrl> dbContext, IMemoryCache cache) : IRequestHandler<CreateNewCodeCommand>
    {
        public async Task Handle(CreateNewCodeCommand request, CancellationToken cancellationToken)
        {
            await dbContext.AddAsync(request.ShortenedUrl);
            cache.Set(request.ShortenedUrl.Code, request.ShortenedUrl);
        }
    }
}
