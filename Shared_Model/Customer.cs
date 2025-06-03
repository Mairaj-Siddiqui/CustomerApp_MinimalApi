using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Shared_Model
{
    public class Customer
    {
        [JsonProperty("id")]  // For CosmosDB (Newtonsoft)
        [JsonPropertyName("id")]  // For System.Text.Json
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Relationship to Salesperson
        public string SalespersonId { get; set; } = string.Empty;
    }
}
