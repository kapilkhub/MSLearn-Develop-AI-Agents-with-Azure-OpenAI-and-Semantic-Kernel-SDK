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

string prompt = @"This is a list of music available to the user:
    {{MusicLibraryPlugin.GetMusicLibrary}} 

    This is a list of music the user has recently played:
    {{MusicLibraryPlugin.GetRecentPlays}}

    Based on their recently played music, suggest a song from
    the list to play next";

var result = await kernel.InvokePromptAsync(prompt);

Console.WriteLine(result);