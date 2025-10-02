// ============================================================================
// HouseHub API - Main Entry Point (Program.cs)
// ============================================================================
// This file configures and starts the ASP.NET Core web application.
// It sets up all services, middleware, and routing for the HouseHub API.
// ============================================================================

using HouseHub.AppDataContext;
using HouseHub.Middleware;
using HouseHub.Models;
using HouseHub.Services;
using HouseHub.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

// ============================================================================
// APPLICATION STARTUP
// ============================================================================

// Create the web application builder - this is the starting point
WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Configure all services (dependency injection container)
ConfigureServices(builder.Services, builder);

// Build the application with all configured services
WebApplication? app = builder.Build();

// Configure the HTTP request pipeline (middleware)
ConfigureApp(app);

// Start the application and listen for incoming requests
app.Run();

// ============================================================================
// SERVICE CONFIGURATION METHOD
// ============================================================================
// This method registers all services needed by the application into the
// dependency injection container. Services registered here can be injected
// into controllers, other services, and middleware.
// ============================================================================
static void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
{
    // ------------------------------------------------------------------------
    // ODATA CONFIGURATION
    // ------------------------------------------------------------------------
    // OData allows clients to query data using URL parameters like:
    // - $filter: Filter results (e.g., /odata/Todos?$filter=IsCompleted eq true)
    // - $select: Choose specific fields (e.g., /odata/Todos?$select=Title,DueDate)
    // - $orderby: Sort results (e.g., /odata/Todos?$orderby=CreatedDate desc)
    // - $expand: Include related entities (e.g., /odata/Todos?$expand=Users)
    // - $top/$skip: Pagination (e.g., /odata/Todos?$top=10&$skip=20)
    // ------------------------------------------------------------------------
    
    // Build the Entity Data Model (EDM) for OData
    ODataConventionModelBuilder? modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<Todo>("Todos");      // Expose Todos at /odata/Todos
    modelBuilder.EntitySet<Event>("Events");    // Expose Events at /odata/Events
    modelBuilder.EntitySet<User>("Users");      // Expose Users at /odata/Users
    Microsoft.OData.Edm.IEdmModel? edmModel = modelBuilder.GetEdmModel();

    // ------------------------------------------------------------------------
    // CONTROLLER & JSON CONFIGURATION
    // ------------------------------------------------------------------------
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Prevent infinite loops when serializing objects with circular references
            // (e.g., Todo -> User -> Todo would normally cause an error)
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            
            // Don't include null properties in JSON responses to reduce payload size
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        })
        .AddOData(options => options
            .Select()                               // Enable $select queries
            .Filter()                               // Enable $filter queries
            .OrderBy()                              // Enable $orderby queries
            .Expand()                               // Enable $expand queries
            .Count()                                // Enable $count queries
            .SetMaxTop(100)                         // Limit maximum results to 100
            .AddRouteComponents("odata", edmModel)); // Register OData routes with prefix /odata
    
    // ------------------------------------------------------------------------
    // API DOCUMENTATION (SWAGGER)
    // ------------------------------------------------------------------------
    // Swagger generates interactive API documentation available at /swagger
    // This allows developers to test endpoints directly from the browser
    // ------------------------------------------------------------------------
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
        { 
            Title = "HouseHub API", 
            Version = "v1" 
        });
        
        // When multiple actions match the same route, use the first one
        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    });
    
    // ------------------------------------------------------------------------
    // AUTOMAPPER
    // ------------------------------------------------------------------------
    // AutoMapper automatically maps between different object types
    // (e.g., mapping CreateTodoRequest -> Todo entity)
    // It scans all assemblies in the domain for mapping profiles
    // ------------------------------------------------------------------------
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    
    // ------------------------------------------------------------------------
    // GLOBAL EXCEPTION HANDLING
    // ------------------------------------------------------------------------
    // Catches all unhandled exceptions and returns a consistent error response
    // instead of exposing internal error details to clients
    // ------------------------------------------------------------------------
    services.AddExceptionHandler<GlobalExceptionHandler>();
    
    // ------------------------------------------------------------------------
    // RESPONSE COMPRESSION
    // ------------------------------------------------------------------------
    // Compresses HTTP responses (gzip/brotli) to reduce bandwidth usage
    // Especially beneficial for JSON responses over HTTPS
    // ------------------------------------------------------------------------
    services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;  // Enable compression even over HTTPS
    });

    // ------------------------------------------------------------------------
    // DATABASE CONFIGURATION
    // ------------------------------------------------------------------------
    // Binds the "DbSettings" section from appsettings.json to the DbSettings class
    services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
    
    // Register the database context with PostgreSQL
    // AddDbContext creates a new instance per HTTP request (scoped lifetime)
    // This ensures each request gets its own isolated database context
    services.AddDbContext<ItemDbContext>((IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
    {
        // Read database settings from configuration
        DbSettings? dbSettings = builder.Configuration.GetSection("DbSettings").Get<DbSettings>();
        if (dbSettings != null)
        {
            // Configure Entity Framework to use PostgreSQL with the connection string
            options.UseNpgsql(dbSettings.ConnectionString);
        }
    });
    
    // ------------------------------------------------------------------------
    // HEALTH CHECKS
    // ------------------------------------------------------------------------
    // Provides a /health endpoint that returns 200 OK if the app is running
    // Useful for container orchestration (Kubernetes, Docker Swarm) and monitoring tools
    // Can be extended to check database connectivity, external service availability, etc.
    // ------------------------------------------------------------------------
    services.AddHealthChecks();
    
    // ------------------------------------------------------------------------
    // BUSINESS LOGIC SERVICES
    // ------------------------------------------------------------------------
    // Register application services with Scoped lifetime
    // Scoped = one instance per HTTP request, ensures proper DbContext usage
    // ------------------------------------------------------------------------
    services.AddScoped<ITodoServices, TodoServices>();           // Handles Todo CRUD operations
    services.AddScoped<IUserServices, UserServices>();           // Handles User CRUD operations
    services.AddScoped<ITodoUserService, TodoUserService>();     // Handles Todo-User relationships

    // ------------------------------------------------------------------------
    // CORS CONFIGURATION (Cross-Origin Resource Sharing)
    // ------------------------------------------------------------------------
    // Allows the Angular frontend (running on different port/domain) to call this API
    // Without CORS, browsers block cross-origin requests for security
    // ------------------------------------------------------------------------
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            // Read allowed origins from configuration, fallback to localhost:4200
            // This allows different origins for dev, staging, and production
            string[]? allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                ?? new[] { "http://localhost:4200" };
            
            policy.WithOrigins(allowedOrigins)           // Which domains can call the API
                   .AllowAnyHeader()                      // Allow any HTTP headers
                   .AllowAnyMethod()                      // Allow GET, POST, PUT, DELETE, etc.
                   .AllowCredentials();                   // Allow cookies/auth headers
        });
    });
}

