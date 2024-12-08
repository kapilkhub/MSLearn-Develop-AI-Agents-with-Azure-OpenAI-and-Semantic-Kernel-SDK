#pragma warning disable SKEXP0050 
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;


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
kernel.ImportPluginFromType<MusicConcertPlugin>();
kernel.ImportPluginFromPromptDirectory("Prompts");

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

string prompt = @"I live in Portland OR USA. Based on my recently 
    played songs and a list of upcoming concerts, which concert 
    do you recommend?";

var result = await kernel.InvokePromptAsync(prompt, new(settings));

Console.WriteLine(result);