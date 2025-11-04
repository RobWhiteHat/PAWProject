using System.Net.Http.Headers;
using System.Text;

namespace PAW3CP1.Architecture.Helpers;

/// <summary>
/// Provides helper methods for creating and managing HTTP requests.
/// </summary>
internal static class RestProviderHelpers
{
    /// <summary>
    /// Creates a clean HttpClient (without BaseAddress) with JSON headers.
    /// </summary>
    internal static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler();

        // Opcional: habilitar esto solo si usas certificados locales de desarrollo.
        // handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    /// <summary>
    /// Creates JSON content for POST/PUT requests.
    /// </summary>
    internal static StringContent CreateContent(string content) =>
        new(content, Encoding.UTF8, "application/json");

    /// <summary>
    /// Validates and reads the HTTP response.
    /// </summary>
    internal static async Task<string> GetResponse(HttpResponseMessage response)
    {
        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Request failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{result}"
            );
        }

        return result;
    }

    /// <summary>
    /// Wraps an exception with additional endpoint context.
    /// </summary>
    internal static Exception ThrowError(string endpoint, Exception ex) =>
        new ApplicationException($"Error accessing endpoint: {endpoint}\nDetails: {ex.Message}", ex);
}
