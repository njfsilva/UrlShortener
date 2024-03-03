using URLShortener.Entities;

namespace URLShortener.Endpoints
{
    using Carter;
    using DTOs;
    using Services;

    public class ShortenEndpoints : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/shorten", ShortenUrl);

            app.MapGet("/redirect/{code}", RedirectToLongUrl);
        }

        private static async Task<IResult> RedirectToLongUrl(string code, UrlShortenService urlShortenService, HttpContext httpContext)
        {
            var origin = httpContext.Request.Headers.Origin.ToString();

            var shortenedUrl = await urlShortenService.GetShortUrl(code, origin);

            return shortenedUrl != null ? Results.Redirect(shortenedUrl.LongUrl) : Results.NotFound();
        }

        public async Task<IResult> ShortenUrl(ShortenRequest request, UrlShortenService urlShortenService, HttpContext httpContext)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("The URL is invalid");
            }

            var code = await urlShortenService.GenerateCode();

            var shortenedUrl = new ShortenedUrl
            {
                Code = code,
                LongUrl = request.Url,
                ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/redirect/{code}",
                CreateDate = DateTimeOffset.UtcNow
            };

            await urlShortenService.InsertNew(shortenedUrl);

            return Results.Ok(shortenedUrl);
        }
    }
}
