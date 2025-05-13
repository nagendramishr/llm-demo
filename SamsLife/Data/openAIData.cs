using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SamsLife.Data
{
     public class OpenAIData
    {
        public DateTime? CreatedAt { get; set; } // Changed to nullable DateTime
        public int FinishReason { get; set; }
        public List<object>? ContentTokenLogProbabilities { get; set; }
        public List<object>? RefusalTokenLogProbabilities { get; set; }
        public int Role { get; set; }
        public List<ContentItem>? Content { get; set; }
        public List<object>? ToolCalls { get; set; }
        public object? Refusal { get; set; }
        public object? FunctionCall { get; set; }
        public string? Id { get; set; }
        public string? Model { get; set; }
        public string? SystemFingerprint { get; set; }
        public Usage? Usage { get; set; }
    }

    public class ContentItem
    {
        public int Kind { get; set; }
        public string? Text { get; set; }
        public string? ImageUri { get; set; }
        public string? ImageBytes { get; set; }
        public string? ImageBytesMediaType { get; set; }
        public string? ImageDetailLevel { get; set; }
        public object? Refusal { get; set; }
    }

    public class Usage
    {
        public int OutputTokenCount { get; set; }
        public int InputTokenCount { get; set; }
        public int TotalTokenCount { get; set; }
        public OutputTokenDetails? OutputTokenDetails { get; set; }
        public InputTokenDetails? InputTokenDetails { get; set; }
    }

    public class OutputTokenDetails
    {
        public int ReasoningTokenCount { get; set; }
        public int AudioTokenCount { get; set; }
    }

    public class InputTokenDetails
    {
        public int AudioTokenCount { get; set; }
        public int CachedTokenCount { get; set; }
    }


}