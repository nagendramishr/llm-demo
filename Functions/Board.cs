using System.Text.Json.Serialization;

namespace Functions
{ 
    public class Board {

        [JsonPropertyName("Owner")]
        public string Owner { get; set; } = "User";
        [JsonPropertyName("Created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("Title")]
        public string Title { get; set; } = "Board";

        // [JsonPropertyName("Id")]
        // public string Id { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("Delete")]
        public bool Delete { get; set; } = false;
        public Board()
        {
            id= Guid.NewGuid().ToString();
        }
    }
}
