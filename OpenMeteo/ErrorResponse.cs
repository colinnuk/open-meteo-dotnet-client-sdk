using System.Text.Json.Serialization;

namespace OpenMeteo
{
    internal class ErrorResponse
    {
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("error")]
        public bool Error { get; set; }
    }
}
