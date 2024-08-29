using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Thread.Sleep(3000);

        // Ensure TLS 1.3 is used for the connection
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls13;

        // Configure the proxy to use 127.0.0.1:8888
        var proxy = new WebProxy("http://127.0.0.1:8888", false)
        {
            UseDefaultCredentials = true
        };

        using (var httpClientHandler = new HttpClientHandler
        {
            Proxy = proxy,
            SslProtocols = System.Security.Authentication.SslProtocols.Tls13,
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator // For testing purposes only
        })
        {
            // Verify that the correct protocols and proxy settings are being used
            Console.WriteLine($"Supported Protocols: {httpClientHandler.SslProtocols}");
            Console.WriteLine($"Proxy: {httpClientHandler.Proxy.GetProxy(new Uri("https://private56fa323fa.azurewebsites.net/"))}");

            using (var httpClient = new HttpClient(httpClientHandler))
            {
                try
                {
                    // Create a JSON object
                    var jsonObject = new
                    {
                        key1 = "value1",
                        key2 = "value2",
                        key3 = "value3"
                    };
                    string jsonContent = System.Text.Json.JsonSerializer.Serialize(jsonObject);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "text/plain"); // application/json");

                    // Send a POST request with the JSON object to https://private56fa323fa.azurewebsites.net
                    Console.WriteLine("Sending Request!!!");
                    var content2 = new StringContent("Hello World!!", Encoding.UTF8, "text/plain");
                    var response = await httpClient.PostAsync("https://private56fa323fa.azurewebsites.net/", content2);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response received successfully.");
                    Console.WriteLine(responseBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request failed: {ex.Message}");
                    Console.WriteLine($"Request failed: {ex.InnerException}");
                }
            }
        }

        Console.ReadLine();
    }
}
