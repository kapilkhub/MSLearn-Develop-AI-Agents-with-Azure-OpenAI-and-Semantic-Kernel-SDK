#pragma warning disable SKEXP0050 
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System.Text.Json;

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

var plugins = kernel.CreatePluginFromPromptDirectory("Prompts");
string input = "G, C";



var result = await kernel.InvokeAsync(
    plugins["SuggestChords"],
    new() { { "startingChords", input } });

// Parse the JSON response
var jsonDocument = JsonDocument.Parse(result.ToString());
var suggestedChordsElement = jsonDocument.RootElement.GetProperty("suggestedChords");

var suggestedChords = new Dictionary<string, List<double>>();

foreach (var chord in suggestedChordsElement.EnumerateObject())
{
    var chordName = chord.Name;
    var frequencies = new List<double>();

    foreach (var frequency in chord.Value.EnumerateArray())
    {
        frequencies.Add(frequency.GetDouble());
    }

    suggestedChords[chordName] = frequencies;
}
PlayChords(suggestedChords);
Console.WriteLine("Suggested Chords:");

//Console.WriteLine(string.Join(", ", suggestedChords));
//PlayChords(["C","C#","D","D#"]);



static void PlayChords(Dictionary<string, List<double>> suggestedChords)
{
    var waveOut = new WaveOutEvent();
    var mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));

    foreach (var chord in suggestedChords)
    {
        foreach (var note in chord.Value)
        {
            var sineWave = new SignalGenerator(44100, 2)
            {
                Gain = 0.2,
                Frequency = note,
                Type = SignalGeneratorType.Sin
            }.Take(TimeSpan.FromSeconds(10));
            mixer.AddMixerInput(sineWave);
        }
    }

    waveOut.Init(mixer);
    waveOut.Play();
    Console.WriteLine("Playing chords...");
    Console.ReadLine();
    waveOut.Stop();
}


