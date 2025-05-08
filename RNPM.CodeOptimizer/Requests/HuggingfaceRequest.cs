using Newtonsoft.Json;

namespace RNPM.CodeOptimizer.Requests;

public class HuggingfaceRequest
{
    public string Inputs { get; set; }
    
    public RequestParameters Parameters { get; set; }
}

public class RequestParameters
{
    [JsonProperty("max_new_tokens")]
    public int MaxNewTokens { get; set; } = 1200;
    
    [JsonProperty("temperature")]
    public float Temperature { get; set; } = 0.7f;

    [JsonProperty("top_p")]
    public float TopP { get; set; } = 0.9f;
 
    [JsonProperty("do_sample")]
    public bool DoSample { get; set; } = true;
}