using System.Diagnostics;

namespace Monitoring.Models.MonitoringModule.checker.ConcreteChecker;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;

public class SSLChecker : IChecker
{
    private MonitoringDbContext _context;
    public int _retries {get; set;}

        public void initialize(string content, int retries, MonitoringDbContext context)
        {
            _context = context;
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
                _context.CheckResults.Add(checkResult);
                await _context.SaveChangesAsync();
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
