// program.cs
using HouseHub.AppDataContext;
using HouseHub.Middleware;
using HouseHub.Models;
using HouseHub.Services;
using HouseHub.Interface;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder);

var app = builder.Build();

ConfigureApp(app);

app.Run();

// Méthode pour configurer les services
static void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
{
    // Build EDM Model for OData
    var modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<Todo>("Todos");
    modelBuilder.EntitySet<Event>("Events");
    modelBuilder.EntitySet<User>("Users");
    var edmModel = modelBuilder.GetEdmModel();

    services.AddControllers()
        .AddOData(options => options
            .Select()
            .Filter()
            .OrderBy()
            .Expand()
            .Count()
            .SetMaxTop(100)
            .AddRouteComponents("odata", edmModel));
    
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "HouseHub API", Version = "v1" });
        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    services.AddExceptionHandler<GlobalExceptionHandler>();

    services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
    
    // Add ItemDbContext to the service collection with PostgreSQL as the database provider
    // The connection string is retrieved from the configuration
    // by not making it a singleton, it will be created each time the context is used for each request
    services.AddDbContext<ItemDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetSection("DbSettings")["ConnectionString"]));
    
    services.AddScoped<ITodoServices, TodoServices>();
    services.AddScoped<IUserServices, UserServices>();

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