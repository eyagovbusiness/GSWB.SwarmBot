using MandrilAPI.Configuration;
using MandrilBot;

namespace MandrilAPI
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Sets-up the discord bot 
            await builder.SetDiscordBotConfiguration();
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Controllers.MandrilController).Assembly);
            });    

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }


    }
}