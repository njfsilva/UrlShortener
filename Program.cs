using URLShortener.Data;
using URLShortener.Entities;
using URLShortener.HostedServices;

namespace URLShortener
{

    using Carter;
    using MongoDB.Driver;
    using URLShortener.CrossCutting;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCarter();
            builder.Services.AddMemoryCache();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

            builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

            builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration.GetSection("MongoDb:ConnectionString").Value));

            builder.Services.AddSingleton<GenericDbContext<ShortenedUrl>>();

            builder.Services.AddSingleton<UrlShortenService>();

            builder.Services.Configure<HostOptions>(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            });

            builder.Services.AddHostedService<CacheStartUpHostedService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapCarter();

            //app.UseAuthorization();

            app.Run();
        }
    }
}
