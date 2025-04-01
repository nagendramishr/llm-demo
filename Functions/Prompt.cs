using System.Text.Json.Serialization;

namespace Functions
{ 
    public class Prompt {
        [JsonPropertyName("Text")]
        public string Text {get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; } = "Prompt";

        // [JsonPropertyName("Id")]
        // public string Id { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("Delete")]
        public bool Delete { get; set; } = false;
        public Prompt()
        {
            id= Guid.NewGuid().ToString();
            Text = "";
            Title = "";
        }
    }
}
