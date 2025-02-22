using CountryBlockerAPI.Services;

namespace CountryBlockerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Load `appsettings.json`
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Register `IConfiguration` to ensure it can be read from services
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            // Add services to the container.
            builder.Services.AddSingleton<BlockService>();
            builder.Services.AddHostedService<BlockCleanupService>();
            builder.Services.AddHttpClient<GeoLocationService>(); // Register the service

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
