using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;

namespace injoice
{
    class Program
    {
        private const string ObosPdf = "https://raw.githubusercontent.com/johnkors/mariusvision/master/samplepdfs/obos/2020-01.pdf";
        private const string ReisePdf = "https://raw.githubusercontent.com/johnkors/mariusvision/master/samplepdfs/eticket/reise.pdf";

        static async Task Main(string[] args)
        {
            var client = Authenticate("https://john-cs-formrecog-test.cognitiveservices.azure.com/", Environment.GetEnvironmentVariable("MARIUSVISION_APIKEY"));

            // var response = await client.StartRecognizeReceiptsFromUriAsync(new Uri("https://raw.githubusercontent.com/johnkors/mariusvision/master/samplepdfs/obos/2020-01.pdf"));
            // RecognizedReceiptCollection receipts = await response.WaitForCompletionAsync();
            // PrintReceipts(receipts);

            var response2 = await client.StartRecognizeContentFromUriAsync(new Uri(ObosPdf));
            var stuff = await response2.WaitForCompletionAsync();
            PrintStuff(stuff);
        }

        private static void PrintStuff(FormPageCollection formPages)
        {
            foreach (var page in formPages)
            {
                Console.WriteLine($"Form Page {page.PageNumber} has {page.Lines.Count} lines.");

                for (var i = 0; i < page.Tables.Count; i++)
                {
                    var table = page.Tables[i];
                    Console.WriteLine($"Table {i} has {table.RowCount} rows an {table.ColumnCount} columns.");
                    foreach (var cell in table.Cells)
                    {
                        Console.WriteLine($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) contains text: '{cell.Text}'.");
                    }
                }
            }
        }

        private static void PrintReceipts(RecognizedReceiptCollection receipts)
        {
            foreach (var receipt in receipts)
            {
                if (receipt.RecognizedForm.Fields.TryGetValue("MerchantName", out FormField merchantNameField))
                {
                    if (merchantNameField.Value.Type == FieldValueType.String)
                    {
                        var merchantName = merchantNameField.Value.AsString();

                        Console.WriteLine($"Merchant Name: '{merchantName}', with confidence {merchantNameField.Confidence}");
                    }
                }

                FormField transactionDateField;
                if (receipt.RecognizedForm.Fields.TryGetValue("TransactionDate", out transactionDateField))
                {
                    if (transactionDateField.Value.Type == FieldValueType.Date)
                    {
                        var transactionDate = transactionDateField.Value.AsDate();
                        Console.WriteLine($"Transaction Date: '{transactionDate}', with confidence {transactionDateField.Confidence}");
                    }
                }

                FormField itemsField;
                if (receipt.RecognizedForm.Fields.TryGetValue("Items", out itemsField))
                {
                    if (itemsField.Value.Type == FieldValueType.List)
                    {
                        foreach (FormField itemField in itemsField.Value.AsList())
                        {
                            Console.WriteLine("Item:");

                            if (itemField.Value.Type == FieldValueType.Dictionary)
                            {
                                IReadOnlyDictionary<string, FormField> itemFields = itemField.Value.AsDictionary();

                                FormField itemNameField;
                                if (itemFields.TryGetValue("Name", out itemNameField))
                                {
                                    if (itemNameField.Value.Type == FieldValueType.String)
                                    {
                                        string itemName = itemNameField.Value.AsString();

                                        Console.WriteLine($"    Name: '{itemName}', with confidence {itemNameField.Confidence}");
                                    }
                                }

                                FormField itemTotalPriceField;
                                if (itemFields.TryGetValue("TotalPrice", out itemTotalPriceField))
                                {
                                    if (itemTotalPriceField.Value.Type == FieldValueType.Float)
                                    {
                                        float itemTotalPrice = itemTotalPriceField.Value.AsFloat();

                                        Console.WriteLine($"    Total Price: '{itemTotalPrice}', with confidence {itemTotalPriceField.Confidence}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static FormRecognizerClient Authenticate(string endpoint, string key)
        {
            var credential = new AzureKeyCredential(key);
            return new FormRecognizerClient(new Uri(endpoint), credential);
        }
    }
}
