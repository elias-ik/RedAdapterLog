using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
var path = Environment.GetEnvironmentVariable("STORE_PATH");
app.MapGet("/log", ([FromQuery]string apiKey,[FromQuery] string file, [FromQuery] string log) =>
{
    if(apiKey != "yeahbuddy") return "Invalid API Key";
    if (path is not null)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    File.AppendAllText((path ?? "")+ file, log + Environment.NewLine);
    return "Ok";
})
.WithName("Log String")
.WithOpenApi();

app.MapGet("/read", ([FromQuery]string apiKey, [FromQuery] string file) =>
{
    if(apiKey != "yeahbuddy") return "Invalid API Key";
    try
    {
        return File.ReadAllText((path ?? "")+ file);
    }
    catch
    {
        return "File could not be read.";
    }
})
.WithName("Read File")
.WithOpenApi();



app.Run();
