using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
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
            string jsonData = LoadJson(client, apiUrl);

            Delay();

            JObject json = JObject.Parse(jsonData);

            return ParseChain(json);
        }
        catch (WebException ex)
        {
            Console.WriteLine($"An error occurred getting option chain. {ex.Message}");
            throw;
        }

    }

    static OptionChain ParseChain(JObject json)
    {
        double stockBid = json["optionChain"]["result"][0]["quote"]["bid"].ToObject<double>();

        List<OptionContract> calls = new List<OptionContract>();

        var jsonCalls = json["optionChain"]["result"][0]["options"][0]["calls"];

        foreach (var c in jsonCalls)
        {
            double strike = c["strike"].ToObject<double>();
            double bid = c["bid"].ToObject<double>();
            double ask = c["ask"].ToObject<double>();

            calls.Add(new OptionContract() { Strike = strike, Bid = bid, Ask = ask });
        }

        OptionChain ret = new OptionChain() { StockBid = stockBid, Calls = calls.ToArray() };

        return ret;
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
            string jsonData = LoadJson(client, apiUrl);

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

    static string LoadJson(WebClient client, string apiUrl)
    {
        return client.DownloadString(apiUrl);
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
        public double StockBid { get; set; }

        public OptionContract[] Calls { get; set; }

    }

    public class OptionContract
    {
        public double Strike { get; set; }

        public double Bid { get; set; }

        public double Ask { get; set; }
    }

}
