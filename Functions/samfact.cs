using System.Text.Json.Serialization;

namespace Functions
{ 
    public class SamFact {
        [JsonPropertyName("Message")]
        public string Message {get; set; }

        // [JsonPropertyName("Id")]
        // public string Id { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("Delete")]
        public bool Delete { get; set; } = false;
        public SamFact()
        {
            id= Guid.NewGuid().ToString();
        }
        public SamFact(string message)
        {
            Message = message;
            id = Guid.NewGuid().ToString();
        }
        public SamFact(string message, string id)
        {
            Message = message;
            this.id = id;
        }

        // public SamFact(string message, string Id, string id)
        // {
        //     Message = message;
        //     this.Id = Id;
        //     this.id = id;
        // }
    
    }
}
