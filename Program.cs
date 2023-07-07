using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main()
    {
        string stockSymbol = "AAPL"; // Replace with the desired stock symbol

        var expirationDates = GetExpirationDates(stockSymbol);

        foreach (var expirationDate in expirationDates) 
        { 
            Console.WriteLine(expirationDate);
        }
    }


    static DateTime[] GetExpirationDates(string symbol)
    {
        // URL for the Yahoo Finance API
        string apiUrl = $"https://query1.finance.yahoo.com/v7/finance/options/{symbol}";

        // Create a WebClient instance to make the HTTP request
        WebClient client = new WebClient();

        try
        {
            // Download the default, providing no date and get the option expirations
            string jsonData = client.DownloadString(apiUrl);

            JObject json = JObject.Parse(jsonData);

            JToken optionChain = json["optionChain"]["result"][0];
            JToken expirationDates = optionChain["expirationDates"];

            // dates are stored as unix dates
            var intDates = expirationDates.ToObject<int[]>();

            List<DateTime> ret = new List<DateTime>();

            foreach (int i in intDates)
            {
                ret.Add(DateTimeOffset.FromUnixTimeSeconds(i).Date);
            }

            return ret.ToArray();
        }
        catch (WebException ex)
        {
            Console.WriteLine($"An error occurred getting expiration dates. {ex.Message}");
            throw;
        }

    }

}
