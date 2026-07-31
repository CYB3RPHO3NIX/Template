
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Template.API.Extensions;
using Template.API.Middleware;
using Template.CommandHandlers;
using Template.Database;
using Template.Database.Domain.Contexts;
using Template.EventHandlers;
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

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDatabaseAccess<TemplateDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddBus();
            builder.Services.AddJwtTokenService();
            builder.Services.AddApiLogging();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCustomHealthChecks(builder.Services.BuildServiceProvider());
            builder.Services.RegisterCommandHandlers();
            builder.Services.RegisterQueryHandlers();
            builder.Services.RegisterEventHandlers();
            builder.Services.RegisterMapster();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapCustomHealthChecks();

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
