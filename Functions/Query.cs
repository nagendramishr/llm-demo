using System;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;


public class Query {
    public string? Text {get; set; }
    public string id { get; set; }

    public Query()
    {
        id = "A213213";
    }

}