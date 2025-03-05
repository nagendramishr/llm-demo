using System;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;


public class SamFact {
    public string? Message {get; set; }
    public string id { get; set; }

    public SamFact()
    {
//        public static RandomNumberGenerator rg = RandomNumberGenerator.Create();

        id = "A213213";
    }

}