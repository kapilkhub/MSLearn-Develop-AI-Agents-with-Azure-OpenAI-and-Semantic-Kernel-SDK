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
var result = await kernel.InvokePromptAsync(
    "Give me a list of breakfast foods with eggs and cheese");

Console.WriteLine(result);