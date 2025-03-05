
namespace Functions
{ 
    public class SamFact {
        public string Message {get; set; }
        public string Id { get; set; }
        public string id { get; set; }
        public SamFact()
        {
            Id = Guid.NewGuid().ToString();
        }
        public SamFact(string message)
        {
            Message = message;
            Id = Guid.NewGuid().ToString();
        }
        public SamFact(string message, string id)
        {
            Message = message;
            Id = id;
        }
    }
}
