using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;
using OllamaAIDemo;
using OllamaAIDemo.AIModelServices;
using OllamaAIDemo.Configuration;
using OllamaAIDemo.MCPClientService;
using OllamaAIDemo.Services;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

string aiModel = builder.Configuration.GetValue<string>("AIModel");

builder.Services.AddOptions<OpenAIConfiguration>()
    .Bind(builder.Configuration.GetSection("OpenAI"))
    .ValidateDataAnnotations();

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
    IChatClient innerClient = new OllamaApiClient(httpClient, aiModel);
    
    // Wrap the client to automatically handle function/tool calls
    return new ChatClientBuilder(innerClient)
        .UseFunctionInvocation()
        .Build();
});

builder.Services.AddSingleton<IMcpClientService, DefaultMcpClientService>();
builder.Services.AddScoped<IAIModelFactory, AIModelFactory>();
builder.Services.AddKeyedScoped<IAIModelService, OllamaAIModelService>(AIModelName.Ollama);
builder.Services.AddKeyedScoped<IAIModelService, GemeniAIModelService>(AIModelName.Gemini);
builder.Services.AddKeyedScoped<IAIModelService, OpenAIModelService>(AIModelName.OpenAI);
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