// ============================================================================
// APPLICATION MIDDLEWARE CONFIGURATION METHOD
// ============================================================================
// This method configures the HTTP request pipeline (middleware).
// Middleware executes in order for each request, and in reverse for responses.
// Order matters! Each middleware can process the request, pass it to the next,
// and then process the response on the way back.
// ============================================================================
static void ConfigureApp(WebApplication app)
{
    // ------------------------------------------------------------------------
    // DEVELOPMENT-ONLY MIDDLEWARE
    // ------------------------------------------------------------------------
    if (app.Environment.IsDevelopment())
    {
        // Enable Swagger UI only in development for security
        // Access at: https://localhost:5001/swagger
        app.UseSwagger();        // Serves the OpenAPI/Swagger JSON
        app.UseSwaggerUI();      // Serves the interactive UI
    }

    // ------------------------------------------------------------------------
    // MIDDLEWARE PIPELINE (EXECUTION ORDER IS CRITICAL)
    // ------------------------------------------------------------------------
    
    // 1. Global exception handler - catches all unhandled exceptions
    //    Returns a clean error response instead of exposing stack traces
    app.UseExceptionHandler(options => { });
    
    // 2. Response compression - compresses responses before sending to client
    //    Reduces bandwidth and improves load times
    app.UseResponseCompression();
    
    // 3. HTTPS redirection - automatically redirects HTTP to HTTPS
    //    Ensures all traffic is encrypted
    app.UseHttpsRedirection();
    
    // 4. CORS - allows cross-origin requests from Angular frontend
    //    Must come before UseAuthorization
    app.UseCors("AllowAngularApp");
    
    // 5. Authorization - checks if user is allowed to access endpoints
    //    Validates JWT tokens, roles, policies, etc.
    app.UseAuthorization();
    
    // ------------------------------------------------------------------------
    // ENDPOINT MAPPING
    // ------------------------------------------------------------------------
    
    // Map health check endpoint at /health
    // Returns: 200 OK with "Healthy" if app is running
    app.MapHealthChecks("/health");
    
    // Map all controller endpoints (including OData controllers)
    // This enables all your API routes defined in controllers
    app.MapControllers();
}