var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () =>
{
    return "Hello DotNed";
});

app.MapGet("/boom", () =>
{
    Environment.Exit(0);   
});

app.Run("http://0.0.0.0:8000");