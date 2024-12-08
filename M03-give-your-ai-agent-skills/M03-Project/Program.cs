#pragma warning disable SKEXP0050 
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();

// Build configuration to read from user secrets
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Read secrets from configuration
string yourDeploymentName = configuration["AI:OpenAI:DEPLOYMENT"]!;
string yourEndpoint = configuration["AI:OpenAI:END_POINT"]!;
string yourApiKey = configuration["AI:OpenAI:API_KEY"]!;

builder.AddAzureOpenAIChatCompletion(
    yourDeploymentName,
    yourEndpoint,
    yourApiKey,
    yourDeploymentName);

var kernel = builder.Build();
kernel.ImportPluginFromType<MusicLibraryPlugin>();

var result = await kernel.InvokeAsync(
    "MusicLibraryPlugin",
    "AddToRecentlyPlayed",
    new()
    {
        ["artist"] = "Tiara",
        ["song"] = "Danse",
        ["genre"] = "French pop, electropop, pop"
    }
);

Console.WriteLine(result);

Console.WriteLine(result);