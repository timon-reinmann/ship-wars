using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Yoo.Trainees.ShipWars.Api.Logic;
using Yoo.Trainees.ShipWars.DataBase;

namespace Yoo.Trainees.ShipWars.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Email

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IVerificationLogic, VerificationLogic>();
            builder.Services.AddTransient<IGameLogic, GameLogic>();

            builder.Services.AddSignalR();

            builder.Services.AddCors();

            // Add services to the container.
            builder.Services.AddControllers();

            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var environmentName = builder.Environment.EnvironmentName;

            builder.Configuration
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer("name=ConnectionStrings:Database"));

            builder.Services.AddScoped<IGameLogic, GameLogic>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseCors(
                options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
            );

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            app.MapPost("broadcast", async (string message, IHubContext<ChatHub, IChatClient> context) =>
            {
                 await context.Clients.All.ReceiveMessage(message);

                return Results.NoContent();
            });

            app.UseHttpsRedirection();
            app.MapHub<ChatHub>("chat-hub");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
