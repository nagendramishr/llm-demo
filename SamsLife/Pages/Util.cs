using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net.Http.Headers;

public class Util
{

    private static HttpClient? sharedClient;
    private static IConfiguration? configuration;

    // Singleton pattern for Configuration
    private static IConfiguration Configuration
    {
        get
        {
            if (configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configuration = builder.Build();
            }
            return configuration;
        }
    }
    // Singleton pattern for HttpClient
    public static HttpClient SharedClient
    {
        get
        {
            if (sharedClient == null)
            {
                var conf = Configuration;
                var SamFuncHost = conf["SamFunction:Hostname"];
                var SamAuth = conf["SamFunction:FunctionKey"];

                sharedClient = new HttpClient();
                sharedClient.BaseAddress = new Uri($"https://{SamFuncHost}");
                sharedClient.DefaultRequestHeaders.Add("x-functions-key", SamAuth);
                sharedClient.DefaultRequestHeaders.Accept.Clear();
                sharedClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            return sharedClient;
        }
    }

    public string statusMessage = string.Empty;
    public bool errorOccurred = false;

    public async Task<List<Board>> GetBoards(string user)
    {
        List<Board> boardList = new List<Board>();
        errorOccurred = false;
        statusMessage = string.Empty;

        try
        {
            HttpClient client = SharedClient;
            var httpResponse =
                await client.GetAsync("api/getBoards?user=" + user);

            if (!httpResponse.IsSuccessStatusCode)
            {
                statusMessage = $"Error: {httpResponse.StatusCode}";
                return boardList;
            }

            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            boardList = JsonSerializer.Deserialize<List<Board>>(jsonString) ?? new List<Board>();
        }
        catch (Exception ex)
        {
            errorOccurred = true;
            statusMessage = $"An error occurred: {ex.Message}";
        }

        return boardList;
    }

    public async Task<List<string>> GetFacts( )
    {
        var responses = new List<string>();
        errorOccurred = false;
        statusMessage = string.Empty;

        try {
            HttpClient client = SharedClient;

            var httpResponse = await client.GetAsync("api/getFacts");

            httpResponse.EnsureSuccessStatusCode();
            var response = await httpResponse.Content.ReadAsStringAsync();
            // Parse the response to get the response text
            responses = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response) ?? new List<string>();
            if (responses == null) {
                errorOccurred = true;
                statusMessage = "No facts found.";
            }

        } catch (HttpRequestException e) {
            errorOccurred = true;
            statusMessage = e.Message;
            Console.WriteLine(e);
        }

        return responses!;
    }

}