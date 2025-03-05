using System;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;


public class SamFact {
    public string? Message {get; set; }
    public string Id { get; set; }

    public SamFact()
    {
//        public static RandomNumberGenerator rg = RandomNumberGenerator.Create();

        Id = "A213213";
    }

}