using OllamaAIDemo.ChatClient;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddChatClient(new OllamaApiClient(new Uri("http://localhost:11434"), "gurubot/phi3-mini-abliterated:q4"));
builder.Services.AddScoped<IApplicationChatClient,ApplicationChatClient>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
