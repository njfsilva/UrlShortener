using MediatR;
using URLShortener.Entities;
using URLShortener.Services.Commands;
using URLShortener.Services.Queries;

namespace URLShortener.Endpoints
{
    using Carter;
    using DTOs;

    public class ShortenEndpoints : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/shorten", ShortenUrl);

            app.MapGet("/redirect/{code}", RedirectToLongUrl);
        }

        private static async Task<IResult> RedirectToLongUrl(string code, ISender mediator, HttpContext httpContext)
        {
            var origin = httpContext.Request.Headers.Origin.ToString();

            var shortenedUrl = await mediator.Send(new GetShortUrlQuery(code, origin));

            return shortenedUrl != null ? Results.Redirect(shortenedUrl.LongUrl) : Results.NotFound();
        }

        public async Task<IResult> ShortenUrl(ShortenRequest request, ISender mediator, HttpContext httpContext)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("The URL is invalid");
            }

            var code = await mediator.Send(new GenerateUrlCodeCommand());

            var shortenedUrl = new ShortenedUrl
            {
                Code = code,
                LongUrl = request.Url,
                ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/redirect/{code}",
                CreateDate = DateTimeOffset.UtcNow
            };

            await mediator.Send(new CreateNewCodeCommand(shortenedUrl));

            return Results.Ok(shortenedUrl);
        }
    }
}
