using MediatR;

namespace URLShortener.Services.Commands
{
    public class GenerateUrlCodeCommand : IRequest<string>
    {
        public string Code { get; set; }
    }
}
