using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CustomerApp_MinimalApi.Data.Entities
{
    public class Salesperson
    {

        [JsonProperty("id")] //Cosmos DB requires an id field
        [JsonPropertyName("id")]

        public string Id { get; set; } = Guid.NewGuid().ToString();//auto-generates a unique ID

        //Initialized to string.Empty to avoid null errors.
        public string Name { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; //email where notifications will be sent
    }
}
