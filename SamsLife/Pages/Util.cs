using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net.Http.Headers;

public class Util
{
    private readonly HttpClient client;
    private readonly IConfiguration configuration;

    public Util(HttpClient httpClient, IConfiguration config)
    {
        client = httpClient;
        configuration = config;

        var SamFuncHost = configuration["SamFunction:Hostname"];
        var SamAuth = configuration["SamFunction:FunctionKey"];

        client.BaseAddress = new Uri($"https://{SamFuncHost}");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-functions-key", SamAuth);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public string statusMessage = string.Empty;
    public bool errorOccurred = false;

    private void reset() {
        statusMessage = string.Empty;
        errorOccurred = false;
    }

    public async Task AddBoard(string user, Board board) {
        reset();

        try
        {
            var jsonString = JsonSerializer.Serialize(board);
            var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            var httpResponse = await client.PostAsync("api/addBoard?user=" + user, content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                statusMessage = $"Error: {httpResponse.StatusCode}";
                return;
            }

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            statusMessage = responseString;
        }
        catch (Exception ex)
        {
            errorOccurred = true;
            statusMessage = $"An error occurred: {ex.Message}";
        }
    }
    public async Task<List<Board>> GetBoards(string user)
    {
        List<Board> boardList = new List<Board>();
        reset();

        try
        {
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

    public async Task<List<string>> GetFacts(string boardId)

    {
        var responses = new List<string>();
        reset();

        try {
            var httpResponse = await client.GetAsync("api/getFacts?boardId=" + boardId);

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

    public async Task ResetFacts(string boardId) {
        try {
            var httpResponse = await client.GetAsync("api/resetDB?boardId=" + boardId);
            httpResponse.EnsureSuccessStatusCode();
            var response = await httpResponse.Content.ReadAsStringAsync();
        } catch (HttpRequestException e) {
            errorOccurred = true;
            statusMessage = e.Message;
        }
    }
}