using AirportDistanceService.Service;
using AirportDistanceService.Service.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the AirportService and configure HttpClient
builder.Services.AddHttpClient<IAirportService, AirportService>(client =>
{
    client.BaseAddress = new Uri("https://places-dev.cteleport.com/");
    // Additional configuration for HttpClient if needed
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
