using System.Net.Http.Headers;
using System.Net.Http.Json;
using System;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Text;

internal class Program
{
    
    private static async Task Main(string[] args)
    {
        string apiKey = "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";
        string prompt = "Create song lyrics with the following theme and in stile of the entrepreneur:";

        // Prompt the user for input
        Console.Write("Enter a Theme: ");
        string Theme = Console.ReadLine();
        Console.Write("Enter an Entrepreneur: ");
        string stile = Console.ReadLine();

        // Generate song lyrics
        string songLyrics = await GenerateSongLyrics(apiKey, prompt + " " + Theme + " " + stile);

        // Display the generated song lyrics
        Console.WriteLine("\nGenerated Song Lyrics:");
        Console.WriteLine(songLyrics);
    }

    static async Task<string> GenerateSongLyrics(string apiKey, string prompt)
    {
        using (HttpClient client = new HttpClient())
        {
            // Set up the API endpoint
            string apiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

            // Set up the request headers
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            // Set up the request content
            string requestBody = $"{{\"prompt\": \"{prompt}\", \"max_tokens\": 150}}";
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // Make the API request
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return null;
            }
        }
    }
}