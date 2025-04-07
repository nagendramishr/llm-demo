using System;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;


public class SamFact {
    public string? Message {get; set; }
    public string id { get; set; }

    public string boardId { get; set; }="";

    public SamFact()
    {
        // Generate a new unique ID for each instance
        id = Guid.NewGuid().ToString();
    }

}