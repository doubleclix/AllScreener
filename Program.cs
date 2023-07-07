using System;
using System.IO;
using System.Net;
using System.Reflection;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main()
    {
        string stockSymbol = "AAPL"; // Replace with the desired stock symbol

        var expirationDates = GetExpirationDates(stockSymbol);

        var chain = GetOptionChain(stockSymbol, expirationDates[5]);

    }


    static OptionChain GetOptionChain(string symbol, DateTime expirationDate)
    {
        // dates are unix dates
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        long date = (long)(expirationDate - unixEpoch).TotalSeconds;

        // URL for the Yahoo Finance API
        string apiUrl = $"https://query1.finance.yahoo.com/v7/finance/options/{symbol}?date={date}";

        // Create a WebClient instance to make the HTTP request
        WebClient client = new WebClient();

        try
        {
            // Download the default, providing no date and get the option expirations
            string jsonData = client.DownloadString(apiUrl);

            Delay();

            JObject json = JObject.Parse(jsonData);


            throw new NotImplementedException();
        }
        catch (WebException ex)
        {
            Console.WriteLine($"An error occurred getting option chain. {ex.Message}");
            throw;
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

            Delay();
            

            JObject json = JObject.Parse(jsonData);

            JToken optionChain = json["optionChain"]["result"][0];
            JToken expirationDates = optionChain["expirationDates"];

            // dates are stored as unix dates
            var intDates = expirationDates.ToObject<long[]>();

            List<DateTime> ret = new List<DateTime>();

            foreach (int i in intDates)
            {
                DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                DateTime date = unixEpoch.AddSeconds(i);

                ret.Add(date);
            }

            return ret.ToArray();
        }
        catch (WebException ex)
        {
            Console.WriteLine($"An error occurred getting expiration dates. {ex.Message}");
            throw;
        }

    }

    /// <summary>
    /// Performs a delay to avoid getting blocked accessing Yahoo finance data
    /// </summary>
    static void Delay()
    {
        Thread.Sleep(10);
    }


    public class OptionChain
    {

    }

}
