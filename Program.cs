var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "QuerySheper API",
        Version = "v1",
        Description = "Simple Multi-Database Query System - Execute SQL queries across all configured databases",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "QuerySheper Support",
            Email = "support@querysheper.com"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuerySheper API v1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
        c.DocumentTitle = "QuerySheper API Documentation";
    });
}

// Only redirect HTTPS in production, not in development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable static files with default file
var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles();

app.MapControllers();

// Auto-launch browser with Swagger in development
if (app.Environment.IsDevelopment())
{
    var urls = app.Urls;
    if (!urls.Any())
    {
        urls.Add("http://localhost:5163");
    }
    
    var url = urls.First();
    _ = Task.Run(async () =>
    {
        await Task.Delay(1000); // Wait for server to start
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore if browser launch fails
        }
    });
}

app.Run();
