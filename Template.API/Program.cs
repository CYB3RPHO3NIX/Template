
using Microsoft.EntityFrameworkCore;
using Template.Database.Domain.Contexts;
using Template.Database;
using Template.Services;
using Template.CommandHandlers;
using Template.QueryHandlers;

namespace Template.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDatabaseAccess<TemplateDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddBus();
            builder.Services.RegisterCommandHandlers();
            builder.Services.RegisterQueryHandlers();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                
            }
            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
