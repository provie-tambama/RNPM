namespace RNPM.CodeOptimizer.Helpers;

public static class FormatHelpers
{
    public static string FormatPrompt(string code)
    {
        return $@"Below is an instruction that describes a task, paired with an input that provides further context.
Write a response that appropriately completes the request.
Before answering, think carefully about the question and create a step-by-step chain of thoughts to ensure a logical and accurate response.
Give a short and succinct response.

### Instruction:
You are a React Native expert specializing in performance optimization. Analyze the provided code for screen render time issues and suggest fixes for these anti-patterns:
1. List Rendering (e.g., ScrollView misuse).
2. Inline Functions in Render.
3. Unmemoized Components.
4. Heavy Computations in Render.
5. Unoptimized Image Loading.

Prioritize suggestions that reduce re-renders and improve render time.

### Question:
{code}

### Response:
";
    }
}