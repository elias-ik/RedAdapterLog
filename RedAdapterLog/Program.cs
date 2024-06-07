using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var realApiKey = Environment.GetEnvironmentVariable("APIKEY") ?? "default";
app.UseHttpsRedirection();
var path = Environment.GetEnvironmentVariable("STORE_PATH");
path = path != null ? path + "/" : null;
app.MapGet("/log", ([FromQuery]string apiKey,[FromQuery] string file, [FromQuery] string log) =>
{
    if(apiKey != realApiKey) return "Invalid API Key";
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
    if(apiKey != realApiKey) return "Invalid API Key";
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

app.MapGet("/list", ([FromQuery] string apiKey) =>
{
    if(apiKey != realApiKey) return "Invalid API Key";
    return string.Join(Environment.NewLine, Directory.GetFiles(path ?? ""));
})
.WithName("List All Files")
.WithOpenApi();

app.MapGet("/list-urls", ([FromQuery] string apiKey) =>
{
    if(apiKey != realApiKey) return "Invalid API Key";
    var files = Directory.GetFiles(path ?? "");
    var urls = files.Select(x => $"https://red-tester-log.s1.elias.dev/read?apiKey=${realApiKey}&file=${x}");
    return string.Join(Environment.NewLine, urls);
})
.WithName("List All Files As Urls")
.WithOpenApi();

app.MapGet("/delete", ([FromQuery] string apiKey) =>
{
    if(apiKey != realApiKey) return "Invalid API Key";
    File.Delete(path ?? "");
    return "Deleted";
})
.WithName("Delete File")
.WithOpenApi();

app.Run();
