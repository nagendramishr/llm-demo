
namespace Functions
{ 
    public class SamFact {
        public string Message {get; set; }
        public string Id { get; set; }
        public string id { get; set; }
        public SamFact()
        {
            id=Id = Guid.NewGuid().ToString();
        }
        public SamFact(string message)
        {
            Message = message;
            id=Id = Guid.NewGuid().ToString();
        }
        public SamFact(string message, string id)
        {
            Message = message;
            Id = id;
            this.id = id;
        }
    }
}
