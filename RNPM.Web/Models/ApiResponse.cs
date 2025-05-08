using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dohwe.Web.Models
{
    public class ApiResponse<T>
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        [JsonProperty("errorCollection")]
        public Dictionary<int, List<string>> ErrorCollection { get; set; }
    }
}
