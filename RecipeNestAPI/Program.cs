using Microsoft.EntityFrameworkCore;

// Program.cs is the entry point for the ASP.NET Core Web API. It wires
// up the dependency injection container, configures the HTTP pipeline,
// and starts the web server.

var builder = WebApplication.CreateBuilder(args);

// --- Service registration ---------------------------------------------------

// Register API controllers so the framework can discover and route to them.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Register the Entity Framework Core SQLite provider. The connection
// string points to a file-based database stored next to the executable,
// which is convenient for development and submission.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=RecipeNest.db"));

// Configure CORS so the React frontend running on http://localhost:3000
// is allowed to call this API. Without this the browser would block the
// cross-origin requests during development.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// --- Database initialisation -----------------------------------------------

// Create the database (and apply seed data) on startup if it doesn't
// already exist. EnsureCreated builds the schema directly from the
// model in AppDbContext, so the project works out of the box without
// having to run dotnet-ef tooling. Delete RecipeNest.db to reset.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// --- HTTP pipeline ---------------------------------------------------------

// CORS must be registered before HttpsRedirection so that the CORS headers
// are present on redirect responses and the browser does not block them.
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
