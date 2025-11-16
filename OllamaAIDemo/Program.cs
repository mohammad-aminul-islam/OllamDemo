using Microsoft.Extensions.AI;
using OllamaAIDemo.AIModelServices;
using OllamaAIDemo.Services;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("Ollama", client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.Timeout = Timeout.InfiniteTimeSpan; 
});

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("Ollama");

    return new OllamaApiClient(httpClient, "llama2");
});

builder.Services.AddKeyedScoped<IAIModelService,OllamaAIModelService>(AIModelName.Ollama);
builder.Services.AddKeyedScoped<IAIModelService,GemeniAIModelService>(AIModelName.Gemini);
builder.Services.AddScoped<EmployeeDataProvider>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (OperationCanceledException)
    {
        // Client disconnected, don't log as error
        context.Response.StatusCode = 499; // Client Closed Request
    }
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowNextJs");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
