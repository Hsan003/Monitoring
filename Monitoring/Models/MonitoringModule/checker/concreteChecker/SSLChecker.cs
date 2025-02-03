using System.Diagnostics;

namespace Monitoring.Models.MonitoringModule.checker.ConcreteChecker;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;

public class SSLChecker : IChecker
{
    public int _retries {get; set;}
    private static readonly HttpClient _httpClient = new HttpClient{
        Timeout = TimeSpan.FromSeconds(10) 
    };
    
        public void initialize(string content, int retries)
        {
            _retries = retries;
        }
    public async Task<CheckResults> check(Website website )
        {
            Console.WriteLine($"Checking SSL status for {website.Url}...");
            CheckResults checkResult = new CheckResults
            {
                websiteId = website.Id,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                HttpResponseMessage message = await _httpClient.GetAsync(website.Url);
                checkResult.status = message.StatusCode;
                
                var start = DateTime.Now;
                var stopwatch = Stopwatch.StartNew();

                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        if (sslPolicyErrors != SslPolicyErrors.None)
                        {
                            checkResult.ErrorMessage = $"SSL Policy Error: {sslPolicyErrors}";
                            Console.WriteLine(checkResult.ErrorMessage);
                            return false;
                        }

                        if (cert != null)
                        {
                            Console.WriteLine($"Certificate Issuer: {cert.Issuer}");
                            Console.WriteLine($"Certificate Subject: {cert.Subject}");
                            Console.WriteLine($"Valid From: {cert.GetEffectiveDateString()}");
                            Console.WriteLine($"Valid To: {cert.GetExpirationDateString()}");

                            var expirationDate = DateTime.Parse(cert.GetExpirationDateString());
                            if (expirationDate < DateTime.UtcNow)
                            {
                                checkResult.ErrorMessage = "Certificate has expired.";
                                Console.WriteLine(checkResult.ErrorMessage);
                                return false;
                            }
                        }

                        return true; 
                    };

                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        var response = httpClient.GetAsync(website.Url).Result;
                        stopwatch.Stop(); 
                        checkResult.ResponseTime = (int)stopwatch.ElapsedMilliseconds;
                        checkResult.status = response.StatusCode;

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("SSL check passed.");
                            checkResult.isUp = true;
                        }
                        else
                        {
                            Console.WriteLine("SSL check failed.");
                            checkResult.isUp = false;
                            checkResult.ErrorMessage = "Non-successful HTTP status code received.";
                        }
                    }
                }
                var end = DateTime.Now;
                checkResult.ResponseTime = (int)(end - start).TotalMilliseconds;
                
            }
            catch (Exception ex)
            {
                checkResult.isUp = false;
                checkResult.ErrorMessage = $"Error during SSL check: {ex.Message}";
                Console.WriteLine(checkResult.ErrorMessage);
            }

            return checkResult;
        }
}
