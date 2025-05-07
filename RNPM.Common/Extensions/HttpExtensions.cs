using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RNPM.Common.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsAsync<T>(this HttpContent content, System.Threading.CancellationToken cancellationToken)
    {
        return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(cancellationToken), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken: cancellationToken);
    }

    public static async Task<T> ReadAsAsync<T>(this HttpContent content)
    {
        return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
