using Newtonsoft.Json;

namespace RNPM.CodeOptimizer.Responses;

public class HuggingfaceResponse
{
    [JsonProperty("generated_text")]
    public string GeneratedText { get; set; }
}