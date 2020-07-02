using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Azure.AI.FormRecognizer.Training;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace injoice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = Authenticate("https://john-cs-compvision-test.cognitiveservices.azure.com/", "");
        }

        public static FormRecognizerClient Authenticate(string endpoint, string key)
        {
            var credential = new AzureKeyCredential(key);
            return new FormRecognizerClient(new Uri(endpoint), credential);
        }
    }
}
