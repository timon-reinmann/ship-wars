using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yoo.Trainees.ShipWars.DataBase;

namespace Yoo.Trainees.ShipWars.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors();
            //builder.Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //SignalR
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseCors(
                options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
            );

            //app.UseMvc();
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

            //SignalR ChatHub
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
