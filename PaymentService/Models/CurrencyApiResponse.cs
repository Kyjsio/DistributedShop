using System.Text.Json.Serialization;

namespace PaymentService.Models
{
    public class CurrencyApiResponse
    {
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}
