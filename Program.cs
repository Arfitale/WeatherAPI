using DotNetEnv;
using WeatherApi.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<IWeatherService, VisualCrossingWeatherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else if (app.Environment.IsProduction())
{
    app.UseMiddleware<ApiKeyMiddleware>();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
builder.WebHost.UseUrls("http://0.0.0.0:5000");
app.Run("http://0.0.0.0:5000");
