using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RNPM.CodeOptimizer.Helpers;

public static class HttpHelpers
{
    public static HttpRequestMessage GetPostRequestMessage<T>(T request, string uri)
    {
        var json = JsonConvert.SerializeObject(
            request,
            new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        
        // Create the HttpRequestMessage object
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(uri)
        };

        // Log the request headers
        Console.WriteLine("Request Headers:");
        foreach (var header in httpRequest.Headers)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

        // Set the content type header
        httpRequest.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

        return httpRequest;
    }

    public static HttpRequestMessage GetGetRequest(string uri)
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(uri)
        };

        if (request.Content != null)
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

        return request;
    }

    public static HttpRequestMessage GetGetRequestGeneral(string uri)
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(uri),
            Headers = { }
        };

        if (request.Content != null)
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

        return request;
    }
}
