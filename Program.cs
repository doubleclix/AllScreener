using System;
using System.IO;
using System.Net;

class Program
{
    static void Main()
    {
        string stockSymbol = "AAPL"; // Replace with the desired stock symbol

        // URL for the Yahoo Finance API
        string apiUrl = $"https://query1.finance.yahoo.com/v7/finance/options/{stockSymbol}";

        // Create a WebClient instance to make the HTTP request
        WebClient client = new WebClient();

        try
        {
            // Download the option data
            string jsonData = client.DownloadString(apiUrl);

            Console.WriteLine(jsonData);

            Console.WriteLine($"Option data for {stockSymbol} downloaded successfully.");
        }
        catch (WebException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
