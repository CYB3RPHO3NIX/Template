
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Template.CommandHandlers;
using Template.Database;
using Template.Database.Domain.Contexts;
using Template.QueryHandlers;
using Template.Services;

namespace Template.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            var sinkOptions = new MSSqlServerSinkOptions
            {
                TableName = "ApplicationLogs",
                SchemaName = "log",
                AutoCreateSqlTable = true
            };

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.MSSqlServer(
                connectionString: connectionString,
                sinkOptions: sinkOptions)
            .CreateLogger();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDatabaseAccess<TemplateDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddBus();
            builder.Services.RegisterCommandHandlers();
            builder.Services.RegisterQueryHandlers();
            //builder.Services.RegisterEventHandlers(); needs to come
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                
            }
            app.UseSwagger();

            app.UseSwaggerUI();

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
