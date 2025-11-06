using System.Net.Http;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public class JokeService : IJokeService
    {
        private readonly HttpClient _httpClient;

        public JokeService(HttpClient http)
        {
            _httpClient = http;
        }

        public async Task<string> GetProgrammingJokeAsync()
        {
            const string route = "https://v2.jokeapi.dev/joke/Any";
            try
            {
                using var resp = await _httpClient.GetAsync(route).ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("joke", out var jokeElement))
                {
                    return jokeElement.GetString() ?? string.Empty;
                }
                else if (doc.RootElement.TryGetProperty("setup", out var setupElement) &&
                         doc.RootElement.TryGetProperty("delivery", out var deliveryElement))
                {
                    // Handle two-part jokes
                    return $"{setupElement.GetString()}... {deliveryElement.GetString()}";
                }

                return "Could not find a joke in the response.";
            } catch (Exception ex)
            {
                return $"Failed to get joke: {ex.Message}";
            }
        }
    }
}
