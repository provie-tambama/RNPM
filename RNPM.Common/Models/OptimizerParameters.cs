namespace RNPM.Common.Models;

public class OptimizerParameters
{

    public const string Parameters = "OptimizerParameters";
    public string HuggingFaceApiUrl { get; set; } = "https://api-inference.huggingface.co";
    
    /// <summary>
    /// The API token for authenticating with the HuggingFace API.
    /// </summary>
    public string HuggingFaceApiToken { get; set; }
    
    /// <summary>
    /// The name of the model on HuggingFace.
    /// </summary>
    public string ModelName { get; set; } = "unsloth/DeepSeek-R1-Distill-Llama-8B-ReactNative-Perf";
    
    /// <summary>
    /// The endpoint for the model on HuggingFace.
    /// </summary>
    public string ModelEndpoint { get; set; } = "inference/api";
    
    /// <summary>
    /// The maximum number of tokens to generate.
    /// </summary>
    public int MaxNewTokens { get; set; } = 1200;
    
    /// <summary>
    /// The temperature for the model generation.
    /// </summary>
    public float Temperature { get; set; } = 0.7f;
    
    /// <summary>
    /// The top-p value for the model generation.
    /// </summary>
    public float TopP { get; set; } = 0.9f;
}