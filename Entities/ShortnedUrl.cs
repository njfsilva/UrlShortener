namespace URLShortener.Entities
{
    public class ShortenedUrl
    {
        public string? Id { get; set; } = Ulid.NewUlid().ToString();

        public string Code { get; set; }

        public string? ShortUrl { get; set; }

        public string? LongUrl { get; set; }

        public int UsageCount { get; set; }

        public List<string>? SourcesUrls { get; set; } = [];

        public DateTimeOffset CreateDate { get; set; }
    }
}
