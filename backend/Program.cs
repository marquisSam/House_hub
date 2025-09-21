// program.cs
using HouseHub.AppDataContext;
using HouseHub.Middleware;
using HouseHub.Models;
using HouseHub.Services;
using HouseHub.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder);

var app = builder.Build();

ConfigureApp(app);

app.Run();

// Méthode pour configurer les services
static void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    services.AddExceptionHandler<GlobalExceptionHandler>();

    services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
    
    // Add ItemDbContext to the service collection with SQL Server as the database provider
    // The connection string is retrieved from the configuration
    // by not making it a singleton, it will be created each time the context is used for each request
    services.AddDbContext<ItemDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
    
    services.AddScoped<ITodoServices, TodoServices>();

    services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
    });
}

// Méthode pour configurer l'application
static void ConfigureApp(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAngularApp");
    app.UseAuthorization();
    app.MapControllers();
}