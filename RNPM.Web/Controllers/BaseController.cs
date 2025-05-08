using System.Security.Claims;
using Dohwe.Web.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RNPM.Common.Extensions;
using RNPM.Web.Models;

namespace RNPM.Web.Controllers
{
    public class BaseController<C> : Controller where C : BaseController<C>
    {
        private IHttpClientFactory _httpClientFactory;
        private IOptions<ScriptTags> _options;

        private const string ApiVersion = "1.0";

        protected IHttpClientFactory HttpClientFactory => _httpClientFactory ??= HttpContext.RequestServices.GetService<IHttpClientFactory>();
        protected IOptions<ScriptTags> Options => _options ??= HttpContext?.RequestServices.GetService<IOptions<ScriptTags>>();

        public async Task<HttpClient> GetHttpClient()
        {
            var token = HttpContext.Session.GetString("access_token");
            var baseUrl = Options.Value.ApiAddress;

            var client = HttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.SetBearerToken(token);

            return await Task.FromResult(client);
        }

        public async Task<HttpClient> GetUtilitiesHttpClient()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var baseUrl = Options.Value.UtilitiesUrl;

            var client = HttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.SetBearerToken(token);

            return await Task.FromResult(client);
        }

        public string GetUserId()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(c => c.Type == ClaimTypes.PrimarySid);
                return userId.Value;
            }

            return null;
        }

        public bool IsAdmin()
        {
            var isAdmin = User.FindFirst(c => c.Type == "IsAdmin")?.Value;

            return Convert.ToBoolean(isAdmin);
        }
        
        public void HandleError<T>(T apiResponse)
        {
            throw new NotImplementedException();
        }


        #region ApiCalls

        public static async Task<T> Get<T>(HttpClient client, string url)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> Get<T>(HttpClient client, string url, int id)
        {
            try
            {
                using (client)
                {
                    var apiUrl = $"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}";
                    var response = await client.GetAsync(apiUrl);

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> Get<T>(HttpClient client, string url, long id)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");
                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> Get<T>(HttpClient client, string url, Guid id)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> Get<T>(HttpClient client, string url, string id)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<ApiResponse<T>> Add<T>(HttpClient client, string url, T entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PostAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ApiResponse<T>> Add<T, TU>(HttpClient client, string url, TU entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PostAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ApiResponse<T>> Update<T>(HttpClient client, string url, T entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PutAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ApiResponse<T>> Update<T>(HttpClient client, string url, string id, T entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PutAsJsonAsync($"{url}/{id}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, int id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, long id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, Guid id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, string id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

    }
}
