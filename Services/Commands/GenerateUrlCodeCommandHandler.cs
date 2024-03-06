using MediatR;
using URLShortener.Data;
using URLShortener.Entities;

namespace URLShortener.Services.Commands
{
    public class GenerateUrlCodeCommandHandler(GenericDbContext<ShortenedUrl> dbContext) : IRequestHandler<GenerateUrlCodeCommand, string>
    {
        public const int UrlSize = 5;
        private static readonly string UpperAlphabet = "ABCDEFGIJKLMNOPQRSTUVWYXZ";
        private static readonly string LowerAlphabet = UpperAlphabet.ToLower();
        private static readonly string Numbers = "1234567890";
        private static readonly string Alphabet = $"{UpperAlphabet}{LowerAlphabet}{Numbers}";


        public async Task<string> Handle(GenerateUrlCodeCommand request, CancellationToken cancellationToken)
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

                if (await dbContext.AnyAsync(x => x.Code.Equals(code))) continue;

                return code;
            }
        }
    }
}
