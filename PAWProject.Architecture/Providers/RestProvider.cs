using PAW3CP1.Architecture.Helpers;

namespace PAW3CP1.Architecture;

/// <summary>
/// Interface defining methods for RESTful operations.
/// </summary>
public interface IRestProvider
{
    Task<string> DeleteAsync(string endpoint, string id);
    Task<string> GetAsync(string endpoint, string? id);
    Task<string> PostAsync(string endpoint, string content);
    Task<string> PutAsync(string endpoint, string id, string content);
}

/// <summary>
/// Implementation of the IRestProvider interface, providing methods for RESTful operations.
/// </summary>
public class RestProvider : IRestProvider
{
    /// <summary>
    /// Retrieves a resource asynchronously.
    /// </summary>
    public async Task<string> GetAsync(string endpoint, string? id)
    {
        try
        {
            using var client = RestProviderHelpers.CreateHttpClient();

            // Construir correctamente la URL completa
            var requestUri = string.IsNullOrEmpty(id)
                ? endpoint
                : $"{endpoint.TrimEnd('/')}/{id}";

            var response = await client.GetAsync(requestUri);
            return await RestProviderHelpers.GetResponse(response);
        }
        catch (Exception ex)
        {
            throw RestProviderHelpers.ThrowError(endpoint, ex);
        }
    }

    /// <summary>
    /// Creates a resource asynchronously.
    /// </summary>
    public async Task<string> PostAsync(string endpoint, string content)
    {
        try
        {
            using var client = RestProviderHelpers.CreateHttpClient();
            var response = await client.PostAsync(endpoint, RestProviderHelpers.CreateContent(content));
            return await RestProviderHelpers.GetResponse(response);
        }
        catch (Exception ex)
        {
            throw RestProviderHelpers.ThrowError(endpoint, ex);
        }
    }

    /// <summary>
    /// Updates a resource asynchronously.
    /// </summary>
    public async Task<string> PutAsync(string endpoint, string id, string content)
    {
        try
        {
            using var client = RestProviderHelpers.CreateHttpClient();
            var requestUri = string.IsNullOrEmpty(id)
                ? endpoint
                : $"{endpoint.TrimEnd('/')}/{id}";

            var response = await client.PutAsync(requestUri, RestProviderHelpers.CreateContent(content));
            return await RestProviderHelpers.GetResponse(response);
        }
        catch (Exception ex)
        {
            throw RestProviderHelpers.ThrowError(endpoint, ex);
        }
    }

    /// <summary>
    /// Deletes a resource asynchronously.
    /// </summary>
    public async Task<string> DeleteAsync(string endpoint, string id)
    {
        try
        {
            using var client = RestProviderHelpers.CreateHttpClient();
            var requestUri = string.IsNullOrEmpty(id)
                ? endpoint
                : $"{endpoint.TrimEnd('/')}/{id}";

            var response = await client.DeleteAsync(requestUri);
            return await RestProviderHelpers.GetResponse(response);
        }
        catch (Exception ex)
        {
            throw RestProviderHelpers.ThrowError(endpoint, ex);
        }
    }
}
