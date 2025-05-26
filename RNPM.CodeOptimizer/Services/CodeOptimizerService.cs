using Microsoft.Extensions.Options;
using OpenAI.Chat;
using RNPM.CodeOptimizer.Helpers;
using RNPM.CodeOptimizer.Requests;
using RNPM.CodeOptimizer.Responses;
using RNPM.Common.Data;
using RNPM.Common.Extensions;
using RNPM.Common.Models;
using Serilog;

namespace RNPM.CodeOptimizer.Services;

public class CodeOptimizerService : ICodeOptimizerService
{
    private readonly RnpmDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OptimizerParameters _optimizerParameters;
    private const string BaseApiUrl = "https://api.openai.com/v1/responses";
    
    public CodeOptimizerService(IHttpClientFactory httpClientFactory, RnpmDbContext context, IOptions<OptimizerParameters> cbzParameters)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _optimizerParameters = cbzParameters.Value;
    }
    
    private HttpClient GetHttpClient()
    {
        var client = _httpClientFactory.CreateClient();
        return client;
    }

    private ChatClient GetChatClient()
    {
        ChatClient client = new(
            model: _optimizerParameters.ModelName,
            apiKey: _optimizerParameters.OpenKey
        );
        return client;
    }
    
    public async Task<CodeOptimizationResponse> OptimizeCodeAsync(CodeOptimizationRequest request)
    {
        try
        {
            var httpClient = GetHttpClient();

            var huggingFaceRequest = new HuggingfaceRequest
            {
                Inputs = FormatHelpers.FormatPrompt(request.Code),
                Parameters = new RequestParameters
                {
                    MaxNewTokens = _optimizerParameters.MaxNewTokens,
                    Temperature = _optimizerParameters.Temperature,
                    TopP = _optimizerParameters.TopP,
                    DoSample = true
                }
            };
            
            var requestUri = $"{BaseApiUrl}api/transactions";
            
            var httpRequest = HttpHelpers.GetPostRequestMessage(huggingFaceRequest, requestUri);
            var response = await httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new CodeOptimizationResponse
                {
                    Success = false,
                    Message = $"Error from optimization service: {response.StatusCode}",
                    OriginalCode = request.Code
                };
            }
            
            var huggingFaceResponse = await response.Content.ReadAsAsync<HuggingfaceResponse>();
            if (huggingFaceResponse == null)
            {
                return new CodeOptimizationResponse
                {
                    Success = false,
                    Message = "No response from optimization service",
                    OriginalCode = request.Code
                };
            }

            var result = huggingFaceResponse.GeneratedText;

            return new CodeOptimizationResponse
            {
                Success = true,
                Message = "Optimization completed successfully",
                OriginalCode = request.Code,
                OptimizationSuggestion = huggingFaceResponse.GeneratedText,
            };
            
        }
        catch (Exception e)
        {
            // Log the exception details
            Log.Error(e, "An error occurred while submitting the transaction");

            return new CodeOptimizationResponse
            {
                Success = false,
                Message = "No response from optimization service",
                OriginalCode = request.Code
            };
        }
    }

    public async Task<CodeOptimizationResponse> OptimizeCodeSubAsync(CodeOptimizationRequest request)
    {
        try
        {
            var chatClient = GetChatClient();
            var systemPrompt = "You are a React Native expert specializing in performance optimization. Analyze the provided code for screen render time issues and suggest fixes for these anti-patterns:\n1. List Rendering (e.g., ScrollView misuse).\n2. Inline Functions in Render.\n3. Unmemoized Components.\n4. Heavy Computations in Render.\n5. Unoptimized Image Loading.\n\nPrioritize suggestions that reduce re-renders and improve render time.";
            List<ChatMessage> messages = new List<ChatMessage>
            {
                
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(request.Code)
            };
            
            ChatCompletion completion = chatClient.CompleteChat(messages);
            
            Console.WriteLine(completion.Content[0].Text);

            if (string.IsNullOrWhiteSpace(completion.Content[0].Text))
            {
                return new CodeOptimizationResponse
                {
                    Success = false,
                    Message = $"Error from optimization service: Error occured",
                    OriginalCode = request.Code
                };
            }

            return new CodeOptimizationResponse
            {
                Success = true,
                Message = "Optimization completed successfully",
                OriginalCode = request.Code,
                OptimizationSuggestion = completion.Content[0].Text
            };
            
        }
        catch (Exception e)
        {
            // Log the exception details
            Log.Error(e, "An error occurred while processing optimization request");

            return new CodeOptimizationResponse
            {
                Success = false,
                Message = "No response from optimization service",
                OriginalCode = request.Code
            };
        }
    }

    public async Task<bool> IsServiceAvailableAsync()
    {
        try
        {
            var httpClient = GetHttpClient();
            var response = await httpClient.GetAsync($"{_optimizerParameters.HuggingFaceApiUrl}/models/{_optimizerParameters.ModelName}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error checking service availability");
            return false;
        }
    }
}